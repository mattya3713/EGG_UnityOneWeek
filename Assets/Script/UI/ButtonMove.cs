using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Easing;

public class UIScaleAnim : MonoBehaviour
{
    [SerializeField]
    float moveTime = 0.5f;
    [SerializeField]
    float maxMove=0.5f;

    float waitTime = 0.5f;
    bool waitStart=false;
    float _time=1f;
    bool enterAnim = false;
    bool exitAnim = false;

    Vector3 startPos;
    Vector3 endPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Update()
    {
        //å≥ÇÃà íuÇ…ñﬂÇ¡ÇƒÇ©ÇÁè≠Çµë“Ç¬
        if (exitAnim && !waitStart)
        {
            waitStart = true;
            StartCoroutine(WaitCoroutine());
        }
    }

    public void OnEnterMouse()
    {
        if(enterAnim||waitStart) return;
        if (startPos == Vector3.zero)
        {
            startPos = transform.position;
        }
        enterAnim = true;
        StartCoroutine(MoveCoroutine());
    }

    public void OnExitMouse()
    {
        if(exitAnim|waitStart) return;
        
        exitAnim = true;
        StartCoroutine(MoveCoroutineReturn());
    }

    IEnumerator MoveCoroutine()
    {
        _time = 0;
        while (_time < moveTime)
        {
            _time += Time.deltaTime;
            Vector3 position = startPos+
                new Vector3(CubicOut(_time, moveTime,0, maxMove),0,0);

            transform.position = position;
            yield return null;
        }
        enterAnim = false;

        if (endPos == Vector3.zero)
        {
            endPos = transform.position;
        }
    }

    IEnumerator MoveCoroutineReturn()
    {
        while (enterAnim)
        {
            yield return null;
        }
        _time = 0;
        while (_time < moveTime)
        {
            _time += Time.deltaTime;
            Vector3 position = endPos +
                new Vector3(CubicOut(_time, moveTime, 0, -maxMove), 0, 0);

            transform.position = position;
            yield return null;
        }
        transform.position = startPos;
        exitAnim = false;
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        waitStart = false;
    }
}
