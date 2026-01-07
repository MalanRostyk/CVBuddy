using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class EducationVM
    {
        [StringLength(95)]
        public string? HighSchool { get; set; } // = NTI

        [StringLength(100)]
        public string? HSProgram { get; set; } // = Programmering
        public string? HSDate { get; set; } // = 2020-2023


        [StringLength(95)]
        public string? Univeristy { get; set; } // = Oru

        [StringLength(100)]
        public string? UniProgram { get; set; } // = Computer Science
        public string? UniDate { get; set; } // = 2023-

    }
}
