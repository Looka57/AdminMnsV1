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
