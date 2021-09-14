DDocUi.prototype.PagesEditor = function (grid) {
    ddoc.CreateModal('/Document/EditPages', ddoc.document, 'documentPagesEditor', 'Editar Páginas', { width: 1000 }, function () {
        ddoc.InitPagesEditor(grid);
    });
};

DDocUi.prototype.InitPagesEditor = function (grid) {

    (function setupControls() {
        if (!ddoc.document.IsNew || ddoc.CreatedNew) {
            ddoc.LoadDocumentPages();
        }

        ddoc.InitScroller();

        if (ddoc.document.IsNew) {
            if (!ddoc.CreatedNew) {
                $('#finishNewDocument').hide();
            }
            $('#closeEditPages').hide();
        }

        $('#saveSort').hide();
        $('#undoSort').hide();
    })();

    (function setupEvents() {
        function onActionButtonClick() {
            switch (this.id) {
                case 'addNewPage':
                    ddoc.AddNewPage(grid);
                    break;
                case 'replacePage':
                    ddoc.ReplacePage(grid);
                    break;
                case 'removePage':
                    ddoc.RemovePage(grid);
                    break;
                case 'sortPages':
                    ddoc.EnablePagesSorting();
                    break;
                case 'saveSort':
                    ddoc.UpdatePagesSequence(false, grid);
                    break;
                case 'undoSort':
                    ddoc.UndoSort();
                    break;
            }
        }

        function onDialogButtonClick() {
            switch (this.id) {
                case 'finishNewDocument':
                    if ($('.page-thumbnail').length == 0) {
                        ddoc.ShowAlert('Debe agregar páginas al documento nuevo');
                    } else {
                        ddoc.ShowAlert('Documento Creado!', function () {
                            ddoc.CreatedNew = false;
                            $('#documentEditor').dialog('destroy');
                            $('#documentPagesEditor').dialog('destroy');
                            if (grid) {
                                ddoc.GetPagedResults(grid.currentPage, grid.pageSize);
                            } else {
                                ddoc.GetPagedResults(ddoc.grdResults.currentPage, ddoc.grdResults.pageSize);
                            }
                        });
                    }
                    break;
                case 'prevPage':
                case 'closeEditPages':
                    $('#documentPagesEditor').dialog('destroy');
                    break;
            }
        }

        $('#documentPagesEditor').on('click', 'button.dialog-button', onDialogButtonClick);
        $('#documentPagesEditor').on('click', 'button.action-button', onActionButtonClick);
    })();

};

DDocUi.prototype.InitScroller = function () {
    var $pagesScroller = $('.pages-scroller');

    $pagesScroller.scrollbox({
        direction: 'h',
        linear: true,
        delay: 0,
        speed: 30,
        autoPlay: false,
        onMouseOverPause: false
    });

    $('#scrollForward')
        .mouseover(function () { $pagesScroller.trigger('forwardHover'); })
        .mouseout(function () { $pagesScroller.trigger('pauseHover'); });

    $('#scrollBackward')
        .mouseover(function () { $pagesScroller.trigger('backwardHover'); })
        .mouseout(function () { $pagesScroller.trigger('pauseHover'); });
};

DDocUi.prototype.LoadDocumentPages = function () {

    ddoc.GET('/Document/GetPages', { documentId: ddoc.document.Id }, function (response) {
        $('.pages-dragger').find('li').remove();

        $.each(response.List, function (index, page) {
            var $pageItem = $('<li>').addClass('page-thumbnail');
            $pageItem.data('dataItem', page);
            $pageItem.attr('data-order', page.Sequence - 1);
            $pageItem.append($('<div>').addClass('thumbnail-loading'));
            $pageItem.append($('<div>').text(index + 1).addClass('indicators-numbers'));
            $pageItem.on('click', ddoc.OnThumbnailClick);
            $pageItem.on('dblclick', function () {
                ddoc.OnThumbnailDblClick(page.DocumentId, index);
            });
            $('.pages-dragger').append($pageItem);
        });

        if ($('.page-thumbnail').length < 6) {
            ddoc.DisableScrollbox();
        }
        else {
            ddoc.ActivateScrollbox();
        }

        ddoc.LoadPageThumbnails();
    });
};


DDocUi.prototype.LoadPageThumbnails = function () {

    ddoc.pageThumbnailLoadQueue = $.jqmq({
        delay: -1,
        batch: 1,
        callback: function (page) {
            var thumbnailId = $(page).data('dataItem').Id;

            var thumbnail = new Image();
            thumbnail.onload = function () {
                $(page).find('.thumbnail-loading').remove();
                var $thbImage = $('<img>');
                $(page).prepend($thbImage);
                $thbImage.attr('src', thumbnail.src);
                ddoc.pageThumbnailLoadQueue.next();
            };
            thumbnail.onerror = function () {
                $(page).find('.thumbnail-loading').remove();
                var $errorThb = $('<img>');
                $(page).prepend($errorThb);
                $errorThb.attr('src', thumbnail.src);
                ddoc.pageThumbnailLoadQueue.next();
            };
            thumbnail.src = ddoc.GetUrl('/api/files/getThumbnail/' + thumbnailId + '/154/200');
        }
    });

    $('.page-thumbnail').jqmqAddEach(ddoc.pageThumbnailLoadQueue);
};

DDocUi.prototype.OnThumbnailClick = function () {
    $('.page-thumbnail').filter('.selected').removeClass('selected');
    $(this).addClass('selected');
};

DDocUi.prototype.OnThumbnailDblClick = function (documentId, fileIndex) {
    window.open(ddoc.GetUrl('/Viewer/OpenDocument/' + documentId) + '?file=' + fileIndex, '_blank');
};

DDocUi.prototype.ActivateScrollbox = function () {
    $('#scrollForward').show();
    $('#scrollBackward').show();
};

DDocUi.prototype.DisableScrollbox = function () {
    tinysort($('.page-thumbnail'), { data: 'order' });
    $('.pages-scroller')[0].scrollLeft = 0;
    $('#scrollForward').hide();
    $('#scrollBackward').hide();
};

DDocUi.prototype.AddNewPage = function (grid) {
    if ($('#EnableWebTwain').val() == 'True') {
        var buttons = '<div><button id="fileUpload" class="action-button"><span class="button-icon file-icon"></span>Seleccionar archivo</button>' +
            '<button id="scannerUpload" class="action-button"><span class="button-icon scan-icon"></span>Escanear página</button></div>' +
            '<div><label">Indique la posición donde desea insertar el Archivo.</label><br><br><input id="idFile" type="number" min="1"></div>' +
            '<div><button class="dialog-button" id="cancelAddNewPage"><span class="button-icon cancel-icon"></span>Cancelar</button></div>';

        ddoc.ShowModal('newPagePrompt', buttons, 'Seleccione origen', { width: 500 });
        $('#fileUpload').click(function () { ddoc.FileUpload(undefined, $('#idFile').val(), grid); });
        $('#scannerUpload').click(function () { ddoc.ScannerUpload(undefined, $('#idFile').val(), grid); });
        $('#cancelAddNewPage').click(function () { $('#newPagePrompt').dialog('destroy'); });
    } else {
        ddoc.FileUpload(undefined, $('#idFile').val(), grid);
    }
};

DDocUi.prototype.ReplacePage = function (grid) {

    var $selectedPage = $('.page-thumbnail').filter('.selected');

    if ($selectedPage.length > 0) {
        var userConfirmation = confirm('¿Está seguro que desea reemplazar la página seleccionada?');
        if (!userConfirmation) {
            $selectedPage.removeClass('selected');
        } else {
            var buttons = '<div><button id="fileUpload" class="action-button"><span class="button-icon file-icon"></span>Seleccionar archivo</button>' +
                '<button id="scannerUpload" class="action-button"><span class="button-icon scan-icon"></span>Escanear página</button></div>' +
                '<div><button class="dialog-button" id="cancelReplacePage"><span class="button-icon cancel-icon"></span>Cancelar</button></div>';

            ddoc.ShowModal('replacePagePrompt', buttons, 'Seleccione origen', { width: 500 });

            var pageId = $selectedPage.data('dataItem').Id;

            $('#fileUpload').click(function () { ddoc.FileUpload(pageId, undefined, grid); });
            $('#scannerUpload').click(function () { ddoc.ScannerUpload(pageId, undefined, grid); });
            $('#cancelReplacePage').click(function () { $('#replacePagePrompt').dialog('destroy'); });
        }
    } else {
        ddoc.ShowAlert('¡Debe seleccionar una página para reemplazar!');
    }
};

DDocUi.prototype.RemovePage = function (grid) {

    var $selectedPage = $('.page-thumbnail').filter('.selected');

    if ($selectedPage.length == 1) {
        var userConfirmation = confirm('¿Está seguro que desea eliminar la página seleccionada?');
        if (!userConfirmation) {
            $selectedPage.removeClass('selected');
        } else {
            ddoc.POST('/Document/DeletePage', { pageId: $selectedPage.data('dataItem').Id }, function () {
                $selectedPage.remove();
                $('.page-thumbnail').each(function (index, item) {
                    $(item).attr('data-order', index);
                });
                ddoc.ShowAlert('Página eliminada!', function () {
                    if ($('.page-thumbnail').length > 0) {
                        ddoc.UpdatePagesSequence(true, grid);
                    }
                });
            });
        }
    } else {
        ddoc.ShowAlert('Debe seleccionar una página!');
    }

    if ($('.page-thumbnail').length < 6) {
        ddoc.DisableScrollbox();
    }
};

DDocUi.prototype.EnablePagesSorting = function () {
    $('#addNewPage').hide();
    $('#replacePage').hide();
    $('#removePage').hide();
    $('#sortPages').hide();
    $('#saveSort').show();
    $('#undoSort').show();

    if ($('.pages-dragger').data('enabled') == '0') {
        $('.page-thumbnail').filter('.selected').removeClass('selected');
        $('.page-thumbnail').off('click dblclick');
        $('.pages-dragger').sortable({
            placeholderClass: 'page-placeholder'
        });
    }
};

DDocUi.prototype.DisableSortable = function () {

    $('#addNewPage').show();
    $('#replacePage').show();
    $('#removePage').show();
    $('#sortPages').show();
    $('#saveSort').hide();
    $('#undoSort').hide();
    $('.pages-dragger').sortable('destroy');
    $('.page-thumbnail').each(function (index, item) {
        $(item).click(ddoc.OnThumbnailClick);
        $(item).dblclick(function () { ddoc.OnThumbnailDblClick($(this).data('dataItem').DocumentId, index); });
    });
};

DDocUi.prototype.UpdatePagesSequence = function (noAlert, grid) {

    var document = (function () {
        var data = { Pages: [] };
        $('.page-thumbnail').each(function (index, item) {
            var page = $(item).data('dataItem');
            page.Sequence = index;
            data.Pages.push(page);
        });
        return data;
    })();

    ddoc.POST('/Document/UpdatePagesSequence', document, function () {
        if (!noAlert) {
            ddoc.ShowAlert('Secuencia de páginas actualizada!', function () {
                ddoc.DisableSortable();
                ddoc.LoadDocumentPages();
                if (grid) {
                    ddoc.GetPagedResults(grid.currentPage, grid.pageSize);
                } else {
                    ddoc.GetPagedResults(ddoc.grdResults.currentPage, ddoc.grdResults.pageSize);
                }
            });
        }
    });
};

DDocUi.prototype.UndoSort = function () {
    tinysort($('.page-thumbnail'), { data: 'order' });
    ddoc.DisableSortable();
};