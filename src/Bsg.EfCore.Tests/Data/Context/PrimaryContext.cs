namespace Bsg.EfCore.Tests.Data.Context
{
    using Bsg.EfCore.Context;
    using Microsoft.EntityFrameworkCore;
    using SupportType.Dtos;

    public class PrimaryContext : EfCoreContext
    {
        public PrimaryContext(DbContextOptions options, ContextSupportTypeDto contextSupport)
            : base(options, contextSupport)
        {
        }
    }
}
