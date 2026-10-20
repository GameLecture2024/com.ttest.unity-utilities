using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace TTest.UnityUtilities.Pooling
{
    /// <summary>A pool for one prefab. It never relies on prefab names or resource-loading conventions.</summary>
    public sealed class GameObjectPool : IDisposable
    {
        private readonly GameObject _prefab;
        private readonly Transform _inactiveRoot;
        private readonly ObjectPool<GameObject> _pool;
        private readonly HashSet<int> _instanceIds = new();
        private bool _disposed;

        public GameObjectPool(GameObject prefab, string poolName = null, Transform poolParent = null,
            int defaultCapacity = 8, int maxSize = 128)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            _prefab = prefab;
            var root = new GameObject(poolName ?? $"{prefab.name} Pool");
            _inactiveRoot = root.transform;
            _inactiveRoot.SetParent(poolParent, false);
            _pool = new ObjectPool<GameObject>(Create, OnGet, OnRelease, OnDestroy, true, defaultCapacity, maxSize);
        }

        public int CountInactive => _pool.CountInactive;

        public GameObject Spawn(Transform parent = null)
        {
            ThrowIfDisposed();
            var instance = _pool.Get();
            instance.transform.SetParent(parent, false);
            InvokeSpawned(instance);
            return instance;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null || _disposed)
                return;
            if (!_instanceIds.Contains(instance.GetInstanceID()))
                throw new ArgumentException("This instance was not created by this pool.", nameof(instance));

            _pool.Release(instance);
        }

        public void Prewarm(int count)
        {
            ThrowIfDisposed();
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var instances = new GameObject[count];
            for (var index = 0; index < count; index++)
                instances[index] = Spawn();
            for (var index = 0; index < count; index++)
                Despawn(instances[index]);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _pool.Clear();
            if (_inactiveRoot != null)
                UnityEngine.Object.Destroy(_inactiveRoot.gameObject);
        }

        private GameObject Create()
        {
            var instance = UnityEngine.Object.Instantiate(_prefab);
            instance.name = _prefab.name;
            _instanceIds.Add(instance.GetInstanceID());
            instance.SetActive(false);
            return instance;
        }

        private static void OnGet(GameObject instance)
        {
            instance.SetActive(true);
        }

        private void OnRelease(GameObject instance)
        {
            InvokeDespawned(instance);
            instance.SetActive(false);
            instance.transform.SetParent(_inactiveRoot, false);
        }

        private void OnDestroy(GameObject instance)
        {
            _instanceIds.Remove(instance.GetInstanceID());
            UnityEngine.Object.Destroy(instance);
        }

        private static void InvokeSpawned(GameObject instance)
        {
            foreach (var behaviour in instance.GetComponentsInChildren<MonoBehaviour>(true))
                if (behaviour is IPoolable poolable)
                    poolable.OnPoolSpawned();
        }

        private static void InvokeDespawned(GameObject instance)
        {
            foreach (var behaviour in instance.GetComponentsInChildren<MonoBehaviour>(true))
                if (behaviour is IPoolable poolable)
                    poolable.OnPoolDespawned();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(GameObjectPool));
        }
    }
}
