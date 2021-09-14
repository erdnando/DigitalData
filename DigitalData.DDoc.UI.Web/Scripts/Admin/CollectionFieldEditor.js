
DDocUi.prototype.NewCollectionField = function () {
    ddoc.collectionField = { IsNew: true };
    ddoc.CreateModal('/Admin/NewCollectionField', undefined, 'fieldEditor', 'Nuevo campo de colección', { width: 710 }, ddoc.InitCollectionFieldEditor);
};

DDocUi.prototype.EditCollectionField = function() {
	ddoc.CreateModal('/Admin/EditCollectionField', ddoc.collectionField, 'fieldEditor', 'Editar campo de colección', { width: 710 }, ddoc.InitCollectionFieldEditor);
};

DDocUi.prototype.InitCollectionFieldEditor = function() {

	(function setupControls() {
		if ($('option:selected', '#TypeString').val().indexOf('CHAR') >= 0 ||
			$('option:selected', '#TypeString').val().indexOf('DECIMAL') >= 0) {
			$('#TypeLength').show().prop('disabled', false);
			$('label[for="TypeLength"]').show();
			if ($('option:selected', '#TypeString').val().indexOf('DECIMAL') >= 0) {
				$('#TypePrecision').show().prop('disabled', false);
				$('label[for="TypePrecision"]').show();
			}
		} else {
			$('#TypeLength').hide().prop('disabled', true);
			$('label[for="TypeLength"]').hide();
			$('#TypePrecision').hide().prop('disabled', true);
			$('label[for="TypePrecision"]').hide();
		}

		if (ddoc.collectionLock) {
			if (ddoc.collectionField.IsNew) {
				if (ddoc.colIsNew == undefined) {
					$('#Nullable', '#fieldEditor').click().prop('disabled', true);
				}
			} else {
				$('#TypeString', '#fieldEditor').prop('disabled', true);
				$('#TypeLength', '#fieldEditor').prop('disabled', true);
				$('#TypePrecision', '#fieldEditor').prop('disabled', true);
				$('#Nullable', '#fieldEditor').prop('disabled', true);
				$('#Inheritable', '#fieldEditor').prop('disabled', true);
				$('#Unique', '#fieldEditor').prop('disabled', true);
				$('#Hidden', '#fieldEditor').prop('disabled', true);
				$('#IncludeInGlobalSearch', '#fieldEditor').prop('disabled', true);
			}
		}
	})();

	(function setupEvents() {
		function initAllowedValuesEditor() {
			(function setupEvents() {
				function onAllowedValuesEditorButtonClick(e) {
					e.preventDefault();
					switch (this.id) {
						case 'saveAllowedValue':
							$('#AllowedValues').append($('<option>' + $('#AllowedValue').val() + '</option>'));
							$('#allowedValuesEditor').dialog('destroy');
							break;
						case 'cancelAllowedValueEdit':
							$('#allowedValuesEditor').dialog('destroy');
							break;
					}
				}

				$('.dialog-button-panel', '#allowedValuesEditor').on('click', 'button', onAllowedValuesEditorButtonClick);
			})();
		}

		function onDialogButtonClick(e) {
			e.preventDefault();
			switch (this.id) {
				case 'saveCollectionField':
					ddoc.SaveCollectionField();
					break;
				case 'cancelCollectionFieldEdit':
					$('#fieldEditor').dialog('destroy');
					break;
			}
		}

		function onFieldTypeChange() {
			if ($('option:selected', '#TypeString').val().indexOf('CHAR') >= 0 ||
				$('option:selected', '#TypeString').val().indexOf('DECIMAL') >= 0) {
				$('#TypeLength').show().prop('disabled', false);
				$('label[for="TypeLength"]').show();
				if ($('option:selected', '#TypeString').val().indexOf('DECIMAL') >= 0) {
					$('#TypePrecision').show().prop('disabled', false);
					$('label[for="TypePrecision"]').show();
				}
			} else {
				$('#TypeLength').hide().prop('disabled', true);
				$('label[for="TypeLength"]').hide();
				$('#TypePrecision').hide().prop('disabled', true);
				$('label[for="TypePrecision"]').hide();
			}
		}

		function onListboxActionClick() {
			switch (this.id) {
				case 'addAllowedValue':
					ddoc.CreateModal('/Admin/NewAllowedValue', undefined, 'allowedValuesEditor', 'Agregar nuevo valor', { width: 400 }, initAllowedValuesEditor);
					break;
				case 'removeAllowedValue':
					$('option:selected', '#AllowedValues').remove();
					break;
			}
		}

		$('#fieldEditor').on('click', 'button', onDialogButtonClick);
		$('.listbox-actions').on('click', '.listbox-action', onListboxActionClick);
		$('#TypeString').on('change', onFieldTypeChange);
	})();
};

DDocUi.prototype.ValidateCollectionField = function() {

	if ($('#Name', '#fieldEditor').val() == '') {
		ddoc.ShowAlert('Debe especificar un nombre para el campo');
		return false;
	}

	if (($('#TypeString').val().indexOf('CHAR') >= 0 || $('#TypeString').val().indexOf('DECIMAL') >= 0) && $('#TypeLength').val() == '') {
		ddoc.ShowAlert('Debe especificar una longitud para el campo');
		return false;
	}

	if (($('#TypeString').val().indexOf('DECIMAL') >= 0)
		&& $('#TypeLength').val() == '') {
		ddoc.ShowAlert('Debe especificar una precisión para el campo');
		return false;
	}

	return true;
};

DDocUi.prototype.SaveCollectionField = function() {

	if (!ddoc.ValidateCollectionField()) {
		return;
	}

	var collectionField = (function getFormData() {
		var data = {};

		var disabled = $('#frmCollectionFieldEdit').find(':input:disabled').removeAttr('disabled');

		$.each($('#frmCollectionFieldEdit').serializeArray(), function (i, field) {
			data[field.name] = field.value;
		});

		$('input[type="checkbox"]', '#frmCollectionFieldEdit').each(function (i, checkbox) {
			if ($(checkbox).is(':checked'))
				data[checkbox.id] = true;
			else {
				data[checkbox.id] = false;
			}
		});

		disabled.attr('disabled', 'disabled');

		if (data.TypeString.indexOf('CHAR') >= 0 || data.TypeString.indexOf('DECIMAL') >= 0) {
			data.TypeString += '(' + data.TypeLength;

			if (data.TypeString.indexOf('DECIMAL') >= 0) {
				data.TypeString += ',' + data.TypePrecision;
			}

			data.TypeString += ')';
		}

		var allowedValues = $('option', '#AllowedValues');

		if (allowedValues.length > 0) {

			data.AllowedValues = [];

			$.each(allowedValues, function (i, item) {
				var value = $(item).text();
				data.AllowedValues.push(value);
			});
		}

		if (data.IsNew === 'True') {
			data.IsNew = true;
		} else {
			data.IsNew = false;
		}

		if (ddoc.colIsNew && ddoc.collectionField.IsNew) {
			data.Id = ddoc.newFieldId;
			ddoc.newFieldId++;
		}

		if (ddoc.collectionLock && ddoc.colIsNew == undefined) {
			data.ForceStructChange = true;
		}

		return data;
	})();

	if (ddoc.colIsNew == undefined) {
        ddoc.POST('/Admin/SaveCollectionField', { collectionId: ddoc.collection.Id, field: collectionField }, function() {
            ddoc.ShowAlert('Campo guardado con éxito!', function() {
                ddoc.collectionField = undefined;
                $('#fieldEditor').dialog('destroy');
                ddoc.LoadCollections();
                ddoc.LoadCollectionFields();
            });
        });
    } else {
        if (ddoc.collectionField.IsNew) {
            collectionField.IsNew = false;
            ddoc.newCollectionFields.push(collectionField);
        } else {
            $.each(ddoc.newCollectionFields, function (i, field) {
                if (field.Id == collectionField.Id) {
                    ddoc.newCollectionFields[i] = collectionField;
                    return;
                }
            });
        }
        ddoc.grdNewCollectionFields.reload(ddoc.newCollectionFields);
        ddoc.collectionField = undefined;
        $('#fieldEditor').dialog('destroy');
    }
};