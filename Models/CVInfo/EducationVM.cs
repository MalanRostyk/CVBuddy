using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models.CVInfo
{
    public class EducationVM
    {
        public int Eid { get; set; }

        [StringLength(95)]
        public string? HighSchool { get; set; }
        [StringLength(100)]
        public string? HSProgram { get; set; } 
        [Range(typeof(DateTime), "1900-01-01", "2049-12-31", ErrorMessage = "Date can only be after year 1900 and before year 2050.")]
        public string? HSDate { get; set; } 

        [StringLength(95)]
        public string? Univeristy { get; set; } 
        [StringLength(100)]
        public string? UniProgram { get; set; } 
        [Range(typeof(DateTime), "1900-01-01", "2049-12-31", ErrorMessage = "Date can only be after year 1900 and before year 2050.")]
        public string? UniDate { get; set; } 
    }
}
