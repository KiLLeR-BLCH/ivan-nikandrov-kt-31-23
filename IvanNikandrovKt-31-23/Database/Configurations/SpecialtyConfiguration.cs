using IvanNikandrovKt_31_23.Database.Helpers;
using IvanNikandrovKt_31_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IvanNikandrovKt_31_23.Database.Configurations
{
    public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
    {
        public void Configure(EntityTypeBuilder<Specialty> builder)
        {
            builder.ToTable("cd_specialty");
            builder.HasKey(p => p.SpecialtyId);

            builder.Property(p => p.SpecialtyId).HasColumnName("specialty_id");
            builder.Property(p => p.Title).HasColumnName("c_title").HasColumnType(ColumnType.String).HasMaxLength(150).IsRequired();
            builder.Property(p => p.Code).HasColumnName("c_code").HasColumnType(ColumnType.String).HasMaxLength(20).IsRequired();
        }
    }
}
