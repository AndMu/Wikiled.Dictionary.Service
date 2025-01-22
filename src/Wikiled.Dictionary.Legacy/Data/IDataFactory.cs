namespace Wikiled.Dictionary.Legacy.Data
{
    public interface IDataFactory
    {
        void PromoteConnection(IDataContextWrapper contextWrapper);

        IDataContextWrapper CreateDataContext<T>()
            where T : new();

        IDataContextWrapper CreateDataContextConnection<T>(string connectionString = "PhD")
            where T : new();

        IDataContextWrapper CreateRawDataContextConnection<T>(string connectionString = "PhD")
            where T : new();
    }
}