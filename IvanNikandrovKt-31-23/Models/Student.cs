using System.Diagnostics;

namespace IvanNikandrovKt_31_23.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int GroupId { get; set; }
        public bool IsDeleted { get; set; }

        public Group? Group { get; set; }
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }

}
