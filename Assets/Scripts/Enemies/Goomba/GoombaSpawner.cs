using UnityEngine;

namespace Enemies
{
    public class GoombaSpawner : MonoBehaviour
    {
        [Header("Goomba Settings")] 
        [SerializeField]
        private int numberOfGoombas = 1;

        [SerializeField] private float spacingDistance = 1f;
        [SerializeField] private GoombaPool goombaPool;
        [SerializeField] private bool spawnOnlyOnce = true;

        private Vector3 _spawnOrigin;
        private bool _hasSpawned;
        private Camera _mainCamera;

        private void Awake()
        {
            if (goombaPool == null)
            {
                Debug.LogError("GoombaPool is not assigned in GoombaSpawner.");
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
                SpawnGoombas();
                _hasSpawned = true;
            }
        }

        private void SpawnGoombas()
        {
            if (goombaPool == null)
            {
                Debug.LogError("GoombaPool is not assigned. Cannot spawn Goombas.");
                return;
            }

            _spawnOrigin = transform.position;

            for (var i = 0; i < numberOfGoombas; i++)
            {
                var spawnPosition = _spawnOrigin + new Vector3(i * spacingDistance, 0f, 0f);
                var goombaInstance = goombaPool.Get();
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
                $"{numberOfGoombas} Goombas spawned at {_spawnOrigin} with spacing of {spacingDistance} units.");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spacingDistance * numberOfGoombas / 2f);
        }
    }
}