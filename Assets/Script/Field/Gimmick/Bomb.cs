using UnityEngine;

public class Bomb : MonoBehaviour
{
    [HideInInspector,Header("ç≈èâÇ…ó^Ç¶ÇÈÉpÉèÅ[")]
    public float _bombImpact;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 dir = (collision.transform.position-transform.parent.position).normalized;

            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(dir * _bombImpact, ForceMode2D.Impulse);
            Debug.Log(dir);
        }
    }
}
