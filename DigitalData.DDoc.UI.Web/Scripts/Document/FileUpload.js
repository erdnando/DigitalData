DDocUi.prototype.FileUpload = function (pageId, sequence, grid) {
    $('#newPagePrompt').dialog('destroy');
    $('#replacePagePrompt').dialog('destroy');

    ddoc.CreateModal('/Document/FileUpload' + (ddoc.StandaloneWindow ? '?apiToken=' + ddoc.ApiToken : ''), { document: ddoc.document }, 'documentAddPage', 'Agregar Páginas', { width: 900 }, function() {
        ddoc.InitFileUpload(pageId, sequence, grid);
    });
};

DDocUi.prototype.InitFileUpload = function (pageId, sequence, grid) {

    (function setupControls() {
        ddoc.prevText = '';

        ddoc.UploadedFiles = new FormData();
    })();
    
    (function setupEvents() {
        function onDialogButtonClick() {
            switch (this.id) {
                case 'cancelAddPages':
                    ddoc.UploadedFiles = undefined;
                    $('#documentAddPage').dialog('destroy');
                    break;
                case 'uploadImages':
                    ddoc.UploadImages(pageId, sequence);
                    break;
            }
        }

        function onFileChange(e) {
            var files = e.target.files;

            if (files.length > 0) {
                if (window.FormData !== undefined) {
                    ddoc.AddFileToQueue(files);
                }
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
            var files = e.originalEvent.dataTransfer.files;
            ddoc.AddFileToQueue(files);
        };

        $('#files').change(onFileChange);
        $('#drop_zone').on('dragover', onDragOver).on('drop', onDrop);
        $('#documentAddPage').on('click', 'button', onDialogButtonClick);
    })();
};

DDocUi.prototype.AddFileToQueue = function(files) {
    var texto = '';
    var output = [];

    for (var i = 0, f; f = files[i]; i++) {
        var nombre = f.name;
        ddoc.UploadedFiles.append(nombre, f);
        output.push('<li><strong>', escape(f.name), '</strong> - ', f.size, ' bytes', '</li>');
    }

    $('#list').html(ddoc.prevText + '<ul>' + output.join('') + '</ul>');
    texto = texto + '<ul>' + output.join('') + '</ul>';
    ddoc.prevText = ddoc.prevText + texto;

    $('#drop_zone, #out_zone').addClass('activated');
};

DDocUi.prototype.UploadImages = function (pageId, sequence) {
    ddoc.POST('/Document/UploadPages?documentId=' + ddoc.document.Id + (pageId ? '&pageId=' + pageId : '') + (sequence ? '&sequence=' + sequence : '') + (ddoc.StandaloneWindow ? '&apiToken=' + ddoc.ApiToken : ''), ddoc.UploadedFiles, function () {
        $('#documentAddPage').dialog('destroy');
        if (!ddoc.document.IsNew) {
            ddoc.ShowAlert('Páginas agregadas!');
        }
        if (ddoc.document.IsNew) {
            ddoc.LoadNewDocumentPages();
        }
        else {
            if (typeof ddoc.LoadDocumentPages == 'function') {
                ddoc.LoadDocumentPages();
            }
            if (typeof ddoc.LoadDocuments == 'function') {
                ddoc.LoadDocuments();
            }
        }
    }, true);
};