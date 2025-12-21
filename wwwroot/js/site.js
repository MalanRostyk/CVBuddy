// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let skillIndex = Model.SkillList.Count;

function addSkill() {
    let container = document.getElementById("Skills-Container");

    let input1 = document.createElement("input");
    input1.name = "Skills[$(skillIndex)]";
    input1.className = "Multi-Skill";
    input1.placeholder = "Skill";

    let input2 = document.createElement("input");
    input2.name = "Description";
    input2.className = "Multi-Skill";
    input2.placeholder = "Description";

    container.appendChild(input1);
    container.appendChild(input2);
    skillIndex++;
}
function addExperience() {
    let container = document.getElementById("Experience-Container");

    let input1 = document.createElement("input");
    input1.name = "Title";
    input1.className = "Multi-Experience";
    input1.placeholder = "Experiences";

    let input2 = document.createElement("input");
    input2.name = "Company";
    input2.className = "Multi-Experience";;
    input2.placeholder = "Company";

    let input2 = document.createElement("input");
    input3.name = "Date";
    input3.className = "Multi-Experience";;
    input3.placeholder = "Date";

    container.appendChild(input1);
    container.appendChild(input2);
    container.appendChild(input3);
    skillIndex++;
}
