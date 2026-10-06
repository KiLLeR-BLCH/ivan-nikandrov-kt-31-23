namespace IvanNikandrovKt_31_23.Dto
{
    /// <summary>Фильтр списка специальностей. Пусто = фильтр не применяется.</summary>
    public class SpecialtyFilter
    {
        /// <summary>Название специальности целиком или частично.</summary>
        public string? Title { get; set; }

        /// <summary>Код специальности целиком или частично, например 09.02.07.</summary>
        public string? Code { get; set; }
    }

    /// <summary>Тело запроса для добавления и изменения специальности.</summary>
    public class SpecialtyRequest
    {
        public string? Title { get; set; }
        public string? Code { get; set; }
    }

    public record SpecialtyDto(int SpecialtyId, string? Title, string? Code);
}
