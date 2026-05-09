using UnityEngine;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Base Settings")]
    public string enemyName;
    public EnemyData enemyData;
    public GameObject itemPrefab;

    [Header("Live Stats")]
    protected float currentHp;
    protected EnemyData.EnemySettings settings;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        settings = enemyData.GetSettings(enemyName);
        if (settings != null) currentHp = settings.hp;
    }

    public virtual void TakeDamage(float damage, Vector2 attackerPosition)
    {
        currentHp -= damage;
        Debug.Log($"{enemyName} HP: {currentHp}");

        if (currentHp <= 0) Die();
    }

    protected virtual void Die()
    {
        TryDropItem();
        Destroy(gameObject);
    }

    private void TryDropItem()
    {
        if (settings == null) return;
        foreach (var drop in settings.drops)
        {
            if (Random.Range(0f, 100f) <= drop.dropRate)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                GameObject go = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                if (go.TryGetComponent(out ItemObject itemObj))
                    itemObj.SetItemData(drop.itemSO, amount);

                if (go.TryGetComponent(out ItemPopUp anim))
                    anim.PlayDropAnimation();
            }
        }
    }
}