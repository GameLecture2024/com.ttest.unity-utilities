using System.Collections.Generic;

namespace TTest.UnityUtilities.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IReadOnlyCollection<T> collection) =>
            collection == null || collection.Count == 0;

        public static bool TryGet<T>(this IReadOnlyList<T> list, int index, out T value)
        {
            if (list != null && index >= 0 && index < list.Count)
            {
                value = list[index];
                return true;
            }

            value = default;
            return false;
        }
    }
}
