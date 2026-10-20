using UnityEngine;

namespace TTest.UnityUtilities.Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject == null)
                throw new MissingReferenceException("Cannot add a component to a null GameObject.");

            return gameObject.TryGetComponent<T>(out var component)
                ? component
                : gameObject.AddComponent<T>();
        }

        public static bool SetActiveIfChanged(this GameObject gameObject, bool active)
        {
            if (gameObject == null)
                return false;

            if (gameObject.activeSelf == active)
                return false;

            gameObject.SetActive(active);
            return true;
        }

        public static void DestroyChildren(this Transform parent)
        {
            if (parent == null)
                return;

            for (var index = parent.childCount - 1; index >= 0; index--)
                Object.Destroy(parent.GetChild(index).gameObject);
        }
    }
}
