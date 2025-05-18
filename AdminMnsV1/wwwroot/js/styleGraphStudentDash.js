// Contenu de ~/js/styleGraphStudentDonuts.js - CODE COMPLET ET CORRECT

// Déclarer la variable du graphique en dehors de la fonction pour pouvoir la mettre à jour plus tard
let pieChartStagiaires;

// Fonction pour initialiser ou mettre à jour le graphique des stagiaires
// Cette fonction recevra l'objet d'événement en argument
function initializeOrUpdateStagiaireChart(event) {
    const pieCanvas = document.querySelector('#stagiaireChart');

    // Récupère les données directement de l'événement s'il est passé,
    // sinon utilise les variables globales comme fallback (moins robuste mais présent)
    const nombreHommes = event && event.detail ? event.detail.hommes : window.nombreHommes;
    const nombreFemmes = event && event.detail ? event.detail.femmes : window.nombreFemmes;

    // Vérifier si le canvas existe et si les données sont définies
    if (!pieCanvas || typeof nombreHommes === 'undefined' || typeof nombreFemmes === 'undefined') {
        // Si le canvas ou les données ne sont pas encore prêts, ne rien faire pour l'instant.
        // Cette fonction sera appelée à nouveau quand les données seront prêtes ou au DOMContentLoaded.
        return;
    }

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
                labels: ['Hommes', 'Femmes'], // Labels pour les segments (même si invisibles pour l'autre anneau)
                datasets: [{
                    // Dataset pour les HOMMES (anneau intérieur)
                    label: 'Hommes',
                    data: [nombreHommes, 0], // Hommes | L'autre segment est 0
                    backgroundColor: ['rgba(21, 26, 55, 0.8)', 'rgba(21, 26, 55, 0.3)'],
                    borderColor: ['rgba(21, 26, 55, 1)', 'rgba(21, 26, 55, 0.5)'],
                    borderWidth: 1,
                    cutout: '70%',
                },
                {
                    // Dataset pour les FEMMES (anneau extérieur)
                    label: 'Femmes',
                    data: [0, nombreFemmes], // L'autre segment est 0 | Femmes
                    backgroundColor: ['rgba(255, 102, 22, 0.3)', 'rgba(255, 102, 22, 0.8)'],
                    borderColor: ['rgba(255, 102, 22, 0.5)', 'rgba(255, 102, 22, 1)'],
                    borderWidth: 1,
                    cutout: '50%',
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
                                const datasetLabel = context.dataset.label;
                                const value = context.raw;

                                // Si la valeur du segment est 0, on ne veut pas l'afficher dans le tooltip
                                if (value === 0) {
                                    return '';
                                }

                                const totalGeneral = nombreHommes + nombreFemmes; // Utilisez les variables récupérées localement
                                const percentage = totalGeneral > 0 ? ((value / totalGeneral) * 100).toFixed(2) : 0;

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

// Écoutez également l'événement DOMContentLoaded comme solution de repli
// au cas où le script se chargerait après que l'événement stagiaireDataReady ait déjà été déclenché.
document.addEventListener('DOMContentLoaded', initializeOrUpdateStagiaireChart);