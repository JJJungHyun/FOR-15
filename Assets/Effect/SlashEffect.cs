using UnityEngine;

public class SlashEffect : MonoBehaviour
{

    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private Transform slashEffectPoint;
    [SerializeField] private float slashEffectLifeTime = 0.25f;

    public void SpawnSlashEffect(Vector3 dir)
    {
        if (slashEffectPrefab == null || slashEffectPoint == null) return;

        if (dir.sqrMagnitude <= 0.01f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject effect = Instantiate(
            slashEffectPrefab,
            slashEffectPoint.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        Destroy(effect, slashEffectLifeTime);
    }
}
