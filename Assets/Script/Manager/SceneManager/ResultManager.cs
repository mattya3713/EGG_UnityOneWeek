using System.Collections;
using UnityEngine;
using static SceneManagerSystem;

using static BGMManager;

public class ResultManager : MonoBehaviour
{
    public float waitTime = 2f; // 待機時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetBgm(BGM.Result,false);
        StartCoroutine(ReturnToTitle());
    }

    IEnumerator ReturnToTitle()
    {
        yield return new WaitForSeconds(waitTime);
        //タイトルシーンに戻る
        LoadScene(GameScene.Title);
    }

}
