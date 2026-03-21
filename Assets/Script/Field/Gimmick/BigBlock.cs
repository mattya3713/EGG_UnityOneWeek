using UnityEngine;
using UnityEngine.InputSystem;

public class BigBlock : MonoBehaviour
{
    Camera mainCam;
    public float windowSize = 5f; //ウィンドウサイズ
    public float zoomSpeed = 0.1f; //ズーム速度

    private InputAction mouseWheel;

    float _time = 0f; //時間計測用
    public float time = 2f; //時間制限

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main; //メインカメラを取得

        mouseWheel = InputSystem.actions.FindAction("Wheel");
        //カメラ初期位置設定
        mainCam.orthographicSize = windowSize;

    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;

        // マウスホイールの入力処理
        float scrollValue = mouseWheel.ReadValue<Vector2>().y;
        //Debug.Log(scrollValue);
        if (scrollValue > 0)
        {
            // ズームイン処理
            windowSize = zoomSpeed;
        }
        else if (scrollValue == 0 && _time > time)
        {
            //下スクロール処理
            windowSize = 5;
            _time = 0f;
        }

        

        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize,
            windowSize, 10.0f * Time.deltaTime);
    }
}
