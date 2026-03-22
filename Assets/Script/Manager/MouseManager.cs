using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    public static event Action<Vector2Int, Vector3> BlockPlaced;

    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float placementZ = 0f;
    [SerializeField] private bool preventDuplicatePlacement = true;

    private readonly HashSet<Vector2Int> _occupiedGridPositions = new HashSet<Vector2Int>();
    private Vector2Int _lastSpawnedGridPos = new Vector2Int(int.MinValue, int.MinValue);
    private Camera _mainCamera;

    private Vector3 _previousMouseWorldPosition = Vector3.zero;
    private Vector2 _currentMouseMoveDirection = Vector2.zero;

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

        // マウスの移動方向を計算
        Vector3 mouseDelta = mouseWorldPosition - _previousMouseWorldPosition;
        if (mouseDelta.magnitude > 0.001f)
        {
            _currentMouseMoveDirection = new Vector2(mouseDelta.x, mouseDelta.y).normalized;
        }
        else
        {
            _currentMouseMoveDirection = Vector2.zero;
        }
        _previousMouseWorldPosition = mouseWorldPosition;

        // オブジェクトをマウス位置に追従させる
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

        // 前回と同じグリッド位置なら何もしない（連続配置防止）
        if (gridPos == _lastSpawnedGridPos)
        {
            return;
        }

        // 重複配置禁止の場合、既に置かれたグリッドには配置しない
        if (preventDuplicatePlacement && _occupiedGridPositions.Contains(gridPos))
        {
            return;
        }

        // ゴール位置にはブロックを置けない
        if (Goal.goalGridPositions.Contains(gridPos))
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

        // 配置位置を記録してイベント発火
        _occupiedGridPositions.Add(gridPos);
        _lastSpawnedGridPos = gridPos;
        BlockPlaced?.Invoke(gridPos, spawnWorldPos);

        Debug.Log($"グリッド {gridPos} にオブジェクトを生成しました。");
    }

    /// <summary>
    /// マウスの移動方向を取得します。
    /// </summary>
    /// <returns>正規化されたマウス移動方向ベクトル</returns>
    public static Vector2 GetMouseMoveDirection()
    {
        MouseManager manager = FindObjectOfType<MouseManager>();
        if (manager != null)
        {
            return manager._currentMouseMoveDirection;
        }
        return Vector2.zero;
    }
}
