using UnityEngine;

namespace TTest.UnityUtilities.Extensions
{
    public static class TransformExtensions
    {
        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void SetParentAndReset(this Transform transform, Transform parent)
        {
            transform.SetParent(parent, false);
            transform.ResetLocal();
        }
    }
}
