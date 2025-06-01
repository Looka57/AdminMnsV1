// wwwroot/js/styleGraphCandidature.js

// Attendez que le DOM soit entièrement chargé pour être sûr que l'élément Script 'chart-data' et le 'canvas' existent
document.addEventListener('DOMContentLoaded', function () {

    // 1. Récupérez l'élément script contenant les données JSON
    const chartDataElement = document.getElementById('chart-data');

    let classStats = [];
    if (chartDataElement) {
        try {
            // 2. Parsez la chaîne JSON pour obtenir l'objet JavaScript
            classStats = JSON.parse(chartDataElement.textContent);
            console.log("Données de statistiques de classe chargées depuis la vue :", classStats);
        } catch (e) {
            console.error("Erreur lors du parsing des données du graphique :", e);
            classStats = []; // Réinitialise pour éviter d'autres erreurs
        }
    } else {
        console.warn("L'élément Script avec l'ID 'chart-data' n'a pas été trouvé. Le graphique sera vide ou ne s'affichera pas.");
    }


    // Préparer les tableaux de données pour Chart.js
    const labels = classStats.map(stat => stat.className);
    const enCoursData = classStats.map(stat => stat.enCoursCount);
    const valideesData = classStats.map(stat => stat.valideesCount);

    console.log("Labels du graphique :", labels); // Pour le débogage
    console.log("Données 'En cours' :", enCoursData); // Pour le débogage
    console.log("Données 'Clôturés' :", valideesData); // Pour le débogage

    const fileCanvas = document.querySelector('#fileClass');

    // Vérifiez si le canvas a été trouvé avant d'initialiser le graphique
    if (!fileCanvas) {
        console.error("L'élément canvas avec l'ID 'fileClass' n'a pas été trouvé. Le graphique ne peut pas être initialisé.");
        return; // Arrête l'exécution si le canvas n'est pas là
    }

    // Initialisez le graphique Chart.js
    const fileChart = new Chart(fileCanvas, {
        type: 'bar',
        data: {
            labels: labels, // Utilisez les labels dynamiques
            datasets: [
                {
                    label: 'Dossiers en attente',
                    data: enCoursData, // Utilisez les données "En cours" dynamiques
                    backgroundColor: 'rgba(255, 102, 22 , 0.8)',
                    borderColor: 'rgba(255, 102, 22, 1)',
                    borderWidth: 1,
                    borderRadius: 25,
                },
                {
                    label: 'Dossiers clôturés',
                    data: valideesData, // Utilisez les données "Validé" dynamiques
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
                y: { // C'est l'axe des valeurs numériques (les comptes de dossiers)
                    beginAtZero: true, // Pour commencer l'axe à 0
                    ticks: {
                        // C'est ici que la magie opère !
                        stepSize: 1, // Force l'intervalle entre les ticks à 1
                        // Callback pour s'assurer que seuls les entiers sont affichés
                        callback: function (value) {
                            if (Number.isInteger(value)) {
                                return value;
                            }
                            return null; // N'affiche pas les valeurs non entières
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'top'
                },
                title: {
                    display: true,
                    text: 'Statut des Candidatures par Classe'
                }
            }
        },
    });
});