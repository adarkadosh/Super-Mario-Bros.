using Managers;
using UnityEngine;

namespace Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        private EnemyFactory _enemyFactory;

        [Header("Spawner Settings")] 
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private int numberOfEnemies = 1;
        [SerializeField] private float spacingDistance = 1f;
        [SerializeField] private bool spawnOnlyOnce = true;

        private Vector3 _spawnOrigin;
        private bool _hasSpawned;
        private Camera _mainCamera;

        private void Awake()
        {
            _enemyFactory = EnemyFactory.Instance;
            if (_enemyFactory == null)
            {
                Debug.LogError("EnemyFactory is not assigned in EnemySpawner.");
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
                SpawnEnemies();
                _hasSpawned = true;
            }
        }

        private void SpawnEnemies()
        {
            _spawnOrigin = transform.position;

            for (var i = 0; i < numberOfEnemies; i++)
            {
                var spawnPosition = _spawnOrigin + new Vector3(i * spacingDistance, 0f, 0f);
                var enemy = _enemyFactory.Spawn(enemyType);

                if (enemy != null)
                {
                    enemy.transform.position = spawnPosition;
                }
            }

            Extensions.Log(
                $"{numberOfEnemies} {enemyType}s spawned at {_spawnOrigin} with spacing of {spacingDistance} units.");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, spacingDistance * numberOfEnemies / 2f);
        }
    }
}