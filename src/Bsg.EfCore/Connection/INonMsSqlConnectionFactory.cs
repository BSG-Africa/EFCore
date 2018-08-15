namespace Bsg.EfCore.Connection
{
    using System.Data.Common;

    public interface INonMsSqlConnectionFactory
    {
        DbConnection BuildConnection(string connection, string providerName);
    }
}
