namespace Bsg.EfCore.Tests.Data.Domain
{
    using Entity;

    public class Alpha : IPrimaryEntity
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }
    }
}
