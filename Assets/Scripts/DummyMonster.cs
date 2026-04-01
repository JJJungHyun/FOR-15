using UnityEngine;
using TMPro;

// 플레이어 피격&공격 테스트용 더미 몬스터
public class DummyMonster : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHp = 50f;
    private float currentHp;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Visual Debug")]
    [SerializeField] private TextMeshPro hpText; 
    private float lastAttackTime;

    private void Awake()
    {
        currentHp = maxHp;
        UpdateHPText();
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        UpdateHPText(); 

        if (currentHp <= 0)
        {
            Debug.Log("몬스터 파괴");
            Destroy(gameObject);
        }
    }

    private void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = $"{currentHp} / {maxHp}";
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                TryAttack(damageable);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out IDamageable damageable))
        {
            TryAttack(damageable);
        }
    }

    private void TryAttack(IDamageable target)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            target.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }
}