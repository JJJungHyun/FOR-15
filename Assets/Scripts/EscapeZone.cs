using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class EscapeZone : MonoBehaviour
{
    public GuideArrow guideArrow;

    [SerializeField] private string _nextSceneName = "TitleScene";
    [SerializeField] private string _cutsceneName = "GameClearCutscene";

    [Header("Random Spawn")]
    [SerializeField] private bool randomizeInEscapeScene = true;
    [SerializeField] private string escapeSceneName = "Escape";
    [SerializeField] private Tilemap spawnTilemap;
    [SerializeField, Min(0)] private int tilePadding = 3;
    [SerializeField, Min(1)] private int maxSpawnAttempts = 500;
    [SerializeField, Min(0f)] private float obstacleCheckRadius = 2f;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    private bool _isCleared = false;

    private void Start()
    {
        SyncGuideArrowTarget();

        if (ShouldRandomizePosition())
        {
            StartCoroutine(RandomizePositionAfterTilemapReady());
        }
    }

    private void OnTriggerEnter2D(Collider2D foreign)
    {
        if (!foreign.CompareTag("Player"))
        {
            return;
        }

        if (guideArrow != null)
        {
            guideArrow.SetInsideZone(true);
        }

        if (!_isCleared)
        {
            _isCleared = true;
            HandleGameClear();
        }
    }

    private void OnTriggerExit2D(Collider2D foreign)
    {
        if (!foreign.CompareTag("Player"))
        {
            return;
        }

        if (guideArrow != null)
        {
            guideArrow.SetInsideZone(false);
        }
    }

    private bool ShouldRandomizePosition()
    {
        return randomizeInEscapeScene && SceneManager.GetActiveScene().name == escapeSceneName;
    }

    private IEnumerator RandomizePositionAfterTilemapReady()
    {
        yield return null;

        Tilemap tilemap = null;
        for (int i = 0; i < 10; i++)
        {
            tilemap = FindSpawnTilemap();
            if (tilemap != null && HasAnyTile(tilemap))
            {
                break;
            }

            yield return null;
        }

        if (tilemap == null || !HasAnyTile(tilemap))
        {
            Debug.LogWarning("[EscapeZone] Tilemap was not found. Keeping the scene position.");
            yield break;
        }

        Physics2D.SyncTransforms();

        if (TryGetRandomSpawnPosition(tilemap, out Vector3 spawnPosition))
        {
            transform.position = spawnPosition + spawnOffset;
            SyncGuideArrowTarget();
            Debug.Log($"[EscapeZone] Spawned at {transform.position}");
        }
        else
        {
            Debug.LogWarning("[EscapeZone] No valid random tile was found. Keeping the scene position.");
        }
    }
    private Tilemap FindSpawnTilemap()
    {
        if (spawnTilemap != null)
        {
            return spawnTilemap;
        }

        RandomMapGenerater generator = FindAnyObjectByType<RandomMapGenerater>();
        if (generator != null && generator.tilemap != null)
        {
            return generator.tilemap;
        }

        GameObject tilemapObject = GameObject.Find("Tilemap");
        if (tilemapObject != null && tilemapObject.TryGetComponent(out Tilemap namedTilemap))
        {
            return namedTilemap;
        }

        return FindAnyObjectByType<Tilemap>();
    }

    private bool HasAnyTile(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(cell))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryGetRandomSpawnPosition(Tilemap tilemap, out Vector3 spawnPosition)
    {
        tilemap.CompressBounds();
        BoundsInt bounds = GetSpawnBounds(tilemap.cellBounds);
        if (bounds.size.x <= 0 || bounds.size.y <= 0)
        {
            spawnPosition = transform.position;
            return false;
        }

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3Int cell = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );

            if (IsValidSpawnCell(tilemap, cell, out spawnPosition))
            {
                return true;
            }
        }

        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (IsValidSpawnCell(tilemap, cell, out spawnPosition))
            {
                return true;
            }
        }

        spawnPosition = transform.position;
        return false;
    }

    private BoundsInt GetSpawnBounds(BoundsInt originalBounds)
    {
        int minX = originalBounds.xMin + tilePadding;
        int maxX = originalBounds.xMax - tilePadding;
        int minY = originalBounds.yMin + tilePadding;
        int maxY = originalBounds.yMax - tilePadding;
        int sizeZ = Mathf.Max(1, originalBounds.size.z);

        if (minX >= maxX || minY >= maxY)
        {
            return originalBounds;
        }

        return new BoundsInt(minX, minY, originalBounds.zMin, maxX - minX, maxY - minY, sizeZ);
    }

    private bool IsValidSpawnCell(Tilemap tilemap, Vector3Int cell, out Vector3 spawnPosition)
    {
        spawnPosition = tilemap.GetCellCenterWorld(cell);
        spawnPosition.z = transform.position.z;

        return tilemap.HasTile(cell) && !IsBlocked(tilemap, spawnPosition);
    }

    private bool IsBlocked(Tilemap tilemap, Vector3 worldPosition)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPosition, obstacleCheckRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null)
            {
                continue;
            }

            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                continue;
            }

            if (hit.gameObject == tilemap.gameObject)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private void SyncGuideArrowTarget()
    {
        if (guideArrow != null)
        {
            guideArrow.SetTarget(transform);
        }
    }

    private void HandleGameClear()
    {
        Debug.Log("Escape succeeded.");

        // ★ 탈출에 성공했으므로 흐르고 있던 60초 타이머를 찾아 멈춰 세웁니다.
        EscapeTimer timer = FindAnyObjectByType<EscapeTimer>();
        if (timer != null) timer.StopTimer();

        if (CutSceneManager.Instance != null)
        {
            // ★ 중요: 컷씬 이름 뒤에 이동할 다음 씬 이름(_nextSceneName 즉, "TitleScene")을 함께 넘겨줍니다!
            CutSceneManager.Instance.StartCutscene(_cutsceneName, _nextSceneName);
        }
    }
}