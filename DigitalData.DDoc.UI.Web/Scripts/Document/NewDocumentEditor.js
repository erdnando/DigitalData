DDocUi.prototype.NewDocument = function (grid) {
	ddoc.document = { CollectionId: $('#CollectionId').val(), Name: $('#Name').val(), IsNew: true };
	ddoc.CreateModal('/Document/New', { collectionId: ddoc.CollectionId }, 'newDocumentEditor', 'Nuevo Documento', false, function () {
		ddoc.document.Id = $('#DocumentId').val();
		ddoc.InitNewDocumentEditor(grid);
		ddoc.StandaloneWindow = false;
	});
};

DDocUi.prototype.InitNewDocumentEditor = function (grid) {

	(function setupControls() {

		$('#frmNewDocument').find('.editableField').prop('disabled', false);

		ddoc.controls = [];

		ddoc.InitNewScroller();

		$('#frmNewDocument').find('.editableField').each(function (i, input) {
			if ($(input).prev().val() === '2') {
				$(input).val($(input).val().substr(0, 10));
				var controlId = ddoc.controls.push(new DDoc.Controls.DatePicker(input)) - 1;
				ddoc.controls[controlId].init();
			}
			if ($(input).prev().val() === '3') {
				$(input).data('orVal', $(this).is(':checked'));
			} else {
				$(input).data('orVal', $(this).val());
			}
		});

		$('#saveSort').hide();
		$('#undoSort').hide();
	})();

	(function setupEvents() {
		function onScrollerOptionClick() {
			switch (this.id) {
				case 'addFilesFromScanner':
					ddoc.ScannerUpload(undefined, undefined, grid);
					break;
				case 'replaceFile':
					ddoc.ReplaceFile(grid);
					break;
				case 'removeFile':
					ddoc.RemoveFile(grid);
					break;
				case 'sortFiles':
					ddoc.EnableNewPagesSorting();
					break;
				case 'saveSort':
					ddoc.UpdateNewPagesSequence(false, grid);
					break;
				case 'undoSort':
					ddoc.UndoNewSort();
					break;
			}
		}

		function onDialogButtonClick(e) {
			e.preventDefault();
			switch (this.id) {
				case 'cancel':
					ddoc.Confirm('¿Está seguro que desea cancelar la creación del documento?', function () {
						ddoc.POST('/Document/Delete' + (ddoc.StandaloneWindow  ? '?apiToken=' + ddoc.ApiToken : ''), { documentId: ddoc.document.Id }, function () {
							if (ddoc.StandaloneWindow) {
								window.location = 'about:blank';
							} else {
								$('#newDocumentEditor').dialog('destroy');
							}
						});
					});
					break;
				case 'saveDocument':
					if (!ddoc.ValidateDocumentFields()) {
						return;
					}
					ddoc.CommitDocument(grid);
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

				ddoc.POST('/Document/UploadPages?documentId=' + ddoc.document.Id + (ddoc.StandaloneWindow ? '&apiToken=' + ddoc.ApiToken : ''), ddoc.UploadedFiles, function () {
					ddoc.LoadNewDocumentPages();
				}, true)
			}
		}

		function onDragOver(e) {
			e.stopPropagation();
			e.preventDefault();
			e.originalEvent.dataTransfer.dropEffect = 'copy';
		};

		function onDrop(e) {
			e.stopPropagation();
			e.preventDefault();

			ddoc.UploadedFiles = new FormData();

			var files = e.originalEvent.dataTransfer.files;

			for (var i = 0, f; f = files[i]; i++) {
				var nombre = f.name;
				ddoc.UploadedFiles.append(nombre, f);
			}

			ddoc.POST('/Document/UploadPages?documentId=' + ddoc.document.Id + (ddoc.StandaloneWindow ? '&apiToken=' + ddoc.ApiToken : ''), ddoc.UploadedFiles, function () {
				ddoc.LoadNewDocumentPages();
			}, true)
		};

		$('#newFiles').change(onFileChange);
		$('#newFileDrop').on('dragover', onDragOver).on('drop', onDrop);
		$('.button-panel', '#newDocumentEditor').on('click', 'button', onScrollerOptionClick);
		$('#frmNewDocument').on('click', 'button', onDialogButtonClick);

		window.onresize = evt => {
			$('canvas', '.viewer-viewport').remove();
			clearTimeout(ddoc.ResizeTimer);
			ddoc.ResizeTimer = setTimeout(() => ddoc.RenderImage(), 500);
		};
	})();
};

DDocUi.prototype.ReplaceFile = function (grid) {

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

DDocUi.prototype.RemoveFile = function (grid) {

	var $selectedPage = $('.page-thumbnail').filter('.selected');

	if ($selectedPage.length == 1) {
		var userConfirmation = confirm('¿Está seguro que desea eliminar la página seleccionada?');
		if (!userConfirmation) {
			$selectedPage.removeClass('selected');
		} else {
			ddoc.POST('/Document/DeletePage' + (ddoc.StandaloneWindow ? '?apiToken=' + ddoc.ApiToken : ''), { pageId: $selectedPage.data('dataItem').Id }, function () {
				$selectedPage.remove();
				$('.page-thumbnail').each(function (index, item) {
					$(item).attr('data-order', index);
				});
				ddoc.ShowAlert('Página eliminada!', function () {
					if ($('.page-thumbnail').length > 0) {
						ddoc.UpdateNewPagesSequence(true, grid);
					}
				});
			});
		}
	} else {
		ddoc.ShowAlert('¡Debe seleccionar una página!');
	}

	if ($('.page-thumbnail').length < 6) {
		ddoc.DisableNewScrollbox();
	}
};

DDocUi.prototype.LoadNewDocumentPages = function (onPagesLoaded) {

	ddoc.GET('/Document/GetPages', { documentId: ddoc.document.Id, apiToken: (ddoc.StandaloneWindow ? decodeURIComponent(ddoc.ApiToken) : null) }, function (response) {
		$('.pages-dragger').find('li').remove();

		$.each(response.List, function (index, page) {
			var $pageItem = $('<li>').addClass('page-thumbnail');
			$pageItem.data('dataItem', page);
			$pageItem.attr('data-order', page.Sequence - 1);
			$pageItem.append($('<div>').addClass('thumbnail-loading'));
			$pageItem.append($('<div>').text(index + 1).addClass('indicators-numbers'));
			$pageItem.on('click', ddoc.OnNewThumbnailClick);
			$('.pages-dragger').append($pageItem);
		});

		if ($('.page-thumbnail').length > 0) {
			if ($('.page-thumbnail').length < 6) {
				ddoc.DisableNewScrollbox();
			}
			else {
				ddoc.ActivateNewScrollbox();
			}
		}

		ddoc.LoadNewPageThumbnails(onPagesLoaded);
	});
};

DDocUi.prototype.LoadNewPageThumbnails = function (onPagesLoaded) {

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
			thumbnail.src = ddoc.GetUrl('/api/files/getThumbnail/' + thumbnailId + '/72/90');

			if (typeof onPagesLoaded === 'function')
				onPagesLoaded();
		}
	});

	$('.page-thumbnail').jqmqAddEach(ddoc.pageThumbnailLoadQueue);
};

DDocUi.prototype.OnNewThumbnailClick = function () {
	$('.page-thumbnail').filter('.selected').removeClass('selected');
	$(this).addClass('selected');
	if ($('.preview-container').length > 0) {
		ddoc.RenderImage();
	}
};

DDocUi.prototype.RenderImage = function () {
	var pageId = $('.page-thumbnail.selected').data('dataItem').Id;
	$('canvas', '.viewer-viewport').remove();
	var canvas = document.createElement('canvas');
	function calculateAspectRatioFit(srcWidth, srcHeight, maxWidth) {
		var ratio = Math.min(maxWidth / srcWidth, srcHeight);
		return { width: srcWidth * ratio, height: srcHeight * ratio };
	}
	ddoc.img = new Image();
	ddoc.img.onload = () => {
		var result = calculateAspectRatioFit(ddoc.img.width, ddoc.img.height, $('.viewer-viewport')[0].offsetWidth - 30);
		canvas.width = result.width;
		canvas.height = result.height;
		ddoc.SetupCanvas(canvas, ddoc.img);
		$('.viewer-viewport').append(canvas);
	};
	ddoc.img.onerror = () => ddoc.ShowAlert('No se puede cargar la imagen');
	ddoc.img.src = ddoc.GetUrl('/api/files/getThumbnail/' + pageId)
};

DDocUi.prototype.ActivateNewScrollbox = function () {
	$('#scrollForward').show();
	$('#scrollBackward').show();
};

DDocUi.prototype.DisableNewScrollbox = function () {
	tinysort($('.page-thumbnail'), { data: 'order' });
	$('.pages-scroller')[0].scrollLeft = 0;
	$('#scrollForward').hide();
	$('#scrollBackward').hide();
};

DDocUi.prototype.EnableNewPagesSorting = function () {
	$('#addFilesFromScanner').hide();
	$('#replaceFile').hide();
	$('#removeFile').hide();
	$('#sortFiles').hide();
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

DDocUi.prototype.DisableNewSortable = function() {

	$('#addFilesFromScanner').show();
	$('#replaceFile').show();
	$('#removeFile').show();
	$('#sortFiles').show();
	$('#saveSort').hide();
	$('#undoSort').hide();
	$('.pages-dragger').sortable('destroy');
	$('.page-thumbnail').each(function(index, item) {
		$(item).click(ddoc.OnNewThumbnailClick);
	});
};

DDocUi.prototype.UpdateNewPagesSequence = function(noAlert, grid) {

	var document = (function() {
		var data = { Pages: [] };
		$('.page-thumbnail').each(function(index, item) {
			var page = $(item).data('dataItem');
			page.Sequence = index;
			data.Pages.push(page);
		});
		return data;
	})();

	ddoc.POST('/Document/UpdatePagesSequence' + (ddoc.StandaloneWindow ? '?apiToken=' + ddoc.ApiToken : ''), document, function() {
		if (!noAlert) {
			ddoc.ShowAlert('Secuencia de páginas actualizada!', function() {
				ddoc.DisableNewSortable();
				ddoc.LoadNewDocumentPages();
			});
		}
	});
};

DDocUi.prototype.UndoNewSort = function() {
	tinysort($('.page-thumbnail'), { data: 'order' });
	ddoc.DisableNewSortable();
};

DDocUi.prototype.InitNewScroller = function () {
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

DDocUi.prototype.ValidateDocumentFields = function () {
	var error = false;
	$('#frmNewDocument').find('input[type="text"]').each(function (i, input) {
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

DDocUi.prototype.CommitDocument = function (grid) {

	var document = (function getFormData() {
		var data = {};
		$.each($('#frmNewDocument').serializeArray(), function (i, field) {
			data[field.name] = field.value;
		});
		$('input[type="checkbox"]', '#frmNewDocument').each(function (i, checkbox) {
			if ($(checkbox).is(':checked')) {
				data[checkbox.name] = 'True';
			} else {
				data[checkbox.name] = 'False';
			}
		});
		$.extend(data, {
			Id: ddoc.document.Id,
			CollectionId: ddoc.document.CollectionId,
			Name: ddoc.document.Name
		});
		return data;
	})();

	ddoc.POST('/Document/Commit' + (ddoc.StandaloneWindow ? '?apiToken=' + ddoc.ApiToken : ''), document, function (response) {
		ddoc.ShowAlert('Documento Guardado!', function () {
			$('#newDocumentEditor').dialog('destroy');
		});
		if (ddoc.StandaloneWindow) {
			window.location = 'about:blank';
		} else {
			if (grid) {
				ddoc.GetPagedResults(grid.currentPage, grid.pageSize);
			} else {
				ddoc.GetPagedResults(ddoc.grdResults.currentPage, ddoc.grdResults.pageSize);
			}
		}
	});
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
		evt => {
			document.body.style.mozUserSelect = document.body.style.webkitUserSelect = document.body.style
				.userSelect = 'none';
			lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
			lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
			dragStart = ctx.transformedPoint(lastX, lastY);
			dragged = false;
		},
		false);

	canvas.addEventListener('mousemove',
		evt => {
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

	var zoom = clicks => {
		const pt = ctx.transformedPoint(lastX, lastY);
		ctx.translate(pt.x, pt.y);
		const factor = Math.pow(scaleFactor, clicks);
		ctx.scale(factor, factor);
		ctx.translate(-pt.x, -pt.y);
		redraw();
	};
	canvas.addEventListener('mouseup',
		evt => {
			dragStart = null;
			if (!dragged) {
				zoom(evt.shiftKey ? -1 : 1);
			}
		},
		false);

	const handleScroll = evt => {
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