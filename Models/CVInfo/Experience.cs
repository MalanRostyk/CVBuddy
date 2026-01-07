using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Experience
    {
        [Key]
        public int Exid { get; set; }

        [Required] //Få ha alla tecken vid fall att "ASP.NET" eller "Fork-lift license"
        //[StringLength(90, MinimumLength = 2)]
        public string Title { get; set; }


        [StringLength(120)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Don't leave company field empty, if it was a freelance experience enter ''Freelance''.")] //Få ha alla tecken vid fall att "ASP.NET" eller "Fork-lift license"
        [StringLength(90, MinimumLength = 2, ErrorMessage = "You must enter a company name with 2-90 characters.")]
        public string Company { get; set; }

        //[Required(ErrorMessage = "An added experience must have a start date, dont leave unentered.")]
        //[DataType(DataType.Date)]//Måste ha en viewmodel för experience, dataannotations
        //                         //hinner inte se att webbläsaren gör värdet till tom sträng innan
        //                         //datannotations får se det.Så måste ha en viewmodel med nullable datetime här
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int CvId { get; set; }
        [ForeignKey("CvId")]
        public Cv? Cv { get; set; }
    }
}
