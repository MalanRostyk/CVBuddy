// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let skillIndex = Model.SkillList.Count;

function addSkill() {
    let container = document.getElementById("Skills-Container");

    let input1 = document.createElement("input");
    input1.name = "Skills[$(skillIndex)]".ASkill;
    input1.className = "Multi-Skill";
    input1.placeholder = "ASkill";

    let input2 = document.createElement("input");
    input2.name = "Skills[$(skillIndex)]".Description;
    input2.className = "Multi-Skill";
    input2.placeholder = "Description";

    let input3 = document.createElement("input");
    input3.name = "Skills[$(skillIndex)]".Date;
    input3.className = "Multi-Skill";
    input3.placeholder = "Date";

    container.appendChild(input1);
    container.appendChild(input2);
    container.appendChild(input3);
    skillIndex++;
}

let expIndex = Model.Experiences.Count;
function addExperience() {
    let container = document.getElementById("Experience-Container");

    let input1 = document.createElement("input");
    input1.name = "Experiences[$(expIndex)]".Title;
    input1.className = "Multi-Experience";
    input1.placeholder = "Title";

    let input2 = document.createElement("input");
    input2.name = "Experiences[$(expIndex)]".Description;
    input2.className = "Multi-Experience";
    input2.placeholder = "Description";

    let input3 = document.createElement("input");
    input3.name = "Experiences[$(expIndex)]".Company;
    input3.className = "Multi-Experience";
    input3.placeholder = "Company";

    let input4 = document.createElement("input");
    input4.name = "Experiences[$(expIndex)]".Date;
    input4.className = "Multi-Experience";
    input4.placeholder = "Date";

    container.appendChild(input1);
    container.appendChild(input2);
    container.appendChild(input3);
    container.appendChild(input4);
    expIndex++;
}


let cerIndex = Model.Certificates.Count;
function addCertificates() {
    let container = document.getElementById("Certificates-Container");

    let input1 = document.createElement("input");
    input1.name = "Certificates[$(expIndex)]";
    input1.className = "Multi-Certificates";
    input1.placeholder = "";

    container.appendChild(input1);

    cerIndex++;
}

let chaIndex = Model.PersonalCharacteristics.Count;
function addCertificates() {
    let container = document.getElementById("PersonalCharacteristics-Container");

    let input1 = document.createElement("input");
    input1.name = "PersonalCharacteristics[$(expIndex)]";
    input1.className = "Multi-PersonalCharacteristics";
    input1.placeholder = "";

    container.appendChild(input1);

    chaIndex++;
}
