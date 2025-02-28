using System.ComponentModel.DataAnnotations;

namespace RestApiLabb.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [StringLength(50)]
        public string MobileNumber { get; set; }
        [Required]
        [StringLength(300)]
        public string Description { get; set; }
        public virtual List<Education> Educations { get; set; }
        public virtual List<Experience> Experiences { get; set; }
    }
}
