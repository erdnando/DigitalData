DDocUi.prototype.InitializeGallery = function () {

    (function setupEvents() {
        function onScrollerOptionClick() {
            switch (this.id) {
                case 'addFilesFromScanner':
                    ddoc.ScannerUpload(undefined, undefined);
                    break;
                case 'replaceFile':
                    ddoc.ReplaceFile();
                    break;
                case 'removeFile':
                    ddoc.RemoveFile();
                    break;
            }
        }

        function onFileChange(e) {
            ddoc.UploadedFiles = new FormData();

            var files = e.target.files;

            if (files.length > 0) {
                for (var i = 0, f; f = files[i]; i++) {
                    var nombre = f.name;
                    ddoc.UploadedFiles.append(nombre, f);
                }

                ddoc.POST('/Document/UploadPages?documentId=' + ddoc.document.Id + '&apiToken=' + ddoc.ApiToken, ddoc.UploadedFiles, function () {
                    ddoc.LoadDocuments();
                }, true)
            }
        }

        function onNavButtonClick(e) {
            switch (e.target.id) {
                case 'prevPage':
                    if (ddoc.pageIndex > 0) {
                        ddoc.pageIndex--;
                        ddoc.RenderImage();
                    }
                    break;

                case 'nextPage':
                    if (ddoc.pageIndex < ddoc.selectedDoc.PageIds.length - 1) {
                        ddoc.pageIndex++;
                        ddoc.RenderImage();
                    }
                    break;
            }
        }

        $('#newFiles').change(onFileChange);
        $('#newFileSelector').on('click', 'button', onScrollerOptionClick);

        $('span', '.nav-buttons').on('click', onNavButtonClick);

        window.onresize = function (evt) {
            $('canvas', '.viewer-viewport').remove();
            clearTimeout(ddoc.ResizeTimer);
            ddoc.ResizeTimer = setTimeout(function () { ddoc.RenderImage() }, 500);
        };
    })();

    ddoc.LoadDocuments();
};

DDocUi.prototype.ReplaceFile = function () {

    var $selectedDocument = $('.document-thumbnail').filter('.selected');

    if ($selectedDocument.length > 0) {
        var userConfirmation = confirm('¿Está seguro que desea reemplazar el documento seleccionado?');
        if (!userConfirmation) {
            $selectedDocument.removeClass('selected');
        } else {
            var buttons = '<div><button id="fileUpload" class="action-button"><span class="button-icon file-icon"></span>Seleccionar archivo</button>' +
                '<button id="scannerUpload" class="action-button"><span class="button-icon scan-icon"></span>Escanear documento</button></div>' +
                '<div><button class="dialog-button" id="cancelReplacePage"><span class="button-icon cancel-icon"></span>Cancelar</button></div>';

            ddoc.ShowModal('replacePagePrompt', buttons, 'Seleccione origen', { width: 500 });

            var pageId = $selectedDocument.data('dataItem').PageIds[ddoc.pageIndex];

            $('#fileUpload').click(function () { ddoc.FileUpload(pageId, undefined); });
            $('#scannerUpload').click(function () { ddoc.ScannerUpload(pageId, undefined); });
            $('#cancelReplacePage').click(function () { $('#replacePagePrompt').dialog('destroy'); });
        }
    } else {
        ddoc.ShowAlert('¡Debe seleccionar un documento para reemplazar!');
    }
};

DDocUi.prototype.RemoveFile = function () {

    var $selectedDocument = $('.document-thumbnail').filter('.selected');

    if ($selectedDocument.length == 1) {
        var userConfirmation = confirm('¿Está seguro que desea eliminar el documento seleccionado?');
        if (!userConfirmation) {
            $selectedDocument.removeClass('selected');
        } else {
            ddoc.POST('/Document/DeletePage?apiToken=' + ddoc.ApiToken, { pageId: ddoc.selectedDoc.PageIds[ddoc.pageIndex] }, function () {
                ddoc.ShowAlert('Página eliminada!');
                $('canvas', '.viewer-viewport').remove();
                $('#prevPage').hide();
                $('#nextPage').hide();
                ddoc.LoadDocuments();
            });
        }
    } else {
        ddoc.ShowAlert('¡Debe seleccionar una página!');
    }

    if ($('.document-thumbnail').length < 6) {
        ddoc.DisableScrollbox();
    }
};

DDocUi.prototype.LoadDocuments = function () {

    $.ajax({
        method: 'POST',
        url: ddoc.GetUrl('/api/gallery/expnumber/' + ddoc.expNumber + '?apiToken=' + ddoc.ApiToken),
        data: JSON.stringify({ expNumber: ddoc.expNumber }),
        contentType: 'application/json; charset=UTF-8',
        global: false,
        success: function (response) {
            $('.pages-scroller').find('li').remove();

            $.each(response.List, function (i, expMember) {
                if (expMember.Documents.length > 0) {
                    var $documentItem = $('<li>').addClass('document-thumbnail');
                    $documentItem.data('dataItem', expMember.Documents[0]);
                    $documentItem.append($('<div>').addClass('thumbnail-container'));
                    $documentItem.append($('<div>').text(expMember.CollectionName).addClass('collection-name'));
                    $documentItem.on('click', ddoc.OnThumbnailClick);
                    if (expMember.Documents[0].PageIds.length > 0) {
                        $documentItem.on('dblclick', function () {
                            ddoc.OnThumbnailDblClick(expMember.Documents[0].DocumentId);
                        });
                    }
                    if (ddoc.Mode == 'Edit' || expMember.Documents[0].PageIds.length > 0) {
                        $('ul', '.pages-scroller').append($documentItem);
                    }
                }
            });

            ddoc.LoadPageThumbnails();
        },
        error: function (xhr) {
            ddoc.ShowAlert('¡Error al obtener documentos del expediente!');
        }
    });

};

DDocUi.prototype.LoadPageThumbnails = function () {

    ddoc.pageThumbnailLoadQueue = $.jqmq({
        delay: -1,
        batch: 1,
        callback: function (page) {
            if ($(page).data('dataItem').PageIds.length > 0) {
                var thumbnailId = $(page).data('dataItem').PageIds[0];
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
                thumbnail.src = ddoc.GetUrl('/api/files/getThumbnail/' + thumbnailId + '/72/90');
            } else {
				ddoc.pageThumbnailLoadQueue.next();
			}
        }
    });

    $('.document-thumbnail').jqmqAddEach(ddoc.pageThumbnailLoadQueue);
};

DDocUi.prototype.OnThumbnailClick = function () {
    $('.document-thumbnail').filter('.selected').removeClass('selected');
    $(this).addClass('selected');
    ddoc.selectedDoc = $(this).data('dataItem');
    ddoc.pageIndex = 0;
    ddoc.document = { Id: ddoc.selectedDoc.DocumentId, IsNew: false };
    if (ddoc.selectedDoc.PageIds.length > 0) {
        if (ddoc.selectedDoc.PageIds.length > 1) {
            $('.nav-buttons').show();
        }
        ddoc.RenderImage();
    }
};

DDocUi.prototype.OnThumbnailDblClick = function (documentId) {
    window.open(ddoc.GetUrl('/Viewer/OpenViewer/' + documentId + '?apiToken=' + ddoc.ApiToken), '_blank');
};

DDocUi.prototype.RenderImage = function () {
    var pageId = ddoc.selectedDoc.PageIds[ddoc.pageIndex];
    $('canvas', '.viewer-viewport').remove();
    var canvas = document.createElement('canvas');
    function calculateAspectRatioFit(srcWidth, srcHeight, maxWidth) {
        var ratio = Math.min(maxWidth / srcWidth, srcHeight);
        return { width: srcWidth * ratio, height: srcHeight * ratio };
    }
    ddoc.img = new Image();
    ddoc.img.onload = function () {
        var result = calculateAspectRatioFit(ddoc.img.width, ddoc.img.height, $('.viewer-viewport')[0].offsetWidth - 30);
        canvas.width = result.width;
        canvas.height = result.height;
        ddoc.SetupCanvas(canvas, ddoc.img);
        $('.viewer-viewport').append(canvas);
    };
    ddoc.img.onerror = function () { ddoc.ShowAlert('No se puede cargar la imagen') };
    ddoc.img.src = ddoc.GetUrl('/api/files/getThumbnail/' + pageId)

    if (ddoc.pageIndex == 0) {
        $('#prevPage').hide();
    }
    else {
        $('#prevPage').show()
    }

    if (ddoc.pageIndex == ddoc.selectedDoc.PageIds.length - 1) {
        $('#nextPage').hide();
    }
    else {
        $('#nextPage').show()
    }
};

DDocUi.prototype.ActivateScrollbox = function () {
    $('#scrollForward').show();
    $('.pages-scroller').css('width', 'calc(100% - 80px)');
    $('#scrollBackward').show();
};

DDocUi.prototype.DisableScrollbox = function () {
    $('.pages-scroller')[0].scrollLeft = 0;
    $('.pages-scroller').css('width', '100%');
    $('#scrollForward').hide();
    $('#scrollBackward').hide();
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

DDocUi.prototype.SetupCanvas = function (canvas, img) {
    var ctx = canvas.getContext('2d');
    ddoc.TrackTransforms(ctx);

    function redraw() {
        const p1 = ctx.transformedPoint(0, 0);
        const p2 = ctx.transformedPoint(canvas.width, canvas.height);
        ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);
        ctx.save();
        ctx.setTransform(1, 0, 0, 1, 0, 0);
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.restore();
        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
    }

    redraw();

    var lastX = canvas.width / 2, lastY = canvas.height / 2;

    var dragStart, dragged;

    canvas.addEventListener('mousedown',
        function (evt) {
            document.body.style.mozUserSelect = document.body.style.webkitUserSelect = document.body.style
                .userSelect = 'none';
            lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
            lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
            dragStart = ctx.transformedPoint(lastX, lastY);
            dragged = false;
        },
        false);

    canvas.addEventListener('mousemove',
        function (evt) {
            lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
            lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
            dragged = true;
            if (dragStart) {
                const pt = ctx.transformedPoint(lastX, lastY);
                ctx.translate(pt.x - dragStart.x, pt.y - dragStart.y);
                redraw();
            }
        },
        false);

    var scaleFactor = 1.1;

    var zoom = function (clicks) {
        const pt = ctx.transformedPoint(lastX, lastY);
        ctx.translate(pt.x, pt.y);
        const factor = Math.pow(scaleFactor, clicks);
        ctx.scale(factor, factor);
        ctx.translate(-pt.x, -pt.y);
        redraw();
    };
    canvas.addEventListener('mouseup',
        function (evt) {
            dragStart = null;
            if (!dragged) {
                zoom(evt.shiftKey ? -1 : 1);
            }
        },
        false);

    const handleScroll = function (evt) {
        const delta = evt.wheelDelta ? evt.wheelDelta / 40 : evt.detail ? -evt.detail : 0;
        if (delta) {
            zoom(delta);
        }
        return evt.preventDefault() && false;
    };

    canvas.addEventListener('DOMMouseScroll', handleScroll, false);
    canvas.addEventListener('mousewheel', handleScroll, false);
}

DDocUi.prototype.TrackTransforms = function (ctx) {
    var svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    var xform = svg.createSVGMatrix();
    ctx.getTransform = function () { return xform; };

    var savedTransforms = [];
    var save = ctx.save;
    ctx.save = function () {
        savedTransforms.push(xform.translate(0, 0));
        return save.call(ctx);
    };

    var restore = ctx.restore;
    ctx.restore = function () {
        xform = savedTransforms.pop();
        return restore.call(ctx);
    };

    var scale = ctx.scale;
    ctx.scale = function (sx, sy) {
        xform = xform.scaleNonUniform(sx, sy);
        return scale.call(ctx, sx, sy);
    };

    var rotate = ctx.rotate;
    ctx.rotate = function (radians) {
        xform = xform.rotate(radians * 180 / Math.PI);
        return rotate.call(ctx, radians);
    };

    var translate = ctx.translate;
    ctx.translate = function (dx, dy) {
        xform = xform.translate(dx, dy);
        return translate.call(ctx, dx, dy);
    };

    var transform = ctx.transform;
    ctx.transform = function (a, b, c, d, e, f) {
        const m2 = svg.createSVGMatrix();
        m2.a = a;
        m2.b = b;
        m2.c = c;
        m2.d = d;
        m2.e = e;
        m2.f = f;
        xform = xform.multiply(m2);
        return transform.call(ctx, a, b, c, d, e, f);
    };

    var setTransform = ctx.setTransform;
    ctx.setTransform = function (a, b, c, d, e, f) {
        xform.a = a;
        xform.b = b;
        xform.c = c;
        xform.d = d;
        xform.e = e;
        xform.f = f;
        return setTransform.call(ctx, a, b, c, d, e, f);
    };

    var pt = svg.createSVGPoint();
    ctx.transformedPoint = function (x, y) {
        pt.x = x;
        pt.y = y;
        return pt.matrixTransform(xform.inverse());
    };
}