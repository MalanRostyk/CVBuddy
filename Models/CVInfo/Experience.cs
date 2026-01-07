using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Experience
    {
        [Key]
        public int Exid { get; set; }

        public string Title { get; set; } = "";

        public string? Description { get; set; }

        public string Company { get; set; } = "";

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int CvId { get; set; }
        [ForeignKey("CvId")]
        public Cv? Cv { get; set; }
    }
}
