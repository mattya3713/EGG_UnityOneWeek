using System.Collections;
using UnityEngine;

public class Logo : MonoBehaviour
{
    [SerializeField]
    GameObject mouseWheel;//マウスホイール操作示唆

    [SerializeField]
    GameObject volumeCanvas;//音量調整UI

    private void Start()
    {
        // ロゴの移動を開始
        StartCoroutine(LogoMove());
    }


    IEnumerator LogoMove()
    {

       // ロゴの位置を初期化
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(startPos.x, startPos.y - 4f, startPos.z);
        
        // ロゴの移動時間
        float duration = 2.0f;
        float elapsedTime = 0.0f;
        // ロゴの移動処理
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // 次のフレームまで待機
        }
        // 最終位置に設定
        transform.position = endPos;
        mouseWheel.SetActive(true);

        volumeCanvas.SetActive(true);
    }
}
