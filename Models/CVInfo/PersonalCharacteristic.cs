using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models.CVInfo
{
    public class PersonalCharacteristic
    {
        [Key]
        public int PCId { get; set; }
        public string CharacteristicName { get; set; }
    }
}
