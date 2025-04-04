using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    private ItemSpawner itemSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemSpawner = GameObject.Find("Item Spawner").GetComponent<ItemSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Spawner Stopped");
        if (true) {
            itemSpawner.spawnOn = !itemSpawner.spawnOn;
        }
    }
}
