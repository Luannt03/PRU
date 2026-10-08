using System;
using System.Collections.Generic;

namespace ChemLab9.Core
{
    public sealed class ProgressLedger
    {
        public static readonly int[] LessonIds = { 1, 2, 8, 30, 31 };
        readonly HashSet<int> completed = new HashSet<int>();
        public int Count => completed.Count;
        public int Score => Count * 100;
        public bool Contains(int id) => completed.Contains(id);
        public bool Complete(int id)
        {
            if (Array.IndexOf(LessonIds, id) < 0) throw new ArgumentOutOfRangeException(nameof(id));
            return completed.Add(id);
        }
        public void Clear() => completed.Clear();
    }
}
