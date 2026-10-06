namespace IvanNikandrovKt_31_23.Dto
{
    /// <summary>Средний балл по предмету в группе.</summary>
    public class GroupDisciplineAverageFilter
    {
        public int GroupId { get; set; }
        public int DisciplineId { get; set; }
    }

    /// <summary>Оценки конкретного студента (при необходимости - только по одному предмету).</summary>
    public class StudentGradesFilter
    {
        public int StudentId { get; set; }
        public int? DisciplineId { get; set; }
    }

    /// <summary>Средний балл за год обучения (курс), при необходимости - по одному предмету.</summary>
    public class YearAverageFilter
    {
        public int Course { get; set; }
        public int? DisciplineId { get; set; }
    }

    /// <summary>Добавление оценки студенту.</summary>
    public class AddGradeRequest
    {
        public int StudentId { get; set; }
        public int DisciplineId { get; set; }
        public int Value { get; set; }
    }

    /// <summary>Изменение значения оценки.</summary>
    public class UpdateGradeRequest
    {
        public int Value { get; set; }
    }

    public record GradeDto(int GradeId, int StudentId, int DisciplineId, string? DisciplineName, int Value);

    public record StudentGradesDto(int StudentId, string FullName, double? Average, List<GradeDto> Grades);

    /// <summary>Average = null, если подходящих оценок нет.</summary>
    public record AverageGradeDto(double? Average, int GradesCount);
}
