namespace Bsg.EfCore.Connection
{
    using System.Data.Common;
    using System.Data.SqlClient;

    public class MsSqlConnectionFactory : IMsSqlConnectionFactory
    {
        public DbConnection BuildConnection(string connection)
        {
            return new SqlConnection(connection);
        }
    }
}