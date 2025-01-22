using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Wikiled.Core.Utility.Arguments;
using Wikiled.Core.Utility.Extensions;

namespace Wikiled.Dictionary.Legacy.Data
{
    public class ThreadObjectHolder<T>
    {
        private readonly Dictionary<int, T> dataTable = new Dictionary<int, T>();
        private readonly Func<T> factory;

        public ThreadObjectHolder(Func<T> factory)
        {
            Guard.NotNull(() => factory, factory);
            this.factory = factory;
        }

        public void DeleteAll()
        {
            lock (dataTable)
            {
                dataTable.Clear();
            }
        }

        public void DeleteById(int threadId)
        {
            lock (dataTable)
            {
                dataTable.GetSafeDelete(Thread.CurrentThread.ManagedThreadId);
            }
        }

        public void DeleteCurrent()
        {
            DeleteById(Thread.CurrentThread.ManagedThreadId);
        }

        public T GetInstance()
        {
            lock (dataTable)
            {
                return dataTable.GetSafeCreate(Thread.CurrentThread.ManagedThreadId, factory);
            }
        }

        public T[] GetAll()
        {
            lock (dataTable)
            {
                return dataTable.Values.ToArray();
            }
        }
    }
}
