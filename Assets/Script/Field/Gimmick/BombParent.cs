using System.Collections;
using UnityEngine;

public class BombParent : MonoBehaviour
{
    Bomb bomb;

    [SerializeField,Header("最初に与えるパワー")]
    private float bombImpact;

    [SerializeField, Header("爆発までの時間(秒)")]
    private float wait;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bomb = GetComponentInChildren<Bomb>();
        bomb._bombImpact = bombImpact;
        bomb.gameObject.SetActive(false);
        StartCoroutine(BombDestroy());
    }

    IEnumerator BombDestroy()
    {
        yield return new WaitForSeconds(wait);
        //エフェクト再生
        
        bomb.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1f);

        bomb.gameObject.SetActive(false);
        //爆弾削除
        Destroy(gameObject);
    }
}
