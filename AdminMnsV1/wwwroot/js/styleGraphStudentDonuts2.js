// Contenu de ~/js/styleGraphStudentDonuts.js - CODE COMPLET ET CORRECT

// Déclarer la variable du graphique en dehors de la fonction pour pouvoir la mettre à jour plus tard
let pieChartStagiaires;

// Fonction pour initialiser ou mettre à jour le graphique des stagiaires
function initializeOrUpdateStagiaireChart() {
    const pieCanvas = document.querySelector('#stagiaireChart');

    // Vérifier si le canvas existe et si les données globales sont disponibles
    if (!pieCanvas || typeof window.nombreHommes === 'undefined' || typeof window.nombreFemmes === 'undefined') {
        // Si le canvas ou les données ne sont pas encore prêts, ne rien faire pour l'instant.
        // Cette fonction sera appelée à nouveau quand les données seront prêtes.
        return;
    }

    const nombreHommes = window.nombreHommes;
    const nombreFemmes = window.nombreFemmes;

    // Pour le débogage : Vérifiez les valeurs dans la console
    console.log('Tente de dessiner le graphique des stagiaires avec :', { nombreHommes, nombreFemmes });

    if (pieChartStagiaires) {
        // Si le graphique existe déjà, mettez à jour ses données
        pieChartStagiaires.data.datasets[0].data = [nombreHommes, nombreFemmes];
        pieChartStagiaires.update(); // Redessine le graphique avec les nouvelles données
    } else {
        // Sinon, créez un nouveau graphique
        pieChartStagiaires = new Chart(pieCanvas, {
            type: 'doughnut',
            data: {
                labels: ['Hommes', 'Femmes'],
                datasets: [{
                    label: 'Total Stagiaires', // Un label pour l'ensemble du graphique
                    data: [nombreHommes, nombreFemmes], // Les deux valeurs dans un seul dataset
                    backgroundColor: [
                        'rgba(21, 26, 55, 0.8)',   // Couleur pour les hommes
                        'rgba(255, 102, 22, 0.8)'  // Couleur pour les femmes
                    ],
                    borderColor: [
                        'rgba(21, 26, 55, 1)',
                        'rgba(255, 102, 22, 1)'
                    ],
                    borderWidth: 1,
                    cutout: '70%',
                }],
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
                        },
                    },
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const label = context.label || '';
                                const value = context.raw;
                                const total = context.chart.data.datasets[0].data.reduce((acc, val) => acc + val, 0);
                                const percentage = total > 0 ? ((value / total) * 100).toFixed(2) : 0;
                                return `${label}: <span class="math-inline">\{value\} \(</span>{percentage}%)`;
                            }
                        }
                    }
                },
            },
        });
    }
}

// Écoutez un événement personnalisé qui signale que les données sont prêtes
document.addEventListener('stagiaireDataReady', initializeOrUpdateStagiaireChart);

// Assurez-vous également que la fonction est appelée une fois que le DOM est complètement chargé,
// pour les cas où l'événement 'stagiaireDataReady' pourrait être déjà passé si Chart.js se charge très tard.
document.addEventListener('DOMContentLoaded', initializeOrUpdateStagiaireChart);