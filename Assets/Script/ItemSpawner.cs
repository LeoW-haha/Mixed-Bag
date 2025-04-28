using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items;      
    public Vector2 spawnPos;      
    public float spawnTime = 1f;

    void Start()
    {
        InvokeRepeating("spawn", 0f, spawnTime);
    }

    void spawn()
    {
        GameObject randomise = items[Random.Range(0, items.Length)];
        GameObject newItem = Instantiate(randomise, spawnPos, Quaternion.identity);
    }

    void Update()
    {
        
    }
}
