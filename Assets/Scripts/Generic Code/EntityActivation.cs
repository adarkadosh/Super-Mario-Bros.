// using UnityEngine;
//
// public class EntityActivation : MonoBehaviour
// {
//     private Transform marioTransform; // Reference to Mario's transform
//     public float activationRadius = 10f; // Distance within which the object becomes active
//
//     private Animator _animator;
//     private Rigidbody2D _rigidbody2D;
//     private EntityMovement _entityMovement;
//     private bool isActive = false;
//     private void Awake()
//     {
//         _rigidbody2D = GetComponent<Rigidbody2D>();
//         if (_rigidbody2D == null) Debug.LogError("Rigidbody2D is missing on " + gameObject.name);
//
//         _animator = GetComponent<Animator>();
//         if (_animator == null) Debug.LogError("Animator is missing on " + gameObject.name);
//         
//         _entityMovement = GetComponent<EntityMovement>();
//         if (_entityMovement == null) Debug.LogError("EntityMovement is missing on " + gameObject.name);
//
//         // gameObject.SetActive(false); // Start disabled until visible
//         // GetComponent<EntityMovement>().enabled = false;
//         _entityMovement = GetComponent<EntityMovement>();
//         SetComponentsActive(false);
//
//     }
//     
//     private void Start()
//     {
//         GameObject mario = GameObject.FindWithTag("Player");
//         if (mario != null)
//         {
//             marioTransform = mario.transform;
//         }
//         else
//         {
//             Debug.LogWarning("Mario (Player) not found. Please ensure there is a GameObject tagged 'Player' in the scene.");
//             enabled = false; // Disable this script to prevent further errors
//         }
//     }
//
//     private void Update()
//     {
//         if (marioTransform == null)
//             return;
//
//         float distanceSquared = (marioTransform.position - transform.position).sqrMagnitude;
//         float activationRadiusSquared = activationRadius * activationRadius;
//
//         if (distanceSquared <= activationRadiusSquared && !isActive)
//         {
//             SetComponentsActive(true);
//             isActive = true;
//         }
//         else if (distanceSquared > activationRadiusSquared && isActive)
//         {
//             SetComponentsActive(false);
//             isActive = false;
//         }
//     }
//
//     private void OnEnable()
//     {
//         GetComponent<EntityMovement>().enabled = true;
//         // _rigidbody2D.WakeUp();
//     }
//
//     private void OnDisable()
//     {
//         GetComponent<EntityMovement>().enabled = false;
//         // _rigidbody2D.linearVelocity = Vector2.zero; // Clear momentum
//         // _rigidbody2D.Sleep();
//     }
//     
//     private void SetComponentsActive(bool isActive)
//     {
//         if (_entityMovement != null)
//             _entityMovement.enabled = isActive;
//         if (_animator != null)
//             _animator.enabled = isActive;
//
//         // if (_rigidbody2D != null)
//         // {
//             // if (isActive)
//             // {
//                 // _rigidbody2D.WakeUp();
//             // }
//             // else
//             // {
//                 // _rigidbody2D.linearVelocity = Vector2.zero; // Clear momentum
//                 // _rigidbody2D.Sleep();
//             // }
//         // }
//     }
// }
// // using UnityEngine;
// //
// // public class EntityActivation : MonoBehaviour
// // {
// //     private Animator _animator;
// //     private Rigidbody2D _rigidbody2D;
// //     private bool hasBeenVisible = false;
// //
// //     private void Awake()
// //     {
// //         _rigidbody2D = GetComponent<Rigidbody2D>();
// //         if (_rigidbody2D == null)
// //             Debug.LogError($"Rigidbody2D is missing on {gameObject.name}");
// //
// //         _animator = GetComponent<Animator>();
// //         if (_animator == null)
// //             Debug.LogError($"Animator is missing on {gameObject.name}");
// //
// //         // Initialize components based on visibility
// //         SetComponentsActive(false);
// //     }
// //
// //     private void OnBecameVisible()
// //     {
// //         SetComponentsActive(true);
// //         hasBeenVisible = true;
// //     }
// //
// //     private void OnBecameInvisible()
// //     {
// //         if (hasBeenVisible)
// //         {
// //             Destroy(gameObject);
// //         }
// //         else
// //         {
// //             SetComponentsActive(false);
// //         }
// //         // SetComponentsActive(false);
// //     }
// //
// //     private void SetComponentsActive(bool isActive)
// //     {
// //         if (_animator != null)
// //             _animator.enabled = isActive;
// //
// //         if (_rigidbody2D != null)
// //         {
// //             if (isActive)
// //             {
// //                 _rigidbody2D.WakeUp();
// //             }
// //             else
// //             {
// //                 _rigidbody2D.linearVelocity = Vector2.zero; // Clear momentum
// //                 _rigidbody2D.Sleep();
// //             }
// //         }
// //     }
// // }
// // using UnityEngine;
// //
// // public class EntityActivation : MonoBehaviour
// // {
// //     public Transform marioTransform; // Reference to Mario's transform
// //     public float activationRadius = 10f; // Distance within which the object becomes active
// //
// //     private Animator _animator;
// //     private Rigidbody2D _rigidbody2D;
// //     private bool isActive = false;
// //
// //     private void Awake()
// //     {
// //         _rigidbody2D = GetComponent<Rigidbody2D>();
// //         if (_rigidbody2D == null)
// //             Debug.LogError($"Rigidbody2D is missing on {gameObject.name}");
// //
// //         _animator = GetComponent<Animator>();
// //         if (_animator == null)
// //             Debug.LogError($"Animator is missing on {gameObject.name}");
// //
// //         gameObject.SetActive(false); // Start disabled until visible
// //         // SetComponentsActive(false);
// //     }
// //     
// //     private void Start()
// //     {
// //         GameObject mario = GameObject.FindWithTag("Player");
// //         if (mario != null)
// //         {
// //             marioTransform = mario.transform;
// //         }
// //         else
// //         {
// //             Debug.LogWarning("Mario (Player) not found. Please ensure there is a GameObject tagged 'Player' in the scene.");
// //             enabled = false; // Disable this script to prevent further errors
// //         }
// //     }
// //
// //     private void Update()
// //     {
// //         if (marioTransform == null)
// //         {
// //             Debug.LogWarning("Mario's transform is not assigned.");
// //             return;
// //         }
// //
// //         float distanceSquared = (marioTransform.position - transform.position).sqrMagnitude;
// //         float activationRadiusSquared = activationRadius * activationRadius;
// //
// //         if (distanceSquared <= activationRadiusSquared && !isActive)
// //         {
// //             gameObject.SetActive(true);
// //             // SetComponentsActive(true);
// //             isActive = true;
// //         }
// //         else if (distanceSquared > activationRadiusSquared && isActive)
// //         {
// //             gameObject.SetActive(false);
// //             // SetComponentsActive(false);
// //             isActive = false;
// //         }
// //     }
// //
// //     private void SetComponentsActive(bool isActive)
// //     {
// //         if (_animator != null)
// //             _animator.enabled = isActive;
// //
// //         if (_rigidbody2D != null)
// //         {
// //             if (isActive)
// //             {
// //                 _rigidbody2D.WakeUp();
// //             }
// //             else
// //             {
// //                 _rigidbody2D.linearVelocity = Vector2.zero; // Clear momentum
// //                 _rigidbody2D.Sleep();
// //             }
// //         }
// //     }
// // }

using UnityEngine;

public class EntityActivation : MonoBehaviour
{
    // [Header("Activation Settings")]
    // [Tooltip("Distance within which the entity becomes active.")]
    private float _activationRadius; // Will be dynamically set in Start()
    private float _deactivationRadius;


    private Transform _marioTransform; // Reference to Mario's transform

    [Header("Component References")]
    [Tooltip("Animator component controlling entity animations.")]
    private Animator _animator;

    [Tooltip("Rigidbody2D component for physics interactions.")]
    private Rigidbody2D _rigidbody2D;

    [Tooltip("EntityMovement script handling entity movement.")]
    private EntityMovement _entityMovement;

    private bool _isActive = false; // Tracks the active state of the entity

    private Camera _mainCamera; // Reference to the main camera
    
    private bool _hasBeenVisible = false; // Tracks if the entity has been visible

    private void Awake()
    {
        // Initialize Rigidbody2D
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_rigidbody2D == null)
        {
            Debug.LogError($"Rigidbody2D is missing on {gameObject.name}");
        }

        // Initialize Animator
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError($"Animator is missing on {gameObject.name}");
        }

        // Initialize EntityMovement
        _entityMovement = GetComponent<EntityMovement>();
        if (_entityMovement == null)
        {
            Debug.LogError($"EntityMovement is missing on {gameObject.name}");
        }

        // Initially deactivate movement and animations
        SetComponentsActive(false);
    }

    private void Start()
    {
        // Find Mario by tag
        GameObject mario = GameObject.FindWithTag("Player");
        if (mario != null)
        {
            _marioTransform = mario.transform;
        }
        else
        {
            Debug.LogWarning("Mario (Player) not found. Please ensure there is a GameObject tagged 'Player' in the scene.");
            enabled = false; // Disable this script to prevent further errors
            return;
        }

        // Get the main camera
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogWarning("Main Camera not found. Ensure a camera is tagged as 'MainCamera'.");
            enabled = false;
            return;
        }

        // Check if the camera is orthographic
        if (_mainCamera.orthographic)
        {
            float halfScreenWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
            // activationRadius = halfScreenWidth / 2f; // Half of half the screen width (quarter the screen width)
            // If you want activationRadius to be half the screen width, remove the division by 2
            _activationRadius = halfScreenWidth * 2f;
            _deactivationRadius = halfScreenWidth * 3f;
            Debug.Log($"Activation Radius set to half the screen width: {_activationRadius} world units.");
        }
        else
        {
            Debug.LogWarning("Main Camera is not orthographic. Activation radius not set automatically.");
            // Optionally, set a default activation radius or implement a different calculation for perspective cameras
            // activationRadius = 10f;
        }
    }


    private void Update()
    {
        if (_marioTransform == null)
            return;

        // Calculate squared distances for performance
        float distanceSquared = (_marioTransform.position - transform.position).sqrMagnitude;
        float activationRadiusSquared = _activationRadius * _activationRadius;
        float deactivationRadiusSquared = _deactivationRadius * _deactivationRadius;

        // Check if Mario is within the activation radius
        if (distanceSquared <= activationRadiusSquared && !_isActive)
        {
            _hasBeenVisible = true;
            ActivateEntity();
        }
        // Optionally, deactivate the entity if Mario moves out of range
        if (distanceSquared > deactivationRadiusSquared && _isActive)
        {
            if (_hasBeenVisible)
            {
                Destroy(gameObject);
            }
            else
            {
                DeactivateEntity();
            }
        }
        // OnDrawGizmosSelected();
    }

    private void ActivateEntity()
    {
        SetComponentsActive(true);
        _isActive = true;
    }

    private void DeactivateEntity()
    {
        SetComponentsActive(false);
        _isActive = false;
    }

    /// <summary>
    /// Enables or disables movement and animation components.
    /// Also manages Rigidbody2D physics state to prevent unintended movement.
    /// </summary>
    /// <param name="active">Whether to activate or deactivate the components.</param>
    private void SetComponentsActive(bool active)
    {
        if (_entityMovement != null)
            _entityMovement.enabled = active;

        if (_animator != null)
            _animator.enabled = active;

        if (_rigidbody2D != null)
        {
            if (active)
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Dynamic; // Allow physics to affect the entity
            }
            else
            {
                _rigidbody2D.linearVelocity = Vector2.zero; // Stop any current movement
                _rigidbody2D.bodyType =
                    RigidbodyType2D.Kinematic; // Prevent physics from affecting the entity
            }
        }
    }

    private void OnEnable()
    {
        if (_isActive)
        {
            SetComponentsActive(true);
        }
    }

    private void OnDisable()
    {
        if (_isActive)
        {
            SetComponentsActive(false);
        }
    }

    // private void OnDrawGizmos()
    // {
    //     // Visualize the activation radius in the Unity Editor
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, _activationRadius);
    // }
}