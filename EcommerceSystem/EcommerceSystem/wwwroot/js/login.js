const container = document.getElementById('container');
const registerBtn = document.getElementById('register');
const loginBtn = document.getElementById('login');

registerBtn.addEventListener('click', () => {
    container.classList.add("active");
    setTimeout(() => { document.title = "Sign Up"; }, 0); // Update the title
});

loginBtn.addEventListener('click', () => {
    container.classList.remove("active");
    setTimeout(() => { document.title = "Sign In"; }, 0); // Update the title
});