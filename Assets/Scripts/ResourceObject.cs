using UnityEngine;
using System.Collections.Generic;

public class ResourceObject : MonoBehaviour, IResourceHarvestable
{
    [Header("데이터 설정")]
    [SerializeField] private ObjectData _objectData;

    [Header("자원 설정")]
    [SerializeField] private ToolType _requiredTool = ToolType.Axe;
    [SerializeField] private float _maxHp = 3f;
    [SerializeField] private float _currentHp;

    [Header("도구 배율")]
    [SerializeField] private float _matchingToolDamage = 1f;
    [SerializeField] private float _wrongToolDamage = 0f;
    [SerializeField] private float _bareHandDamage = 0.5f;

    private ObjectFaller _faller;
    private ObjectFader _fader;
    private bool _isDestroyed = false;

    private ObjectSize _currentSize;
    private List<ObjDropItem> _currentDropTable;

    void Awake()
    {
        _currentHp = _maxHp;
        _faller = GetComponent<ObjectFaller>();
        _fader = GetComponent<ObjectFader>();
    }

    void Start()
    {
        InitializeObjectSize();
    }

    private void InitializeObjectSize()
    {
        if (_objectData == null || _objectData.sizeSettings.Count == 0) return;

        int randomIndex = Random.Range(0, _objectData.sizeSettings.Count);
        SizeDependentDrop selectedSetting = _objectData.sizeSettings[randomIndex];

        _currentSize = selectedSetting.size;
        _currentDropTable = selectedSetting.dropTable;

        float randomScale = Random.Range(selectedSetting.minScale, selectedSetting.maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);
    }

    public void Harvest(float damage, ToolType toolType, Vector2 attackerPos)
    {
        if (_isDestroyed) return;

        float finalDamage = 0f;

        if (toolType == _requiredTool) finalDamage = _matchingToolDamage;
        else if (toolType == ToolType.None) finalDamage = _bareHandDamage;
        else finalDamage = _wrongToolDamage;

        if (finalDamage > 0)
        {
            _currentHp -= finalDamage;
        }

        if (_currentHp <= 0)
        {
            BreakObject();
        }
    }

    private void BreakObject()
    {
        _isDestroyed = true;

        if (TryGetComponent<Collider2D>(out var col)) col.enabled = false;

        if (_faller != null) _faller.Fall();
        if (_fader != null) _fader.FadeOut(_faller != null ? _faller.FallTime : 0f);

        DropItems();
    }

    private void DropItems()
    {
        if (_objectData == null || _objectData.defaultItemPrefab == null || _currentDropTable == null) return;

        foreach (var dropData in _currentDropTable)
        {
            if (dropData.itemData == null) continue;

            if (Random.value <= dropData.dropChance)
            {
                int dropCount = Random.Range(dropData.minAmount, dropData.maxAmount + 1);

                for (int i = 0; i < dropCount; i++)
                {
                    GameObject newItem = Instantiate(_objectData.defaultItemPrefab, transform.position, Quaternion.identity);

                    ItemObject itemObj = newItem.GetComponent<ItemObject>();
                    if (itemObj != null)
                    {
                        itemObj.SetItemData(dropData.itemData, 1);
                    }

                    ItemPopUp popUp = newItem.GetComponent<ItemPopUp>();
                    if (popUp != null)
                    {
                        popUp.PlayDropAnimation();
                    }
                }
            }
        }
    }
}