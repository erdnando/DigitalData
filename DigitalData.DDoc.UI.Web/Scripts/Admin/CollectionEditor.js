
DDocUi.prototype.NewRootCollection = function () {
    ddoc.colType = 1;
    ddoc.colIsNew = true;
	ddoc.CreateModal('/Admin/NewRootCollection', undefined, 'collectionEditor', 'Nueva Colección Raíz', { width: 650 }, ddoc.InitCollectionEditor);
};

DDocUi.prototype.AddFolderCollection = function () {
    ddoc.newFieldId = 1;
    ddoc.colType = 2;
    ddoc.colIsNew = true;
    ddoc.CreateModal('/Admin/NewFolderCollection', { parentCollectionId: ddoc.collection.Id, securityGroupId: ddoc.collection.SecurityGroupId }, 'collectionEditor', 'Nueva Colección de Folders', { width: 800 }, ddoc.InitCollectionEditor);
};

DDocUi.prototype.AddDocumentCollection = function () {
    ddoc.newFieldId = 1;
    ddoc.colType = 3;
    ddoc.colIsNew = true;
    ddoc.CreateModal('/Admin/NewDocumentCollection', { parentCollectionId: ddoc.collection.Id, securityGroupId: ddoc.collection.SecurityGroupId }, 'collectionEditor', 'Nueva Colección de Documentos', { width: 800 }, ddoc.InitCollectionEditor);
};

DDocUi.prototype.EditCollection = function () {
    ddoc.CreateModal('/Admin/EditCollection', ddoc.collection, 'collectionEditor', 'Editar Colección', { width: 600 }, ddoc.InitCollectionEditor);
};

DDocUi.prototype.InitCollectionEditor = function() {

    if (ddoc.colIsNew && ddoc.colType > 1) {
        ddoc.newCollectionFields = [];
        ddoc.newParentFields = [];
    } else {
        ddoc.newCollectionFields = undefined;
        ddoc.newParentFields = undefined;
	}

	(function setupControls() {
		function setupGrid() {
			function onGridAction(e) {
				ddoc.collectionField = e.dataRow;
				if (ddoc.collectionField.TypeString.indexOf('CHAR') >= 0) {
					ddoc.collectionField.TypeString = ddoc.collectionField.TypeString.substr(0, ddoc.collectionField.TypeString.indexOf('('));
				}
				switch (e.command) {
					case 'editNewField':
						ddoc.EditCollectionField();
						break;
					case 'moveNewUp':
						ddoc.MoveNewCollectionField(1);
						break;
					case 'moveNewDown':
						ddoc.MoveNewCollectionField(0);
						break;
					case 'deleteNewField':
						ddoc.DeleteNewCollectionField(ddoc.collectionField.Id);
						break;
				}
			};

			ddoc.grdNewCollectionFields = new DDoc.Controls.Grid('grdNewCollectionFields', { height: 200, showTools: false, pager: { show: false } });

			var colType = DDoc.Constants.columnType;
			var columns = [
				{ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'editNewField', cssClass: { normal: 'imgEditField' }, tooltip: 'Editar campo' } },
				{ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'moveNewUp', cssClass: { normal: 'imgMoveUp' }, tooltip: 'Subir' } },
				{ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'moveNewDown', cssClass: { normal: 'imgMoveDown' }, tooltip: 'Bajar' } },
				{ text: 'Nombre', field: 'Name', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Tipo', field: 'TypeString', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Longitud', field: 'TypeLength', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Opcional', field: 'Nullable', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Heredable', field: 'Inheritable', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'deleteNewField', cssClass: { normal: 'imgDeleteField' }, tooltip: 'Eliminar campo' } }
			];

			ddoc.grdNewCollectionFields.init(columns);
			ddoc.grdNewCollectionFields.addListener('onAction', onGridAction);
		}

		if (ddoc.colIsNew && ddoc.colType > 1) {
			setupGrid();
			$('#collectionEditor').dialog('option', 'position', { my: 'center', at: 'center', of: window });
        }

        ddoc.GET('/Collection/GetFields', { collectionId: $('#ParentCollectionId').val() }, function (response) {
	        $.each(response.List, function (i, field) {
                if (field.TypeString.indexOf('CHAR') > 0) {
                    field.TypeString = field.TypeString + '(' + field.TypeLength + ')';
                }
                $('#ParentFields').append($('<option>').data(field).val(field.Id).text(field.Name));
	        });
        });
	})();

	(function setupEvents() {
		function onDialogButtonClick(e) {
			e.preventDefault();
			switch (this.id) {
				case 'saveCollection':
					ddoc.SaveCollection();
					break;
				case 'cancelCollectionEdit':
					$('#collectionEditor').dialog('destroy');
					break;
			}
		}

		function onWarehouseIdChange() {
			if ($('option:selected', '#WarehouseId').val() == 0) {
				$('label[for="NewWarehousePath"]').show();
				$('#NewWarehousePath').show().prop('disabled', false);
			} else {
				$('label[for="NewWarehousePath"]').hide();
				$('#NewWarehousePath').hide().prop('disabled', true);
			}
		}
		
		$('#collectionEditor').on('click', 'button', onDialogButtonClick);
		$('#WarehouseId').on('change', onWarehouseIdChange);
		$('#editFilenameTemplate').on('click', ddoc.EditFilenameTemplate);
        $('#addNewField').on('click', ddoc.NewCollectionField);
		$('#ParentFields').on('dblclick', 'option', ddoc.InsertParentField);
	})();
};

DDocUi.prototype.ValidateCollection = function () {

	if ($('#Name').val() == '') {
		ddoc.ShowAlert('Debe especificar un nombre para la colección');
		return false;
	}

	if (ddoc.colIsNew && ddoc.colType > 1 && ddoc.newCollectionFields.length == 0) {
		ddoc.ShowAlert('Debe agregar campos a la nueva colección');
		return false;
	}

	return true;
};


DDocUi.prototype.SaveCollection = function() {

	if (!ddoc.ValidateCollection()) {
		return;
	}

	var collection = (function getFormData() {
		var data = {};

		$.each($('#frmEditCollection').serializeArray(), function(i, field) {
			data[field.name] = field.value;
        });

        if (data.SecurityGroupId === '')
            data.SecurityGroupId = 0;

		$('input[type="checkbox"]', '#frmEditCollection').each(function(i, checkbox) {
			if ($(checkbox).is(':checked'))
				data[checkbox.id] = true;
			else {
				data[checkbox.id] = false;
			}
		});

		if (data.IsNew === 'True') {
            data.IsNew = true;
            data.Type = ddoc.colType;
            if (data.Type != DDocUi.Enums.CollectionType.Collection) {
                data = $.extend(data,
                    {
                        ParentCollectionId: ddoc.collection.Id,
                        SecurityGroupId: data.SecurityGroupId //ddoc.collection.SecurityGroupId
                    });
            }
        } else {
			data.IsNew = false;
		}

		if (data.IsNew) {
            data.Fields = ddoc.newCollectionFields;
            data.ParentFieldIds = ddoc.newParentFields;
        }

		return data;
	})();

    ddoc.POST('/Admin/SaveCollection', collection, function (response) {
		ddoc.ShowAlert('Coleccion guardada con éxito!', function () {
			$('#collectionEditor').dialog('destroy');
            ddoc.LoadSecurityGroups();
            ddoc.LoadCollections(response.TextResult);
        });
    }, false, function () {
        //$('#collectionEditor').dialog('destroy');
	});
};

DDocUi.prototype.InsertParentField = function (e) {
    var $option = $(e.target);
	var inheritedField = $option.data();
	$option.hide();
	ddoc.newCollectionFields.push({
        Hidden: inheritedField.Hidden,
        InMask: inheritedField.InMask,
        IncludeInGlobalSearch: inheritedField.IncludeInGlobalSearch,
        Inheritable: inheritedField.Inheritable,
        IsNew: false,
		AllowedValues: inheritedField.AllowedValues,
        Id: inheritedField.Id,
        Name: inheritedField.Name,
        Nullable: inheritedField.Nullable,
        OutMask: inheritedField.OutMask,
        TypeLength: inheritedField.TypeLength,
        TypeString: inheritedField.TypeString,
        Unique: inheritedField.Unique,
		CreateRule: true
	});
	ddoc.grdNewCollectionFields.reload(ddoc.newCollectionFields);
    ddoc.newParentFields.push(inheritedField.Id);
};

DDocUi.prototype.MoveNewCollectionField = function (newFieldId, direction) {
	function moveField(from, to) {
		ddoc.newCollectionFields.splice(to, 0, ddoc.newCollectionFields.splice(from, 1)[0]);
	}

	var fieldIndex;

	$.each(ddoc.newCollectionFields, function (i, field) {
		if (field.Id == newFieldId) {
			fieldIndex = i;
			return;
		}
	});

	switch(direction) {
		case 0:
			moveField(i, 1 + 1);
			break;
		case 1:
			moveField(i, 1 - 1);
			break;
	}

	ddoc.grdNewCollectionFields.reload(ddoc.newCollectionFields);
};

DDocUi.prototype.DeleteNewCollectionField = function(newFieldId) {
	ddoc.Confirm('¿Está seguro que desea eliminar el campo seleccionado?', function () {
        ddoc.newParentFields.splice(ddoc.newParentFields.findIndex(function (fieldId) {
			return fieldId == newFieldId;
        }), 1);
		var field = ddoc.newCollectionFields.splice(ddoc.newCollectionFields.findIndex(function (field) {
			return field.Id == newFieldId;
		}), 1);
        ddoc.grdNewCollectionFields.reload(ddoc.newCollectionFields);
        $('#ParentFields').find('option:hidden').each(function (i, item) {
	        if (item.Id == field.Id)
		        $(item).show();
        });
	});
};