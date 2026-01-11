using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Education
    {
        [Key]
        public int Eid { get; set; }

        public string? HighSchool{ get; set; } 
        public string? HSProgram{ get; set; }
        public string? HSDate{ get; set; } 

        public string? Univeristy { get; set; }
        public string? UniProgram { get; set; } 
        public string? UniDate { get; set; } 

        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public Cv? Cv { get; set; }
    }
}