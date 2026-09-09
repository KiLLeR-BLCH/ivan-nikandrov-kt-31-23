using IvanNikandrovKt_31_23.Database.Helpers;
using IvanNikandrovKt_31_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IvanNikandrovKt_31_23.Database.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.ToTable("cd_grade");
            builder.HasKey(p => p.GradeId);

            builder.Property(p => p.GradeId).HasColumnName("grade_id");
            builder.Property(p => p.Value).HasColumnName("c_value").HasColumnType(ColumnType.Int).IsRequired();
            builder.Property(p => p.StudentId).HasColumnName("f_student_id");
            builder.Property(p => p.DisciplineId).HasColumnName("f_discipline_id");

            builder.HasOne(p => p.Student)
                .WithMany(p => p.Grades)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Discipline)
                .WithMany(p => p.Grades)
                .HasForeignKey(p => p.DisciplineId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

