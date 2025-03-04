using System.ComponentModel.DataAnnotations;

namespace RestApiLabb.DTOs.PersonDTOs
{
    public class PersonCreateDto
    {
        [Required, StringLength(50, MinimumLength = 5)]
        public string FullName { get; set; }
        [Required, EmailAddress, StringLength(50, MinimumLength = 5)]
        public string Email { get; set; }
        [Required, Phone, StringLength(50, MinimumLength = 5)]
        public string MobileNumber { get; set; } 
        [Required, StringLength(300, MinimumLength = 5)]
        public string Description { get; set; }
    }
}
