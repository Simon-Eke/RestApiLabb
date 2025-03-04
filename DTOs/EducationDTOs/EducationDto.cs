namespace RestApiLabb.DTOs.EducationDTOs
{
    public class EducationDto
    {
        public string School { get; set; }
        public string Degree { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; } 
    }
}
