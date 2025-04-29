using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector2 lastDirection = Vector2.down;
    private bool isCarrying = false;
    private bool isDead = false;
    private bool isPlayingDeath = false;
    
    [SerializeField] private float movementThreshold = 0.1f;
    [SerializeField] private float deathAnimationDuration = 0.5f;
    
    // Animation parameter names
    private readonly string IS_MOVING = "IsMoving";
    private readonly string MOVE_X = "MoveX";
    private readonly string MOVE_Y = "MoveY";
    private readonly string IS_CARRYING = "IsCarrying";
    private readonly string DIE = "Die";
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("Missing Animator component on PlayerAnimator!");
            enabled = false;
            return;
        }

        // Set initial direction
        SetDirection(Vector2.down);
    }
    
    private void SetDirection(Vector2 direction)
    {
        // Ensure clean values
        direction = new Vector2(
            Mathf.Round(direction.x),
            Mathf.Round(direction.y)
        ).normalized;
        
        lastDirection = direction;
        
        // Set animator parameters
        animator.SetFloat(MOVE_X, direction.x);
        animator.SetFloat(MOVE_Y, direction.y);
    }
    
    public void UpdateAnimation(Vector2 movement)
    {
        if (isDead || isPlayingDeath) return;

        bool isMoving = movement.magnitude > movementThreshold;
        
        if (isMoving)
        {
            // Determine primary direction
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                // Horizontal movement
                movement = new Vector2(Mathf.Sign(movement.x), 0);
            }
            else
            {
                // Vertical movement
                movement = new Vector2(0, Mathf.Sign(movement.y));
            }
            
            // Only update direction if it's different
            if (movement != lastDirection)
            {
                SetDirection(movement);
            }
        }
        
        // Update movement state
        animator.SetBool(IS_MOVING, isMoving);
    }
    
    public void SetCarrying(bool carrying)
    {
        if (isCarrying == carrying) return; // Don't update if state hasn't changed
        
        isCarrying = carrying;
        animator.SetBool(IS_CARRYING, carrying);
        
        // When starting to carry, ensure we're in the correct direction
        if (carrying)
        {
            SetDirection(lastDirection);
        }
    }

    public void PlayDeathAnimation()
    {
        if (isDead || isPlayingDeath) return;
        isDead = true;
        isPlayingDeath = true;
        
        // Reset other states before playing death
        animator.SetBool(IS_MOVING, false);
        animator.SetBool(IS_CARRYING, false);
        
        // Start death animation sequence
        StartCoroutine(PlayDeathAnimationOnce());
    }

    private IEnumerator PlayDeathAnimationOnce()
    {
        // Play death animation
        animator.SetTrigger(DIE);
        
        // Wait for the animation duration
        yield return new WaitForSeconds(deathAnimationDuration);
        
        // Reset trigger to prevent looping
        animator.ResetTrigger(DIE);
        
        // Disable the animator to freeze on the last frame
        animator.enabled = false;
        
        // Mark death animation as complete
        isPlayingDeath = false;
    }

    public void ResetToIdle()
    {
        // Re-enable animator
        animator.enabled = true;
        
        // Reset the animator completely
        animator.Rebind();
        animator.Update(0f);
        
        // Reset our internal state
        isDead = false;
        isPlayingDeath = false;
        isCarrying = false;
        lastDirection = Vector2.down;
        
        // Set initial parameters
        animator.SetBool(IS_MOVING, false);
        animator.SetBool(IS_CARRYING, false);
        animator.SetFloat(MOVE_X, 0);
        animator.SetFloat(MOVE_Y, -1);
        
        // Reset any triggers
        animator.ResetTrigger(DIE);
    }
} 