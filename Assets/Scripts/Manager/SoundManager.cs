using UnityEngine;
using UnityEngine.Audio; // 오디오 믹서 제어를 위해 필요
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 믹서 연결")]
    public AudioMixer mainMixer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작 시 저장된 볼륨 불러오기 (없으면 0.75 기본값)
        LoadVolume("MasterVol");
        LoadVolume("BGMVol");
        LoadVolume("SFXVol");
    }

    // 슬라이더에서 호출할 함수 (0 ~ 1 사이의 값을 받음)
    public void SetMasterVolume(float sliderValue) => SetMixerVolume("MasterVol", sliderValue);
    public void SetBGMVolume(float sliderValue) => SetMixerVolume("BGMVol", sliderValue);
    public void SetSFXVolume(float sliderValue) => SetMixerVolume("SFXVol", sliderValue);

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        // 믹서 볼륨은 데시벨(dB) 단위를 사용하므로 로그 계산이 필요함
        // 0.0001f는 슬라이더가 0일 때 로그 계산 오류를 방지하기 위함
        float volume = Mathf.Log10(Mathf.Max(0.0001f, sliderValue)) * 20;
        mainMixer.SetFloat(parameterName, volume);

        // 값 저장
        PlayerPrefs.SetFloat(parameterName, sliderValue);
    }

    private void LoadVolume(string parameterName)
    {
        float savedValue = PlayerPrefs.GetFloat(parameterName, 0.75f);
        SetMixerVolume(parameterName, savedValue);
    }

    // 현재 설정된 볼륨값을 가져오는 함수 (UI 초기화용)
    public float GetSavedVolume(string parameterName)
    {
        return PlayerPrefs.GetFloat(parameterName, 0.75f);
    }
}