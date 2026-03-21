using System.Collections;
using System.Collections.Generic;
using Unity.Burst;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager.Requests;

#endif
using UnityEngine;

using UnityEngine.SceneManagement;
using static SceneManagerSystem;

using static AudioManager;
using static BGMManager;

// Game flow controller
public enum GameMode
{
    score,
    count,
    time,
}

public class GameManager : MonoBehaviour
{

    public static bool PauseFlag=false;     // pause true when game paused
    public static bool goalFlag=false;      // true when goal reached
    public static bool gameEndFlag=false;   // true when stage end processing

    // Prevent double-processing during end flow
    bool endProcessing = false;


    [SerializeField]
    private FieldManager fieldManager;

    [SerializeField]
    private GameObject pauseUI;

    // Start is called before the first frame update
    void Start()
    {
        if(nowStageNum+1==7||nowStageNum+1==8||nowStageNum+1==17||nowStageNum+1==18|| nowStageNum + 1 == 29 || nowStageNum + 1 == 30)
        {
            SetBgm(BGM.InGame_Hard,true);
        }
        else
        {
            SetBgm(BGM.InGame,true);
        }
            
        stageCount = fieldManager.stageCount;
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayerDie();
        }*/
        // Stage end flow
        if (gameEndFlag)
        {
            GameEndCheck();
            return;
        }

        // Pause toggle
        /*if (Input.GetKeyDown(KeyCode.Escape)|| Input.GetKeyDown(KeyCode.R))
        {
            if (PauseFlag == false)
            {
                OnPauseFlagTrue();
                pauseUI.SetActive(true);
            }
            else if (PauseFlag == true)
            {
                OnPauseFlagFalse();
                pauseUI.SetActive(false);
            }
        }*/
    }

    // When damaged
    public static void Damage()
    {
        gameEndFlag = true;
        goalFlag = false;
    }

    // Check when stage ends
    public void GameEndCheck()  
    {
        if (gameEndFlag)
        {
            if (goalFlag)
            {
                if (endProcessing) return; // already handled

                // Stage clear: stop BGM immediately, play SFX and pause gameplay, then continue after delay
                StopBgm();
                SetAudio(SE.goal);
                OnPauseFlagTrue();
                endProcessing = true;
                StartCoroutine(ProceedAfterGoalDelay());
                Debug.Log("Player Goal!!!");
            }
            else
            {
                // Die -> retry/next flow
                NextStage();
                Debug.Log("Player Die");
            }
        }
    }

    // Wait 1 second in real time (unscaled), then proceed to next stage or result
    IEnumerator ProceedAfterGoalDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        // Determine whether this clear is new or replay
        bool isNewClear = nowStageNum >= clearStageCount;
        if (isNewClear)
        {
            clearStageCount++;
        }

        // Only go to ending when all stages become cleared by this clear
        if (isNewClear && clearStageCount >= stageCount)
        {
            OnPauseFlagFalse();
            LoadScene(GameScene.Result);
            Debug.Log("Game Clear!!!");
        }
        else
        {
            if(nowStageNum < stageCount)
            {
                nowStageNum++;
            }
            OnPauseFlagFalse();
            NextStage();
            Debug.Log("Next Stage!!!");
        }

        // Reset flags for safety
        gameEndFlag = false;
        endProcessing = false;
    }

    IEnumerator Result()
    {
        if (goalFlag)
        {
            yield return new WaitForSeconds(2f);
        }
        else
        {
            yield return null;
        }
        OnPauseFlagTrue();
    }

    // pauseflag true -> pause time
    public static void OnPauseFlagTrue()
    {
        PauseFlag = true;
        Time.timeScale = 0.0f;
    }
    // pauseflag false -> resume time
    public static void OnPauseFlagFalse()
    {
        PauseFlag = false;
        Time.timeScale = 1.0f;
    }

    // Reset stage-end flags
    public void Reset()
    {
        gameEndFlag = false;
        OnPauseFlagFalse();
    }

    public void NextStage()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public static IEnumerator GameEnd(float waitTime,bool _goalFlag)
    {
        yield return new WaitForSeconds(waitTime);
        gameEndFlag = true;
        goalFlag = _goalFlag;
    }

    public void OnReturnGameBt()
    {
        OnPauseFlagFalse();
        pauseUI.SetActive(false);
    }

    public void OnRetryBt()
    {
        OnPauseFlagFalse();
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnTitleBt()
    {
        OnPauseFlagFalse();
        LoadScene(GameScene.Title);
    }
}

#if UNITY_EDITOR
/**
 * Inspector GUI (disabled)
 
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
   // bool folding = false;

    public override void OnInspectorGUI()
    {
        GameManager manager = target as GameManager;

        manager.gameMode = (GameMode)EditorGUILayout.EnumPopup("GameMode", manager.gameMode);
        if (manager.gameMode == GameMode.score)
        {
            EditorGUI.BeginDisabledGroup(true);
            manager.nowScore = EditorGUILayout.FloatField("Score", manager.nowScore);
            EditorGUI.EndDisabledGroup();
            manager.goalScore = EditorGUILayout.FloatField("GoalScore", manager.goalScore);
        }
        else if (manager.gameMode == GameMode.count)
        {
            EditorGUI.BeginDisabledGroup(true);
            manager.nowCount = EditorGUILayout.FloatField("Count", manager.nowCount);
            EditorGUI.EndDisabledGroup();
            manager.goalCount = EditorGUILayout.FloatField("GoalCount", manager.goalCount);
        }
        else if (manager.gameMode == GameMode.time)
        {
            EditorGUI.BeginDisabledGroup(true);
            manager.nowTime = EditorGUILayout.FloatField("Time", manager.nowTime);
            EditorGUI.EndDisabledGroup();
            manager.goalTime = EditorGUILayout.FloatField("GoalTime", manager.goalTime);
        }
    }
}*/
#endif
