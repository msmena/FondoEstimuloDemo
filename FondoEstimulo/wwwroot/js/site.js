// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function DisableButtons() {
    var inputs = document.getElementsByClassName("btn");
for (var i in inputs) {
        if (inputs[i].type == "button" || inputs[i].type == "submit") {
    inputs[i].disabled = true;
        }
    }
}

window.onbeforeunload = DisableButtons;

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});