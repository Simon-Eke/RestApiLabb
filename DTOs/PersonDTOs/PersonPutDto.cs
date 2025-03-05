using System.ComponentModel.DataAnnotations;

namespace RestApiLabb.DTOs.PersonDTOs
{
    public class PersonPutDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Full Name must be between 5 and 50 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Email Address must be between 5 and 50 characters.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Phone number must be between 5 and 50 characters.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(300, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 300 characters.")]
        public string Description { get; set; }
    }
}
