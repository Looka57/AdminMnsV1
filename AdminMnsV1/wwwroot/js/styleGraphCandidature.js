// 7. Graphique des dossiers par classes
const fileCanvas = document.querySelector('#fileClass');

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
