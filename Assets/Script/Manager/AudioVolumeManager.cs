using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioVolumeManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioSource seSource;

    public Slider bgmSlider;
    public Slider seSlider;

    //���ʏ����l
    static float bgmVolumeDef;
    static float seVolumeDef;
    bool isInit = false;

    //�ύX���m
    static float _bgmVolume;
    static float _seVolume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (bgmSource != null && bgmVolumeDef == 0f) bgmVolumeDef = bgmSource.volume;
        if (seSource != null && seVolumeDef == 0f) seVolumeDef = seSource.volume;
        BGMManager.SetBgmMasterVolume(_bgmVolume);
        if (seSource != null) seSource.volume = _seVolume * (seVolumeDef == 0f ? 1f : seVolumeDef);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(_bgmVolume);
        if (bgmSlider == null && seSlider == null)
        {
            //�X���C�_�[�̎擾
            bgmSlider = (GameObject.FindGameObjectWithTag("BGMSlider")) ? (GameObject.FindGameObjectWithTag("BGMSlider").GetComponent<Slider>()) : null;
            seSlider = (GameObject.FindGameObjectWithTag("SESlider")) ? (GameObject.FindGameObjectWithTag("SESlider").GetComponent<Slider>()) : null;
        }
        else
        {
            if (!isInit)
            {
                //Debug.Log("volume Init");
                isInit = true;
                bgmSlider.value = _bgmVolume;
                seSlider.value = _seVolume;

                //�������ʔ{���ێ�
                if (bgmVolumeDef == 0 || seVolumeDef == 0)
                {
                    bgmVolumeDef = bgmSource.volume;
                    seVolumeDef = seSource.volume;
                }
                // 初期値を適用
                BGMManager.SetBgmMasterVolume(_bgmVolume);
                if (seSource != null) seSource.volume = _seVolume * seVolumeDef;
            }
        }
        if(!isInit)
            return;
        //�X���C�_�[��null�̂Ƃ��͏������Ȃ�
        if (bgmSlider == null)
            return;
        if (seSlider == null)
            return;
        if (bgmSlider.value != _bgmVolume)
        {
            _bgmVolume = bgmSlider.value;
            BGMManager.SetBgmMasterVolume(_bgmVolume);
            if (bgmSource != null) { bgmSource.volume = _bgmVolume * bgmVolumeDef; }
        }
        if (seSlider.value != _seVolume)
        {
            seSource.volume = seSlider.value * seVolumeDef;
            _seVolume = seSlider.value;
        }
    }

    //�V�[���J�ڎ���init��false�ɂ���
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInit = false;
    }
}
