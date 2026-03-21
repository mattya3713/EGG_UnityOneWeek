using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public enum GameScene
{
    Title,
    Select,
    Game,
    Result
}

public class SceneManagerSystem : MonoBehaviour
{
    private static SceneManagerSystem instance;

    public static int clearStageCount = 0;
    public static int stageCount = 0; //ステージの総数
    public static int nowStageNum = 0;//現在のステージ番号

    public string[] _sceneName;
    public static string[] sceneName = new string[4]; //遷移先のシーン名

    [SerializeField]
    bool isDebug = false;
    [SerializeField]
    int DebugStageNum = 0;
    
    private void Awake()
    {
        if (isDebug)
            clearStageCount = DebugStageNum;
        if (instance== null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            for(int i = 0; i < _sceneName.Length; i++)
            {
                sceneName[i] = _sceneName[i];
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadScene(GameScene sceneIndex)
    {
        SceneManager.LoadScene(sceneName[(int)sceneIndex]);
    }

    public static void LoadScene(GameScene sceneIndex, int _sceneNum)
    {
        SceneManager.LoadScene(sceneName[(int)sceneIndex]);
        nowStageNum = _sceneNum;
    }

    public static void LoadScene(string sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public static void LoadScene(string sceneIndex,int _sceneNum)
    {
        nowStageNum = _sceneNum;
        SceneManager.LoadScene(sceneIndex);
    }
}
