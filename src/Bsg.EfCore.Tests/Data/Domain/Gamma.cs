namespace Bsg.EfCore.Tests.Data.Domain
{
    using Entity;

    public class Gamma : IPrimaryEntity
    {
        public virtual int Id { get; set; }

        public virtual string Category { get; set; }

        public virtual decimal Cost { get; set; }

        public virtual decimal Price { get; set; }
    }
}
