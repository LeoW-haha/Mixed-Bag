using UnityEngine;
using System.Collections;
using TMPro;

public class MapSwitcher : MonoBehaviour
{
    public GameObject truckNormal;
    public GameObject truckAfterLeft;
    public GameObject truckAfterRight;
    public float switchInterval = 10f;
    public TextMeshProUGUI warningPopup;

    private GameObject currentMap;
    private bool turnLeftNext = true;

    void Start()
    {
        truckNormal.SetActive(true);
        truckAfterLeft.SetActive(false);
        truckAfterRight.SetActive(false);

        currentMap = truckNormal;
        StartCoroutine(SwitchMapRoutine());
    }

    IEnumerator SwitchMapRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(switchInterval);

            // Show warning
            string direction = turnLeftNext ? "Left" : "Right";
            warningPopup.text = $"Warning: Turning {direction}!";
            warningPopup.gameObject.SetActive(true);

            yield return new WaitForSeconds(2f);
            warningPopup.gameObject.SetActive(false);

            // Switch maps
            currentMap.SetActive(false);
            if (turnLeftNext)
            {
                truckAfterLeft.SetActive(true);
                currentMap = truckAfterLeft;
            }
            else
            {
                truckAfterRight.SetActive(true);
                currentMap = truckAfterRight;
            }

            // Toggle direction for next time
            turnLeftNext = !turnLeftNext;
        }
    }
}
