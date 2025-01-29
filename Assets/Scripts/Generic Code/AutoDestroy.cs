using Managers;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float destroyDistance = 15f; // Distance threshold
    private IPoolable _poolable;
    private Transform _marioTransform;
    private Camera _mainCamera;
    

    private void Start()
    {
        _poolable = GetComponent<IPoolable>();
        _marioTransform = GameObject.FindGameObjectWithTag("Player").transform; // Ensure Mario has "Player" tag
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_marioTransform == null) return;

        float distanceToMario = Vector3.Distance(transform.position, _marioTransform.position);

        // Check if the enemy is beyond the destroy distance
        if (distanceToMario > destroyDistance && !_mainCamera.IsVisibleToCamera(transform))
        {
            if (_poolable != null)
            {
                _poolable.Kill();
            }
            Debug.Log($"Enemy {gameObject.name} destroyed, too far from Mario.");
        }
    }
}