using System;
using System.Configuration;
using Wikiled.Core.Utility.Arguments;

namespace Wikiled.Dictionary.Legacy.Data
{
    public static class ConnectionStringHelper
    {
        public static string GetConnectionString(string name)
        {
            Guard.NotNullOrEmpty(() => name, name);
            var connection = ConfigurationManager.ConnectionStrings[name];
            if (connection == null)
            {
                throw new NullReferenceException("Failed to resolve " + name + " connection string");
            }
            return connection.ConnectionString;
        }
    }
}
