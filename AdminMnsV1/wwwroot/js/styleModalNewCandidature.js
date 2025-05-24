
<script>
    $(document).ready(function () {
        // Fonction pour charger les classes
        function loadClasses() {
            $.ajax({
                url: '/api/classes', // Point d'API pour récupérer les classes
                method: 'GET',
                success: function (data) {
                    var select = $('#classeSelect');
                    select.empty();
                    select.append($('<option>', { value: '', text: 'Sélectionner une classe' }));
                    $.each(data, function (i, classe) {
                        select.append($('<option>', {
                            value: classe.classeId,
                            text: classe.name
                        }));
                    });
                },
                error: function () {
                    alert('Erreur lors du chargement des classes.');
                }
            });
        }

			// Fonction pour charger les types de documents en fonction de la classe sélectionnée
			function loadDocumentTypesForClass(classeId) {
				var container = $('#documentTypesCheckboxes');
    container.empty();
    if (!classeId) {
        container.append('<p class="text-muted">Sélectionnez une classe pour voir les documents par défaut.</p>');
    return;
				}

    $.ajax({
        url: '/api/classes/' + classeId + '/documenttypes', // Point d'API pour les documents par classe
    method: 'GET',
    success: function (data) {
						if (data.length === 0) {
        container.append('<p class="text-muted">Aucun document par défaut pour cette classe. Veuillez sélectionner manuellement ci-dessous.</p>');
						}
    // Afficher les documents par défaut de la classe avec des checkboxes cochées
    $.each(data, function (i, docType) {
							var isChecked = docType.isMandatoryByDefault ? 'checked' : '';
    container.append(
    `<div class="form-check">
        <input class="form-check-input" type="checkbox" value="${docType.documentTypeId}" id="docType_${docType.documentTypeId}" name="SelectedDocumentTypeIds" ${isChecked}>
            <label class="form-check-label" for="docType_${docType.documentTypeId}">
                ${docType.typeName} ${docType.isMandatoryByDefault ? ' (par défaut)' : ''}
            </label>
    </div>`
    );
						});

						// Charger TOUS les types de documents disponibles pour que l'administrateur puisse en rajouter
						loadAllDocumentTypes(container, data.map(d => d.documentTypeId));
					},
    error: function () {
        alert('Erreur lors du chargement des types de documents pour la classe.');
					}
				});
			}

    // Fonction pour charger tous les types de documents (pour ajout manuel par l'admin)
    function loadAllDocumentTypes(container, existingDocTypeIds) {
        $.ajax({
            url: '/api/documenttypes', // Point d'API pour récupérer tous les types de documents
            method: 'GET',
            success: function (allDocTypes) {
                // Ajouter une séparation
                if (existingDocTypeIds.length > 0) {
                    container.append('<hr class="my-3">');
                    container.append('<p class="text-muted">Documents supplémentaires (facultatif) :</p>');
                } else {
                    container.append('<p class="text-muted">Sélectionnez les documents requis :</p>');
                }


                $.each(allDocTypes, function (i, docType) {
                    // N'ajoute la checkbox que si elle n'a pas déjà été ajoutée comme document par défaut de la classe
                    if (!existingDocTypeIds.includes(docType.documentTypeId)) {
                        container.append(
                            `<div class="form-check">
										<input class="form-check-input" type="checkbox" value="${docType.documentTypeId}" id="docType_${docType.documentTypeId}" name="SelectedDocumentTypeIds">
										<label class="form-check-label" for="docType_${docType.documentTypeId}">
											${docType.typeName}
										</label>
									</div>`
                        );
                    }
                });
            },
            error: function () {
                alert('Erreur lors du chargement de tous les types de documents.');
            }
        });
			}


    // Écouteur de changement pour la sélection de classe
    $('#classeSelect').change(function () {
				var selectedClasseId = $(this).val();
    loadDocumentTypesForClass(selectedClasseId);
			});

    // Charger les classes au chargement de la modale
    $('#newDossierModal').on('show.bs.modal', function () {
        loadClasses();
    $('#documentTypesCheckboxes').empty().append('<p class="text-muted">Sélectionnez une classe pour voir les documents par défaut.</p>');
			});

    // Gestion de la soumission du formulaire (via AJAX si possible pour meilleure UX)
    $('#createDossierForm').submit(function (e) {
        e.preventDefault(); // Empêche la soumission normale du formulaire

    var formData = $(this).serialize(); // Serialize les données du formulaire
    var selectedDocTypes = $('input[name="SelectedDocumentTypeIds"]:checked').map(function() {
					return $(this).val();
				}).get();

    formData += '&' + $.map(selectedDocTypes, function(val, i) {
					return 'SelectedDocumentTypeIds[' + i + ']=' + val;
				}).join('&');


    $.ajax({
        url: $(this).attr('asp-action'),
    method: $(this).attr('method'),
    data: formData,
    success: function (response) {
						if (response.success) {
        alert('Dossier créé avec succès et email envoyé !');
    $('#newDossierModal').modal('hide'); // Ferme la modale
    location.reload(); // Recharge la page pour voir le nouveau dossier
						} else {
        alert('Erreur: ' + response.errorMessage);
						}
					},
    error: function (xhr) {
        alert('Une erreur est survenue lors de la création du dossier.');
    console.error(xhr.responseText);
					}
				});
			});
		});
</script>