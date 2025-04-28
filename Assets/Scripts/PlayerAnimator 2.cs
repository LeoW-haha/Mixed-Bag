using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastDirection = Vector2.down; // Default facing down
    
    // Animation parameter names
    private readonly string IS_MOVING = "IsMoving";
    private readonly string MOVE_X = "MoveX";
    private readonly string MOVE_Y = "MoveY";
    private readonly string IS_CARRYING = "IsCarrying";
    private readonly string DIE = "Die";
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (animator == null || spriteRenderer == null)
        {
            Debug.LogError("Missing required components on PlayerAnimator!");
            enabled = false;
            return;
        }

        // Set initial direction
        UpdateAnimation(Vector2.zero);
    }
    
    public void UpdateAnimation(Vector2 movement)
    {
        bool isMoving = movement.magnitude > 0.1f;
        
        // Store last direction when moving
        if (isMoving)
        {
            lastDirection = movement.normalized;
        }
        
        // Use last direction when idle, current direction when moving
        Vector2 directionToUse = isMoving ? movement : lastDirection;
        
        // Update Animator parameters
        animator.SetFloat(MOVE_X, directionToUse.x);
        animator.SetFloat(MOVE_Y, directionToUse.y);
        animator.SetBool(IS_MOVING, isMoving);
    }
    
    public void SetCarrying(bool isCarrying)
    {
        animator.SetBool(IS_CARRYING, isCarrying);
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger(DIE);
    }

    public void ResetToIdle()
    {
        // Reset all parameters
        animator.SetBool(IS_MOVING, false);
        animator.SetBool(IS_CARRYING, false);
        animator.SetFloat(MOVE_X, 0);
        animator.SetFloat(MOVE_Y, -1); // Face down by default
        
        // Reset any triggers
        animator.ResetTrigger(DIE);
        
        // Force the animator to return to the default state
        animator.Play("Idle");
        
        // Reset the last direction
        lastDirection = Vector2.down;
    }
} 