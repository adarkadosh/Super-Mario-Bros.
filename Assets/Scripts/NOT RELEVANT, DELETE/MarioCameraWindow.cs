using UnityEngine;

// TODO: NOT WORKING, DELETE THIS SCRIPT
[RequireComponent(typeof(Camera))]
public class MarioCameraWindow : MonoBehaviour
{
    [Header("Target (Mario)")]
    [SerializeField] private Transform player;

    [Header("Viewport Window (no camera movement if player is inside this range)")]
    [Range(0f, 1f)]
    [SerializeField] private float minX = 0.25f; // left boundary in viewport space
    [Range(0f, 1f)]
    [SerializeField] private float maxX = 0.50f; // right boundary in viewport space

    [Header("Smoothing & Speed")]
    [Tooltip("How quickly the camera moves to the new position.")]
    [SerializeField] private float smoothTime = 0.3f;
    [Tooltip("Maximum speed for SmoothDamp (world units/sec).")]
    [SerializeField] private float maxSpeed = 10f;

    [Header("No-Backtracking")]
    [Tooltip("True = camera will never move left once it has scrolled right.")]
    [SerializeField] private bool oneSidedScroll = true;
    [Tooltip("The camera won't go left of this world X (e.g. 0 if level starts at 0).")]
    [SerializeField] private float worldLeftLimit = 0f; 

    private Camera _cam;
    private Vector3 _velocity = Vector3.zero; // used by SmoothDamp
    private float _prevCameraX;               // track last camera X for no-backtracking

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Start()
    {
        _prevCameraX = transform.position.x;
    }

    private void LateUpdate()
    {
        if (!player) return;

        // 1) Convert player to viewport space (0..1 across screen)
        Vector3 playerViewportPos = _cam.WorldToViewportPoint(player.position);
        float playerViewportX = playerViewportPos.x;

        // 2) We'll figure out where we WANT the camera to be (desiredPos).
        Vector3 desiredPos = transform.position;

        // 3) We need the camera's full width in world units (Orthographic only)
        float halfHeight = _cam.orthographicSize;
        float halfWidth  = halfHeight * _cam.aspect;
        float worldWidth = halfWidth * 2f;

        // -- HANDLING THE LEFT BOUNDARY (minX) --
        // If the player is to the LEFT of minX, we shift camera so player is at minX again.
        // But if oneSidedScroll = true and we've already moved beyond that point, we won't go back.
        if (playerViewportX < minX)
        {
            // How far the player is *behind* minX in viewport space
            float overshoot = minX - playerViewportX;
            // Convert that overshoot to a world offset
            float worldOffset = overshoot * worldWidth;
            desiredPos.x -= worldOffset;
        }

        // -- HANDLING THE RIGHT BOUNDARY (maxX) --
        // If the player is to the RIGHT of maxX, shift camera so player is at maxX
        if (playerViewportX > maxX)
        {
            float overshoot = playerViewportX - maxX;
            float worldOffset = overshoot * worldWidth;
            desiredPos.x += worldOffset;
        }

        // 4) If we want no-backtracking, never move camera left of _prevCameraX
        if (oneSidedScroll && desiredPos.x < _prevCameraX)
        {
            desiredPos.x = _prevCameraX;
        }

        // 5) Also clamp to the world's left boundary (like if your level starts at X=0)
        if (desiredPos.x < worldLeftLimit)
        {
            desiredPos.x = worldLeftLimit;
        }

        // 6) Smoothly move from current to desired using SmoothDamp to avoid snaps
        Vector3 newPos = Vector3.SmoothDamp(
            transform.position, 
            desiredPos, 
            ref _velocity, 
            smoothTime, 
            maxSpeed
        );

        transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);

        // 7) Update _prevCameraX so we don't backtrack next frame
        _prevCameraX = transform.position.x;
    }
}
