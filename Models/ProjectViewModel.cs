using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class ProjectViewModel
    {
        public int Pid { get; set; }

        [Required(ErrorMessage = "Title field can not be empty")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title can be 3-100 characters long")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description field can not be empty")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Description can be 3-200 characters long")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Please choose projects start date")]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1900-01-01", "2049-12-31", ErrorMessage = "Date can only be after year 1900 and before year 2050.")]
        public DateTime? StartDate { get; set; } 

        public DateTime? Enddate { get; set; } 

        public DateTime PublishDate { get; set; } = DateTime.Now;

        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

        [NotMapped]
        public List<User> UsersInProject { get; set; } = new();

        [NotMapped]
        public string? UserId { get; set; }
    }
}
