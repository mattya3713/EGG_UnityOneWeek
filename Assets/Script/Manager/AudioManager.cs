using UnityEngine;

public enum SE
{
    goal,
    keyDoorOpen,
    keyGet,
    playerGround,
    playerJumpLong,
    playerJumpShort,
    playerMove,
    retry,
    stageChange,
    useSuperPower,
    convayor,
    isZoomMax,
    isZoomMin,
}

public class AudioManager : MonoBehaviour
{
    static GameObject instance;

    [EnumIndex(typeof(SE))]
    public AudioClip[] _clip;
    public static AudioClip[] clip;
    static AudioSource source;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this.gameObject;

            clip = new AudioClip[_clip.Length];
            source = GetComponent<AudioSource>();
            for (int i = 0; i < _clip.Length; i++)
            {
                clip[i] = _clip[i];
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
    }

    private void Update()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }
    }

    public static void SetAudio(SE se)
    {
        //Debug.Log(source);
        //source.PlayOneShot(clip[(int)se]);
    }
    public static void SetAudio(SE se, float volume)
    {
        //Debug.Log(source);
        //source.PlayOneShot(clip[(int)se], volume);
    }

    public static void StopAudio()
    {
        source.Stop();
    }

    public static void StopBGM(AudioSource audio)
    {
        audio.Stop();
    }
}
