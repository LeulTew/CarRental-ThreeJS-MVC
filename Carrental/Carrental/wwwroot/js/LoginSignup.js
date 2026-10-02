document.addEventListener('DOMContentLoaded', function () {
    const loginLink = document.querySelector(".form-container a");
    const login2main = document.querySelector(".login2main");
    const rightside = document.getElementById("rightside")
    const loginContainer = document.querySelector(".form-container");
    const signupForm = document.getElementById('signupForm');
    const loginForm = document.getElementById('loginForm');
    const showLoginFormLink = document.getElementById('showLoginForm');
    const showSignupFormLink = document.getElementById('showSignupForm');

    // Function to show login form and hide signup form
    function showLoginForm() {
        signupForm.style.display = 'none';
        loginForm.style.display = 'block';
        if (window.innerWidth >= 800) {
            loginContainer.classList.add("slide-right");
            setTimeout(function () {
                rightside.style.display = 'none';
                loginContainer.style.width = '50%';
            }, 450);
            // After the animation ends
            setTimeout(function () {
                login2main.classList.toggle("flex-reverse");
                loginContainer.classList.remove("slide-right");
                rightside.style.display = 'flex';
            }, 700); // Adjust this value to match the animation duration
        } else {
            login2main.classList.toggle("flex-reverse");
        }
    }

    // Function to show signup form and hide login form
    function showSignupForm() {
        if (window.innerWidth >= 800) {
            signupForm.style.display = 'block';
            loginForm.style.display = 'none';
            loginContainer.classList.add("slide-left");
            setTimeout(function () {
                rightside.style.display = 'none';
                loginContainer.style.width = '50%';
            }, 450);
            // After the animation ends
            setTimeout(function () {
                login2main.classList.toggle("flex-reverse");
                loginContainer.classList.remove("slide-left");
                rightside.style.display = 'flex';
            }, 900); // Adjust this value to match the animation duration
        } else {
            signupForm.style.display = 'block';
            loginForm.style.display = 'none';
            rightside.style.display = 'none';
            loginContainer.style.width = '100%';
        }
    }

    // Show login form when clicking the login link
    loginLink.addEventListener('click', function (event) {
        event.preventDefault();
        showLoginForm();
    });

    // Show login form when clicking the login form link
    showLoginFormLink.addEventListener('click', function (event) {
        event.preventDefault();
        showLoginForm();
    });

    // Show signup form when clicking the signup form link
    showSignupFormLink.addEventListener('click', function (event) {
        event.preventDefault();
        showSignupForm();
    });

    // Check if the current URL path contains "Login" to show the login form
    const currentPath = window.location.pathname;
    if (currentPath.toLowerCase().includes('login')) {
        showLoginForm();
    } else {
        loginForm.style.display = 'none';
    }

    // Add event listener to resize event to check viewport width
    window.addEventListener('resize', function () {
        if (window.innerWidth < 800) {
            loginContainer.style.width = '100%';
            rightside.style.display = 'none';
        }
        else {
            loginContainer.style.width = '50%';
            rightside.style.display = 'flex';

        }
    });
});
