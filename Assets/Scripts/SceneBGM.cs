using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    public AudioClip bgmClip;

    void Start()
    {
        Debug.Log("SceneBGM: 음악 재생 시도 중...");
        if (SoundManager.Instance != null && bgmClip != null)
        {
            Debug.Log("SceneBGM: " + bgmClip.name + " 재생 요청 보냄!");
            SoundManager.Instance.ChangeBGM(bgmClip);
        }
        else
        {
            Debug.LogWarning("SceneBGM: 매니저가 없거나 클립이 비어있음!");
        }
    }
}