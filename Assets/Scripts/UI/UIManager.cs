using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 어디서나 UIManager.Instance로 접근할 수 있게 해주는 싱글톤 변수
    public static UIManager Instance { get; private set; }

    [Header("UI Canvases")]
    [SerializeField] private GameObject furnaceCanvas; // 씬에 있는 화로 UI 오브젝트를 여기에 드래그앤드롭

    private void Awake()
    {
        // 싱글톤 초기화 및 예외 처리
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 화로 UI를 켜고 끄는 함수
    public void ToggleFurnaceUI()
    {
        if (furnaceCanvas != null)
        {
            bool isActive = furnaceCanvas.activeSelf;
            furnaceCanvas.SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("UIManager: 화로 UI Canvas가 인스펙터에 등록되지 않았습니다!");
        }
    }

    // 화로 UI를 강제로 끄는 함수 (플레이어가 멀어졌을 때 사용)
    public void CloseFurnaceUI()
    {
        if (furnaceCanvas != null)
        {
            furnaceCanvas.SetActive(false);
        }
    }
}