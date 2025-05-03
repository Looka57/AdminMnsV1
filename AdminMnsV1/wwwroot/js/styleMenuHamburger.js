
// 2. Gestion du menu toggle
let toggle = document.querySelector('.toggle');
let navigation = document.querySelector('.navigation');
let mainMenu = document.querySelector('.mainDashboard ');
if (toggle && navigation && mainMenu) {
    toggle.addEventListener('click', () => {
        navigation.classList.toggle('active');
        mainMenu.classList.toggle('active');
    });
}

