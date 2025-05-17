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
        // L'ordre des datasets doit correspondre à leur création
        pieChartStagiaires.data.datasets[0].data = [nombreHommes, 0]; // Pour les hommes
        pieChartStagiaires.data.datasets[1].data = [0, nombreFemmes]; // Pour les femmes
        pieChartStagiaires.update();
    } else {
        // Sinon, créez un nouveau graphique
        pieChartStagiaires = new Chart(pieCanvas, {
            type: 'doughnut',
            data: {
                // Les labels globaux du graphique sont 'Hommes' et 'Femmes' pour les segments
                labels: ['Hommes', 'Femmes'],
                datasets: [{
                    // Dataset pour les HOMMES (anneau intérieur)
                    label: 'Hommes', // Label pour ce dataset
                    data: [nombreHommes, 0], // Valeur pour Hommes, 0 pour le segment 'Femmes' invisible
                    backgroundColor: ['rgba(21, 26, 55, 0.8)', 'rgba(21, 26, 55, 0.3)'], // Couleur pour hommes, couleur semi-transparente pour le segment à 0
                    borderColor: ['rgba(21, 26, 55, 1)', 'rgba(21, 26, 55, 0.5)'],
                    borderWidth: 1,
                    cutout: '70%', // Plus grand trou central = anneau plus fin et intérieur
                },
                {
                    // Dataset pour les FEMMES (anneau extérieur)
                    label: 'Femmes', // Label pour ce dataset
                    data: [0, nombreFemmes], // 0 pour le segment 'Hommes' invisible, valeur pour Femmes
                    backgroundColor: ['rgba(255, 102, 22, 0.3)', 'rgba(255, 102, 22, 0.8)'], // Couleur semi-transparente pour le segment à 0, couleur pour femmes
                    borderColor: ['rgba(255, 102, 22, 0.5)', 'rgba(255, 102, 22, 1)'],
                    borderWidth: 1,
                    cutout: '50%', // Plus petit trou central = anneau plus large et extérieur
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
                            // Ceci génère des labels de légende personnalisés
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
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                // context.dataset.label sera 'Hommes' ou 'Femmes' (le label du dataset survolé)
                                // context.raw sera la valeur du segment spécifique (nombreHommes, 0, ou nombreFemmes)

                                const datasetLabel = context.dataset.label; // 'Hommes' ou 'Femmes'
                                const value = context.raw;

                                // Si la valeur du segment est 0, on ne veut pas l'afficher dans le tooltip
                                if (value === 0) {
                                    return '';
                                }

                                // Calcul du total pour le pourcentage global
                                const totalGeneral = window.nombreHommes + window.nombreFemmes;
                                const percentage = totalGeneral > 0 ? ((value / totalGeneral) * 100).toFixed(2) : 0;

                                // Affiche le label du dataset (Hommes/Femmes), sa valeur et le pourcentage global
                                return `${datasetLabel}: ${value} (${percentage}%)`;
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