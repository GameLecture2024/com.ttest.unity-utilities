namespace TTest.UnityUtilities.Pooling
{
    /// <summary>Receives lifecycle callbacks when a GameObject is spawned from or returned to a pool.</summary>
    public interface IPoolable
    {
        void OnPoolSpawned();
        void OnPoolDespawned();
    }
}
