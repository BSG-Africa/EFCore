namespace Bsg.EfCore.Tests.Data.Domain
{
    using Context;
    using EfCore.Domain;

    public class Two : IEntity<SecondaryContext>
    {
        public virtual int Id { get; set; }

        public virtual string Code { get; set; }

        public virtual int OneId { get; set; }

        public virtual One One { get; set; }
    }
}
