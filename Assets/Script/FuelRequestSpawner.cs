using UnityEngine;
using System.Collections;
using TMPro;

public class FuelRequestSpawner : MonoBehaviour
{
    [Header("Countdown Display")]
    public TextMeshProUGUI timerText;

    [Header("Fuel Request Timing")]
    [SerializeField] private float requestCooldown = 5f; // Time before next request appears
    [SerializeField] private float fuelTimeLimit = 7f;   // Time allowed to deliver fuel

    [Header("Fuel Barrel Sprites")]
    public Sprite redBarrel;
    public Sprite greenBarrel;
    public Sprite blackBarrel;
    public Sprite whiteBarrel;

    private SpriteRenderer spriteRenderer;
    private string selectedFuelTag;
    private bool fuelDelivered = false;
    private GameUIManager uiManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiManager = FindObjectOfType<GameUIManager>();

        if (spriteRenderer == null)
            Debug.LogError("[FuelRequestSpawner] Missing SpriteRenderer!");

        if (timerText == null)
            Debug.LogWarning("[FuelRequestSpawner] Timer Text not assigned.");

        SpawnRandomFuelRequest();
        StartCoroutine(FuelTimer());
    }

    public string GetRequiredFuelTag() => selectedFuelTag;

    public void DeliverFuel(string deliveredTag)
    {
        if (fuelDelivered) return;

        if (deliveredTag == selectedFuelTag)
        {
            fuelDelivered = true;
            spriteRenderer.enabled = false;
            if (timerText != null) timerText.text = "";

            uiManager?.ShowFeedback("Correct Fuel Delivered!", 2f);

            StopAllCoroutines();
            StartCoroutine(RespawnRequestAfterDelay());
        }
        else
        {
            uiManager?.ShowFeedback("Wrong Fuel! Try again!", 2f);
        }
    }

    private void SpawnRandomFuelRequest()
    {
        string[] fuelTags = { "Red", "Green", "Black", "White" };
        selectedFuelTag = fuelTags[Random.Range(0, fuelTags.Length)];

        spriteRenderer.sprite = GetSpriteByTag(selectedFuelTag);
        spriteRenderer.enabled = true;
        if (timerText != null) timerText.gameObject.SetActive(true);

        uiManager?.ShowFeedback($"Refuel Needed: {selectedFuelTag} Barrel", 3f);
    }

    private Sprite GetSpriteByTag(string tag)
    {
        return tag switch
        {
            "Red" => redBarrel,
            "Green" => greenBarrel,
            "Black" => blackBarrel,
            "White" => whiteBarrel,
            _ => null,
        };
    }

    private IEnumerator FuelTimer()
    {
        float timer = fuelTimeLimit;

        while (timer > 0f && !fuelDelivered)
        {
            timer -= Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(timer).ToString();

            yield return null;
        }

        if (!fuelDelivered)
        {
            if (timerText != null) timerText.text = "0";

            uiManager?.ShowFeedback("REFUEL USING BARRELS!", 3f);
            yield return new WaitForSeconds(0.1f); // Ensures feedback displays
            GameManagerJasper.Instance.MarkFuelFailure();
            GameManagerJasper.Instance.FailLevel("REFUEL USING BARRELS!");
        }
        else if (timerText != null)
        {
            timerText.text = "";
        }
    }

    private IEnumerator RespawnRequestAfterDelay()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(false);

        yield return new WaitForSeconds(requestCooldown);

        fuelDelivered = false;
        SpawnRandomFuelRequest();
        StartCoroutine(FuelTimer());
    }
}
