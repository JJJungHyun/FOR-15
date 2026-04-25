using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/Activate Device")]
public class ToggleObjectEffect : UsableItemEffect
{
    [SerializeField] private string targetDeviceID;
    [SerializeField] private GameObject devicePrefab; 

    public override void ExecuteEffect(UsableItem parentItem, Character character)
    {
        if (character.TryGetDevice(targetDeviceID, out IActivatableDevice device))
        {
            device.Activate();
        }

        else if (devicePrefab != null)
        {
            GameObject go = Instantiate(devicePrefab, character.transform);
            IActivatableDevice newDevice = go.GetComponent<IActivatableDevice>();

            if (newDevice != null)
            {
                character.RegisterDevice(targetDeviceID, newDevice);
                newDevice.Activate();
            }
        }
    }

    public override string GetDescription() => $"{targetDeviceID} 장치를 활성화";
}
