using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(menuName = "Items/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private string id;
    public string ID => id;

    public string ItemName;
    public Sprite Icon;

    [Range(1, 999)]
    public int MaximumStacks = 1;

    // 생존 점수 시스템 대비
    public int SurvivalScore;

    protected static readonly StringBuilder sb = new StringBuilder();

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
    }
#endif

    // 외부 데이터(JSON 등) 연동 대비용 가상 메서드
    public virtual void LoadFromRawData(string name, int maxStack, int score)
    {
        this.ItemName = name;
        this.MaximumStacks = maxStack;
        this.SurvivalScore = score;
    }
    public virtual void Use(Character c) { }

    public virtual Item GetCopy() => Instantiate(this);

    public virtual void Destroy() => Resources.UnloadUnusedAssets();

    public virtual string GetItemType() => "";
    public virtual string GetDescription() => "";
}
