using System;
using TTest.UnityUtilities.Pooling;
using UnityEngine;

namespace TTest.UnityUtilities.Samples.PoolingDemo
{
    /// <summary>An enemy that returns itself to its originating pool near the player.</summary>
    public sealed class TopViewEnemy : MonoBehaviour, IPoolable
    {
        [SerializeField, Min(.1f)] private float speed = 2.5f;
        [SerializeField, Min(.05f)] private float despawnDistance = .7f;

        private Transform _player;
        private Action<GameObject> _despawn;
        private bool _returning;

        public void Configure(Transform player, Action<GameObject> despawn)
        {
            _player = player;
            _despawn = despawn;
            _returning = false;
        }

        private void Update()
        {
            if (_player == null || _despawn == null || _returning)
                return;

            var offset = _player.position - transform.position;
            if (offset.sqrMagnitude > despawnDistance * despawnDistance)
            {
                transform.position += offset.normalized * speed * Time.deltaTime;
                return;
            }

            _returning = true;
            _despawn(gameObject);
        }

        public void OnPoolSpawned() => _returning = false;

        public void OnPoolDespawned()
        {
            _player = null;
            _despawn = null;
            _returning = false;
        }
    }
}
