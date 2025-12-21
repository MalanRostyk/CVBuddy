// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


//Javascript för att kunna lägga till en till Skill att lägga till i cv
let skillIndex = Model.SkillList.Count;

function addSkill() {
    let container = document.getElementById("Skills-Container");

    let input = document.createElement("input");
    input.name = "Skills[$(skillIndex)]";
    input.className = "form-control";
    input.placeholder = "Skill";

    container.appendChild(input);
    skillIndex++;
}