using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [Header("재생할 사운드 선택")]
    public SFXType soundType;

    // 이 함수를 호출하면 설정된 사운드가 재생됩니다.
    public void Play()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(soundType);
        }
    }
}