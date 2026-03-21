using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static Easing2D;
using static SceneManagerSystem;
using static BGMManager;

public class TitleManager : MonoBehaviour
{
    [Header("スクロール移動速度")]
    public float moveSpeed = 0.1f;
    [Header("自動で戻る速度")]
    public float autoReturnSpeed = 0.1f;

    Vector3 startPos;

    float yPos = 0f; // 更新後y座標

    private InputAction mouseWheel;

    public GameObject bg;
    public GameObject[] star;

    public float time = 0.5f;
    float _time;

    Camera mainCamera;

    public GameObject stageSelect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetBgm(BGM.Title,true);
        mouseWheel = InputSystem.actions.FindAction("Wheel");

        mainCamera = Camera.main;

        startPos = mainCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (yPos > 0)
        {
            _time += Time.deltaTime;
        }

        // マウスホイールの入力処理
        float scrollValue = mouseWheel.ReadValue<Vector2>().y;

        if (scrollValue > 0)
        {
            if (!stageSelect.activeSelf)
            {
                stageSelect.SetActive(true);
            }
                if (yPos > 0)
            {
                yPos = moveSpeed;
            }
            //上スクロール処理
            yPos = moveSpeed - 5;
            //StartCoroutine(SceneChange());
        }/*
        else if (scrollValue == 0 && _time > time)
        {
            //下スクロール処理
            yPos = autoReturnSpeed;
            _time = 0f;
        }*/
        SetLerp();

        Debug.Log(yPos);
    }

    void SetLerp()
    {
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            startPos + new Vector3(0, -yPos, 0),
            5.0f * Time.deltaTime);
    }

    IEnumerator SceneChange(){
        yield return new WaitForSeconds(1.5f);
        SceneManagerSystem.LoadScene(GameScene.Game);
    }
}
