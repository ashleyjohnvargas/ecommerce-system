//const container = document.getElementById('container');
//const registerBtn = document.getElementById('register');
//const loginBtn = document.getElementById('login');

//registerBtn.addEventListener('click', () => {
//    container.classList.add("active");

//});

//loginBtn.addEventListener('click', () => {
//    container.classList.remove("active");
//});

    document.addEventListener("DOMContentLoaded", function () {
        var activePanel = document.getElementById("activePanel").value;
    var container = document.getElementById("container");

    if (activePanel === "register") {
        container.classList.add("active");
        }
    });

    const container = document.getElementById('container');
    const registerBtn = document.getElementById('register');
    const loginBtn = document.getElementById('login');

    registerBtn.addEventListener('click', () => {
        container.classList.add("active");
    });

    loginBtn.addEventListener('click', () => {
        container.classList.remove("active");
    });

