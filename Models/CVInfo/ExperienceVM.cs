using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class ExperienceVM
    {
        public int Exid { get; set; }
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
        [Range(typeof(DateTime), "1900-01-01", "2049-12-31", ErrorMessage = "Date can only be after year 1900 and before year 2050.")]
        //[DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        
        [DisplayName("End Date")]
        [Range(typeof(DateTime), "1900-01-01", "2049-12-31", ErrorMessage = "Date can only be after year 1900 and before year 2050.")]
        public DateTime? EndDate { get; set; }

    }
}
