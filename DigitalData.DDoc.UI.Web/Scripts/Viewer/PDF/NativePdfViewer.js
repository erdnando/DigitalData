
DDocUi.prototype.InitViewerPage = function() {

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

		var fileIndexToOpen = $('#fileIndex').val();
		if (fileIndexToOpen) {
			ddoc.currentFile = parseInt(fileIndexToOpen);
		}

		if ($('#NoPermission').length > 0 && $('#NoPermission').val() == '1') {
			ddoc.ShowAlert('No tiene permiso para ver este elemento', function() {
				window.history.back(1);
			});
		} else {
			ddoc.OpenViewer(documentFiles[ddoc.currentFile]);
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

		(function showDocumentData() {
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
		})();
	})();

	(function setupEvents() {
		$('#firstDdocPage').click(function() {
			ddoc.currentFile = 0;
			window.location.search = '?file=' + ddoc.currentFile;
		});

		$('#prevDdocPage').click(function() {
			ddoc.currentFile--;
			window.location.search = '?file=' + ddoc.currentFile;
		});

		$('#nextDdocPage').click(function() {
			ddoc.currentFile++;
			window.location.search = '?file=' + ddoc.currentFile;
		});

		$('#lastDdocPage').click(function() {
			ddoc.currentFile = documentFiles.Length - 1;
			window.location.search = '?file=' + ddoc.currentFile;
		});

		var mainContainer = document.getElementById('mainContainer');
		var outerContainer = document.getElementById('outerContainer');
		mainContainer.addEventListener('transitionend', function(e) {
			if (e.target === mainContainer) {
				var event = document.createEvent('UIEvents');
				event.initUIEvent('resize', false, false, window, 0);
				window.dispatchEvent(event);
				outerContainer.classList.remove('sidebarMoving');
			}
		}, true);

		document.getElementById('sidebarToggle').addEventListener('click', function() {
			this.classList.toggle('toggled');
			outerContainer.classList.add('sidebarMoving');
			outerContainer.classList.toggle('sidebarOpen');
		});
	})();
};

DDocUi.prototype.OpenViewer = function(page) {
	$('#viewerFrame').attr('src', ddoc.GetUrl('/api/files/getDocument/' + page.Id));
	$('#fileNumber').val(ddoc.currentFile + 1);
};

DDocUi.prototype.Download = function() {

	function onDownloadButtonClick() {
		switch (this.id) {
			case 'pageDownload':
				ddoc.PageDownload();
				break;
			case 'documentDownload':
				ddoc.DocumentDownload();
				break;
			case 'cancelDownload':
				$('#downloadMode').dialog('destroy');
				break;
		}
	}

	ddoc.CreateModal('/Viewer/DownloadMode', undefined, 'downloadMode', 'Descarga', { width: 520 }, function() {
		$('#downloadMode').on('click', 'button', onDownloadButtonClick);
	});

};

DDocUi.prototype.PageDownload = function() {
	function onPageDownloadModeButtonClick() {
		switch (this.id) {
			case 'tifDownload':
				window.open(ddoc.GetUrl('/files/downloadPage/' + page.Id + '/tif'), '_self');
				break;
			case 'pdfDownload':
				window.open(ddoc.GetUrl('/files/downloadPage/' + page.Id + '/pdf'), '_self');
				break;
		}
		$('#downloadMode').dialog('destroy');
		$('#pageDownloadMode').dialog('destroy');
	}

	var page = documentFiles[ddoc.currentFile];

	ddoc.CreateModal('/Viewer/PageDownloadMode', undefined, 'pageDownloadMode', 'Seleccione formato', { width: 520 }, function() {
		$('#pageDownloadMode').on('click', 'div.file-type-icon', onPageDownloadModeButtonClick);
		$('#cancelPageDownload').on('click', function() { $('#pageDownloadMode').dialog('destroy'); });
	});
};

DDocUi.prototype.DocumentDownload = function() {
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

	ddoc.CreateModal('/Viewer/DocumentDownloadMode', undefined, 'documentDownloadMode', 'Seleccione formato', { width: 520 }, function() {
		$('#documentDownloadMode').on('click', 'div.file-type-icon', onDocumentDownloadModeButtonClick);
		$('#cancelDocumentDownload').on('click', function() { $('#documentDownloadMode').dialog('destroy'); });
	});
};

var ddoc = new DDocUi();
ddoc.InitViewerPage();