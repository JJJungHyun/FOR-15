using UnityEngine;

public class EscapeZone : MonoBehaviour
{
    public GuideArrow guideArrow;

    private void OnTriggerEnter2D(Collider2D foreign)
    {
        if (foreign.CompareTag("Player"))
        {
            guideArrow.SetInsideZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D foreign)
    {
        if (foreign.CompareTag("Player"))
        {
            guideArrow.SetInsideZone(false);
        }
    }
}