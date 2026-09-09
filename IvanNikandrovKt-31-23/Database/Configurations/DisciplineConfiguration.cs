using IvanNikandrovKt_31_23.Database.Helpers;
using IvanNikandrovKt_31_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IvanNikandrovKt_31_23.Database.Configurations
{
    public class DisciplineConfiguration : IEntityTypeConfiguration<Discipline>
    {
        public void Configure(EntityTypeBuilder<Discipline> builder)
        {
            builder.ToTable("cd_discipline");
            builder.HasKey(p => p.DisciplineId);

            builder.Property(p => p.DisciplineId).HasColumnName("discipline_id");
            builder.Property(p => p.Name).HasColumnName("c_discipline_name").HasColumnType(ColumnType.String).HasMaxLength(150).IsRequired();
            builder.Property(p => p.IsDeleted).HasColumnName("b_is_deleted").HasColumnType(ColumnType.Bool).HasDefaultValue(false);
        }
    }
}

