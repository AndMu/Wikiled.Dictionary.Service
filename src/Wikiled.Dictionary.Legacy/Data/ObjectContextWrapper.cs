using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using NLog;
using Wikiled.Core.Utility.Extensions;

namespace Wikiled.Dictionary.Legacy.Data
{
    class ObjectContextWrapper<T> : IDataContextWrapper
        where T : new()
    {
        private readonly Dictionary<Type, object> cachedTables = new Dictionary<Type, object>();

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public event EventHandler Disposed;

        private bool disposed;

        public ObjectContextWrapper()
        {
            var t = typeof(T);
            log.Debug("Creating: {0}", t);
            RawConnection = (ObjectContext)Activator.CreateInstance(t);
            Name = t.Name;
        }

        public ObjectContextWrapper(string name, string connectionString)
        {
            var t = typeof(T);
            log.Debug("Creating: {0}, name = {1}, connectionString = {2}", t, name, connectionString);
            RawConnection = (ObjectContext)Activator.CreateInstance(t, connectionString);
            Name = name;
        }

        public IEnumerable<TResult> ExecuteQuery<TResult>(string command, params SqlParameter[] arguments)
        {
            string sql = string.Format(command, arguments);
            log.Debug("ExecuteCommand: {0}", sql);
            return RawConnection.ExecuteStoreQuery<TResult>(command, arguments);
        }

        public int ExecuteProcedure(string command, params SqlParameter[] arguments)
        {
            string sql = string.Format(command, arguments);
            log.Debug("ExecuteCommand: {0}", sql);
            var entityConnection = (EntityConnection)RawConnection.Connection;
            DbConnection conn = entityConnection.StoreConnection;

            ConnectionState initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                {
                    conn.Open(); // open connection if not already open
                }
                using (DbCommand cmd = conn.CreateCommand())
                {
                    foreach (var argument in arguments)
                    {
                        cmd.Parameters.Add(argument);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sql;
                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                {
                    conn.Close(); // only close connection if not initially open
                }
            }
        }

        public int ExecuteCommand(string command, params SqlParameter[] arguments)
        {
            string sql = string.Format(command, arguments);
            log.Debug("ExecuteCommand: {0}", sql);
            return RawConnection.ExecuteStoreCommand(command, arguments);
        }

        public IQueryable<TTableName> Table<TTableName>()
            where TTableName : class
        {
            log.Debug("Table: {0}", typeof(TTableName));
            var table = cachedTables.GetSafeCreate(
                typeof(TTableName),
                () => RawConnection.CreateObjectSet<TTableName>());
            return (IQueryable<TTableName>)table;
        }

        public void DeleteAllOnSubmit<TTableName>(IEnumerable<TTableName> entities) where TTableName : class
        {
            log.Debug("DeleteAllOnSubmit: {0}", typeof(TTableName));
            ExecuteCommand("DELETE FROM " + EntityExtension.GetEntitySetName<TTableName>());
            cachedTables.Remove(typeof(TTableName));
        }

        public void DeleteOnSubmit<TTableName>(TTableName entity) where TTableName : class
        {
            log.Debug("DeleteOnSubmit: {0}", typeof(TTableName));
            RawConnection.DeleteObject(entity);
        }

        public void InsertOnSubmit<TTableName>(TTableName entity) where TTableName : class
        {
            log.Debug("InsertOnSubmit: {0}", typeof(TTableName));
            RawConnection.AddObject(EntityExtension.GetEntitySetName<TTableName>(), entity);
        }

        public void SubmitChanges()
        {
            log.Debug("SubmitChanges");
            RawConnection.SaveChanges();
        }

        public TRaw GetRawConnection<TRaw>() where TRaw : class
        {
            log.Debug("GetRawConnection: {0}", typeof(TRaw));
            var context = RawConnection as TRaw;
            if (context == null)
            {
                throw new ArgumentOutOfRangeException("TRaw", "Connection is not of type: " + typeof(TRaw));
            }
            return context;
        }

        public bool StrategySet { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            log.Debug("Dispose");
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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

        public string Name { get; private set; }

        public ObjectContext RawConnection { get; private set; }
    }
}
