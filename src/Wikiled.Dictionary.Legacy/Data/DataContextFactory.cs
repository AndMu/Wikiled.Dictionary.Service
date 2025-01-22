using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Linq;

namespace Wikiled.Dictionary.Legacy.Data
{
    public class DataContextFactory : IDataFactory
    {
        private readonly ThreadObjectHolder<Dictionary<string, GlobalContextWrapper>> saved = 
            new ThreadObjectHolder<Dictionary<string, GlobalContextWrapper>>(() => new Dictionary<string, GlobalContextWrapper>());

        public IDataContextWrapper CreateDataContext<T>()
            where T : new()
        {
            if (typeof(DataContext).IsAssignableFrom(typeof(T)))
            {
                return new DataContextWrapper<T>();
            }

            if (typeof(ObjectContext).IsAssignableFrom(typeof(T)))
            {
                return new ObjectContextWrapper<T>();
            }

            throw new NotSupportedException();
        }

        public void PromoteConnection(IDataContextWrapper contextWrapper)
        {
            if (contextWrapper == null)
            {
                throw new ArgumentNullException("contextWrapper");
            }

            var context = new GlobalContextWrapper(contextWrapper);
            context.Disposed += ContextDisposed;
            saved.GetInstance()[contextWrapper.Name] = context;
        }

        void ContextDisposed(object sender, EventArgs e)
        {
            saved.GetInstance().Remove(((IDataContextWrapper) sender).Name);
        }

        public IDataContextWrapper CreateRawDataContextConnection<T>(string connectionString = "PhD")
           where T : new()
        {
            var connection = ConnectionStringHelper.GetConnectionString(connectionString);
            if (typeof(DataContext).IsAssignableFrom(typeof(T)))
            {
                return new DataContextWrapper<T>(connectionString, connection);
            }

            if (typeof(ObjectContext).IsAssignableFrom(typeof(T)))
            {
                return new ObjectContextWrapper<T>(connectionString, connection);
            }

            throw new ArgumentOutOfRangeException("T", "Type is not supported - " + typeof(T));
        }

        public IDataContextWrapper CreateDataContextConnection<T>(string connectionString = "PhD")
           where T : new()
        {
            GlobalContextWrapper global;
            if (saved.GetInstance().TryGetValue(connectionString, out global))
            {
                global.Increment();
                return global;
            }
            return CreateRawDataContextConnection<T>(connectionString);
        }

    }
}
