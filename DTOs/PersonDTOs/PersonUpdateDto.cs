using System.ComponentModel.DataAnnotations;

namespace RestApiLabb.DTOs.PersonDTOs
{
    public class PersonUpdateDto
    {
        [StringLength(50, MinimumLength = 5)]
        public string? FullName { get; set; }
        [EmailAddress, StringLength(50, MinimumLength = 5)]
        public string? EmailAddress { get; set; }
        [Phone, StringLength(50, MinimumLength = 5)]
        public string? Phone { get; set; }
        [StringLength(300, MinimumLength = 5)]
        public string? Description { get; set; }
    }
}
