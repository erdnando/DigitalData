DDocUi.prototype.NewFileServer = function() {
    ddoc.fileServer = { IsNew: true };
    ddoc.CreateModal('/Admin/NewFileServer', undefined, 'fileServerEditor', 'Nuevo servidor de archivos', { width: 700 }, ddoc.InitFileServerEditor);
};

DDocUi.prototype.EditFileServer = function() {
    ddoc.CreateModal('/Admin/EditFileServer', ddoc.fileServer, 'fileServerEditor', 'Editar servidor de archivos', { width: 700 }, ddoc.InitFileServerEditor);
};

DDocUi.prototype.InitFileServerEditor = function() {

    (function setupEvents() {
        function onDialogButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'saveFileServer':
                    ddoc.SaveFileServer();
                    break;
                case 'cancel':
                    ddoc.fileServer = undefined;
                    $('#fileServerEditor').dialog('destroy');
                    break;
            }
        }

        $('#frmFileServerEdit').on('click', 'button', onDialogButtonClick);
    })();
};

DDocUi.prototype.SaveFileServer = function() {

    var fileServer = (function getFormData() {
        var data = {};

        $.each($('#frmFileServerEdit').serializeArray(), function(i, field) {
            data[field.name] = field.value;
        });

        if (data.IsNew === 'True') {
            data.IsNew = true;
        } else {
            data.IsNew = false;
        }

        return data;
    })();

    ddoc.POST('/Admin/SaveFileServer', fileServer, function() {
        ddoc.ShowAlert('Servidor de archivos guardado con éxito!', function() {
            ddoc.fileServer = undefined;
            $('#fileServerEditor').dialog('destroy');
            ddoc.LoadFileServers();
        });
    });
};