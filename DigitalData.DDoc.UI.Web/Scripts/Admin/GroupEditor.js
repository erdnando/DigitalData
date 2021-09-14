
DDocUi.prototype.NewDdocGroup = function(type) {
    ddoc.group = { IsNew: true, Type: type };
    ddoc.CreateModal('/Admin/NewDdocGroup', { groupType: type }, 'groupEditor', 'Nuevo grupo de ' + (ddoc.group.Type == 1 ? 'usuarios' : 'seguridad'), { width: 650 }, ddoc.InitDdocGroupEditor);
};

DDocUi.prototype.EditDdocGroup = function() {
    ddoc.CreateModal('/Admin/EditDdocGroup', ddoc.group, 'groupEditor', 'Editar grupo de ' + (ddoc.group.Type == 1 ? 'usuarios' : 'seguridad'), { width: 650 }, ddoc.InitDdocGroupEditor);
};

DDocUi.prototype.InitDdocGroupEditor = function() {

    (function setupEvents() {
        function onDialogButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'cancelEditGroup':
                    ddoc.group = undefined;
                    $('#groupEditor').dialog('destroy');
                    break;
                case 'saveGroup':
                    ddoc.SaveDdocGroup();
                    break;
            }
        }

        $('#frmGroupEdit').on('click', 'button', onDialogButtonClick);
    })();
};

DDocUi.prototype.SaveDdocGroup = function() {

    var group = (function getFormData() {
        var data = {};

        $.each($('#frmGroupEdit').serializeArray(), function(i, field) {
            data[field.name] = field.value;
        });

        if (data.IsNew === 'True') {
            data.IsNew = true;
        } else {
            data.IsNew = false;
        }

        return data;
    })();

    ddoc.POST('/Admin/SaveDdocGroup', group, function() {
        ddoc.ShowAlert('Grupo de ' + (ddoc.group.Type == 1 ? 'usuarios' : 'seguridad') + ' guardado!', function() {
            $('#groupEditor').dialog('destroy');
            switch (ddoc.group.Type) {
                case 1:
                    ddoc.LoadUserGroups();
                    break;
                case 2:
                    ddoc.LoadSecurityGroups();
                    break;
            }
            ddoc.group = undefined;
        });
    });
};