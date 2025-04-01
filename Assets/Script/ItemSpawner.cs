using UnityEngine;

public class ItemSpawner : MonoBehaviour
{

    public GameObject itemPrefab;    
    public Sprite[] items;      
    public Vector2 spawnPos;      
    public float spawnTime = 15f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("spawn", 0f, spawnTime);
    }

    void spawn()
    {
        GameObject newItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);

        Sprite randomise = items[Random.Range(0, items.Length)];

        SpriteRenderer s = newItem.GetComponent<SpriteRenderer>();
        if (s != null)
        {
            s.sprite = randomise;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
