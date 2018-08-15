namespace Bsg.EfCore.Tests.Data.Domain
{
    using Entity;

    public class Beta : IPrimaryEntity
    {
        public virtual int Id { get; set; }
        
        public virtual string Code { get; set; }

        public virtual int AlphaId { get; set; }

        public virtual Alpha Alpha { get; set; }
    }
}
