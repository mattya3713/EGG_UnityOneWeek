using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static Easing2D;

public class ToglleRotate : MonoBehaviour
{
    [SerializeField]
    float rotateTime = 0.5f;
    [SerializeField]
    float maxRotate=0.5f;

    float _time=1f;
    public bool rotateStartFlag = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnEnterMouse()
    {
        RotateStart();
    }

    public void RotateStart()
    {
        _time = 0;
        rotateStartFlag = true;
        StartCoroutine(RotateCoroutine());
    }

    IEnumerator RotateCoroutine()
    {

        while (_time < rotateTime)
        {
            _time += Time.deltaTime;
            float angle = CubicOut(_time, rotateTime,new Vector2(0, 0), new Vector2(maxRotate, 0)).x;
            transform.localEulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0, 0, 0);
        rotateStartFlag = false;
    }
}
