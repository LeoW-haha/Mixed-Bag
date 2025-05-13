using UnityEngine;

public class PlayerFuel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Fuel Settings")]
    public string currentFuelTag = ""; // Holds the fuel type: "Red", "Green", etc.

    // Optionally show the currently equipped fuel in UI
    public GameUIManager uiManager;

    private void Start()
    {
        if (uiManager == null)
            uiManager = FindObjectOfType<GameUIManager>();
    }

    public void EquipFuel(string fuelTag)
    {
        currentFuelTag = fuelTag;

        if (uiManager != null)
        {
            uiManager.ShowFeedback($"Picked up {fuelTag} Fuel", 2f);
            uiManager.SetEquippedFuel(fuelTag); // Shows correct sprite in UI
        }
    }

    public void ClearFuel()
    {
        currentFuelTag = "";

        if (uiManager != null)
        {
            uiManager.SetEquippedFuel(""); // Clear or hide UI fuel icon
        }
    }

    public bool HasFuel()
    {
        return !string.IsNullOrEmpty(currentFuelTag);
    }

    public string GetFuelTag()
    {
        return currentFuelTag;
    }
}

