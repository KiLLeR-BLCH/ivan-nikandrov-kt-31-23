using System.Diagnostics;

namespace IvanNikandrovKt_31_23.Models
{
    public class Discipline
    {
        public int DisciplineId { get; set; }
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }

}
