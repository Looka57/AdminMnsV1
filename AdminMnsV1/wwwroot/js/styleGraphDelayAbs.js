
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