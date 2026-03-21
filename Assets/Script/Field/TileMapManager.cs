using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    private void Start()
    {
        this.GetComponent<TilemapRenderer>().enabled=false;
    }
}
