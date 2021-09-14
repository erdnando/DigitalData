DDocUi.prototype.InitViewerPage = function () {

    (function setupControls() {
        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }

        ddoc.currentFile = 0;
        ddoc.pdfPages = 0;

        var fileIndexToOpen = $('#fileIndex').val();
        if (fileIndexToOpen) {
            ddoc.currentFile = parseInt(fileIndexToOpen);
        }

        var newWindow = getUrlVars()['nw'];
        if (newWindow != '' && newWindow != null && newWindow != 'undefined') {
            ddoc.ViewerOnNewWindow = true;
        } else {
            ddoc.ViewerOnNewWindow = false;
        }

        if ($('#NoPermission').length > 0 && $('#NoPermission').val() == '1') {
            ddoc.ShowAlert('No tiene permiso para ver este elemento', function () {
                if (ddoc.ViewerOnNewWindow)
                    window.close();
                else
                    window.history.back(1);
            });
        } else {
            ddoc.InitializeViewer();

            var pageNumber = getUrlVars()['pn'];
            var search = getUrlVars()['search'];
            search = decodeURIComponent(search);
            if (search != '' && search != null && search != 'undefined') {
                $('#findInput').val(search);
            }
            ddoc.OpenViewer(documentFiles[ddoc.currentFile], pageNumber, search);
            ddoc.UpdateAvailableFeatures(documentFiles[ddoc.currentFile].Type);
        }

        var $firstDdocPage = $('#firstDdocPage');
        var $prevDdocPage = $('#prevDdocPage');
        var $nextDdocPage = $('#nextDdocPage');
        var $lastDdocPage = $('#lastDdocPage');

        if (ddoc.currentFile == 0) {
            $firstDdocPage.prop('disabled', true);
            $prevDdocPage.prop('disabled', true);
            if (ddoc.pdfPages === 1) {
                $nextDdocPage.prop('disabled', true);
                $lastDdocPage.prop('disabled', true);
            }
        } else {
            if (ddoc.currentFile == documentFiles.length - 1) {
                $nextDdocPage.prop('disabled', true);
                $lastDdocPage.prop('disabled', true);
            }
        }

        if (documentFiles.length == 1) {
            $prevDdocPage.parent().parent().hide();
        }

        ddoc.CustomData = getUrlVars()['customData'];

        function showDocumentData() {
            var documentData = ddocDocument.Data;

            $.each(documentData, function (i, field) {
                if (!field.Hidden) {
                    var $dataRow = $('<tr></tr>');
                    var $name = $('<td></td>').html(field.Name);
                    var value = field.Value;
                    if (field.Type == 3) {
                        switch (field.Value) {
                            case 'True':
                                value = 'Si';
                                break;
                            case 'False':
                                value = 'No';
                                break;
                        }
                    } else if (field.Type == 4)
                        value = ddoc.FormatCurrency(parseFloat(value));
                    var $value = $('<td></td>').html(value);
                    $dataRow.append($name).append($value);
                    $('#documentData').append($dataRow);
                }
            });
        }

        showDocumentData();

        if (typeof ddoc.ViewerCustomActions === "function") {
            var customActions = ddoc.ViewerCustomActions(ddocDocument);
            $.each(customActions, function (i, action) {
                if (action.collectionId == ddocDocument.CollectionId) {
                    $('#customPanelActivator').show();
                    var $action = $('<a class="' + action.ImageClass + '" title="' + action.Tooltip + '">' + action.Tooltip + '</a>');
                    if (typeof action.Handler === "function")
                        $action.on("click", action.Handler);
                    $('#customPanel').append($action);
                }
            });
        }
    })();

    (function setupEvents() {

        $('#firstDdocPage').click(function () {
            ddoc.currentFile = 0;
            ddoc.OpenViewer(documentFiles[ddoc.currentFile]);
        });

        $('#prevDdocPage').click(function () {
            ddoc.currentFile--;
            ddoc.OpenViewer(documentFiles[ddoc.currentFile]);
        });

        $('#nextDdocPage').click(function () {
            ddoc.currentFile++;
            ddoc.OpenViewer(documentFiles[ddoc.currentFile]);
        });

        $('#lastDdocPage').click(function () {
            ddoc.currentFile = documentFiles.length - 1;
            ddoc.OpenViewer(documentFiles[ddoc.currentFile]);
        });

        $('#fileNumber').focusin(function (e) {
            $(e.target).data('origValue', e.target.value);
        }).focusout(function (e) {
            if (!$(e.target).data('changed')) {
                e.target.value = $(e.target).data('origValue');
                $(e.target).data('changed', false);
            }
        }).keydown(function (e) {
            if (e.which == 13) {
                ddoc.currentFile = e.target.value - 1;
                $(e.target).data('changed', true);
                ddoc.OpenViewer(documentFiles[ddoc.currentFile]);
            }
        });

        $('#viewBookmark').click(function () {
            window.prompt('Para compartir esta vista, comparta este link:', this.href);
        });

        $('#pageNumber').focusin(function (e) {
            $(e.target).data('origValue', e.target.value);
        }).focusout(function (e) {
            if (!$(e.target).data('changed')) {
                e.target.value = $(e.target).data('origValue');
                $(e.target).data('changed', false);
            }
        }).keydown(function (e) {
            if (e.which == 13) {
                $(e.target).data('changed', true);
                ddoc.PDFViewerApplication.page = e.target.value;
            }
        });

        $(document).on('keydown', function (e) {
            if (e.metaKey || e.ctrlKey) {
                switch (String.fromCharCode(e.which).toLowerCase()) {
                    case 'd':
                        e.preventDefault();
                        $('#download').click();
                        break;
                }
            }
        });
    })();
};

DDocUi.prototype.OpenViewer = function (file, pageNumber, searchQuery) {
    console.log(":::::::::::::::::::::::::");
    console.log(file);
    if (pageNumber != '' && pageNumber != null && pageNumber != 'undefined') {
        ddoc.PDFViewerApplication.initialBookmark = 'page=' + pageNumber;
    }
    ddoc.PDFViewerApplication.open(ddoc.GetUrl('/api/files/getDocument/' + file.Id), searchQuery);
    ddoc.UpdateNavigationButtons();
};

DDocUi.prototype.UpdateNavigationButtons = function () {

    var $firstDdocPage = $('#firstDdocPage');
    var $prevDdocPage = $('#prevDdocPage');
    var $nextDdocPage = $('#nextDdocPage');
    var $lastDdocPage = $('#lastDdocPage');

    $firstDdocPage.prop('disabled', false);
    $prevDdocPage.prop('disabled', false);
    $nextDdocPage.prop('disabled', false);
    $lastDdocPage.prop('disabled', false);

    if (ddoc.currentFile == 0) {
        $firstDdocPage.prop('disabled', true);
        $prevDdocPage.prop('disabled', true);
        if (documentFiles.length === 1) {
            $nextDdocPage.prop('disabled', true);
            $lastDdocPage.prop('disabled', true);
        }
    } else {
        if (ddoc.currentFile == documentFiles.length - 1) {
            $nextDdocPage.prop('disabled', true);
            $lastDdocPage.prop('disabled', true);
        }
    }

    if (documentFiles.length == 1) {
        $prevDdocPage.parent().parent().hide();
    }

    $('#fileNumber').val(ddoc.currentFile + 1);
    $('#numFiles').text(Resources.file_of.format(documentFiles.length));
};

DDocUi.prototype.UpdateAvailableFeatures = function (fileType) {

    if ($('#enablePrint').val() == 0) $('#print').hide();
    else $('#print').show();
    if ($('#enableDownload').val() == 0) $('#download').hide();
    else $('#download').show();

    switch (fileType.toLowerCase()) {
        case 'pdf':
            $('#viewFind').show();
            break;

        default:
            $('#viewFind').hide();
            break;
    }
};

DDocUi.prototype.Download = function () {

    function onDownloadButtonClick() {
        switch (this.id) {
            case 'fileDownload':
                ddoc.FileDownload();
                break;
            case 'pagesDownload':
                ddoc.PagesDownload();
                break;
            case 'documentDownload':
                ddoc.DocumentDownload();
                break;
            case 'cancelDownload':
                $('#downloadMode').dialog('destroy');
                break;
        }
    }

    ddoc.CreateModal('/Viewer/DownloadMode', { fileType: documentFiles[ddoc.currentFile].Type }, 'downloadMode', 'Descarga', { width: 520 }, function () {
        $('#downloadMode').on('click', 'button', onDownloadButtonClick);
        $('button', '#downloadMode').eq(0).focus();
    });
};

DDocUi.prototype.FileDownload = function () {
    function onFileDownloadModeButtonClick(target) {
        switch (target.id) {
            case 'bmpDownload':
                window.open(ddoc.GetUrl('/api/files/downloadFile/' + page.Id + '/bmp?user=' + ddoc.GetUsername()), '_self');
                break;
            case 'jpgDownload':
                window.open(ddoc.GetUrl('/api/files/downloadFile/' + page.Id + '/jpg?user=' + ddoc.GetUsername()), '_self');
                break;
            case 'pngDownload':
                window.open(ddoc.GetUrl('/api/files/downloadFile/' + page.Id + '/png?user=' + ddoc.GetUsername()), '_self');
                break;
            case 'tifDownload':
                window.open(ddoc.GetUrl('/api/files/downloadFile/' + page.Id + '/tif?user=' + ddoc.GetUsername()), '_self');
                break;
            case 'pdfDownload':
                window.open(ddoc.GetUrl('/api/files/downloadFile/' + page.Id + '/pdf?user=' + ddoc.GetUsername()), '_self');
                break;
            case 'xmlDownload':
                window.open(ddoc.GetUrl('/api/files/downloadFile/' + page.Id + '/xml?user=' + ddoc.GetUsername()), '_self');
                break;
        }
        $('#downloadMode').dialog('destroy');
        $('#fileDownloadMode').dialog('destroy');
    }

    var page = documentFiles[ddoc.currentFile];

    ddoc.CreateModal('/Viewer/FileDownloadMode', { fileType: documentFiles[ddoc.currentFile].Type }, 'fileDownloadMode', 'Seleccione formato', { width: 520 }, function () {
        $('#fileDownloadMode').on('click', 'div.file-type-icon', function (e) { onFileDownloadModeButtonClick(e.target) });
        $('#cancelFileDownload').on('click', function () { $('#fileDownloadMode').dialog('destroy'); });
        $('div.file-type-icon', '#fileDownloadMode').eq(0).focus();
        $('#fileDownloadMode').on('keydown', 'div.file-type-icon', function (e) {
            if (e.which == 13) {
                onFileDownloadModeButtonClick(e.target)
            }
        });
    });
};

DDocUi.prototype.PagesDownload = function () {
    function onPagesDownloadModeButtonClick() {
        switch (this.id) {
            case 'tifDownload':
                window.open(ddoc.GetUrl('/api/files/downloadPages/' + page.Id + '/tif/' + $('#startPage').val() + '/' + $('#endPage').val() + '?user=' + ddoc.GetUsername()), '_self');
                break;
            case 'pdfDownload':
                window.open(ddoc.GetUrl('/api/files/downloadPages/' + page.Id + '/pdf/' + $('#startPage').val() + '/' + $('#endPage').val() + '?user=' + ddoc.GetUsername()), '_self');
                break;
        }
        $('#downloadMode').dialog('destroy');
        $('#pagesDownloadMode').dialog('destroy');
    }

    var page = documentFiles[ddoc.currentFile];

    ddoc.CreateModal('/Viewer/PagesDownloadMode', undefined, 'pagesDownloadMode', 'Seleccione formato', { width: 520 }, function () {
        $('#pagesDownloadMode').on('click', 'div.file-type-icon', onPagesDownloadModeButtonClick);
        $('#cancelPagesDownload').on('click', function () { $('#pagesDownloadMode').dialog('destroy'); });
    });
};

DDocUi.prototype.DocumentDownload = function () {
    function onDocumentDownloadModeButtonClick() {
        switch (this.id) {
            case 'tifDownload':
                window.open(ddoc.GetUrl('/api/files/downloadDocument/' + documentId + '/tif?user=' + ddoc.GetUsername()), '_self');
                break;
            case 'pdfDownload':
                window.open(ddoc.GetUrl('/api/files/downloadDocument/' + documentId + '/pdf?user=' + ddoc.GetUsername()), '_self');
                break;
            case 'zipDownload':
                window.open(ddoc.GetUrl('/api/files/downloadDocument/' + documentId + '/zip?user=' + ddoc.GetUsername()), '_self');
                break;
        }
        $('#downloadMode').dialog('destroy');
        $('#documentDownloadMode').dialog('destroy');
    }

    var documentId = ddocDocument.Id;

    ddoc.CreateModal('/Viewer/DocumentDownloadMode', undefined, 'documentDownloadMode', 'Seleccione formato', { width: 520 }, function () {
        $('#documentDownloadMode').on('click', 'div.file-type-icon', onDocumentDownloadModeButtonClick);
        $('#cancelDocumentDownload').on('click', function () { $('#documentDownloadMode').dialog('destroy'); });
    });
};

var ddoc = new DDocUi();
ddoc.InitViewerPage();