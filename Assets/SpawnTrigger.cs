using UnityEngine;

public class SpawnTrigger : MonoBehaviour, IInteractable
{
    public bool CanInteract() {
        return true;
    }
    public void Interact() {
        Debug.Log("Spawn Toggle");
        itemSpawner.spawnOn = !itemSpawner.spawnOn;
    }
    private ItemSpawner itemSpawner;
    public string buttonID {get; private set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonID ??= GlobalHelper.GenerateUniqueID(gameObject);
        itemSpawner = GameObject.Find("Item Spawner").GetComponent<ItemSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}