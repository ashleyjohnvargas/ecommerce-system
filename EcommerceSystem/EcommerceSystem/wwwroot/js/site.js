// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Show/Hide Password
function togglePasswordVisibility(fieldId, toggleIcon) {
	const passwordField = document.getElementById(fieldId);
	const icon = toggleIcon.querySelector("i");
	if (passwordField.type === "password") {
		passwordField.type = "text";
		icon.classList.remove("bx-hide");
		icon.classList.add("bx-show");
	} else {
		passwordField.type = "password";
		icon.classList.remove("bx-show");
		icon.classList.add("bx-hide");
	}
}