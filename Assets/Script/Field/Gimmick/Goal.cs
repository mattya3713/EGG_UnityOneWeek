using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Goal : MonoBehaviour
{

    private void Start()
    {
      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameManager.goalFlag = true;
            GameManager.gameEndFlag = true;
        }
    }
}
