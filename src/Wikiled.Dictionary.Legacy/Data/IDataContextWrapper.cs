using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Wikiled.Dictionary.Legacy.Data
{
    public interface IDataContextWrapper : IDisposable
    {
        event EventHandler Disposed;

        IEnumerable<TResult> ExecuteQuery<TResult>(string command, params SqlParameter[] arguments);

        int ExecuteProcedure(string command, params SqlParameter[] arguments);

        int ExecuteCommand(string command, params SqlParameter[] arguments);

        IQueryable<T> Table<T>()
            where T : class;

        void DeleteAllOnSubmit<T>(IEnumerable<T> entities)
            where T : class;

        void DeleteOnSubmit<T>(T entity)
            where T : class;

        void SubmitChanges();

        void InsertOnSubmit<T>(T entity)
            where T : class;

        TRaw GetRawConnection<TRaw>()
            where TRaw : class;

        bool StrategySet { get; set; }

        string Name { get; }
    }
}