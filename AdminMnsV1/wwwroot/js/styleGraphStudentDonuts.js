
// 5. Graphique en donut des stagiaires
const pieCanvas = document.querySelector('#stagiaireChart');

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

