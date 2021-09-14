DDocUi.prototype.InitCollectionManagement = function() {

	ddoc.collectionLock = $('#CollectionLock').val() == 'True';

    ddoc.LoadSecurityGroups();

	(function setupControls() {
		function setupGrid() {
			function onGridAction(e) {
				ddoc.collectionField = e.dataRow;
				switch (e.command) {
					case 'editField':
						ddoc.EditCollectionField();
						break;
					case 'moveUp':
						ddoc.MoveCollectionField(1);
						break;
					case 'moveDown':
						ddoc.MoveCollectionField(0);
						break;
					case 'deleteField':
						ddoc.DeleteCollectionField();
						break;
				}
			};

			ddoc.grdCollectionFields = new DDoc.Controls.Grid('grdCollectionFields', { title: 'Campos de la colección', showFilter: false, showColumnMenu: false, pager: { show: false } });

			var colType = DDoc.Constants.columnType;
			var columns = [
				{ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'editField', cssClass: { normal: 'imgEditField' }, tooltip: 'Editar campo' } },
				{ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'moveUp', cssClass: { normal: 'imgMoveUp' }, tooltip: 'Subir' } },
				{ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'moveDown', cssClass: { normal: 'imgMoveDown' }, tooltip: 'Bajar' } },
				{ text: 'Id', field: 'Id', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Nombre', field: 'Name', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Orden', field: 'Sequence', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Tipo', field: 'TypeString', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Longitud', field: 'TypeLength', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Opcional', field: 'Nullable', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ text: 'Heredable', field: 'Inheritable', columnType: colType.TEXT, sort: { sortable: false }, disableHider: true, visible: true, tooltip: true },
				{ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'deleteField', cssClass: { normal: 'imgDeleteField' }, tooltip: 'Eliminar campo' } }
			];

			ddoc.grdCollectionFields.init(columns);
			ddoc.grdCollectionFields.addListener('onAction', onGridAction);
		};

		$('.tree-view-menu li').addClass('disabled-item');

		setupGrid();
	})();

	(function setupEvents() {
		function onMenuBarOptionClick() {
			switch (this.id) {
				case 'newCollection':
					ddoc.NewRootCollection();
					break;
				case 'editRules':
					ddoc.EditCollectionRules();
					break;
				case 'finalizeCollections':
					ddoc.FinalizeCollections();
					break;
			}
		}

		function onRightTreeviewMenuOptionClick() {
			if (!$(this).hasClass('disabled-item')) {
				switch (this.id) {
					case 'editCollection':
						ddoc.EditCollection();
						break;
					case 'addField':
						ddoc.NewCollectionField();
						break;
					case 'addFolderChild':
						ddoc.AddFolderCollection();
						break;
					case 'addDocumentChild':
						ddoc.AddDocumentCollection();
						break;
					case 'deleteCollection':
						ddoc.DeleteCollection();
						break;
					default:
				}
			}
		}

		$('.menu-bar').on('click', 'li', onMenuBarOptionClick);
		$('.tree-view-menu li').on('click', onRightTreeviewMenuOptionClick);
	})();

	$('#collectionTree').fancytree({
		activate: function (e, data) {
			ddoc.collection = JSON.parse(data.node.data.collection);

			$('#scId').val(ddoc.collection.Id);
			$('#scName').val(ddoc.collection.Name);
			$('#scDescription').val(ddoc.collection.Description);

			$('#scSecurityGroupId').val(ddoc.securityGroups.find(function (group) {
				return group.Id == ddoc.collection.SecurityGroupId;
			}).Name);

			$('.tree-view-menu li').removeClass('disabled-item');

			if (ddoc.collection.Type == 1) {
				$('#addField').addClass('disabled-item');
			}
			if (data.node.hasChildren()) {
				$('#deleteCollection').addClass('disabled-item');
			}

			if (ddoc.collection.Type > 1 && ddoc.collection.Fields.every(function (f) {
				return f.Inheritable == false;
			})) {
				$('#addDocumentChild').addClass('disabled-item');
				$('#addFolderChild').addClass('disabled-item');
			}

			switch (ddoc.collection.Type) {
				case 1:
					$('#editCollection span').removeClass('folderCollection-edit documentCollection-edit').addClass('rootCollection-edit');
					$('#deleteCollection span').removeClass('folderCollection-delete documentCollection-delete').addClass('rootCollection-delete');
					break;
				case 2:
					$('#editCollection span').removeClass('rootCollection-edit documentCollection-edit').addClass('folderCollection-edit');
					$('#deleteCollection span').removeClass('rootCollection-delete documentCollection-delete').addClass('folderCollection-delete');
					break;
				case 3:
					$('#editCollection span').removeClass('rootCollection-edit folderCollection-edit').addClass('documentCollection-edit');
					$('#deleteCollection span').removeClass('rootCollection-delete folderCollection-delete').addClass('documentCollection-delete');
					break;
			}

			ddoc.LoadCollectionFields();
		},
		source: {
			url: ddoc.GetUrl('/Admin/GetCollectionTree'),
			cache: false
		}
	});

	ddoc.CollectionTree = $('#collectionTree').fancytree('getTree');

	$('.fancytree-container').addClass('fancytree-connectors');

	ddoc.LoadCollections();
};

DDocUi.prototype.LoadSecurityGroups = function() {
    ddoc.GET('/Admin/GetDdocGroups', { groupType: 2 }, function (response) {
        ddoc.securityGroups = response.List;
    });
}

DDocUi.prototype.LoadCollections = function() {

	$('#scId').val('');
	$('#scName').val('');
	$('#scDescription').val('');
	$('#scSecurityGroupId').val('');
	ddoc.CollectionTree.reload();	
};

DDocUi.prototype.DeleteCollection = function () {
	ddoc.Confirm('¿Está seguro que desea eliminar la colección seleccionada?', function() {
		ddoc.POST('/Admin/DeleteCollection', { collectionId: ddoc.collection.Id }, function () {
			ddoc.collection = undefined;
			$('.tree-view-menu li').addClass('disabled-item');
			$('.tree-view-datapanel input').val('');
			ddoc.grdCollectionFields.reload(undefined);
			ddoc.LoadCollections();
		});
	});
};

DDocUi.prototype.LoadCollectionFields = function () {
	ddoc.grdCollectionFields.showLoader();
	ddoc.GET('/Collection/GetFields', { collectionId: ddoc.collection.Id }, function (response) {
		ddoc.grdCollectionFields.reload(response.List);
	});
};

DDocUi.prototype.MoveCollectionField = function(direction) {
	ddoc.POST('/Admin/MoveCollectionField', { fieldId: ddoc.collectionField.Id, direction: direction }, function () {
		ddoc.LoadCollectionFields();
	});
};

DDocUi.prototype.DeleteCollectionField = function() {
	ddoc.Confirm('¿Está seguro que desea eliminar el campo seleccionado?', function () {
		ddoc.POST('/Admin/DeleteCollectionField', { fieldId: ddoc.collectionField.Id } , function () {
			ddoc.collectionField = undefined;
			ddoc.LoadCollectionFields();
		});
	});
};

DDocUi.prototype.FinalizeCollections = function() {
	ddoc.Confirm('¿Está seguro que desea finalizar la estructura de colecciones? No se podrá modificar la estructura más adelante', function() {
		ddoc.POST('/Admin/FinalizeCollections', undefined, function() {
			ddoc.collection = undefined;
			ddoc.collectionField = undefined;
			ddoc.grdCollectionFields.reload(undefined);
			ddoc.LoadCollections();
			ddoc.collectionLock = true;
			$('#finalizeCollections', '.menu-bar').hide();
		});
	});
};

var ddoc = new DDocUi();
ddoc.InitCollectionManagement();