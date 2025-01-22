using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using NLog;

namespace Wikiled.Dictionary.Legacy.Data
{
    /// <summary>
    ///     A linq to sql wrapper class for the Datacontext object. This is the real implementation of IDataContextWrapper
    ///     that works directly with a database
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataContextWrapper<T> : IDataContextWrapper
        where T : new()
    {
        public event EventHandler Disposed;

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private bool disposed;

        public DataContextWrapper()
        {
            var t = typeof(T);
            log.Debug("Creating: {0}", t);
            RawConnection = (DataContext)Activator.CreateInstance(t);
            Name = t.Name;
        }

        public DataContextWrapper(string name, string connectionString)
        {
            var t = typeof(T);
            log.Debug("Creating: {0}, name = {1}, connectionString = {2}", t, name, connectionString);
            RawConnection = (DataContext)Activator.CreateInstance(t, connectionString);
            Name = name;
        }

        public string Name { get; }

        public DataContext RawConnection { get; }

        public bool StrategySet { get; set; }

        public void DeleteAllOnSubmit<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            log.Debug("DeleteAllOnSubmit: {0}", typeof(TEntity));
            RawConnection.GetTable(typeof(TEntity)).DeleteAllOnSubmit(entities);
        }

        public void DeleteOnSubmit<TEntity>(TEntity entity)
            where TEntity : class
        {
            log.Debug("DeleteOnSubmit: {0}", typeof(TEntity));
            RawConnection.GetTable(typeof(TEntity)).DeleteOnSubmit(entity);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            log.Debug("Dispose");
            Dispose(true);
        }

        public int ExecuteCommand(string command, params SqlParameter[] arguments)
        {
            log.Debug("ExecuteCommand: {0}", command);
            return RawConnection.ExecuteCommand(command, arguments);
        }

        public int ExecuteProcedure(string command, params SqlParameter[] arguments)
        {
            return ExecuteCommand(command, arguments);
        }

        public IEnumerable<TResult> ExecuteQuery<TResult>(string command, params SqlParameter[] arguments)
        {
            log.Debug("ExecuteQuery: {0}", command);
            return RawConnection.ExecuteQuery<TResult>(command, arguments);
        }

        public TRaw GetRawConnection<TRaw>()
            where TRaw : class
        {
            log.Debug("GetRawConnection: {0}", typeof(TRaw));
            var context = RawConnection as TRaw;
            if (context == null)
            {
                throw new ArgumentOutOfRangeException("TRaw", "Connection is not of type: " + typeof(TRaw));
            }

            return context;
        }

        public void InsertOnSubmit<TEntity>(TEntity entity)
            where TEntity : class
        {
            log.Debug("InsertOnSubmit: {0}", typeof(TEntity));
            RawConnection.GetTable(typeof(TEntity)).InsertOnSubmit(entity);
        }

        public void SubmitChanges()
        {
            log.Debug("SubmitChanges");
            RawConnection.SubmitChanges();
        }

        /// <summary>
        ///     Tables this instance.
        /// </summary>
        /// <typeparam name="TTableName"></typeparam>
        /// <returns></returns>
        public IQueryable<TTableName> Table<TTableName>()
            where TTableName : class
        {
            log.Debug("Table: {0}", typeof(TTableName));
            var table = (Table<TTableName>)RawConnection.GetTable(typeof(TTableName));
            return table.AsQueryable();
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                RawConnection.Dispose();
            }

            disposed = true;
            if (Disposed != null)
            {
                Disposed(this, EventArgs.Empty);
            }
        }
    }
}
