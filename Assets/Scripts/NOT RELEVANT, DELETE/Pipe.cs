using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


//TODO: NOT WORKING, DELETE THIS SCRIPT
public class Pipe : MonoBehaviour
{
    [SerializeField] private Transform exitPipe;
    [SerializeField] private Transform entryPipe;
    // [SerializeField] private float exitDelay = 0.5f;
    [SerializeField] private float entryDelay = 0.5f;
    [SerializeField] private InputAction crouchAction;
    public Vector3 entryPoint = Vector2.down;
    public Vector3 exitPoint = Vector2.up;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (crouchAction.triggered)
            {
                Debug.Log("Player entered a pipe");
                MarioEvents.OnMarioEnteredPipe?.Invoke();
            }
        }
    }
    
    private IEnumerator Enter(Transform player)
    {
        // player.position = entryPoint;
        Vector3 enteredPosition = player.position + entryPoint;
        yield return new WaitForSeconds(entryDelay);
        player.position = enteredPosition;
        
    }
}