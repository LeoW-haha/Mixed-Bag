using UnityEngine;
using System.Collections;
using TMPro;


public class FuelRequestSpawner : MonoBehaviour
{
    
    // Update is called once per frame
    private void Update()
    {
        
    }
    [Header("Countdown Display")]
    public TextMeshProUGUI timerText;
    [Header("Fuel Request Timing")]
    [SerializeField] private float requestCooldown = 5f; // how long it disappears after success

    [Header("Fuel Barrel Sprites")]
    public Sprite redBarrel;
    public Sprite greenBarrel;
    public Sprite blackBarrel;
    public Sprite whiteBarrel;

    [Header("Fuel Timer")]
    [SerializeField] private float fuelTimeLimit = 7f; // customizable in inspector

    private SpriteRenderer spriteRenderer;
    private string selectedFuelTag;
    private bool fuelDelivered = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SpawnRandomFuelRequest();
        StartCoroutine(FuelTimer());
    }

    public void SpawnRandomFuelRequest()
    {
        string[] fuelTags = { "Red", "Green", "Black", "White" };
        selectedFuelTag = fuelTags[Random.Range(0, fuelTags.Length)];

        Sprite fuelSprite = GetSpriteByTag(selectedFuelTag);
        spriteRenderer.sprite = fuelSprite;

        GameUIManager uiManager = FindObjectOfType<GameUIManager>();
        if (uiManager != null)
        {
            uiManager.ShowFeedback($"Refuel Needed: {selectedFuelTag} Barrel", 3f);
        }
    }

    private Sprite GetSpriteByTag(string tag)
    {
        switch (tag)
        {
            case "Red": return redBarrel;
            case "Green": return greenBarrel;
            case "Black": return blackBarrel;
            case "White": return whiteBarrel;
            default: return null;
        }
    }

    public string GetRequiredFuelTag()
    {
        return selectedFuelTag;
    }

    public void DeliverFuel(string deliveredTag)
    {
        if (deliveredTag == selectedFuelTag && !fuelDelivered)
        {
            fuelDelivered = true;

            // Disable sprite + timer temporarily
            spriteRenderer.enabled = false;
            if (timerText != null) timerText.text = "";

            GameUIManager uiManager = FindObjectOfType<GameUIManager>();
            if (uiManager != null)
            {
                uiManager.ShowFeedback("Correct Fuel Delivered!", 2f);
            }

            StopAllCoroutines(); // stop the fail timer
            StartCoroutine(RespawnRequestAfterDelay());
        }
        else
        {
            GameUIManager uiManager = FindObjectOfType<GameUIManager>();
            if (uiManager != null)
            {
                uiManager.ShowFeedback("Wrong Fuel! Try again!", 2f);
            }
        }
    }

    private IEnumerator RespawnRequestAfterDelay()
    {
        yield return new WaitForSeconds(requestCooldown);

        fuelDelivered = false;
        spriteRenderer.enabled = true;

        SpawnRandomFuelRequest(); // respawn a new random request
        StartCoroutine(FuelTimer());
    }


    private IEnumerator FuelTimer()
    {
        float timer = fuelTimeLimit;

        while (timer > 0 && !fuelDelivered)
        {
            timer -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(timer).ToString();
            }

            yield return null;
        }

        if (!fuelDelivered)
        {
            if (timerText != null)
                timerText.text = "0";

            GameUIManager uiManager = FindObjectOfType<GameUIManager>();
            if (uiManager != null)
            {
                uiManager.ShowFeedback("You failed to refuel in time!", 3f);
            }

            GameManagerJasper.Instance.EndLevel();
        }
        else
        {
            if (timerText != null)
                timerText.text = "";
        }
    }


}




