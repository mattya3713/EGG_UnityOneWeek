using UnityEngine;
using static GameManager;

public class Damage : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ˆê‘Î‰: €–S”»’è‚ğ‘—‚ç‚È‚¢iŒŸØ—pj
            return;

            // collision.gameObject.GetComponent<PlayerController>().PlayerDie();
        }
    }
}
