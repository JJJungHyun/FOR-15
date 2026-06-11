using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// 1. 사운드 종류 목록
public enum SFXType
{
    GameOver, GameClear, WoodCut, StoneMine, MapMove,
    Eat, Campfire, Button, HerbCollect, MonsterRoar,
    PlayerHit, WeaponSwing
}

// 2. 데이터 구조체
[System.Serializable]
public struct SFXData
{
    public SFXType type;
    public AudioClip clip;
}

// 3. 메인 사운드 매니저
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 믹서 연결")]
    public AudioMixer mainMixer;
    public AudioMixerGroup sfxGroup;

    [Header("BGM 설정")]
    public AudioSource bgmSource;

    [Header("SFX 풀 설정")]
    public int sfxPoolSize = 10;
    private AudioSource[] sfxSources;

    [Header("사운드 데이터베이스")]
    public SFXData[] sfxDatabase;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupSFXPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadVolume("MasterVol");
        LoadVolume("BGMVol");
        LoadVolume("SFXVol");
    }

    void SetupSFXPool()
    {
        sfxSources = new AudioSource[sfxPoolSize];
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxObj = new GameObject("SFXSource_" + i);
            sfxObj.transform.SetParent(transform);
            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = sfxGroup;
            source.playOnAwake = false;
            sfxSources[i] = source;
        }
    }

    public void PlaySFX(SFXType type)
    {
        foreach (var data in sfxDatabase)
        {
            if (data.type == type)
            {
                PlayClip(data.clip);
                return;
            }
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.Play();
                return;
            }
        }
    }

    public void ChangeBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }

   public void SetMasterVolume(float v) => SetMixerVolume("MasterVol", v);

    public void SetBGMVolume(float v) => SetMixerVolume("BGMVol", v);
    public void SetSFXVolume(float v) => SetMixerVolume("SFXVol", v);

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        float volume = Mathf.Log10(Mathf.Max(0.0001f, sliderValue)) * 20;
        mainMixer.SetFloat(parameterName, volume);
        PlayerPrefs.SetFloat(parameterName, sliderValue);
    }

    private void LoadVolume(string parameterName)
    {
        float savedValue = PlayerPrefs.GetFloat(parameterName, 1.0f);
        SetMixerVolume(parameterName, savedValue);
    }

    public float GetSavedVolume(string parameterName)
    {
        return PlayerPrefs.GetFloat(parameterName, 0.75f);
    }
}

// 4. 개별 사운드 컴포넌트들 (메뉴에 나타나도록 설정)

[AddComponentMenu("Sounds/Sound_WoodCut")]
public class Sound_WoodCut : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.WoodCut); }

[AddComponentMenu("Sounds/Sound_StoneMine")]
public class Sound_StoneMine : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.StoneMine); }

[AddComponentMenu("Sounds/Sound_WeaponSwing")]
public class Sound_WeaponSwing : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.WeaponSwing); }

[AddComponentMenu("Sounds/Sound_PlayerHit")]
public class Sound_PlayerHit : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.PlayerHit); }

[AddComponentMenu("Sounds/Sound_Eat")]
public class Sound_Eat : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.Eat); }

[AddComponentMenu("Sounds/Sound_HerbCollect")]
public class Sound_HerbCollect : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.HerbCollect); }

[AddComponentMenu("Sounds/Sound_MapMove")]
public class Sound_MapMove : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.MapMove); }

[AddComponentMenu("Sounds/Sound_MonsterRoar")]
public class Sound_MonsterRoar : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.MonsterRoar); }

[AddComponentMenu("Sounds/Sound_Campfire")]
public class Sound_Campfire : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.Campfire); }

[AddComponentMenu("Sounds/Sound_GameOver")]
public class Sound_GameOver : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.GameOver); }

[AddComponentMenu("Sounds/Sound_GameClear")]
public class Sound_GameClear : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.GameClear); }

[AddComponentMenu("Sounds/Sound_Button")]
public class Sound_Button : MonoBehaviour { public void Play() => SoundManager.Instance.PlaySFX(SFXType.Button); }