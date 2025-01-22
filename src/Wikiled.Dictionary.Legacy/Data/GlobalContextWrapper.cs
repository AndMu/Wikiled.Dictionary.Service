using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace Wikiled.Dictionary.Legacy.Data
{
    class GlobalContextWrapper : IDataContextWrapper
    {
        public event EventHandler Disposed;
        private readonly IDataContextWrapper real;
        private readonly object syncRoot = new object();

        public GlobalContextWrapper(IDataContextWrapper context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            real = context;
            real.Disposed += RealDisposed;
        }

        void RealDisposed(object sender, EventArgs e)
        {
            if (Disposed != null)
            {
                Disposed(this, EventArgs.Empty);
            }
        }

        public void Increment()
        {
            Monitor.Enter(syncRoot);
        }


        public IEnumerable<TResult> ExecuteQuery<TResult>(string command, params SqlParameter[] arguments)
        {
            return real.ExecuteQuery<TResult>(command, arguments);
        }

        public int ExecuteProcedure(string command, params SqlParameter[] arguments)
        {
            return real.ExecuteProcedure(command, arguments);
        }

        public int ExecuteCommand(string command, params SqlParameter[] arguments)
        {
            return real.ExecuteCommand(command, arguments);
        }

        public IQueryable<T> Table<T>()
            where T : class
        {
            return real.Table<T>();
        }

        public void DeleteAllOnSubmit<T>(IEnumerable<T> entities) where T : class
        {
            real.DeleteAllOnSubmit(entities);
        }

        public void DeleteOnSubmit<T>(T entity) where T : class
        {
            real.DeleteOnSubmit(entity);
        }

        public void InsertOnSubmit<T>(T entity) where T : class
        {
            real.InsertOnSubmit(entity);
        }

        public void SubmitChanges()
        {
            real.SubmitChanges();
        }

        public TRaw GetRawConnection<TRaw>() where TRaw : class
        {
            return real.GetRawConnection<TRaw>();
        }

        public bool StrategySet { get; set; }

        public void Dispose()
        {
            Monitor.Exit(syncRoot);
        }

        public string Name
        {
            get { return real.Name; }
        }
    }
}
