using CharacterStats;
using UnityEngine;

public class StatPanel : MonoBehaviour
{
    [SerializeField] private StatDisplay[] statDisplays;
    [SerializeField] private string[] statNames;

    public void SetStats(params Stat[] charStats)
    {
        for (int i = 0; i < statDisplays.Length; i++)
        {
            if (i < charStats.Length)
            {
                statDisplays[i].gameObject.SetActive(true);
                statDisplays[i].Init(charStats[i], statNames[i]);
            }
            else
            {
                statDisplays[i].gameObject.SetActive(false);
            }
        }
    }
}