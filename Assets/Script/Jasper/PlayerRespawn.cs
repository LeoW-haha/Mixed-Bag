using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float deathAnimationDuration = 2f;
    private Vector3 startPosition;
    private Vector3 deathPosition;
    private PlayerAnimator playerAnimator;
    private Rigidbody2D rb;
    private bool isDead = false;

    private void Start()
    {
        // Store the initial position as the respawn point
        startPosition = transform.position;
        playerAnimator = GetComponent<PlayerAnimator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !isDead)
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        isDead = true;
        deathPosition = transform.position;
        
        // Immediately stop all movement
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;  // Disable physics simulation
        }

        // Disable player movement
        PlayerCtrlJasper playerCtrl = GetComponent<PlayerCtrlJasper>();
        if (playerCtrl != null)
        {
            playerCtrl.enabled = false;
        }

        // Play death animation
        if (playerAnimator != null)
        {
            playerAnimator.PlayDeathAnimation();
        }

        // Wait for animation
        yield return new WaitForSeconds(deathAnimationDuration);

        // Teleport to spawn
        transform.position = startPosition;
        
        // Re-enable physics and movement
        if (rb != null)
        {
            rb.simulated = true;
        }
        
        if (playerCtrl != null)
        {
            playerCtrl.enabled = true;
        }

        isDead = false;
    }
} 