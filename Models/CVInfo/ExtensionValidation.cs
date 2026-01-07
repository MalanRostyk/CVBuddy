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
            if (value != null)
            {
                if (value is IFormFile file)
                {
                    var extension = Path.GetExtension(file.FileName)
                        .TrimStart('.') //"" betyder string, '' betyder char, alltså character, tecken.
                        .ToLower();

                    if (extensionsArray.Contains(extension))//Vaidera om filens extension är rätt format som angetts i model klassen
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            return new ValidationResult($"Only filetypes allowed are: {extensionsString}");//Lägg till ett felmeddelande i ViewData.ModelState.Values 




            //if (value != null) //Om värdet på property är null, ge ModelState true för objektets property
            //    return ValidationResult.Success;

            ////Om propertys värde är en fil, ge ModelState true för objektets property
            //if (value is not IFormFile file)
            //    return ValidationResult.Success;

            ////Om värdet på property var en fil, hämta dens extension
            //var extension = Path.GetExtension(file.FileName)
            //    .TrimStart('.') //"" betyder string, '' betyder char, alltså character, tecken.
            //    .ToLower();



            //if (extensionsArray.Contains(extension))//Vaidera om filens extension är rätt format som angetts i model klassen
            //{
            //    return new ValidationResult($"Only filetypes allowed are: {extensionsString}");//Lägg till ett felmeddelande i ViewData.ModelState.Values 
            //}
            //return ValidationResult.Success;
        }
    }
}
