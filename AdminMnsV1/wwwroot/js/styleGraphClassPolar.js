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
