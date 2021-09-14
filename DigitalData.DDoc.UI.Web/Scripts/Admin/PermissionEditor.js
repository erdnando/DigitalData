
DDocUi.prototype.NewPermission = function(isGroupPermission, groupType) {
    ddoc.permission = { IsNew: true };
    ddoc.isGroupPermission = isGroupPermission;
    ddoc.CreateModal('/Admin/NewPermission', { isGroupPermission: isGroupPermission, groupType: groupType }, 'permissionEditor', 'Nuevo Permiso', { width: 500 }, ddoc.InitPermissionEditor);
};

DDocUi.prototype.EditPermission = function (isGroupPermission, groupType) {
    ddoc.isGroupPermission = isGroupPermission;
    ddoc.CreateModal('/Admin/EditPermission', { permission: ddoc.permission, isGroupPermission: isGroupPermission, groupType: groupType }, 'permissionEditor', 'Editar Permiso', { width: 500 }, ddoc.InitPermissionEditor);
};

DDocUi.prototype.InitPermissionEditor = function() {

    (function setupEvents() {
        function onDialogButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'cancelPermissionEdit':
                    ddoc.permission = undefined;
                    $('#permissionEditor').dialog('destroy');
                    break;
                case 'savePermission':
                    ddoc.SavePermission();
                    break;
            }
        }

        $('#frmPermissionEdit').on('click', 'button', onDialogButtonClick);
    })();
};

DDocUi.prototype.SavePermission = function () {

    var permission = (function getFormData() {
        var data = {};
        $.each($('#frmPermissionEdit').serializeArray(), function(i, field) {
            data[field.name] = field.value;
        });
        $('input[type="checkbox"]', '#frmPermissionEdit').each(function(i, checkbox) {
            if ($(checkbox).is(':checked'))
                data[checkbox.id] = true;
            else {
                data[checkbox.id] = false;
            }
        });
        if (data.IsNew === 'True') {
            data.IsNew = true;
        } else {
            data.IsNew = false;
        }
        return data;
    })();

    ddoc.POST('/Admin/SavePermission', permission, function() {
        ddoc.ShowAlert('Permiso guardado con éxito!', function() {
            ddoc.permission = undefined;
            if (ddoc.isGroupPermission) {
                $('#permissionEditor').dialog('destroy');
                ddoc.LoadGroupPermissions();
            } else {
                $('#permissionEditor').dialog('destroy');
                ddoc.LoadAllPermissions();
            }
        });
    });
};