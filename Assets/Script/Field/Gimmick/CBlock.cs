using UnityEngine;
using UnityEngine.Tilemaps;

public class CBlock : MonoBehaviour
{
    public GameObject tileMapObj;
    Tilemap tilemap;
    TilemapRenderer tilemapRenderer;
    public GameObject goalObj;
    public Sprite goal;

    GameObject _goalObj;//生成済みゴールオブジェクトを格納する配列

    bool oneShotFlag = false;//一回だけ通す

    private void Start()
    {
        tilemap = tileMapObj.GetComponent<Tilemap>();
        tilemapRenderer = tileMapObj.GetComponent<TilemapRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x > 8f)
        {
            if (!oneShotFlag)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                //タイルマップの位置にオブジェクトを設置する
                ReplaceTilemap();
                //タイルマップを表示する
                tilemapRenderer.enabled = false;
                oneShotFlag = true;
            }
        }
        else
        {
            if (oneShotFlag)
            {
                GetComponent<BoxCollider2D>().enabled = true;
                //タイルマップの位置のオブジェを全削除する
                GameObject.Destroy(_goalObj);
                //タイルマップを非表示にする
                tilemapRenderer.enabled = true;
                oneShotFlag = false;
            }
        }
    }


    public void ReplaceTilemap()
    {
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            // 取り出した位置情報からタイルマップ用の位置情報(セル座標)を取得
            Vector3Int cellPosition = new Vector3Int(pos.x, pos.y, pos.z);

            // tilemap.HasTile -> タイルが設定(描画)されている座標であるか判定
            if (tilemap.HasTile(cellPosition))
            {
                _goalObj = Instantiate(goalObj, tilemap.GetCellCenterWorld(cellPosition), Quaternion.identity, transform);
                _goalObj.transform.SetParent(tileMapObj.transform);
                _goalObj.transform.localScale = Vector3.one;
            }
        }
    }
}
