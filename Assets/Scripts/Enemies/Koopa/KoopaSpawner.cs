using Enemies.Koopa;
using UnityEngine;
using UnityEngine.Serialization;

public class KoopaSpawner : MonoBehaviour
{
    [Header("Koopa Settings")] [SerializeField]
    private int numberOfKoopas = 1;

    [SerializeField] private float spacingDistance = 1f;
    [SerializeField] private KoopaPool koopaPool;
    [SerializeField] private bool spawnOnlyOnce = true;

    private Vector3 _spawnOrigin;
    private bool _hasSpawned;
    private Camera _mainCamera;

    private void Awake()
    {
        if (koopaPool == null)
        {
            Debug.LogError("KoopaPool is not assigned in KoopaSpawner.");
        }

        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera not found. Please tag your main camera as 'MainCamera'.");
        }
    }

    private void Update()
    {
        if (_hasSpawned && spawnOnlyOnce)
            return;

        if (_mainCamera.IsVisibleToCamera(transform))
        {
            SpawnKoopas();
            _hasSpawned = true;
        }
    }

    private void SpawnKoopas()
    {
        if (koopaPool == null)
        {
            Debug.LogError("GoombaPool is not assigned. Cannot spawn Goombas.");
            return;
        }

        _spawnOrigin = transform.position;

        for (var i = 0; i < numberOfKoopas; i++)
        {
            var spawnPosition = _spawnOrigin + new Vector3(i * spacingDistance, 0f, 0f);
            var goombaInstance = koopaPool.Get();
            if (goombaInstance != null)
            {
                goombaInstance.transform.position = spawnPosition;
            }
            else
            {
                Debug.LogWarning("GoombaPool returned null. Check your pooling system.");
            }
        }

        Debug.Log(
            $"{numberOfKoopas} Goombas spawned at {_spawnOrigin} with spacing of {spacingDistance} units.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spacingDistance * numberOfKoopas / 2f);
    }
}