using UnityEngine;

public class Convayor : MonoBehaviour
{
    [SerializeField]
    bool isLeft;

    [SerializeField]
    float speed = 1.0f;

    private void Start()
    {
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        collision.transform.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(
            isLeft ? -speed : speed,
            collision.transform.GetComponent<Rigidbody2D>().linearVelocity.y
        );

        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<PlayerController>().convayorS = isLeft ? -speed : speed;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.GetComponent<PlayerController>().convayorS = 0;
        }
    }

}
