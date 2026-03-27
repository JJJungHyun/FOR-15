using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "New Item Database", menuName = "Items/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<Item> items = new List<Item>();
    private Dictionary<string, Item> _itemCache;

    private void OnEnable()
    {
        InitializeCache();
#if UNITY_EDITOR
        EditorApplication.projectChanged -= LoadItems;
        EditorApplication.projectChanged += LoadItems;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.projectChanged -= LoadItems;
#endif
    }

    private void InitializeCache()
    {
        _itemCache = new Dictionary<string, Item>();
        if (items == null) return;

        foreach (var item in items)
        {
            if (item != null && !string.IsNullOrEmpty(item.ID))
                _itemCache[item.ID] = item;
        }
    }

    public Item GetItemReference(string itemID)
    {
        if (_itemCache == null || _itemCache.Count == 0) InitializeCache();
        return _itemCache.TryGetValue(itemID, out var item) ? item : null;
    }

    public Item GetItemCopy(string itemID)
    {
        var item = GetItemReference(itemID);
        return item != null ? item.GetCopy() : null;
    }

#if UNITY_EDITOR
    private void OnValidate() => LoadItems();

    private void LoadItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:Item");
        items = new List<Item>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Item asset = AssetDatabase.LoadAssetAtPath<Item>(path);
            if (asset != null) items.Add(asset);
        }

        EditorUtility.SetDirty(this);
        InitializeCache();
    }
#endif
}
