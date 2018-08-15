namespace Bsg.EfCore.Tests.Data.Configurations
{
    using EfCore.Configurations;

    public abstract class TestConfiguration<TType> : SchemaScopedConfiguration<TType> 
        where TType : class
    {
        protected TestConfiguration()
            : base("Test")
        {
        }
    }
}