using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepOnLoad : MonoBehaviour
{
    private static KeepOnLoad persistentPlayer;

    [SerializeField] private bool destroyOnTitleScene = true;
    [SerializeField] private string titleSceneName = "TitleScene";

    private void Awake()
    {
        if (CompareTag("Player"))
        {
            if (persistentPlayer != null && persistentPlayer != this)
            {
                Destroy(gameObject);
                return;
            }

            persistentPlayer = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (persistentPlayer == this)
        {
            persistentPlayer = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (CompareTag("Player") && destroyOnTitleScene && scene.name == titleSceneName)
        {
            Destroy(gameObject);
        }
    }

    public static void DestroyPersistentPlayer()
    {
        if (persistentPlayer != null)
        {
            Destroy(persistentPlayer.gameObject);
        }
    }
}