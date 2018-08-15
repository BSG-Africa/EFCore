namespace Bsg.EfCore.Tests.Data.Domain
{
    using Entity;

    public class Delta : IPrimaryEntity
    {
        public virtual int Id { get; set; }

        public virtual string Category { get; set; }

        public virtual decimal? NullDec { get; set; }

        public virtual decimal Dec { get; set; }

        public virtual int SomeId { get; set; }

        public virtual int? NullSomeId { get; set; }
    }
}
