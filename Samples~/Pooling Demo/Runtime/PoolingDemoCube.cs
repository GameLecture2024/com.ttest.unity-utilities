using TTest.UnityUtilities.Pooling;
using UnityEngine;

namespace TTest.UnityUtilities.Samples.PoolingDemo
{
    public sealed class PoolingDemoCube : MonoBehaviour, IPoolable
    {
        private Vector3 _velocity;
        private float _despawnAt;

        public bool IsExpired => Time.time >= _despawnAt;

        public void Launch(Vector3 velocity, float lifetime)
        {
            _velocity = velocity;
            _despawnAt = Time.time + lifetime;
        }

        private void Update() => transform.position += _velocity * Time.deltaTime;

        public void OnPoolSpawned()
        {
            transform.localScale = Vector3.one;
            GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, .6f, 1f, .7f, 1f);
        }

        public void OnPoolDespawned()
        {
            _velocity = Vector3.zero;
        }
    }
}
