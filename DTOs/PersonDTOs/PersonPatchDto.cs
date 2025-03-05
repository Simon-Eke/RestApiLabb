using System.ComponentModel.DataAnnotations;

namespace RestApiLabb.DTOs.PersonDTOs
{
    public class PersonPatchDto
    {
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Full Name must be between 5 and 50 characters.")]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Email Address must be between 5 and 50 characters.")]
        public string? EmailAddress { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Phone number must be between 5 and 50 characters.")]
        public string? Phone { get; set; }

        [StringLength(300, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 300 characters.")]
        public string? Description { get; set; }
    }
}
