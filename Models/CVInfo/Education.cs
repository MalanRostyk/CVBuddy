using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Education
    {
        [Key]
        public int Eid { get; set; } // = 1
        public string? HighSchool{ get; set; } // = NTI
        public string? HSProgram{ get; set; } // = Programmering
        public string? HSDate{ get; set; } // = 2020-2023
        public string? Univeristy { get; set; } // = Oru
        public string? UniProgram { get; set; } // = Computer Science
        public string? UniDate { get; set; } // = 2023-

        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public Cv Cv { get; set; }
    }
}
