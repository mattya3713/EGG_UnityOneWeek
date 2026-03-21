using UnityEngine;

public class NotMoveBlock : MonoBehaviour
{
    [SerializeField]
    private Sprite[] blockRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int rand = Random.Range(0, blockRenderer.Length);
        GetComponent<SpriteRenderer>().sprite = blockRenderer[rand];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
