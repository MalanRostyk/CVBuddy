// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



//let skillIndex = Model.SkillList.Count();

//function addSkill() {
//    let container = document.getElementById("Skill-Container");

//    let input1 = document.createElement("input");
//    input1.name = "Skills[$(skillIndex)]".ASkill;
//    input1.idName = "Multi-Skill";
//    input1.placeholder = "ASkill";

//    let input2 = document.createElement("input");
//    input2.name = "Skills[$(skillIndex)]".Description;
//    input2.idName = "Multi-Skill";
//    input2.placeholder = "Description";

//    let input3 = document.createElement("input");
//    input3.name = "Skills[$(skillIndex)]".Date;
//    input3.idName = "Multi-Skill";
//    input3.placeholder = "Date";

//    container.appendChild(input1);
//    container.appendChild(input2);
//    container.appendChild(input3);
//    skillIndex++;
//}

//Hämta notification HTML elementet som ska hanteras
const notification = document.getElementById("notification");

//Eventlyssnare som lyssnar på när domänen laddas om(när sidan refreshas på något sätt)
document.addEventListener("DOMContentLoaded", function () {
    ShowNotification();//Funktionen anropas då
});

//Funktionen som anropas
function ShowNotification() {
    const notReadCount = Number(notification.textContent)//parsear elementets text innehåll till ett nummer
    if (notReadCount > 0) {
        notification.classList.add("notification-container");//lägg till till styling för elemetet
    } else {
        notification.textContent = "";//tömm elementets text innehåll
        notification.classList.remove("notification-container");//ta bort styling, eftersom padding används, då när text innehållet är "" så syns elementets styling ändå, därför tas den bort när den inte har något innehåll, då syns inget
    }
}


function addSkill() {
    let containerSkills = document.getElementById("Skill-Container");
    let skillIndex = parseInt(containerSkills.dataset.skillIndex);
    containerSkills.insertAdjacentHTML('beforeend', `<p>Skill Title</p>
    <input type="text" name="Skills[${skillIndex}].ASkill" class="border-primary"/>
        <p>Skill Description</p>
    <input type="text" name="Skills[${skillIndex}].Description" class="border-primary"/>
        <p>Skill Date</p>
    <input type="date" name="Skills[${skillIndex}].Date" class="border-primary"/>`);
    skillIndex++;

}


function addExperience() { //LA TILL ASP-FOR="XXXXX" FÖR VALIDERING
    let containerExperience = document.getElementById("Experience-Container");
    let experienceIndex = parseInt(containerExperience.dataset.experienceIndex);
    containerExperience.insertAdjacentHTML('beforeend', `<p class="mb-0">Enter title for working experience</p>
     <input asp-for="Experiences[i].Title" type="text" name="Experiences[${experienceIndex}].Title"  class="border-primary"/>

     <p class="mb-0">Give a short description of the working experience</p>
     <input asp-for="Experiences[i].Description" type="text" name="@Experiences[${experienceIndex}].Description"  class="border-primary"/>
            
     <p class="mb-0">What company was this working experience at?</p>
     <input asp-for="Experiences[i].Company" type="text" name="@Experiences[${experienceIndex}].Company"  class="border-primary"/>
            
     <p class="mb-0">Enter a the starting date for your experience</p>
     <input asp-for="Experiences[i].StarDate" type="date" name="@Experiences[${experienceIndex}].StarDate"  class="border-primary"/> SDGAGDGADGADGAD

     <p class="mb-0">Enter a the ending date for your experience</p>
     <input asp-for="Experiences[i].EndDate" type="date" name="@Experiences[${experienceIndex}].EndDate"  class="border-primary"/>`);
    experienceIndex++;

}


function addCertificates() {
    let containerCertificates = document.getElementById("Certificates-Container");
    let certificatesIndex = parseInt(containerCertificates.dataset.certificatesIndex);
    containerCertificates.insertAdjacentHTML('beforeend', `<p class="mb-0">Add certificate</p>
            <input type="text" name="Certificates[${certificatesIndex}].CertName"  class="border-primary"/>`);
    certificatesIndex++;
}





function addPersonalCharacteristic() {
    let containerPersonalCharacteristics = document.getElementById("PersonalCharacteristics-Container");
    let personalCharacteristicsIndex = parseInt(containerPersonalCharacteristics.dataset.personalcharacteristicsIndex);
    containerPersonalCharacteristics.insertAdjacentHTML('beforeend', `<p class="mb-0">Add Personal Characteristics</p>
            <input type="text" name="PersonalCharacteristics[${personalCharacteristicsIndex}].CharacteristicName" class="border-primary"/>`);
    personalCharacteristicsIndex++;
}


function addInterests() {
    let containerInterests = document.getElementById("Interests-Container");
    let interestsIndex = parseInt(containerInterests.dataset.interestsIndex);
    containerInterests.insertAdjacentHTML('beforeend', `<p class="mb-0">Add your interest(s)</p>
            <input type="text" name="Interests[${interestsIndex}].InterestName" class="border-primary"/>`);
    interestsIndex++;
}

//let expIndex = Model.Experiences.Count();
//function addExperience() {
//    let container = document.getElementById("Experience-Container");

//    let input1 = document.createElement("input");
//    input1.name = "Experiences[$(expIndex)]".Title;
//    input1.idName = "Multi-Experience";
//    input1.placeholder = "Title";

//    let input2 = document.createElement("input");
//    input2.name = "Experiences[$(expIndex)]".Description;
//    input2.idName = "Multi-Experience";
//    input2.placeholder = "Description";

//    let input3 = document.createElement("input");
//    input3.name = "Experiences[$(expIndex)]".Company;
//    input3.idName = "Multi-Experience";
//    input3.placeholder = "Company";

//    let input4 = document.createElement("input");
//    input4.name = "Experiences[$(expIndex)]".Date;
//    input4.idName = "Multi-Experience";
//    input4.placeholder = "Date";

//    container.appendChild(input1);
//    container.appendChild(input2);
//    container.appendChild(input3);
//    container.appendChild(input4);
//    expIndex++;
//}


//let cerIndex = Model.Certificates.Count();
//function addCertificates() {
    
//    let container = document.getElementById("Certificates-Container");

//    let input1 = document.createElement("input");
//    input1.name = "Certificates[$(cerIndex)]".CertName;
//    input1.idName = "Multi-Certificates";
//    input1.placeholder = "CertName";

//    container.appendChild(input1);

//    cerIndex++;
//}

//let chaIndex = Model.PersonalCharacteristics.Count();
//function addPersonalCharacteristic() {
//    let container = document.getElementById("PersonalCharacteristics-Container");

//    let input1 = document.createElement("input");
//    input1.name = "PersonalCharacteristics[$(chaIndex)]".CharacteristicName;
//    input1.idName = "Multi-PersonalCharacteristics";
//    input1.placeholder = "CharacteristicName";

//    container.appendChild(input1);

//    chaIndex++;
//}
