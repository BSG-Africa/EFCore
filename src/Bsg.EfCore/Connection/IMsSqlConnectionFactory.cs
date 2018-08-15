namespace Bsg.EfCore.Connection
{
    using System.Data.Common;

    public interface IMsSqlConnectionFactory
    {
        DbConnection BuildConnection(string connection);
    }
}
