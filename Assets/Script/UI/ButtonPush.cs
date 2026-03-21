using UnityEngine;
using UnityEngine.UI;
using static SceneManagerSystem;

public class ButtonPush : MonoBehaviour
{
    public Button[] button;

    private void Start()
    {
        // タイトル直後は stageCount が未設定(0)のことがあるため、
        // 保存されている clearStageCount とボタン配列長から有効化上限を決定する。
        int maxIndex = Mathf.Min(clearStageCount, button.Length - 1);
        for (int i = 0; i <= maxIndex; i++)
        {
            button[i].interactable = true;
        }

        // 任意で stageCount を補完（他所参照のための保険）
        if (stageCount == 0)
        {
            stageCount = button.Length - 1;
        }
    }

    public void OnClick(int stageNum)
    {
        LoadScene(GameScene.Game,stageNum);

    }
}
