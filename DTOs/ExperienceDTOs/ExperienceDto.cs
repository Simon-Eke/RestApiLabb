using System.ComponentModel.DataAnnotations;

namespace RestApiLabb.DTOs.ExperienceDTOs
{
    public class ExperienceDto
    {
        public string Company { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
    }
}
