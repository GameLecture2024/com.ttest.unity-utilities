using TTest.UnityUtilities.Pooling;
using UnityEngine;

namespace TTest.UnityUtilities.Samples.PoolingDemo
{
    /// <summary>Periodically chooses one pooled enemy type and places it around the player.</summary>
    public sealed class TopViewDemoSpawner : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private PoolSpawner[] enemyPools;
        [SerializeField, Min(.05f)] private float spawnInterval = .65f;
        [SerializeField, Min(1f)] private float spawnRadius = 8f;

        private float _nextSpawnTime;

        private void Update()
        {
            if (player == null || enemyPools == null || enemyPools.Length == 0 || Time.time < _nextSpawnTime)
                return;

            var pool = enemyPools[Random.Range(0, enemyPools.Length)];
            var direction = Random.insideUnitCircle.normalized;
            if (direction == Vector2.zero)
                direction = Vector2.right;

            var enemy = pool.Spawn<TopViewEnemy>();
            enemy.transform.position = player.position + (Vector3)(direction * spawnRadius);
            enemy.Configure(player, pool.Despawn);
            _nextSpawnTime = Time.time + spawnInterval;
        }
    }
}
