using System.Collections.Generic;

namespace Inside.Extenssions
{
    public static class ListExtension
    {
        public static void AddMany<T>(this IList<T> list, params T[] elements)
        {
            foreach (var t in elements)
            {
                list.Add(t);
            }
        }
    }
}
