namespace RealTime.BL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionHelpers
    {
        public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> collection, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize));
            }

            return collection.Select((v, i) => new { v, groupIndex = i / chunkSize })
                             .GroupBy(x => x.groupIndex)
                             .Select(g => g.Select(x => x.v));
        }
    }
}
