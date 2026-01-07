using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class ExperienceVM
    {

        //[Required(ErrorMessage = "You need to enter what the experience title is, don't leave empty.")] //Få ha alla tecken vid fall att "ASP.NET" eller "Fork-lift license"
        [Required(ErrorMessage = "You need to enter what the experience title is, don't leave empty.")] //Få ha alla tecken vid fall att "ASP.NET" eller "Fork-lift license"
        [StringLength(90, MinimumLength = 3)]
        public string Title { get; set; } = ""; //GLÖM EJ ATT JAG ÄR TILLDELAD TOM STRÄNG


        [StringLength(120)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Don't leave company field empty, if it was a freelance experience enter ''Freelance''.")] //Få ha alla tecken vid fall att "ASP.NET" eller "Fork-lift license"
        [StringLength(90, MinimumLength = 2)]
        public string Company { get; set; } = "";

        [DisplayName("Start Date")]
        [Required(ErrorMessage = "An added experience must have a start date, dont leave unentered.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
