using System.Collections;
using UnityEngine;

public class TranspertyBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    private SpriteRenderer spriteRenderer;
    private float timer;
    private bool isTransparent = false;
    private const float flashInterval = 0.033f; // Approximately 2 frames at 60 FPS

    // Called when the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get the SpriteRenderer component from the GameObject
        spriteRenderer = animator.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer not found on the GameObject.");
            return;
        }

        // Initialize timer
        timer = 0f;
        isTransparent = false;
        SetSpriteAlpha(1f); // Ensure sprite starts fully opaque
    }

    // Called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (spriteRenderer == null)
            return;

        // Increment timer by the time elapsed since the last frame
        timer += Time.deltaTime;

        // Check if it's time to toggle transparency
        if (timer >= flashInterval)
        {
            // Toggle transparency
            isTransparent = !isTransparent;
            SetSpriteAlpha(isTransparent ? 0.5f : 1f); // Adjust alpha values as needed

            // Reset timer
            timer = 0f;
        }
    }

    // Called when the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (spriteRenderer == null)
            return;

        // Ensure the sprite is fully opaque when exiting
        SetSpriteAlpha(1f);
    }

    // Helper method to set the sprite's alpha value
    private void SetSpriteAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
}
