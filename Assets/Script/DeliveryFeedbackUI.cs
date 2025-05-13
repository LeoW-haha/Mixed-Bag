using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeliveryFeedbackUI : MonoBehaviour
{
    public Image resultIcon;
    public Sprite tickSprite;
    public Sprite crossSprite;
    public float displayTime = 2f;

    public void ShowResult(bool success)
    {
        if (resultIcon == null) return;

        resultIcon.sprite = success ? tickSprite : crossSprite;
        resultIcon.enabled = true;

        StopAllCoroutines();
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        resultIcon.enabled = false;
    }
}
