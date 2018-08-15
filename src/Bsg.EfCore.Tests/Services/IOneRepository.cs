namespace Bsg.EfCore.Tests.Services
{
    using System.Linq;
    using Bsg.EfCore.Tests.Data.Context;
    using Bsg.EfCore.Tests.Data.Domain;
    using Repo;

    public interface IOneRepository : IBulkInsertRepository<One, SecondaryContext>
    {
        IQueryable<One> AllJohns();
    }
}