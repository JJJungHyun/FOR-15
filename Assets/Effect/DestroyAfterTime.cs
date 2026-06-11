using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.25f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}