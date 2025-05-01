// 1. Gestion du hover sur la liste de navigation et du responsive
document.addEventListener('DOMContentLoaded', function () {
    let list = document.querySelectorAll('.navigation li');
    if (list.length > 0) {
        function activeLink() {
            list.forEach((item) => {
                item.classList.remove('hovered');
                let img = item.querySelector('.icon img');
                if (img) {
                    img.classList.remove('iconHovered');
                }
            });
            this.classList.add('hovered');
            let img = this.querySelector('.icon img');
            if (img) {
                img.classList.add('iconHovered');
            }
        }
        list.forEach((item) => item.addEventListener('mouseover', activeLink));
    }

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

    // 3. Compteur de chiffres sur les cartes
    if (typeof $ !== 'undefined' && $.fn.counterUp) {
        $('.cardNumber').counterUp({
            delay: 10,
            time: 1200,
        });
    }

    // 4. Graphique des absences et retards
    const barCanvas = document.querySelector('#absencesChart');
    if (barCanvas && typeof Chart !== 'undefined') {
        const barChart = new Chart(barCanvas, {
            type: 'bar',
            data: {
                labels: ['Janvier', 'Février', 'Mars', 'Avril', 'Mai', 'Juin', 'Juillet', 'Août', 'Septembre', 'Octobre', 'Novembre', 'Décembre'],
                datasets: [
                    {
                        label: 'Retards',
                        data: [12, 9, 14, 10, 8, 6, 7, 9, 12, 11, 15, 10],
                        backgroundColor: 'rgba(255, 102, 22 , 0.8)',
                        borderColor: 'rgba(255, 102, 22, 1)',
                        borderWidth: 1,
                        borderRadius: 10,
                    },
                    {
                        label: 'Absences',
                        data: [5, 7, 6, 9, 10, 4, 3, 6, 8, 7, 5, 6],
                        backgroundColor: 'rgba(21, 26, 55, 0.8)',
                        borderColor: 'rgba(21, 26, 55, 1)',
                        borderWidth: 1,
                        borderRadius: 10,
                    },
                ],
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                    },
                },
            },
        });
    }

    // 5. Graphique en donut des stagiaires
    const pieCanvas = document.querySelector('#stagiaireChart');
    if (pieCanvas && typeof Chart !== 'undefined') {
        const pieChart = new Chart(pieCanvas, {
            type: 'doughnut',
            data: {
                labels: ['Hommes', 'Femmes'],
                datasets: [
                    {
                        label: 'Hommes',
                        data: [45, 0],
                        backgroundColor: ['rgba(21, 26, 55, 0.8)', 'rgba(21, 26, 55, 0.3)'],
                        borderColor: ['rgba(21, 26, 55, 1)', 'rgba(21, 26, 55, 0.5)'],
                        borderWidth: 1,
                        cutout: '70%',
                    },
                    {
                        label: 'Femmes',
                        data: [0, 30],
                        backgroundColor: ['rgba(255, 102, 22, 0.3)', 'rgba(255, 102, 22, 0.8)'],
                        borderColor: ['rgba(255, 102, 22, 0.5)', 'rgba(255, 102, 22, 1)'],
                        borderWidth: 1,
                        cutout: '50%',
                    },
                ],
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            usePointStyle: true,
                            font: { size: 14 },
                            generateLabels: function (chart) {
                                return [
                                    {
                                        text: 'Hommes',
                                        fillStyle: 'rgba(21, 26, 55, 0.8)',
                                        strokeStyle: 'rgba(21, 26, 55, 1)',
                                        lineWidth: 1,
                                        hidden: false,
                                    },
                                    {
                                        text: 'Femmes',
                                        fillStyle: 'rgba(255, 102, 22, 0.8)',
                                        strokeStyle: 'rgba(255, 102, 22, 1)',
                                        lineWidth: 1,
                                        hidden: false,
                                    },
                                ];
                            },
                        },
                    },
                },
            },
            plugins: [
                {
                    beforeDraw: function (chart) {
                        let width = chart.width,
                            height = chart.height,
                            ctx = chart.ctx;
                        if (window.innerWidth > 768) {
                            ctx.restore();
                            let image = new Image();
                            image.src = 'https://img.icons8.com/ios-filled/50/couple-posing.png';
                            let imageSize = 80;
                            let x = width / 2.37 - imageSize / 2,
                                y = height / 2 - imageSize / 2;
                            ctx.drawImage(image, x, y, imageSize, imageSize);
                            ctx.save();
                        }
                    },
                },
            ],
        });
    }

    // 6. Graphique polaire des classes
    const polarCanvas = document.querySelector('#polarChart');
    const polarCanvasLate = document.querySelector('#polarChartLate');

    if (polarCanvas && typeof Chart !== 'undefined') {
        const polarChart = new Chart(polarCanvas, {
            type: 'polarArea',
            data: {
                labels: ['CDA', 'FullStack', 'DevWeb', 'RAN1', 'Ran2', 'Réseau'],
                datasets: [
                    {
                        label: 'Dossiers traités',
                        data: [35, 40, 28, 50, 30, 45],
                        backgroundColor: ['rgba(21, 26, 55, 0.8)', 'rgba(21, 26, 55, 0.6)', 'rgba(21, 26, 55, 0.4)', 'rgba(255, 102, 22, 0.8)', 'rgba(255, 102, 22, 0.6)', 'rgba(255, 102, 22, 0.4)'],
                        borderColor: '#fff',
                        borderWidth: 2,
                    },
                ],
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    r: {
                        suggestedMin: 0,
                        suggestedMax: 60,
                        grid: { color: 'rgba(0, 0, 0, 0.1)' },
                        ticks: { backdropColor: 'transparent', font: { size: 14 } },
                    },
                },
                plugins: {
                    legend: { position: 'right', labels: { font: { size: 14 }, color: '#333' } },
                },
            },
        });
    }

    if (polarCanvasLate && typeof Chart !== 'undefined') {
        const polarChartLateInstance = new Chart(polarCanvasLate, {
            type: 'polarArea',
            data: {
                labels: ['CDA', 'FullStack', 'DevWeb', 'RAN1', 'Ran2', 'Réseau'],
                datasets: [
                    {
                        label: 'Dossiers traités',
                        data: [35, 40, 28, 50, 30, 45],
                        backgroundColor: ['rgba(21, 26, 55, 0.8)', 'rgba(21, 26, 55, 0.6)', 'rgba(21, 26, 55, 0.4)', 'rgba(255, 102, 22, 0.8)', 'rgba(255, 102, 22, 0.6)', 'rgba(255, 102, 22, 0.4)'],
                        borderColor: '#fff',
                        borderWidth: 2,
                    },
                ],
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    r: {
                        suggestedMin: 0,
                        suggestedMax: 60,
                        grid: { color: 'rgba(0, 0, 0, 0.1)' },
                        ticks: { backdropColor: 'transparent', font: { size: 14 } },
                    },
                },
                plugins: {
                    legend: { position: 'right', labels: { font: { size: 14 }, color: '#333' } },
                },
            },
        });
    }



    // 7. Graphique des dossiers par classes
    const fileCanvas = document.querySelector('#fileClass');
    if (fileCanvas && typeof Chart !== 'undefined') {
        const labels = ['cda', 'dev', 'dev2', 'Réseau', 'java', 'C#', 'ran1', 'ran4'];

        const fileChart = new Chart(fileCanvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Dossiers en attente',
                        data: [12, 9, 14, 10, 8, 6, 7, 9],
                        backgroundColor: 'rgba(255, 102, 22 , 0.8)',
                        borderColor: 'rgba(255, 102, 22, 1)',
                        borderWidth: 1,
                        borderRadius: 25,

                    },
                    {
                        label: 'Dossiers clôturés',
                        data: [7, 5, 9, 6, 4, 3, 8, 10],
                        backgroundColor: 'rgba(21, 26, 55, 0.8)',
                        borderColor: 'rgba(21, 26, 55, 1)',
                        borderWidth: 1,
                        borderRadius: 25,
                    },
                ],
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                    },
                },
            },
        });
    }































    // 7. Gestion du clic sur la carte pour afficher/cacher le tableau
    let card = document.getElementById('cardDevWeb2');
    let tableContainer = document.getElementById('tableContainer');
    if (card && tableContainer) {
        card.addEventListener('click', function () {
            if (tableContainer.style.display === 'none' || tableContainer.style.display === '') {
                tableContainer.style.display = 'block';
                tableContainer.style.width = '100%'; // Réappliquez la largeur ici
                tableContainer.classList.add('table-custom');
            } else {
                tableContainer.style.display = 'none';
            }
        });
    }
});

// DATEPICKER
document.addEventListener("DOMContentLoaded", function () {
    // Initialisation des deux Flatpickr
    let dateDebutPicker = flatpickr("#dateDebut", {
        dateFormat: "d/m/Y",
        minDate: "today",
        locale: "fr",
        allowInput: true,
        onChange: function (selectedDates, dateStr) {
            // Définit la date de fin minimum après la date de début choisie
            dateFinPicker.set("minDate", dateStr);
        }
    });

    let dateFinPicker = flatpickr("#dateFin", {
        dateFormat: "d/m/Y",
        minDate: "today",
        locale: "fr",
        allowInput: true
    });

    // Bouton de validation
    document.getElementById("validerAbsence").addEventListener("click", function () {
        let debut = document.getElementById("dateDebut").value;
        let fin = document.getElementById("dateFin").value;

        if (debut && fin) {
            alert("Période d'absence du " + debut + " au " + fin);
        } else {
            alert("Merci de sélectionner une date de début et une date de fin.");
        }
    });
});


document.addEventListener("DOMContentLoaded", function () {
    // Cibler tous les inputs type="file"
    const fileInputs = document.querySelectorAll("input[type='file']");

    fileInputs.forEach((input) => {
        input.addEventListener("change", function () {
            const file = input.files[0];

            if (file) {
                // ✅ Exemple de validation simple : fichier < 5 Mo
                const isValid = file.size <= 5 * 1024 * 1024;

                if (isValid) {
                    // 🔒 Griser l'input
                    input.disabled = true;
                    input.style.opacity = "0.6";

                    // 🟢 Passer le bouton à "Valider"
                    const row = input.closest("tr");
                    const btn = row.querySelector("button");

                    if (btn) {
                        btn.textContent = "Valider";
                        btn.classList.remove("btn-warning");
                        btn.classList.add("btn-success");
                    }
                } else {
                    alert("Fichier trop volumineux !");
                    input.value = ""; // réinitialiser
                }
            }
        });
    });
});



