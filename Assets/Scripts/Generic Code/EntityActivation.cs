using UnityEngine;

public class EntityActivation : MonoBehaviour
{
    private Transform marioTransform; // Reference to Mario's transform
    public float activationRadius = 10f; // Distance within which the object becomes active

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private EntityMovement _entityMovement;
    private bool isActive = false;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_rigidbody2D == null) Debug.LogError("Rigidbody2D is missing on " + gameObject.name);

        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.LogError("Animator is missing on " + gameObject.name);
        
        _entityMovement = GetComponent<EntityMovement>();
        if (_entityMovement == null) Debug.LogError("EntityMovement is missing on " + gameObject.name);

        // gameObject.SetActive(false); // Start disabled until visible
        // GetComponent<EntityMovement>().enabled = false;
        _entityMovement = GetComponent<EntityMovement>();
        SetComponentsActive(false);

    }
    
    private void Start()
    {
        GameObject mario = GameObject.FindWithTag("Player");
        if (mario != null)
        {
            marioTransform = mario.transform;
        }
        else
        {
            Debug.LogWarning("Mario (Player) not found. Please ensure there is a GameObject tagged 'Player' in the scene.");
            enabled = false; // Disable this script to prevent further errors
        }
    }

    private void Update()
    {
        if (marioTransform == null)
            return;

        float distanceSquared = (marioTransform.position - transform.position).sqrMagnitude;
        float activationRadiusSquared = activationRadius * activationRadius;

        if (distanceSquared <= activationRadiusSquared && !isActive)
        {
            SetComponentsActive(true);
            isActive = true;
        }
        else if (distanceSquared > activationRadiusSquared && isActive)
        {
            SetComponentsActive(false);
            isActive = false;
        }
    }

    private void OnEnable()
    {
        GetComponent<EntityMovement>().enabled = true;
        // _rigidbody2D.WakeUp();
    }

    private void OnDisable()
    {
        GetComponent<EntityMovement>().enabled = false;
        // _rigidbody2D.linearVelocity = Vector2.zero; // Clear momentum
        // _rigidbody2D.Sleep();
    }
    
    private void SetComponentsActive(bool isActive)
    {
        if (_entityMovement != null)
            _entityMovement.enabled = isActive;
        if (_animator != null)
            _animator.enabled = isActive;

        // if (_rigidbody2D != null)
        // {
            // if (isActive)
            // {
                // _rigidbody2D.WakeUp();
            // }
            // else
            // {
                // _rigidbody2D.linearVelocity = Vector2.zero; // Clear momentum
                // _rigidbody2D.Sleep();
            // }
        // }
    }
}
// using UnityEngine;
//
// public class EntityActivation : MonoBehaviour
// {
//     private Animator _animator;
//     private Rigidbody2D _rigidbody2D;
//     private bool hasBeenVisible = false;
//
//     private void Awake()
//     {
//         _rigidbody2D = GetComponent<Rigidbody2D>();
//         if (_rigidbody2D == null)
//             Debug.LogError($"Rigidbody2D is missing on {gameObject.name}");
//
//         _animator = GetComponent<Animator>();
//         if (_animator == null)
//             Debug.LogError($"Animator is missing on {gameObject.name}");
//
//         // Initialize components based on visibility
//         SetComponentsActive(false);
//     }
//
//     private void OnBecameVisible()
//     {
//         SetComponentsActive(true);
//         hasBeenVisible = true;
//     }
//
//     private void OnBecameInvisible()
//     {
//         if (hasBeenVisible)
//         {
//             Destroy(gameObject);
//         }
//         else
//         {
//             SetComponentsActive(false);
//         }
//         // SetComponentsActive(false);
//     }
//
//     private void SetComponentsActive(bool isActive)
//     {
//         if (_animator != null)
//             _animator.enabled = isActive;
//
//         if (_rigidbody2D != null)
//         {
//             if (isActive)
//             {
//                 _rigidbody2D.WakeUp();
//             }
//             else
//             {
//                 _rigidbody2D.linearVelocity = Vector2.zero; // Clear momentum
//                 _rigidbody2D.Sleep();
//             }
//         }
//     }
// }
// using UnityEngine;
//
// public class EntityActivation : MonoBehaviour
// {
//     public Transform marioTransform; // Reference to Mario's transform
//     public float activationRadius = 10f; // Distance within which the object becomes active
//
//     private Animator _animator;
//     private Rigidbody2D _rigidbody2D;
//     private bool isActive = false;
//
//     private void Awake()
//     {
//         _rigidbody2D = GetComponent<Rigidbody2D>();
//         if (_rigidbody2D == null)
//             Debug.LogError($"Rigidbody2D is missing on {gameObject.name}");
//
//         _animator = GetComponent<Animator>();
//         if (_animator == null)
//             Debug.LogError($"Animator is missing on {gameObject.name}");
//
//         gameObject.SetActive(false); // Start disabled until visible
//         // SetComponentsActive(false);
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
//         {
//             Debug.LogWarning("Mario's transform is not assigned.");
//             return;
//         }
//
//         float distanceSquared = (marioTransform.position - transform.position).sqrMagnitude;
//         float activationRadiusSquared = activationRadius * activationRadius;
//
//         if (distanceSquared <= activationRadiusSquared && !isActive)
//         {
//             gameObject.SetActive(true);
//             // SetComponentsActive(true);
//             isActive = true;
//         }
//         else if (distanceSquared > activationRadiusSquared && isActive)
//         {
//             gameObject.SetActive(false);
//             // SetComponentsActive(false);
//             isActive = false;
//         }
//     }
//
//     private void SetComponentsActive(bool isActive)
//     {
//         if (_animator != null)
//             _animator.enabled = isActive;
//
//         if (_rigidbody2D != null)
//         {
//             if (isActive)
//             {
//                 _rigidbody2D.WakeUp();
//             }
//             else
//             {
//                 _rigidbody2D.linearVelocity = Vector2.zero; // Clear momentum
//                 _rigidbody2D.Sleep();
//             }
//         }
//     }
// }