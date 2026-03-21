using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

public class CameraController : MonoBehaviour
{
    Camera mainCam;

    private Transform player; //プレイヤーオブジェクト
    PlayerController playerController;

    Vector3 moveAngle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCam = Camera.main; //メインカメラを取得
        playerController=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player = playerController.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 endpos = player.position + new Vector3(0,  3, -10);

        if (playerController.onGround)
        {
            //マウスの移動量を足す
            moveAngle = playerController.moveValue * 2f;
            //moveAngle.y=0; //y軸は動かさない
        }
        endpos += moveAngle;

        transform.position = Vector3.Lerp(
            transform.position,
            endpos, // カメラzの位置
            5.0f * Time.deltaTime);
    }
}
