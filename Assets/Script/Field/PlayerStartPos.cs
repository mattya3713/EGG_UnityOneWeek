using UnityEditor;
using UnityEngine;

public class PlayerStartPos : MonoBehaviour
{
    public GameObject playerObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(playerObj, transform.position, Quaternion.identity);
        //Debug.Log("ÉvÉåÉCÉÑÅ[è¢ä´");
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
