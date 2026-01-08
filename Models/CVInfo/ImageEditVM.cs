using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class ImageEditVM
    {
        //Används inte ta bort efter commit
        public int Cid { get; set; }

        //[NotMapped]
        [Required(ErrorMessage = "Upload an image please.")]
        [ExtensionValidation("jpg,png,jfif,webp")]
        public IFormFile ImageFile { get; set; } //Borde vara nullable
    }
}
