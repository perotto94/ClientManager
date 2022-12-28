using System;
using System.Collections.Generic;

namespace ClientManager
{
    /// <summary>
    /// Object with a some relevant date
    /// </summary>
    public interface IDated
    {
        /// <summary>
        /// Most relevant date
        /// </summary>
        public DateTime DateRelevant { get; }
    }

    public static partial class Toolkit
    {
        /// <summary>
        /// Sort list by date
        /// </summary>
        public static void SortByDate<T>(List<T> list) where T : IDated
        {
            list.Sort((x, y) => x.DateRelevant.CompareTo(y.DateRelevant));
        }
    }
}
