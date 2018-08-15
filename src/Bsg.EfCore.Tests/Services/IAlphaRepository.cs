namespace Bsg.EfCore.Tests.Services
{
    using System.Linq;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;

    public interface IAlphaRepository : IPrimaryRepository<Alpha>
    {
        IQueryable<Alpha> Active();
    }
}