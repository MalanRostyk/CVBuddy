using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models.CVInfo
{
    public abstract class Education : Experience
    {
        public string? HighSchool{ get; set; } // = NTI
        public string? HSProgram{ get; set; } // = Programmering
        public string? HSDate{ get; set; } // = 2020-2023
        public string? Univeristy { get; set; } // = Oru
        public string? UniProgram { get; set; } // = Computer Science
        public string? UniDate { get; set; } // = 2023-

    }
}
