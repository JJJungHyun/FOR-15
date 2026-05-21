using UnityEngine;

public interface IActivatableDevice
{
    string DeviceID { get; }
    void Activate();
    void Deactivate();
}