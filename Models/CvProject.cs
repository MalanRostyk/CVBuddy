using CVBuddy.Models.CVInfo;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class CvProject
    {
        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public Cv OneCv { get; set; }

        public int ProjId { get; set; }
        [ForeignKey(nameof(ProjId))]
        public Project OneProject { get; set; }
    }
}
