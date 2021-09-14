DDocUi.prototype.InitGlobalSearchResults = function () {

	ddoc.ViewerAnchorTarget = $('#ViewerAnchorTarget').val();
	parent.ddoc.Navigate = false;
	ddoc.NavigationTree = parent.$('#tree').fancytree('getTree');
	ddoc.NavigationTree.clearFilter();
	ddoc.NavigationTree.visit(function (node) { node.unselectable = false });
	ddoc.NavigationTree.visit(function (node) { node.setActive(false); });
	ddoc.NavigationTree.visit(function (node) { node.setSelected(false); });
	ddoc.TextQuery = $('#TextSearchQuery').val();

    ddoc.itemCollections = [];
    ddoc.totalDocuments = 0;
    ddoc.totalFolders = 0;

    oboe(ddoc.GetUrl('/search/global/3/' + ddoc.TextQuery))
        .node('!.*', ddoc.RenderDocument)
        .done(function () {
            if (ddoc.totalDocuments >= parseInt($('#MaxHits').val()))
                $('.list-title-label', '.document-list-title').text('Mostrando los primeros ' + ddoc.totalDocuments + ' documentos. Para obtener más resultados realice una búsqueda avanzada.');
            else
                $('.list-title-label', '.document-list-title').text(ddoc.totalDocuments + ' Documentos encontrados');

            $('.loading', '.document-list-title').hide();

        });

    oboe(ddoc.GetUrl('/search/global/2/' + ddoc.TextQuery))
        .node('!.*', ddoc.RenderFolder)
        .done(function () {
            if (ddoc.totalFolders >= parseInt($('#MaxHits').val())) {
                $('.list-title-label', '.list-title').text('Mostrando los primeros ' + ddoc.totalFolders + ' folders. Para obtener más resultados realice una búsqueda avanzada.');
            } else
                $('.list-title-label', '.folder-list-title').text(ddoc.totalFolders + ' Folders encontrados');

            $('.loading', '.folder-list-title').hide();
        });
};

DDocUi.prototype.RenderFolder = function (item) {
    ddoc.totalFolders++;
    if (!ddoc.itemCollections.includes(item.CollectionId))
        ddoc.itemCollections.push(item.CollectionId);
    ddoc.NavigationTree.filterNodes(function (node) {
        return ddoc.itemCollections.indexOf(node.key) > -1;
    }, { autoExpand: true });
    var $item = $('<li>').addClass('w3-bar');
    $item.append($('<span>').addClass('imgFolderView list-action w3-bar-item'));
    $item.append($('<span>').addClass('imgFolderEdit list-action w3-bar-item').attr('title', 'Ver Propiedades').on('click', function() {
        ddoc.folder = JSON.parse(JSON.stringify(item));
        ddoc.EditFolder(ddoc.grdDocumentResults);
    }));
    var $itemData = $('<div>').addClass('w3-bar-item');
    var text = '';
    for (var prop in item) {
        if (item.hasOwnProperty(prop)) {
            text += `: ${prop} - ${item[prop]}`;
        }
    }
    $itemData.append($('<span>').addClass('w3-large').text(text));
    $item.append($itemData);
    $('#folderList').append($item);
    return oboe.drop;
};

DDocUi.prototype.RenderDocument = function (item) {
    ddoc.totalDocuments++;
    if (!ddoc.itemCollections.includes(item.CollectionId))
        ddoc.itemCollections.push(item.CollectionId);
    ddoc.NavigationTree.filterNodes(function (node) {
        return ddoc.itemCollections.indexOf(node.key) > -1;
    }, { autoExpand: true });
    var $item = $('<li>').addClass('w3-bar');
    $item.append($('<span>').addClass('imgDocumentView list-action w3-bar-item'));
    $item.append($('<span>').addClass('imgDocumentEdit list-action w3-bar-item').attr('title', 'Ver Propiedades').on('click', function () {
        ddoc.document = JSON.parse(JSON.stringify(item));
        ddoc.EditDocument(ddoc.grdDocumentResults);
    }));
    var $itemData = $('<div>').addClass('w3-bar-item');
    var text = '';
    for (var prop in item) {
        if (item.hasOwnProperty(prop)) {
            text += `${prop} - ${item[prop]} `;
        }
    }
    $itemData.append($('<span>').addClass('w3-large').text(text));
    $item.append($itemData);
    $('#documentList').append($item);
    return oboe.drop;
}

var ddoc = new DDocUi();
ddoc.InitGlobalSearchResults();