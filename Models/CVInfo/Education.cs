using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Education
    {
        [Key]
        public int Eid { get; set; } // = 1
        
        [StringLength(95)]
        public string? HighSchool{ get; set; } // = NTI

        [StringLength(100)]
        public string? HSProgram{ get; set; } // = Programmering
        public string? HSDate{ get; set; } // = 2020-2023


        [StringLength(95)]
        public string? Univeristy { get; set; } // = Oru

        [StringLength(100)]
        public string? UniProgram { get; set; } // = Computer Science
        public string? UniDate { get; set; } // = 2023-


        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public Cv? Cv { get; set; }

        public bool IsEmpty(string schoolType)
        {
            bool IsEmpty = false;
            schoolType = schoolType.ToLower();

            //Båda
            if(schoolType != "uni" && schoolType != "hs")
            {
                IsEmpty = string.IsNullOrEmpty(Univeristy)
                && string.IsNullOrEmpty(UniProgram)
                && string.IsNullOrEmpty(UniDate)
                && string.IsNullOrEmpty(HighSchool)
                && string.IsNullOrEmpty(HSProgram)
                && string.IsNullOrEmpty(HSDate);
            }

            //Uni
            if(schoolType == "uni")
            {
                IsEmpty = string.IsNullOrEmpty(Univeristy)
                && string.IsNullOrEmpty(UniProgram)
                && string.IsNullOrEmpty(UniDate);
            }

            //HighSchool
            if(schoolType == "hs")
            {
                IsEmpty = string.IsNullOrEmpty(HighSchool)
                && string.IsNullOrEmpty(HSProgram)
                && string.IsNullOrEmpty(HSDate);
            }

            return IsEmpty;
        }
        public bool IsPartiallyFilled(string schoolType)
        {
            if (IsEmpty(schoolType))
                return false;

            bool IsPartial = false;

            //Båda
            if (schoolType != "uni" && schoolType != "hs")
            {
                IsPartial = string.IsNullOrEmpty(Univeristy)
                || string.IsNullOrEmpty(UniProgram)
                || string.IsNullOrEmpty(UniDate);

                IsPartial = string.IsNullOrEmpty(HighSchool)
                || string.IsNullOrEmpty(HSProgram)
                || string.IsNullOrEmpty(HSDate);
            }

            //Uni
            if (schoolType == "uni")
            {
                IsPartial = string.IsNullOrEmpty(Univeristy)
                || string.IsNullOrEmpty(UniProgram)
                || string.IsNullOrEmpty(UniDate);
            }

            //HighSchool
            if (schoolType == "hs")
            {
                IsPartial = string.IsNullOrEmpty(HighSchool)
                || string.IsNullOrEmpty(HSProgram)
                || string.IsNullOrEmpty(HSDate);
            }

            return IsPartial;
        }

        public bool DateOnlyEntered(string schoolType)//Man ska inte tillåtas att bara ange datum för Education
        {

            if (IsEmpty(schoolType))
                return false;

            bool IsDateOnly = false;

            if (schoolType != "uni" && schoolType != "hs")
            {
                //Om University och UniProgram är null eller mellanslag
                //men ett UniDate är angett och... då blir IsDateOnlyTrue.
                IsDateOnly = string.IsNullOrEmpty(Univeristy)
                && string.IsNullOrEmpty(UniProgram)
                && !string.IsNullOrEmpty(UniDate)
                &&
                string.IsNullOrEmpty(HighSchool)
                && string.IsNullOrEmpty(HSProgram)
                && !string.IsNullOrEmpty(HSDate);
            }

            //Uni
            if (schoolType == "uni")
            {
                IsDateOnly = string.IsNullOrEmpty(Univeristy)
                && string.IsNullOrEmpty(UniProgram)
                && !string.IsNullOrEmpty(UniDate);
            }

            //HighSchool
            if (schoolType == "hs")
            {
                IsDateOnly = string.IsNullOrEmpty(HighSchool)
                && string.IsNullOrEmpty(HSProgram)
                && !string.IsNullOrEmpty(HSDate);
            }

            return IsDateOnly;
        }

    }
}
