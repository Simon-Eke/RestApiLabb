using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiLabb.Models
{
    public class Experience
    {
        [Key]
        public int ExperienceId { get; set; }
        [StringLength(50)]
        public string Company { get; set; }
        [StringLength(50)]
        public string JobTitle { get; set; }
        [StringLength(300)]
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateOnly StartYear { get; set; }
        [DataType(DataType.Date)]
        public DateOnly? EndYear { get; set; }
        [ForeignKey("Person")]
        public int PersonId_FK { get; set; }
        public virtual Person Person { get; set; }
    }
}
