using UnityEngine;

public class FuelSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
        public float fuel = 100f;
        public float lowFuelThreshold = 30f;
        public GameObject fuelNeededIcon; // Reference to the icon object near the station

        private void Update()
        {
            fuel -= Time.deltaTime;

            if (fuel <= lowFuelThreshold)
                fuelNeededIcon.SetActive(true);
            else
                fuelNeededIcon.SetActive(false);
        }

        public void Refuel(float amount)
        {
            fuel += amount;
            if (fuel > 100f) fuel = 100f;
        }
    


    // Update is called once per frame

}
