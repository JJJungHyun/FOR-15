using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RandomMapGenerater : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] groundTile;
    public int width;
    public int height;
    private string seed;

    [Header("오브젝트 설정")]
    public GameObject[] objectPrefabs; // 나무, 바위 등 프리팹들
    [Range(0, 1)] public float objectSpawnChance = 0.05f; // 생성 확률 (0.05 = 5%)
    public Transform objectParent;

    void Start()
    {
        GeneratePerlinMap();
    }
    void GeneratePerlinMap()
    {
        seed = System.DateTime.Now.ToString();
        if (groundTile == null || groundTile.Length == 0) return;

        Random.InitState(seed.GetHashCode());
        float offset = Random.Range(0f, 99999f);
        float objOffset = Random.Range(0f, 99999f); // 오브젝트용 오프셋

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 1. 지형 타일 생성
                float noiseValue = Mathf.PerlinNoise(x * 0.1f + offset, y * 0.1f + offset);
                int tileIndex = Mathf.Clamp(Mathf.FloorToInt(noiseValue * groundTile.Length), 0, groundTile.Length - 1);
                tilemap.SetTile(new Vector3Int(x, y, 0), groundTile[tileIndex]);
                if (objectPrefabs != null && objectPrefabs.Length > 0)
                {
                    if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
                    {
                        float objDensity = Mathf.PerlinNoise(x * 0.5f + objOffset, y * 0.5f + objOffset);

                        // 0.6f 이상의 밀도 구역에서만 실행
                        if (objDensity > 0.6f)
                        {
                            int targetObjIndex = -1; // 소환할 오브젝트의 인덱스

                            // --- 타일 인덱스에 따른 오브젝트 매칭 로직 ---
                            // 타일 0, 1, 2번 (예: 풀, 흙, 숲 타일)
                            if (tileIndex >= 0 && tileIndex <= 2)
                            {
                                targetObjIndex = UnityEngine.Random.Range(0, 2);
                            }
                            // 타일 3, 4, 5번 (예: 돌, 산, 사막 타일)
                            else if (tileIndex >= 3 && tileIndex <= 5)
                            {
                                // 오브젝트 배열 1, 2, 3번 중 랜덤 소환
                                targetObjIndex = UnityEngine.Random.Range(2, 4);
                            }

                            // 매칭되는 오브젝트가 있고, 최종 확률에 당첨되면 소환
                            if (targetObjIndex != -1 && UnityEngine.Random.value < objectSpawnChance)
                            {
                                SpawnObject(x, y, targetObjIndex);
                            }
                        }
                    }
                }
            }
            
        }
    }

    void SpawnObject(int x, int y, int index)
    {
        Vector3 tileWorldPos = tilemap.CellToWorld(new Vector3Int(x, y, 0));

        // 1. 위치 흔들기 (Jitter)
        float jitterX = UnityEngine.Random.Range(-0.2f, 0.2f);
        float jitterY = UnityEngine.Random.Range(-0.2f, 0.2f);
        Vector3 spawnPos = tileWorldPos + new Vector3(0.5f + jitterX, 0.5f + jitterY, 0);

        // 2. [핵심] 겹침 방지 체크 (OverlapCircle)
        // 반지름 0.4f 안에 이미 다른 오브젝트(Collider2D)가 있는지 확인합니다.
        // 이 기능을 쓰려면 나무/바위 프리팹에 'Circle Collider 2D'가 붙어 있어야 합니다.
        Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.4f);

        if (hit == null) // 주변에 아무도 없을 때만 소환!
        {
            GameObject obj = Instantiate(objectPrefabs[index], spawnPos, Quaternion.identity);
            if (objectParent != null) obj.transform.parent = objectParent;
        }
    }
}

