using System;
using System.Collections.Generic;

namespace WinConStartup.RunLogic
{
    public class RunnerPool
    {
        private readonly List<RunItem> _pool = new List<RunItem>();
        private readonly bool _verbose;
        public int Length;

        public RunnerPool(bool verbose = false)
        {
            Length = 0;
            _verbose = verbose;
        }

        public RunnerPool(RunItem[] items, bool verbose = false)
        {
            _pool.AddRange(items);
            Length = _pool.Count;
            _verbose = verbose;
        }

        public void Add(RunItem item)
        {
            _pool.Add(item);
            Length++;
        }

        public void removeItem(RunItem item)
        {
            _pool.Remove(item);
            Length--;
        }

        public void removeItem(string itemQualifierName)
        {
            foreach (RunItem item in _pool)
            {
                if (item.Name.Equals(itemQualifierName))
                {
                    removeItem(item);
                }
            }
        }

        public void RunPool()
        {
            foreach (RunItem item in _pool)
            {
                if (_verbose)
                {
                    Console.WriteLine("Starting {0} !\n\n", item.Name);
                }
                item.run();
            }
        }
    }
}