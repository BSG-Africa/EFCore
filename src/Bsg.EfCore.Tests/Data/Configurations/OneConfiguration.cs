namespace Bsg.EfCore.Tests.Data.Configurations
{
    using Domain;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OneConfiguration : TestConfiguration<One>
    {
        protected override void OnConfigure(EntityTypeBuilder<One> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(50);
        }
    }
}
