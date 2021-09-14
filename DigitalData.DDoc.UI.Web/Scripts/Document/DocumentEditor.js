DDocUi.prototype.EditDocument = function (grid) {
	ddoc.CreateModal('/Document/Edit', { documentId: ddoc.document.Id } , 'documentEditor', 'Propiedades del Documento', false, function () {
		ddoc.InitDocumentEditor(grid);
		if (ddoc.InitCustomDocumentEditor && typeof ddoc.InitCustomDocumentEditor == 'function') {
			ddoc.InitCustomDocumentEditor(grid);
		}
	});
};

DDocUi.prototype.InitDocumentEditor = function (grid) {

	(function setupControls () {
		
		$('#updateDocument').hide();
		$('#cancelEditDocument').hide();

		ddoc.controls = [];
		
		$('#frmEditDocument').find('.editableField').each(function (i, input) {
			if ($(input).prev().val() === 'DATETIME') {
				$(input).val($(input).val().substr(0, 10));
			}
			if ($(input).prev().val() === 'BIT') {
				$(input).data('orVal', $(this).is(':checked'));
			} else {
				$(input).data('orVal', $(this).val());
			}
		});
	})();

	(function setupEvents () {
		function onMenuBarOptionClick () {
			switch (this.id) {
				case 'editData':
					ddoc.EnableDocumentEdit();
					break;
				case 'editPages':
					ddoc.PagesEditor(grid);
					break;
				case 'deleteDocument':
					ddoc.DeleteDocument(grid);
					break;
			}
		}

		function onDialogButtonClick (e) {
			e.preventDefault();
			switch (this.id) {
				case 'updateDocument':
					ddoc.UpdateDocument(grid);
					break;
				case 'cancelEditDocument':
					ddoc.CancelDocumentEdit();
					break;
				case 'closeEditDocument':
					$('#documentEditor').dialog('destroy');
					break;
			}
		}

		$('.menu-bar', '#documentEditor').on('click', 'li', onMenuBarOptionClick);
		$('#frmEditDocument').on('click', 'button', onDialogButtonClick);
	})();
};

DDocUi.prototype.EnableDocumentEdit = function () {

	$('#frmEditDocument').find('input[type="text"]').each(function (i, input) {
		if ($(input).prev().val() === 'DATETIME') {
			var controlId = ddoc.controls.push(new DDoc.Controls.DatePicker(input)) - 1;
			ddoc.controls[controlId].init();
		}
	});

	$('#frmEditDocument').find('.editableField').prop('disabled', false);
	$('#closeEditDocument').hide();
	$('#updateDocument').show();
	$('#cancelEditDocument').show();
};

DDocUi.prototype.CancelDocumentEdit = function () {

	ddoc.Confirm('¿Está seguro que desea deshacer sus cambios?', function () {
		$.each(ddoc.controls, function (i, control) { control.destroy(); });
		ddoc.controls.length = 0;

		$('#frmEditDocument').find('.editableField').prop('disabled', true).each(function (i, input) {
			if ($(input).prev().val() === 'BIT') {
				$(input).prop('checked', $(input).data('orVal'));
			} else {
				$(input).val($(input).data('orVal'));
			}
		});
		$('#closeEditDocument').show();
		$('#updateDocument').hide();
		$('#cancelEditDocument').hide();
	});
};

DDocUi.prototype.ValidateDocumentFields = function () {
	var error = false;
	$('#frmEditDocument').find('input[type="text"]').each(function (i, input) {
		if ($(input).prev().prev().val() === 'False') {
			if ($(input).val() == '' || $(input).val() == null) {
				var fieldName = $(input).prev().prev().prev().val().trim();
				ddoc.ShowAlert('El campo ' + fieldName + ' es requerido');
				error = true;
				return false;
			}
		}
	});
	return !error;
};

DDocUi.prototype.UpdateDocument = function (grid) {

	if (!ddoc.ValidateDocumentFields()) {
		return;
	}

	var document = (function getFormData () {
		var data = {};
		$.each($('#frmEditDocument').serializeArray(), function (i, field) {
			data[field.name] = field.value;
		});
		$('input[type="checkbox"]', '#frmEditDocument').each(function (i, checkbox) {
			if ($(checkbox).is(':checked')) {
				data[checkbox.name] = 'True';
			} else {
				data[checkbox.name] = 'False';
			}
		});
		$.extend(data, {
			CollectionId: ddoc.document.CollectionId,
			Name: ddoc.document.Name,
			IsNew: "False"
		});
		return data;
	})();

	ddoc.POST('/Document/Save', document, function (response) {
		ddoc.document.Id = response.TextResult;
		ddoc.ShowAlert('Documento Guardado!', function () {
			$('#documentEditor').dialog('destroy');
			$('#documentPagesEditor').dialog('destroy');
			$('#documentAddPage').dialog('destroy');
			$('#documentAddScannerPage').dialog('destroy');
		});
		if (grid) {
			ddoc.GetPagedResults(grid.currentPage, grid.pageSize);
		} else {
			ddoc.GetPagedResults(ddoc.grdResults.currentPage, ddoc.grdResults.pageSize);
		}
	});
};

DDocUi.prototype.DeleteDocument = function (grid) {
	ddoc.Confirm('¿Está seguro que desea eliminar este documento?', function () {
		ddoc.POST('/Document/Delete', { documentId: ddoc.document.Id }, function () {
			ddoc.ShowAlert('Documento Eliminado!', function () { $('#documentEditor').dialog('destroy'); });
			if (grid) {
				ddoc.GetPagedResults(grid.currentPage, grid.pageSize);
			} else {
				ddoc.GetPagedResults(ddoc.grdResults.currentPage, ddoc.grdResults.pageSize);
			}
		});
	});
};