using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class PersonalCharacteristic
    {
        [Key]
        public int PCId { get; set; }
        public string CharacteristicName { get; set; }

        public int CvId { get; set; }
        [ForeignKey("CvId")]
        public Cv Cv { get; set; }
    }
}
