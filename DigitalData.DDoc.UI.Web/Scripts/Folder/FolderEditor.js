DDocUi.prototype.NewFolder = function(grid) {
	ddoc.folder = { CollectionId: $('#CollectionId').val(), Name: $('#Name').val(), IsNew: true };
	ddoc.CreateModal('/Folder/New', { collectionId: ddoc.CollectionId }, 'folderEditor', 'Nuevo Folder', false, function() {
		ddoc.InitFolderEditor(grid);
	});
};

DDocUi.prototype.EditFolder = function(grid) {
	ddoc.CreateModal('/Folder/Edit', { folderId: ddoc.folder.Id }, 'folderEditor', 'Propiedades del Folder', false, function() {
		ddoc.InitFolderEditor(grid);
	});
};

DDocUi.prototype.InitFolderEditor = function(grid) {

	(function setupControls() {
		if (ddoc.folder.IsNew) {
			$('#frmEditFolder').find('input[type="text"], select, input[type="checkbox"]').prop('disabled', false);
		} else {
			$('#saveFolder').hide();
			$('#cancelEditFolder').hide();
		}
		ddoc.controls = [];
		$('#frmEditFolder').find('input[type="text"], select, input[type="checkbox"]').each(function (i, input) {
			if ($(input).prev().val() === 'DATETIME') {
				$(input).val($(input).val().substr(0, 10));
				if (ddoc.folder.IsNew) {
					var controlId = ddoc.controls.push(new DDoc.Controls.DatePicker(input)) - 1;
					ddoc.controls[controlId].init();
				}
			}
			if ($(input).prev().val() === 'BIT') {
				$(input).data('orVal', $(this).is(':checked'));
			}
			else {
				$(input).data('orVal', $(this).val());
			}
		});
	})();

	(function setupEvents() {
		function onMenuBarOptionClick() {
			switch (this.id) {
				case 'editData':
					ddoc.EnableFolderEdit();
					break;
				case 'deleteFolder':
					ddoc.DeleteFolder(grid);
					break;
			}
		}
		function onDialogButtonClick(e) {
			e.preventDefault();
			switch (this.id) {
				case 'cancelNewFolder':
					ddoc.Confirm('¿Está seguro que desa cancelar la creación del folder?', function() {
						$('#folderEditor').dialog('destroy');
					});
					break;
				case 'cancelEditFolder':
					ddoc.CancelFolderEdit();
					break;
				case 'closeEditFolder':
					$('#folderEditor').dialog('destroy');
					break;
				case 'saveFolder':
					ddoc.SaveFolder(grid);
					break;
			}
		}

		$('.menu-bar', '#folderEditor').on('click', 'li', onMenuBarOptionClick);
		$('#frmEditFolder').on('click', 'button', onDialogButtonClick);
	})();
};

DDocUi.prototype.EnableFolderEdit = function() {

	$('#frmEditFolder').find('input[type="text"]').each(function(i, input) {
		if ($(input).prev().val() === 'DATETIME') {
			var controlId = ddoc.controls.push(new DDoc.Controls.DatePicker(input)) - 1;
			ddoc.controls[controlId].init();
		}
	});

	$('#frmEditFolder').find('input[type="text"], select, input[type="checkbox"]').prop('disabled', false);
	$('#closeEditFolder').hide();
	$('#saveFolder').show();
	$('#cancelEditFolder').show();
};

DDocUi.prototype.CancelFolderEdit = function() {

	ddoc.Confirm('¿Está seguro que desea deshacer sus cambios?', function () {
		$.each(ddoc.controls, function (i, control) { control.destroy(); });
		ddoc.controls.length = 0;

		$('#frmEditFolder').find('input[type="text"], select, input[type="checkbox"]').prop('disabled', true).each(function (i, input) {
			if ($(input).prev().val() === 'BIT') {
				$(input).prop('checked', $(input).data('orVal'));
			}
			else {
				$(input).val($(input).data('orVal'));
			}
		});
		$('#closeEditFolder').show();
		$('#saveFolder').hide();
		$('#cancelEditFolder').hide();
	});
};

DDocUi.prototype.SaveFolder = function(grid) {

	var folder = (function getFormData() {

		var validationOk = (function validateFields() {
			var error = false;
			$('#frmEditFolder').find('input[type="text"]').each(function(i, input) {
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
		})();

		if (validationOk) {
			var data = {};
			$.each($('#frmEditFolder').serializeArray(), function(i, field) {
				data[field.name] = field.value;
			});
			$('input[type="checkbox"]', '#frmEditFolder').each(function (i, checkbox) {
				if ($(checkbox).is(':checked'))
					data[checkbox.id] = 'True';
				else {
					data[checkbox.id] = 'False';
				}
			});
			$.extend(data, {
				CollectionId: ddoc.folder.CollectionId,
				Name: ddoc.folder.Name,
				IsNew: ddoc.folder.IsNew
			});
			return data;
		} else {
			return undefined;
		}
	})();

	if (folder) {
		ddoc.POST('/Folder/Save', folder, function() {
			ddoc.ShowAlert('Folder Guardado!', function () {
				if (grid) {
					ddoc.GetPagedResults(grid.currentPage, grid.pageSize);
				} else {
					ddoc.GetPagedResults(ddoc.grdResults.currentPage, ddoc.grdResults.pageSize);
				}
				$('#folderEditor').dialog('destroy');
			});
		});
	}
};

DDocUi.prototype.DeleteFolder = function(grid) {

	ddoc.Confirm('¿Está seguro que desea eliminar este folder?', function () {
		ddoc.POST('/Folder/Delete', { folderId: ddoc.folder.Id }, function () {
			ddoc.ShowAlert('Folder Eliminado!', function () {
				if (grid) {
					ddoc.GetPagedResults(grid.currentPage, grid.pageSize);
				} else {
					ddoc.GetPagedResults(ddoc.grdResults.currentPage, ddoc.grdResults.pageSize);
				}
				$('#folderEditor').dialog('destroy');
			});
			
		});
	});
};