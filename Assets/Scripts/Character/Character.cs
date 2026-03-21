using UnityEngine;
using CharacterStats;

public class Character : MonoBehaviour
{
    public Stat Strength;
    public Stat Defense;

    public ClampedStat Health;
    public ClampedStat Hunger;

    private void Awake()
    {
        // 기본값 설정
        Strength = new Stat(10);
        Defense = new Stat(0);

        Health = new ClampedStat(100);
        Hunger = new ClampedStat(100);
    }

    // 이후 추가할 기능
    // 스탯 변경 이벤트 전파 (UI와 연결)
    // 상태 이상 및 버프 관리
    // 피격 및 사망
    // 이후 많아지면 컴포넌트 분리 후 중앙 연결 기능 수행
}