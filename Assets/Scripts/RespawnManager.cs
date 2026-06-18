using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
    private const string TemplateSuffix = "_RespawnTemplate";
    private static RespawnManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Schedule(GameObject source, float delay)
    {
        if (source == null) return;

        GetOrCreate().QueueRespawn(source, Mathf.Max(0f, delay));
    }

    private static RespawnManager GetOrCreate()
    {
        if (instance != null) return instance;

        instance = FindFirstObjectByType<RespawnManager>();
        if (instance != null) return instance;

        GameObject managerObject = new GameObject(nameof(RespawnManager));
        instance = managerObject.AddComponent<RespawnManager>();
        return instance;
    }

    private void QueueRespawn(GameObject source, float delay)
    {
        Scene sourceScene = source.scene;
        string sceneName = sourceScene.name;
        string originalName = CleanName(source.name);
        Transform originalTransform = source.transform;
        Transform originalParent = originalTransform.parent;
        Vector3 position = originalTransform.position;
        Quaternion rotation = originalTransform.rotation;
        Vector3 localScale = originalTransform.localScale;

        GameObject template = Instantiate(source, position, rotation);
        template.name = originalName + TemplateSuffix;
        template.hideFlags = HideFlags.HideInHierarchy;
        template.transform.SetParent(transform, true);
        template.SetActive(false);

        StartCoroutine(RespawnRoutine(
            template,
            originalName,
            sceneName,
            originalParent,
            position,
            rotation,
            localScale,
            delay));
    }

    private IEnumerator RespawnRoutine(
        GameObject template,
        string originalName,
        string sceneName,
        Transform originalParent,
        Vector3 position,
        Quaternion rotation,
        Vector3 localScale,
        float delay)
    {
        float elapsed = 0f;

        while (elapsed < delay)
        {
            if (!IsSceneLoaded(sceneName))
            {
                if (template != null) Destroy(template);
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Scene targetScene = SceneManager.GetSceneByName(sceneName);
        if (!targetScene.IsValid() || !targetScene.isLoaded || template == null)
        {
            if (template != null) Destroy(template);
            yield break;
        }

        GameObject spawned = Instantiate(template);
        spawned.name = originalName;
        spawned.hideFlags = HideFlags.None;

        Transform spawnedTransform = spawned.transform;
        if (originalParent != null)
        {
            spawnedTransform.SetParent(originalParent, false);
        }
        else
        {
            SceneManager.MoveGameObjectToScene(spawned, targetScene);
        }

        spawnedTransform.position = position;
        spawnedTransform.rotation = rotation;
        spawnedTransform.localScale = localScale;
        spawned.SetActive(true);

        Destroy(template);
    }

    private static bool IsSceneLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.IsValid() && scene.isLoaded;
    }

    private static string CleanName(string sourceName)
    {
        return sourceName
            .Replace("(Clone)", string.Empty)
            .Replace(TemplateSuffix, string.Empty)
            .Trim();
    }
}