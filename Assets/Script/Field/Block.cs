using System;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static HashSet<Vector2Int> stageBlockGridPositions = new HashSet<Vector2Int>();

    [SerializeField] private float overlapCheckInterval = 0.05f;

    private float _nextCheckTime;

    private void Start()
    {
        // ステージ上のブロック位置をグリッド座標で登録
        if (GridChanager.Instance != null)
        {
            Vector2Int gridPos = GridChanager.Instance.GetGridPosition(transform.position);
            stageBlockGridPositions.Add(gridPos);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryResetPlayer(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryResetPlayer(other.gameObject);
    }

    private void TryResetPlayer(GameObject target)
    {
        // 一時停止:
        // 押し出し不可時はPlayerDie()でゲームオーバーにする仕様に変更したため、
        // ここで初期位置へ戻す処理は無効化する。
        return;
    }
}
