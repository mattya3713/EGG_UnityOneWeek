using UnityEngine;
using static GameManager;

public class FieldManager : MonoBehaviour
{
    public GameObject[] Stage;

    [HideInInspector]
    public int stageCount;

    private void Start()
    {
        stageCount = Stage.Length;
        //ステージ生成
        StageCreate(SceneManagerSystem.nowStageNum);
    }
    
    public void StageCreate(int i)
    {
        Stage[i].SetActive(true);
        //Debug.Log(Stage[i].name);
        //    GameObject obj= Instantiate(Stage[i], Vector3.zero, Quaternion.identity);
        //    obj.transform.SetParent(this.transform);
    }
}
