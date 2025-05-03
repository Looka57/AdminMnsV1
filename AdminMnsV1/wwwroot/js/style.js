

   


   
















    












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



