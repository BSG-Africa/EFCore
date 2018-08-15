namespace Bsg.EfCore.Tests.Data.Domain
{
    using Context;
    using EfCore.Domain;

    public class One : IEntity<SecondaryContext>
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }
}
