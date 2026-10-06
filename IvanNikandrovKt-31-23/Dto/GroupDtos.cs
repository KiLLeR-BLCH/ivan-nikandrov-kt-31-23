namespace IvanNikandrovKt_31_23.Dto
{
    /// <summary>Фильтр списка групп. null = фильтр не применяется.</summary>
    public class GroupFilter
    {
        /// <summary>Специальность (SpecialtyId).</summary>
        public int? SpecialtyId { get; set; }

        /// <summary>Год обучения (курс).</summary>
        public int? Course { get; set; }

        /// <summary>false - только неудалённые (по умолчанию), true - только удалённые, null - все.</summary>
        public bool? IsDeleted { get; set; } = false;
    }

    /// <summary>Тело запроса для добавления и изменения группы.</summary>
    public class GroupRequest
    {
        public string? Name { get; set; }
        public int Course { get; set; }
        public int SpecialtyId { get; set; }
    }

    public record GroupDto(int GroupId, string? Name, int Course, int SpecialtyId, string? SpecialtyTitle, bool IsDeleted);
}
