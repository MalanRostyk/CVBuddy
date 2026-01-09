using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models.CVInfo
{

    //Skapar en egen DataAnnotation attribut för att validera vilken filtyp som får laddas upp.
    public class ExtensionValidation : ValidationAttribute//Ärver från en subklass av DataAnnotations
    {
        private readonly string[] extensionsArray; //Innehåller tillåtna extensions
        private readonly string extensionsString;

        public ExtensionValidation(string extensions) //parametern här är det som skrivs i attributet i model
        {
            extensionsString = extensions;
            //tilldela arrayen varje extension
            extensionsArray = SplitExtensionStringIntoArray(extensions);
        }

        private string[] SplitExtensionStringIntoArray(string extensions)
        {
            return extensions.Split(",").Select(e => e.Trim().ToLower()).ToArray();
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            //Object? value = själva värdet som kommer från den property på den model som valideras
            //ValidationContext validationContext = objektet som valideras och dess properties
            
            if (value != null)//Om property på objektet som valideras är null, returnera felmeddelande till ViewData.ModelState.Values, är samma som [Requiered]
            {
                if (value is IFormFile file)//om value är IFormFile, skapa då en variabel som heter file,tilldela den values värde och anänd den i resten av i satsen
                {
                    var extension = Path.GetExtension(file.FileName)
                        .TrimStart('.') //"" betyder string, '' betyder char, alltså character, tecken.
                        .ToLower();

                    if (extensionsArray.Contains(extension))//Vaidera om filens extension är rätt format som angetts i model klassen
                    {
                        return ValidationResult.Success;
                    }
                    else//Har fel extension
                    {
                        return new ValidationResult($"Only filetypes allowed are: {extensionsString}");//Lägg till ett felmeddelande i ViewData.ModelState.Values 
                    }
                }
            }
            return null;//Inget ValidationResult, för att vi vill tillåta att bara error message från [Required] ska skrivas ut om IFormFile är null
        }
    }
}
