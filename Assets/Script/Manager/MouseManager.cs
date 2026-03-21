using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    public static event Action<Vector2Int, Vector3, Vector2Int> BlockPlaced;

    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnParent; // 生成したオブジェクトの親となるオブジェクト
    [SerializeField] private float placementZ = 0f;
    [SerializeField] private bool preventDuplicatePlacement = true;

    private readonly HashSet<Vector2Int> _occupiedGridPositions = new HashSet<Vector2Int>();
    private Vector2Int _lastSpawnedGridPos = new Vector2Int(int.MinValue, int.MinValue);
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        // オブジェクトをマウス位置に追従させる（このスクリプトがアタッチされたオブジェクト）
        transform.position = mouseWorldPosition;

        // 左クリック中（長押し含む）は配置処理
        if (Mouse.current.leftButton.isPressed)
        {
            HandleClick(mouseWorldPosition);
        }

        // ボタンを離したら連続配置チェック用の座標をリセット
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _lastSpawnedGridPos = new Vector2Int(int.MinValue, int.MinValue);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mouseScreenPosition = new Vector3(mousePosition.x, mousePosition.y, 0f);
        float distance = Mathf.Abs(_mainCamera.transform.position.z - placementZ);
        mouseScreenPosition.z = distance;

        Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = placementZ;
        return mouseWorldPosition;
    }

    private void HandleClick(Vector3 position)
    {
        if (GridChanager.Instance == null || prefabToSpawn == null)
        {
            return;
        }

        // クリックされた座標からグリッド座標を取得
        Vector2Int gridPos = GridChanager.Instance.GetGridPosition(position);

        if (gridPos == _lastSpawnedGridPos)
        {
            return;
        }

        if (preventDuplicatePlacement && _occupiedGridPositions.Contains(gridPos))
        {
            return;
        }

        // グリッドの中心のワールド座標を取得
        Vector3 spawnWorldPos = GridChanager.Instance.GetWorldPosition(gridPos);
        spawnWorldPos.z = placementZ;

        // オブジェクトを実体化
        if (spawnParent != null)
        {
            Instantiate(prefabToSpawn, spawnWorldPos, Quaternion.identity, spawnParent);
        }
        else
        {
            Instantiate(prefabToSpawn, spawnWorldPos, Quaternion.identity);
        }

        Vector2Int placementDirection = Vector2Int.zero;
        if (_lastSpawnedGridPos.x != int.MinValue)
        {
            Vector2Int rawDirection = gridPos - _lastSpawnedGridPos;
            if (Mathf.Abs(rawDirection.x) >= Mathf.Abs(rawDirection.y))
            {
                placementDirection = new Vector2Int(Mathf.Clamp(rawDirection.x, -1, 1), 0);
            }
            else
            {
                placementDirection = new Vector2Int(0, Mathf.Clamp(rawDirection.y, -1, 1));
            }
        }

        _occupiedGridPositions.Add(gridPos);
        _lastSpawnedGridPos = gridPos;
        BlockPlaced?.Invoke(gridPos, spawnWorldPos, placementDirection);

        Debug.Log($"グリッド {gridPos} にオブジェクトを生成しました。");
    }
}
