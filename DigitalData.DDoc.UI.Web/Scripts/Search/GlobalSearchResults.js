DDocUi.prototype.InitGrid = function (grid, contentType, collectionFields) {
	function onGridAction(e) {
		switch (e.command) {
			case 'edit':
				switch (contentType) {
					case DDocUi.Enums.CollectionType.Document:
						ddoc.document = e.dataRow;
						ddoc.EditDocument(ddoc.grdDocumentResults);
						break;
					case DDocUi.Enums.CollectionType.Folder:
						ddoc.folder = e.dataRow;
						ddoc.EditFolder(ddoc.grdFolderResults);
						break;
				}
				break;
			case 'send':
				ddoc.ShowAlert('¡Funcionalidad en construcción! ¡Vuelva más tarde!');
				break;
            default:
                if (typeof ddoc.CustomCommands === 'function') {
                    ddoc.CustomCommands(e.command);
                }
                break;
		}
	}

    var columns = [];

	switch (contentType) {
		case DDocUi.Enums.CollectionType.Folder:
			columns.push({ width: 24, field: 'Id', cellType: { type: DDoc.Constants.cellType.ICONLINK, url: ddoc.GetUrl('/Navigation/OpenFolder/{0}'), target: '_self', cssClass: { normal: 'imgFolderView' }, tooltip: 'Ver folder' } });
			columns.push({ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'edit', cssClass: { normal: 'imgFolderEdit' }, tooltip: 'Ver propiedades' } });
			break;
		case DDocUi.Enums.CollectionType.Document:
			columns.push({ width: 24, field: 'Id', cellType: { type: DDoc.Constants.cellType.THUMBNAILLINK, url: ddoc.GetUrl('/Viewer/OpenViewer/{0}'), target: ddoc.ViewerAnchorTarget, cssClass: { normal: 'imgThumb' }, tooltip: 'Ver documento' }, visible: false });
			columns.push({ width: 24, field: 'Id', cellType: { type: DDoc.Constants.cellType.ICONLINK, url: ddoc.GetUrl('/Viewer/OpenViewer/{0}'), target: ddoc.ViewerAnchorTarget, cssClass: { normal: 'imgDocumentView' }, tooltip: 'Ver documento' } });
			columns.push({ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'edit', cssClass: { normal: 'imgDocumentEdit' }, tooltip: 'Ver propiedades' } });

			//if (typeof ddoc.ListCustomActions === 'function') {
   //             var customActions = ddoc.ListCustomActions(ddoc.CollectionId);
   //             $.each(customActions, function (i, item) {
   //                 columns.push(item);
   //             });
   //         }
    }

	$.each(collectionFields, function (i, field) {
		if (!field.Hidden) {
			var column = {
				text: field.Name,
				fieldId: 'Data[' + i + '].Id',
				field: 'Data[' + i + '].Value',
				sort: { sortable: true, sortKey: field.Id, sorted: 0 },
				disableHider: false,
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

	grid.init(columns);
	grid.addListener('onAction', onGridAction);
	grid.addListener('onSelectRow', function (e) {
		ddoc.NavigationTree.getNodeByKey(e.grid.CollectionId).setActive();
	});

    if (typeof ddoc.CustomGridEvents === 'function') {
        ddoc.CustomGridEvents(grid);
    }
};

DDocUi.prototype.InitGlobalSearchResults = function () {

	ddoc.ViewerAnchorTarget = $('#ViewerAnchorTarget').val();
	parent.ddoc.Navigate = false;
	ddoc.NavigationTree = parent.$('#tree').fancytree('getTree');
	ddoc.NavigationTree.clearFilter();
	ddoc.NavigationTree.visit(function (node) { node.unselectable = false });
	ddoc.NavigationTree.visit(function (node) { node.setActive(false); });
	ddoc.NavigationTree.visit(function (node) { node.setSelected(false); });
	ddoc.TextQuery = $('#TextSearchQuery').val();

	ddoc.Collections = searchableCollections;

	$.each(ddoc.Collections, function (i, collection) {
		$('.result-grids').append($('<div class="grid-container ' + collection.Id + '"><div class= "grid-title">' + collection.Name + '</div><div id="grd' + collection.Id + '"></div></div>'));
		ddoc['grd' + collection.Id] = new DDoc.Controls.Grid('grd' + collection.Id, { height: 362, showFilter: false, showTools: true, pager: { show: true, pageSize: 10 }, thumbnailSrc: ddoc.GetUrl('/api/files/getThumbnail/{0}/115/150') });
		ddoc['grd' + collection.Id].CollectionId = collection.Id;
		ddoc.InitGrid(ddoc['grd' + collection.Id], collection.Type, collection.Fields);
		ddoc['grd' + collection.Id].reload(collection.SearchResults);
        ddoc['grd' + collection.Id].setPager(1, Math.ceil(collection.TotalSearchResults / 10));
        if (collection.TotalSearchResults >= parseInt($('#MaxHits').val())) 
            $('.grid-title', '.' + collection.Id).text($('.grid-title', '.' + collection.Id).text() + ' - Mostrando los primeros ' + collection.TotalSearchResults + ' ' + (collection.Type == DDocUi.Enums.CollectionType.Document ? 'Documentos' : 'Folders') + ', para obtener mas resultados, realice una busqueda avanzada.');
        else 
            $('.grid-title', '.' + collection.Id).text($('.grid-title', '.' + collection.Id).text() + ' - ' + collection.TotalSearchResults + ' ' + (collection.Type == DDocUi.Enums.CollectionType.Document ? 'Documentos' : 'Folders') + ' encontrados');
		$(ddoc['grd' + collection.Id].grid).closest('.grid-container').hide();
		if (collection.Type == DDocUi.Enums.CollectionType.Document)
			ddoc.NavigationTree.getNodeByKey(collection.Id).setSelected();
	});

	ddoc.NavigationTree.filterNodes(function (node) {
		return ddoc.Collections.map(function (col) { return col.Id }).indexOf(node.key) > -1;
	}, { autoExpand: true });
};

var ddoc = new DDocUi();
ddoc.InitGlobalSearchResults();