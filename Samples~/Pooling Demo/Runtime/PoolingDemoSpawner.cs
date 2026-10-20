using System.Collections.Generic;
using TTest.UnityUtilities.Pooling;
using UnityEngine;

namespace TTest.UnityUtilities.Samples.PoolingDemo
{
    public sealed class PoolingDemoSpawner : MonoBehaviour
    {
        [SerializeField] private PoolingDemoCube cubePrefab;
        [SerializeField] private Transform spawnRoot;
        [SerializeField, Min(1)] private int prewarmCount = 16;
        [SerializeField, Min(.05f)] private float spawnInterval = .2f;
        [SerializeField, Min(.1f)] private float lifetime = 2f;

        private readonly List<PoolingDemoCube> _activeCubes = new();
        private GameObjectPool _pool;
        private float _nextSpawnTime;

        private void Awake()
        {
            _pool = new GameObjectPool(cubePrefab.gameObject, "Pooling Demo Cubes", transform);
            _pool.Prewarm(prewarmCount);
        }

        private void Update()
        {
            if (Time.time >= _nextSpawnTime)
            {
                SpawnCube();
                _nextSpawnTime = Time.time + spawnInterval;
            }

            for (var index = _activeCubes.Count - 1; index >= 0; index--)
            {
                var cube = _activeCubes[index];
                if (!cube.IsExpired)
                    continue;

                _activeCubes.RemoveAt(index);
                _pool.Despawn(cube.gameObject);
            }
        }

        private void OnDestroy() => _pool?.Dispose();

        private void SpawnCube()
        {
            var cube = _pool.Spawn(spawnRoot).GetComponent<PoolingDemoCube>();
            cube.transform.position = new Vector3(Random.Range(-7f, 7f), -4f, 0f);
            cube.Launch(Vector3.up * Random.Range(2f, 5f), lifetime);
            _activeCubes.Add(cube);
        }
    }
}
