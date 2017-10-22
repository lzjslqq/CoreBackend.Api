using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreBackend.Api.Entities
{
    public class MaterialConfiguration : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(50);
            // 声明与Product是一对多关系
            builder.HasOne(m => m.Product).WithMany(m => m.Materials).HasForeignKey(m => m.ProductId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
