DDocUi.prototype.InitViewerPage = function () {

	(function setupControls () {

        if ($('#ShowCustomPanel').val() == 'True') {
			this.customPanel = new DDocUi.CustomPanel({
				panel: document.getElementById('customPanel'),
				toggleButton: document.getElementById('customPanelActivator')
			});

            $('#customPanelActivator').hide();
        }

		$('.download').attr('title', 'Descargar PDF o XML');

		ddoc.currentFile = 0;

		if ($('#NoPermission').length > 0 && $('#NoPermission').val() == '1') {
			ddoc.ShowAlert('No tiene permiso para ver este elemento', function () { window.history.back(1); });
		}

        if (typeof ddoc.CfdiViewerCustomActions === "function") {
            var customActions = ddoc.CfdiViewerCustomActions();

            for (var action of customActions) {
                if (action.collectionId == collectionId) {
                    $('#customPanelActivator').show();
                    var $action = $('<a id="CfdiViewerCustomPdf" class="' + action.ImageClass + '" title="' + action.Tooltip + '">Descargar PDF.</a>');
                    if (typeof action.Handler === "function")
                        $action.on("click", action.Handler);
                    $('#customPanel').append($action);
                }
            }
        }
	})();

    (function setupEvents () {
        $('.download', '.toolbarViewerLeft').on('click', ddoc.Download);

        $('#goBack').click(function () {
            window.history.back();
        });
    })();

    if (typeof CfdiViewerPlugin === "function") {
        CfdiViewerPlugin();
    }
};

DDocUi.prototype.Download = function () {

	function onPageDownloadModeButtonClick () {
    
        var file = $(this).parent().data();

        var type = file.Type.toLowerCase().trim('.');

        var action = type == 'pdf' ? '/Viewer/OpenPdf/' : '/api/files/downloadFile/';
        var parameter = type == 'pdf' ? documentId + '/' + parseInt(this.id.substr(9, 1)) : documentFiles[parseInt(this.id.substr(9, 1))].Id + '/' + file.Type.toLowerCase().trim('.') + '?user=' + ddoc.GetUsername();
        var target = type == 'pdf' ? '_blank' : '_self';
        var windowSize = type == 'pdf' ? 'width=1024,height=768' : undefined;

        window.open(ddoc.GetUrl(action + parameter), target, windowSize);

		$('#cfdiDownload').dialog('destroy');
	}

	ddoc.CreateModal('/Viewer/CfdiDownload', undefined, 'cfdiDownload', 'Seleccione archivo a descaragar', { width: 520 }, function () {
        for (var i = 0; i < documentFiles.length; i++) {
            var file = documentFiles[i];
            var $icon = $('<div>').text('Archivo ' + file.Type.toUpperCase().trim('.')).data(file);
            $icon.append($('<div>').attr('id', 'download_' + i).addClass('file-type-icon file-icon-lg')
                .attr('data-type', file.Type.toLowerCase().trim('.')));
            $('#fileTypes').append($icon);
        }
        $('#cfdiDownload').on('click', 'div.file-type-icon', onPageDownloadModeButtonClick);
		$('#cancelCfdiDownload').on('click', function () { $('#cfdiDownload').dialog('destroy'); });
    });
};

DDocUi.prototype.ProcessCfdi = function () {
	
};

var ddoc = new DDocUi();
ddoc.InitViewerPage();