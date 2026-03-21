using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public enum BGM
{
    Title,
    InGame,
    Result,
    InGame_Hard,
}

public class BGMManager : MonoBehaviour
{
    static GameObject instance;
    static BGMManager self;

    [EnumIndex(typeof(BGM))]
    public AudioClip[] _bgmClip;      // メイン（ループ）
    public static AudioClip[] bgmClip;

    // イントロ（任意）
    [EnumIndex(typeof(BGM))]
    public AudioClip[] _bgmIntroClip;
    public static AudioClip[] bgmIntroClip;

    // 2ソースでスケジュール再生
    static AudioSource bgmSourceIntro;
    static AudioSource bgmSourceLoop;

    // 音量管理（AudioVolumeManager から制御）
    static float defaultLoopVolume = 1f;
    static float defaultIntroVolume = 1f;
    static float masterVolume = 1f; // 0..1（UIスライダー想定）

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this.gameObject;
            self = this;

            // ループ側
            bgmSourceLoop = GetComponent<AudioSource>();
            if (bgmSourceLoop == null) bgmSourceLoop = gameObject.AddComponent<AudioSource>();
            bgmSourceLoop.playOnAwake = false;
            defaultLoopVolume = bgmSourceLoop.volume;

            // イントロ側
            bgmSourceIntro = gameObject.AddComponent<AudioSource>();
            bgmSourceIntro.playOnAwake = false;
            bgmSourceIntro.loop = false;
            // 可能ならミキサー設定と音量を合わせる（ベース音量も一致させる）
            bgmSourceIntro.outputAudioMixerGroup = bgmSourceLoop.outputAudioMixerGroup;
            bgmSourceIntro.volume = bgmSourceLoop.volume;
            defaultIntroVolume = bgmSourceIntro.volume;

            // クリップ配列コピー
            bgmClip = new AudioClip[_bgmClip.Length];
            for (int i = 0; i < _bgmClip.Length; i++)
            {
                bgmClip[i] = _bgmClip[i];
            }

            if (_bgmIntroClip != null && _bgmIntroClip.Length == _bgmClip.Length)
            {
                bgmIntroClip = new AudioClip[_bgmIntroClip.Length];
                for (int i = 0; i < _bgmIntroClip.Length; i++)
                {
                    bgmIntroClip[i] = _bgmIntroClip[i];
                }
            }
            else
            {
                bgmIntroClip = new AudioClip[_bgmClip.Length];
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Update()
    {
        if (bgmSourceLoop == null)
        {
            bgmSourceLoop = GetComponent<AudioSource>();
        }
        if (bgmSourceIntro == null)
        {
            bgmSourceIntro = gameObject.AddComponent<AudioSource>();
            bgmSourceIntro.playOnAwake = false;
            bgmSourceIntro.loop = false;
            bgmSourceIntro.outputAudioMixerGroup = bgmSourceLoop.outputAudioMixerGroup;
            // イントロ側もループ側と同じ初期音量に揃える
            bgmSourceIntro.volume = bgmSourceLoop != null ? bgmSourceLoop.volume : bgmSourceIntro.volume;
            defaultIntroVolume = bgmSourceIntro.volume;
        }
    }

    public static void SetBgm(BGM bgm)
    {
        PlayBgmInternal(bgm, true, true);
    }

    public static void SetBgm(BGM bgm,bool loop)
    {
        PlayBgmInternal(bgm, loop, useIntroIfAvailable: loop);
    }

    static void PlayBgmInternal(BGM bgm, bool loop, bool useIntroIfAvailable)
    {
        // 停止・初期化
        if (bgmSourceIntro != null) { bgmSourceIntro.Stop(); }
        if (bgmSourceLoop != null) { bgmSourceLoop.Stop(); }

        // Ensure clips are initialized (handles early SetBgm calls)
        if (bgmClip == null)
        {
            BGMManager inst = null;
#if UNITY_2022_3_OR_NEWER
            inst = Object.FindFirstObjectByType<BGMManager>();
#else
            inst = Object.FindObjectOfType<BGMManager>();
#endif
            if (inst == null || inst._bgmClip == null)
            {
                Debug.LogWarning("BGMManager: instance or _bgmClip not found. Abort.");
                return;
            }
            bgmClip = new AudioClip[inst._bgmClip.Length];
            for (int i = 0; i < inst._bgmClip.Length; i++) bgmClip[i] = inst._bgmClip[i];
            int introSrcLen = (inst._bgmIntroClip != null) ? inst._bgmIntroClip.Length : 0;
            bgmIntroClip = new AudioClip[inst._bgmClip.Length];
            for (int i = 0; i < inst._bgmClip.Length; i++)
                bgmIntroClip[i] = (i < introSrcLen) ? inst._bgmIntroClip[i] : null;
            if (bgmSourceLoop == null)
                bgmSourceLoop = inst.GetComponent<AudioSource>() ?? inst.gameObject.AddComponent<AudioSource>();
            if (bgmSourceIntro == null)
            {
                bgmSourceIntro = inst.gameObject.AddComponent<AudioSource>();
                bgmSourceIntro.playOnAwake = false;
                bgmSourceIntro.loop = false;
                if (bgmSourceLoop != null)
                    bgmSourceIntro.outputAudioMixerGroup = bgmSourceLoop.outputAudioMixerGroup;
            }
        }

        if ((int)bgm < 0 || (bgmClip == null) || (int)bgm >= bgmClip.Length || bgmClip[(int)bgm] == null)
        {
            Debug.LogWarning($"BGMManager: Main clip missing for index {(int)bgm}. Abort.");
            return;
        }

        var main = bgmClip[(int)bgm];
        var intro = (useIntroIfAvailable && bgmIntroClip != null && bgmIntroClip.Length > (int)bgm)
            ? bgmIntroClip[(int)bgm]
            : null;

        if (loop && intro != null)
        {
            // イントロ1回 + メイン無限ループ
            bgmSourceIntro.clip = intro;
            bgmSourceIntro.loop = false;

            bgmSourceLoop.clip = main;
            bgmSourceLoop.loop = true;

            ApplyVolume();

#if UNITY_WEBGL
            if (self != null)
            {
                self.StartCoroutine(self.WebGLPlayIntroThenLoop(intro, main));
            }
            else
            {
                bgmSourceLoop.Play();
            }
#else
            double start = AudioSettings.dspTime + 0.1;
            double introLen = intro.length;
            bgmSourceIntro.PlayScheduled(start);
            bgmSourceIntro.SetScheduledEndTime(start + introLen);
            bgmSourceLoop.PlayScheduled(start + introLen);
#endif
        }
        else
        {
            // メインのみ再生
            bgmSourceLoop.loop = loop;
            bgmSourceLoop.clip = main;
            ApplyVolume();
            bgmSourceLoop.Play();
        }
    }

    static void ApplyVolume()
    {
        if (bgmSourceLoop != null) bgmSourceLoop.volume = defaultLoopVolume * Mathf.Clamp01(masterVolume);
        if (bgmSourceIntro != null) bgmSourceIntro.volume = defaultIntroVolume * Mathf.Clamp01(masterVolume);
    }

    // UI から呼ぶ音量設定（0..1）
    public static void SetBgmMasterVolume(float normalized)
    {
        masterVolume = Mathf.Clamp01(normalized);
        ApplyVolume();
    }

    // Immediately stop current BGM (Intro/Loop)
    public static void StopBgm()
    {
        if (bgmSourceIntro != null) bgmSourceIntro.Stop();
        if (bgmSourceLoop != null) bgmSourceLoop.Stop();
    }

#if UNITY_WEBGL
    // WebGL: Introを通常再生→リアルタイム待機→Loop再生
    IEnumerator WebGLPlayIntroThenLoop(AudioClip intro, AudioClip loop)
    {
        // 事前ロード（必要なら）
        if (intro != null && intro.loadState == AudioDataLoadState.Unloaded) intro.LoadAudioData();
        if (loop  != null && loop.loadState  == AudioDataLoadState.Unloaded)  loop.LoadAudioData();
        while ((intro != null && intro.loadState == AudioDataLoadState.Loading) ||
               (loop  != null && loop.loadState  == AudioDataLoadState.Loading))
        {
            yield return null;
        }

        // 念のため停止→クリップ反映（ここでtimeは触らない）
        bgmSourceIntro.Stop();
        bgmSourceLoop.Stop();
        bgmSourceIntro.clip = intro;
        bgmSourceIntro.loop = false;
        bgmSourceLoop.clip = loop;
        bgmSourceLoop.loop = true;
        ApplyVolume();

        // Intro再生
        bgmSourceIntro.time = 0f;
        bgmSourceIntro.Play();

        // Intro長（固定0.275sが正ならそれを優先）
        const float FixedIntroSeconds = 0.275f;
        float pitch = Mathf.Max(0.01f, bgmSourceIntro.pitch);
        float wait = (intro != null && intro.length > 0.0001f)
            ? intro.length / pitch
            : FixedIntroSeconds / pitch;

        yield return new WaitForSecondsRealtime(wait);

        // Loopへ切替
        bgmSourceIntro.Stop();
        bgmSourceLoop.time = 0f;
        bgmSourceLoop.pitch = bgmSourceIntro.pitch;
        bgmSourceLoop.Play();
    }
#endif
}
