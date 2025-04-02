using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    
    public GameObject[] items;      
    public Vector2 spawnPos;      
    public float spawnTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("spawn", 0f, spawnTime);
    }

    void spawn()
    {
        GameObject randomise = items[Random.Range(0, items.Length)];
        GameObject newItem = Instantiate(randomise, spawnPos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
