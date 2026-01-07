using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class PersonalCharacteristicVM
    {

        [Required(ErrorMessage = "An added personal characteristic cannot be left empty.")]
        [StringLength(90)]
        public string CharacteristicName { get; set; }

    }
}
