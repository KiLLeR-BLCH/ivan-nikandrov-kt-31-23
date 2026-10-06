namespace IvanNikandrovKt_31_23.Dto
{
    /// <summary>Фильтр списка студентов. null/пусто = фильтр не применяется.</summary>
    public class StudentFilter
    {
        /// <summary>Название группы, например KT-31-23.</summary>
        public string? GroupName { get; set; }

        /// <summary>ФИО целиком или частично, например "Иванов Иван" или просто "Иван".</summary>
        public string? FullName { get; set; }

        /// <summary>false - только неудалённые (по умолчанию), true - только удалённые, null - все.</summary>
        public bool? IsDeleted { get; set; } = false;
    }

    /// <summary>Тело запроса для добавления и изменения студента.</summary>
    public class StudentRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int GroupId { get; set; }
    }

    public record StudentDto(int StudentId, string? FirstName, string? LastName, int GroupId, string? GroupName, bool IsDeleted);
}
