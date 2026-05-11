using UnityEngine;
using UnityEngine.UI;

public class VolumeUI : MonoBehaviour
{
    [Header("슬라이더들 연결")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        // SoundManager가 아직 생성 안됐을 수도 있으니 안전하게 체크
        if (SoundManager.Instance == null) return;

        // 1. 저장된 값을 불러와서 슬라이더의 위치(value)를 먼저 세팅함
        // 이 때 UI의 OnValueChanged 이벤트가 자동으로 한 번씩 실행되면서 믹서 볼륨도 맞춰집니다.
        masterSlider.value = SoundManager.Instance.GetSavedVolume("MasterVol");
        bgmSlider.value = SoundManager.Instance.GetSavedVolume("BGMVol");
        sfxSlider.value = SoundManager.Instance.GetSavedVolume("SFXVol");

        // 2. 혹시 모르니 리스너가 중복 등록되지 않게 한 번 지워주고 다시 연결 (선택 사항)
        masterSlider.onValueChanged.AddListener(SoundManager.Instance.SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SoundManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
    }
}