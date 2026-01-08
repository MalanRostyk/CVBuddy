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

        [Range(typeof(DateTime), "1900-01-01", "2049-12-31", ErrorMessage = "Date can only be after year 1900 and before year 2050.")]
        public string? HSDate { get; set; } // = 2020-2023


        [StringLength(95)]
        public string? Univeristy { get; set; } // = Oru

        [StringLength(100)]
        public string? UniProgram { get; set; } // = Computer Science


        [Range(typeof(DateTime), "1900-01-01", "2049-12-31", ErrorMessage = "Date can only be after year 1900 and before year 2050.")]
        public string? UniDate { get; set; } // Ska inte

    }
}
