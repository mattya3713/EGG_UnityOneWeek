using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Goal : MonoBehaviour
{
    public static HashSet<Vector2Int> goalGridPositions = new HashSet<Vector2Int>();

    private void Start()
    {
        // ゴール位置をグリッド座標で登録
        if (GridChanager.Instance != null)
        {
            // ゴールタイルのコライダーを登録
            Collider2D[] goalColliders = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider in goalColliders)
            {
                Vector2Int gridPos = GridChanager.Instance.GetGridPosition(collider.transform.position);
                goalGridPositions.Add(gridPos);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameManager.goalFlag = true;
            GameManager.gameEndFlag = true;
        }
    }
}
