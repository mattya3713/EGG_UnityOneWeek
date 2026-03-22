using UnityEngine;

// MEMO : [RequireComponent] をつけると、自動でUnity標準の Grid コンポーネントが追加される.
[RequireComponent(typeof(Grid))]
public class GridChanager : MonoBehaviour
{
    public static GridChanager Instance { get; private set; }
    
    private Grid _unityGrid;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _unityGrid = GetComponent<Grid>(); // Unity標準のGridを取得.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 与えられたワールド座標から、どのグリッド(インデックス)に属するか.
    /// </summary>
    /// <param name="worldPosition">ワールド座標</param>
    /// <returns>グリッドの整数座標 (Vector2Int)</returns>
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        // Unity標準の関数を使って計算
        Vector3Int cellPos = _unityGrid.WorldToCell(worldPosition);
        return new Vector2Int(cellPos.x, cellPos.y);
    }

    /// <summary>
    /// グリッド座標からそのグリッドの中心のワールド座標.
    /// </summary>
    /// <param name="gridPosition">グリッド座標</param>
    /// <returns>中心のワールド座標 (Vector3)</returns>
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        Vector3Int cellPos = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        return _unityGrid.GetCellCenterWorld(cellPos);
    }
}
