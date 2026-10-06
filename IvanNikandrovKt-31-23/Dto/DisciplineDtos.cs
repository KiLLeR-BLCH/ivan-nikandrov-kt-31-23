namespace IvanNikandrovKt_31_23.Dto
{
    /// <summary>Фильтр списка дисциплин. null/пусто = фильтр не применяется.</summary>
    public class DisciplineFilter
    {
        /// <summary>Название дисциплины целиком или частично.</summary>
        public string? Name { get; set; }

        /// <summary>false - только неудалённые (по умолчанию), true - только удалённые, null - все.</summary>
        public bool? IsDeleted { get; set; } = false;
    }

    /// <summary>Тело запроса для добавления и изменения дисциплины.</summary>
    public class DisciplineRequest
    {
        public string? Name { get; set; }
    }

    public record DisciplineDto(int DisciplineId, string? Name, bool IsDeleted);
}
