namespace Bsg.EfCore.Connection
{
    using System;
    using System.Data.Common;

    public class NonMsSqlConnectionFactory : INonMsSqlConnectionFactory
    {
        public DbConnection BuildConnection(string connection, string providerName)
        {
            throw new NotImplementedException("Implement your own Non MsSql Factory Service (if required) to create new Connections inheriting off Db Connection for Non MsSql databases");
        }
    }
}