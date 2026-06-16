using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // 오디오 믹서 제어용

public class OpTionManager : MonoBehaviour
{
    [Header("연결할 설정 창 패널")]
    public GameObject optionPanel;

    [Header("오디오 믹서 및 슬라이더 설정")]
    public AudioMixer mainMixer; // 여기에 MainMixer 파일 연결
    public Slider musicSlider;   // 하이어라키의 MusicSlider 연결
    public Slider sfxSlider;

    private void Start()
    {
        // 게임 시작 시 설정 창이 실수로 켜져 있다면 확실하게 꺼줍니다.
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }
        if (musicSlider != null)
        {
            musicSlider.minValue = 0.0001f;
            musicSlider.maxValue = 1f;

            // 믹서에 설정된 BGMVolume 데시벨 값을 가져와서 슬라이더 바 위치(0~1)로 변환
            if (mainMixer != null && mainMixer.GetFloat("BGMVolume", out float currentDb))
            {
                musicSlider.value = Mathf.Pow(10f, currentDb / 20f);
            }
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0.0001f;
            sfxSlider.maxValue = 1f;

            if (mainMixer != null && mainMixer.GetFloat("SFXVolume", out float currentSfxDb))
            {
                // 아까 스크린샷을 보니 SFX는 기본이 0dB 부근이었습니다.
                // 믹서 세팅에 맞게 슬라이더 위치를 역산해 맞춥니다.
                sfxSlider.value = Mathf.Pow(10f, currentSfxDb / 20f);
            }
        }
    }

    // ★ Option 버튼을 눌렀을 때 호출할 함수
    public void OpenOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(true); // 설정 창 켜기
        }
    }

    // ★ 설정 창 내부의 '닫기(Back)' 버튼을 눌렀을 때 호출할 함수
    public void CloseOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(false); // 설정 창 끄기
        }
    }
    public void SetBGMVolume(float sliderValue)
    {
        if (mainMixer != null)
        {
            // 0~1 값을 자연스러운 청각 볼륨 데시벨(-80dB ~ 0dB)로 변환
            float dbValue = Mathf.Log10(sliderValue) * 20f;

            // 믹서의 BGMVolume 파라미터에 데시벨 적용
            mainMixer.SetFloat("BGMVol", dbValue);
        }
    }
    public void SetSFXVolume(float sliderValue)
    {
        if (mainMixer != null)
        {
            // 왼쪽 끝으로 밀면 완벽히 음소거
            if (sliderValue <= 0.001f)
            {
                mainMixer.SetFloat("SFXVolume", -80f);
            }
            else
            {
                // SFX의 기본 최대 수치는 오디오 믹서에서 0dB 부근이므로,
                // 슬라이더 1일 때 0dB, 왼쪽으로 갈수록 데시벨이 줄어들도록 정석 공식 적용
                float dbValue = Mathf.Log10(sliderValue) * 20f;
                dbValue = Mathf.Clamp(dbValue, -80f, 0f);

                mainMixer.SetFloat("SFXVol", dbValue);
            }
        }
    }
}