namespace IvanNikandrovKt_31_23.Models
{
    public class Specialty
    {
        public int SpecialtyId { get; set; }
        public string? Title { get; set; }
        public string? Code { get; set; }

        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
