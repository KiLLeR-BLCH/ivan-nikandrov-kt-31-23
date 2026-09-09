using IvanNikandrovKt_31_23.Database.Helpers;
using IvanNikandrovKt_31_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IvanNikandrovKt_31_23.Database.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("cd_student");
            builder.HasKey(p => p.StudentId);

            builder.Property(p => p.StudentId).HasColumnName("student_id");
            builder.Property(p => p.FirstName).HasColumnName("c_first_name").HasColumnType(ColumnType.String).HasMaxLength(100).IsRequired();
            builder.Property(p => p.LastName).HasColumnName("c_last_name").HasColumnType(ColumnType.String).HasMaxLength(100).IsRequired();
            builder.Property(p => p.GroupId).HasColumnName("f_group_id");
            builder.Property(p => p.IsDeleted).HasColumnName("b_is_deleted").HasColumnType(ColumnType.Bool).HasDefaultValue(false);

            builder.HasOne(p => p.Group)
                .WithMany(p => p.Students)
                .HasForeignKey(p => p.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
