using UnityEngine;

public class SpawnTrigger : MonoBehaviour, IInteractable
{
    private Notifier notifier;
    public bool CanInteract() {
        return true;
    }
    public void Interact() {
        if (itemSpawner.spawnOn) {
            notifier.Notify("Spawn Turned Off");
        } else {
            notifier.Notify("Spawn Turned On");
        }
        itemSpawner.spawnOn = !itemSpawner.spawnOn;
    }
    private ItemSpawner itemSpawner;
    public string buttonID {get; private set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonID ??= GlobalHelper.GenerateUniqueID(gameObject);
        itemSpawner = GameObject.Find("Item Spawner").GetComponent<ItemSpawner>();
        notifier = GameObject.Find("NotificationHolder").GetComponent<Notifier>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}