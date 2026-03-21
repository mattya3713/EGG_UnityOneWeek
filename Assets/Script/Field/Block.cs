using System;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] private float overlapCheckInterval = 0.05f;

    private float _nextCheckTime;

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
