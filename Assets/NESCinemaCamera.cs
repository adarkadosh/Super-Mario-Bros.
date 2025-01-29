using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
public class NESCinemaCamera : MonoBehaviour
{
    
    private float _previousCameraX;
    
    [Header("Player & Camera References")]
    [SerializeField] private Transform player; 
    [SerializeField] private Camera mainCamera;

    [Header("Dead Zone Range")]
    // The smallest & largest dead zone widths (in normalized screen units, 0..1)
    [SerializeField, Range(0f, 1f)] private float minDeadZoneWidth = 0.1f;
    [SerializeField, Range(0f, 1f)] private float maxDeadZoneWidth = 0.25f;

    [Header("Smoothing")]
    [SerializeField] private float deadZoneLerpSpeed = 3f; 

    private CinemachinePositionComposer _framing;
    
    private void Awake()
    {
        // Get the framing transposer from this virtual camera
        var cinemachineCamera = GetComponent<CinemachineCamera>();
        _framing = cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
    }

    private void Update()
    {
        if (player == null || mainCamera == null || _framing == null)
            return;

        // 1) Convert the player's position to viewport space: (0,0) bottom-left -> (1,1) top-right
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(player.position);

        // 2) Check horizontal distance from screen center. Center is at x=0.5
        float distFromCenter = Mathf.Abs(viewportPos.x - 0.5f);
        
        // 3) Decide on a target dead zone based on that distance.
        //    - If Mario is near center (small dist), use a bigger dead zone so camera won't move.
        //    - If Mario is near the edge (dist close to 0.5), use a smaller dead zone so camera starts following sooner.
        
        // For example, we can map distFromCenter of [0..0.5] to a dead zone of [maxDeadZoneWidth..minDeadZoneWidth].
        float t = distFromCenter / 0.5f;  // normalized from 0..1
        float targetDeadZone = Mathf.Lerp(maxDeadZoneWidth, minDeadZoneWidth, t);

        // 4) Smoothly lerp from current to target so it doesn't snap
        float newDeadZone = Mathf.Lerp(_framing.DeadZoneDepth, targetDeadZone, Time.deltaTime * deadZoneLerpSpeed);

        // 5) Apply it
        _framing.DeadZoneDepth = newDeadZone;
    }
    
    // void LateUpdate()
    // {
    //     // Get the actual camera transform Cinemachine is controlling
    //     var brain = CinemachineCore.GetActiveBrain(0);
    //     if (!brain) return;
    //
    //     // This is the *actual* camera transform after Cinemachine updates
    //     var camTransform = brain.OutputCamera.transform;
    //
    //     // If the camera tries to go left (x < previous x), clamp it
    //     Vector3 pos = camTransform.position;
    //     if (pos.x < _previousCameraX)
    //     {
    //         pos.x = _previousCameraX;
    //         camTransform.position = pos;
    //     }
    //     else
    //     {
    //         _previousCameraX = pos.x;
    //     }
    // }
}
