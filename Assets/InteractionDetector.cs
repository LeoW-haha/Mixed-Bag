using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown("space")) {
            interactableInRange?.Interact();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract()) {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }
        private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange) {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }

}
