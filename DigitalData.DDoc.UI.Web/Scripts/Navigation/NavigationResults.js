DDocUi.prototype.InitNavigationScreen = function() {

	ddoc.ViewerAnchorTarget = $('#ViewerAnchorTarget').val();
	ddoc.CollectionId = $('#CollectionId').val();
	ddoc.CollectionType = parseInt($('#CollectionType').val());

    (function setupControls() {
		function setupGrid() {
			function initGrid(cols) {
				function onGridAction(e) {
					switch (e.command) {
						case 'edit':
							switch (ddoc.CollectionType) {
								case DDocUi.Enums.CollectionType.Folder:
									ddoc.folder = e.dataRow;
									ddoc.EditFolder();
									break;
								case DDocUi.Enums.CollectionType.Document:
									ddoc.document = e.dataRow;
									ddoc.EditDocument();
									break;
							}
							break;
						case 'send':
							ddoc.ShowAlert('¡Funcionalidad en construcción! ¡Vuelva más tarde!');
							break;
						default:
							if (typeof ddoc.CustomCommands === 'function') {
								ddoc.CustomCommands(e.command, e.dataRow);
                            }
                            break;
					}
				}

				function onTextSeek(e) {
					ddoc.grdResults.showLoader();
					if (e.text !== '') {
						ddoc.GET('/Search/TextSeek', { collectionId: ddoc.CollectionId, collectionType: ddoc.CollectionType, searchText: e.text }, function(response) {
							ddoc.grdResults.reload(response.List);
							if (ddoc.CollectionType == DDocUi.Enums.CollectionType.Document) {
								ddoc.grdResults.loadThumbnails('Pages[0].Id');
							}
						});
					} else {
						ddoc.GetPagedResults(ddoc.grdResults.currentPage, ddoc.grdResults.pageSize);
					}
				}

				function onGetPage(e) { ddoc.GetPagedResults(e.page, e.pageSize, e.sortBy, e.sortDirection); }

				function onTextSeekerKeyPress(e) {
					if (e.keyCode == 13)
						e.preventDefault();
				}

				ddoc.grdResults.init(cols);
				ddoc.grdResults.addListener('onAction', onGridAction);
				ddoc.grdResults.addListener('onTextSeek', onTextSeek);
				ddoc.grdResults.addListener('onGetPage', onGetPage);

				if (typeof ddoc.CustomGridEvents === 'function') {
                    ddoc.CustomGridEvents(ddoc.grdResults);
                }

				$('#grdResults').on('keypress', '.text-seeker', onTextSeekerKeyPress);
				ddoc.GetPagedResults(ddoc.grdResults.currentPage, ddoc.grdResults.pageSize);
			}

			ddoc.grdResults = new DDoc.Controls.Grid('grdResults', { height: 488, title: ' ', showFilter: false, defaultFilter: false, pager: { show: true, sizes: [15], pageSize: 15, onServer: true }, thumbnailSrc: ddoc.GetUrl('/api/files/getThumbnail/{0}/115/150') });

			var viewClass = ddoc.CollectionType == DDocUi.Enums.CollectionType.Document ? 'imgDocumentView' : 'imgFolderView';
			var editClass = ddoc.CollectionType == DDocUi.Enums.CollectionType.Document ? 'imgDocumentEdit' : 'imgFolderEdit';

			var columns = [];

            var nw = '';

            if (ddoc.ViewerAnchorTarget == '_blank')
                nw = '?nw=true';

			if (ddoc.CollectionType == DDocUi.Enums.CollectionType.Document) {
				columns.push({ width: 20, field: 'Id', cellType: { type: DDoc.Constants.cellType.THUMBNAILLINK, url: ddoc.GetUrl('/Viewer/OpenViewer/{0}' + nw), target: ddoc.ViewerAnchorTarget, cssClass: { normal: 'imgThumb' }, tooltip: 'Ver documento' }, visible: false });
				columns.push({ width: 20, field: 'Id', cellType: { type: DDoc.Constants.cellType.ICONLINK, url: ddoc.GetUrl('/Viewer/OpenViewer/{0}' + nw), target: ddoc.ViewerAnchorTarget, cssClass: { normal: viewClass }, tooltip: 'Ver documento' } });
			} else {
				columns.push({ width: 20, field: 'Id', cellType: { type: DDoc.Constants.cellType.ICONLINK, url: ddoc.GetUrl('/Navigation/OpenFolder/{0}'), target: '_self', cssClass: { normal: 'imgFolderView' }, tooltip: 'Ver folder' } });
			}
			columns.push({ width: 20, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'edit', cssClass: { normal: editClass }, tooltip: 'Ver propiedades' } });

            if (typeof ddoc.ListCustomActions === 'function') {
				var customActions = ddoc.ListCustomActions(ddoc.CollectionId);
                $.each(customActions, function(i, item) {
                     columns.push(item);
                });
            }

            ddoc.GET('/Collection/GetFields', { collectionId: ddoc.CollectionId }, function(response) {
				$.each(response.List, function (i, field) {
					if (!field.Hidden) {
						var column = {
							text: field.Name,
							fieldId: 'Data[' + i + '].Id',
							field: 'Data[' + i + '].Value',
							sort: { sortable: true, sortKey: field.Id, sorted: 0 },
							disableHider: i === 0,
							tileVisibility: i <= 5,
							visible: i <= 8,
							hidden: field.Hidden,
							tooltip: true,
							columnType: field.Type
						};

						if (field.Type == 4) {
							column.Type = DDoc.Constants.columnType.NUMBER;
							column.currency = true;
						}

						columns.push(column);
					}
				});
				columns.push({ text: 'Fecha de Creación', field: 'CreationDate', sort: { sortable: false }, disableHider: false, tileVisibility: true, visible: false, hidden: false, tooltip: true, columnType: DDoc.Constants.columnType.DATE });
				initGrid(columns);
			});
		}

		if (ddoc.CollectionType == DDocUi.Enums.CollectionType.Folder) {
			$('.style-changer').hide();
		}

		setupGrid();
	})();

	(function setupEvents() {
		function onMenuBarOptionClick() {
			switch (this.id) {
				case 'addNewElement':
					ddoc.AddNewElement();
					break;
			}
		}

		$('.style-changer').click(function() {
			ddoc.SetActiveStyleSheet($(this).attr('id'));
			$(this).siblings().removeClass('toggle-on');
			$(this).addClass('toggle-on');
			ddoc.grdResults.autoHeight();
		});

		$('.menu-bar').on('click', 'li', onMenuBarOptionClick);
	})();
};

DDocUi.prototype.AddNewElement = function() {
	switch (ddoc.CollectionType) {
		case DDocUi.Enums.CollectionType.Folder:
			ddoc.NewFolder(ddoc.grdResults);
			break;
		case DDocUi.Enums.CollectionType.Document:
			ddoc.NewDocument(ddoc.grdResults);
			break;
	}
};

DDocUi.prototype.GetPagedResults = function(page, pageSize, sortBy, sortDirection) {

	ddoc.grdResults.showLoader();

	switch (ddoc.CollectionType) {

		case DDocUi.Enums.CollectionType.Document:
			ddoc.GET('/Document/GetDocuments', { collectionId: ddoc.CollectionId, page: page, pageSize: pageSize, sortBy: sortBy, sortDirection: sortDirection}, function(response) {
				if (response.Count === 0) {
					ddoc.ShowAlert('No hay elementos en esta colección');
				}
				ddoc.grdResults.reload(response.List);
                ddoc.grdResults.loadThumbnails('Pages[0].Id');
                ddoc.SetGridTotal(page, pageSize);
            });
			break;

		case DDocUi.Enums.CollectionType.Folder:
			ddoc.GET('/Folder/GetFolders', { collectionId: ddoc.CollectionId, page: page, pageSize: pageSize, sortBy: sortBy, sortDirection: sortDirection }, function (response) {
				if (response.Count === 0) {
					ddoc.ShowAlert('No hay elementos en esta colección');
				}
				ddoc.grdResults.reload(response.List);
                ddoc.SetGridTotal(page, pageSize);
            });
			break;
	}
};

DDocUi.prototype.SetGridTotal = function(page, pageSize) {
    
    function setTotal(response) {
        ddoc.grdResults.setPager(page, Math.ceil(response.Total / pageSize));
        $('#grdResults .grid-title-label').html(response.Total + ' Elementos');
    }
    
    switch (ddoc.CollectionType) {
        case DDocUi.Enums.CollectionType.Document:
            ddoc.GET('/Document/CountDocuments', { collectionId: ddoc.CollectionId }, setTotal);
            break;

        case DDocUi.Enums.CollectionType.Folder:
            ddoc.GET('/Folder/CountFolders', { collectionId: ddoc.CollectionId }, setTotal);
            break;
    }
}

var ddoc = new DDocUi();
ddoc.InitNavigationScreen();

window.onload = ddoc.onWindowLoad;
window.onunload = ddoc.onWindowUnload;