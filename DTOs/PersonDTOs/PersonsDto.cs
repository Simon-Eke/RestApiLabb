using RestApiLabb.DTOs.EducationDTOs;
using RestApiLabb.DTOs.ExperienceDTOs;
using RestApiLabb.Models;

namespace RestApiLabb.DTOs.PersonDTOs
{
    public class PersonsDto
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public List<EducationDto> Educations { get; set; }
        public List<ExperienceDto> Experiences { get; set; }
    }
}
