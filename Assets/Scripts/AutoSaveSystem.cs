using UnityEngine;
using System.Collections;

public class AutoSaveSystem : MonoBehaviour
{
    public float saveInterval = 60f; // 60초마다 저장

    private void Start()
    {
        StartCoroutine(AutoSaveCoroutine());
    }

    IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(saveInterval);
            if (GameSaveManager.Instance != null)
            {
                GameSaveManager.Instance.SaveGame();
            }
        }
    }
}