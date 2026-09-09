using IvanNikandrovKt_31_23.Database.Helpers;
using IvanNikandrovKt_31_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IvanNikandrovKt_31_23.Database.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("cd_group");
            builder.HasKey(p => p.GroupId);

            builder.Property(p => p.GroupId).HasColumnName("group_id");
            builder.Property(p => p.Name).HasColumnName("c_group_name").HasColumnType(ColumnType.String).HasMaxLength(50).IsRequired();
            builder.Property(p => p.Course).HasColumnName("c_course").HasColumnType(ColumnType.Int).IsRequired();
            builder.Property(p => p.SpecialtyId).HasColumnName("f_specialty_id");
            builder.Property(p => p.IsDeleted).HasColumnName("b_is_deleted").HasColumnType(ColumnType.Bool).HasDefaultValue(false);

            builder.HasOne(p => p.Specialty)
                .WithMany(p => p.Groups)
                .HasForeignKey(p => p.SpecialtyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

