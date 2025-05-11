using UnityEngine;

public class PlayerPackagingTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    // Update is called once per frame
    void Update()
    {
        
    }

    public RectTransform inventoryUI; // Ensure this is a RectTransform (Canvas element)
    private Vector3 originalScale;
    private Vector3 originalPosition;

    public Vector3 enlargedScale = new Vector3(1.5f, 1.5f, 1f);
    public Vector3 shiftedPosition = new Vector3(0, 0, 0); // Update in Inspector to match center

    private void Start()
    {
        if (inventoryUI != null)
        {
            originalScale = inventoryUI.localScale;
            originalPosition = inventoryUI.anchoredPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PackageStation") && inventoryUI != null)
        {
            inventoryUI.localScale = enlargedScale;
            inventoryUI.anchoredPosition = shiftedPosition;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PackageStation") && inventoryUI != null)
        {
            inventoryUI.localScale = originalScale;
            inventoryUI.anchoredPosition = originalPosition;
        }
    }
}
