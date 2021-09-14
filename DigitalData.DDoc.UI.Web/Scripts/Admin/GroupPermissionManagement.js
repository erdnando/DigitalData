
DDocUi.prototype.ViewDdocGroupPermissions = function () {
    ddoc.CreateModal('/Admin/ViewDdocGroupPermissions', { groupName: ddoc.group.Name, groupType: ddoc.group.Type }, 'groupPermissions', 'Permisos del grupo de ' + (ddoc.group.Type == 1 ? 'usuarios' : 'seguridad'), { width: 900 }, ddoc.InitGroupPermissions);
};

DDocUi.prototype.InitGroupPermissions = function() {

    (function setupControls() {
        function setupGrid() {
            function onGridAction(e) {
                ddoc.permission = e.dataRow;
                switch (e.command) {
                    case 'editPermission':
                        ddoc.EditPermission(ddoc.group.Id, ddoc.group.Type);
                        break;
                    case 'deletePermission':
                        ddoc.DeleteGroupPermission();
                        break;
                }
            };

            ddoc.grdGroupPermissions = new DDoc.Controls.Grid('grdGroupPermissions', { height: 305, defaultFilter: true, showColumnMenu: false, pager: { sizes: [8], pageSize: 8, show: true } });

            var colType = DDoc.Constants.columnType;

            var columns = [
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'editPermission', cssClass: { normal: 'imgPermissionEdit' }, tooltip: 'Editar permiso' } }
            ];
            switch (ddoc.group.Type) {
                case 1:
                    columns.push({ text: 'Grupo de seguridad', field: 'SecurityGroupName', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true });
                    break;
                case 2:
                    columns.push({ text: 'Grupo de usuarios', field: 'UserGroupName', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true });
                    break;
            }
            columns.push({ text: 'Ver', field: 'ReadPermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true });
            columns.push({ text: 'Modificar', field: 'WritePermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true });
            columns.push({ text: 'Comentar', field: 'CommentPermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true });
            columns.push({ text: 'Imprimir', field: 'PrintPermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true });
            columns.push({ text: 'Exportar', field: 'ExportPermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true });
            columns.push({ text: 'Eliminar', field: 'DeletePermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true });
            columns.push({ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'deletePermission', cssClass: { normal: 'imgPermissionDelete' }, tooltip: 'Eliminar permiso' } });

            ddoc.grdGroupPermissions.init(columns);
            ddoc.grdGroupPermissions.addListener('onAction', onGridAction);
        }

        setupGrid();
        $('#groupPermissions').dialog('option', 'position', { my: 'center', at: 'center', of: window });
    })();

    (function setupEvents() {
        function onMenuBarOptionClick() {
            switch (this.id) {
                case 'addPermission':
                    ddoc.NewPermission(ddoc.group.Id, ddoc.group.Type);
                    break;
            }
        }

        function onDialogButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'closeGroupPermissionsList':
                    $('#groupPermissions').dialog('destroy');
                    break;
            }
        }

        $('.menu-bar', '#groupPermissions').on('click', 'li', onMenuBarOptionClick);
        $('#groupPermissions').on('click', 'button', onDialogButtonClick);
    })();

    ddoc.LoadGroupPermissions();
};

DDocUi.prototype.LoadGroupPermissions = function() {

    ddoc.grdGroupPermissions.showLoader();

    ddoc.GET('/Admin/GetDdocGroupPermissions', { groupId: ddoc.group.Id, groupType: ddoc.group.Type }, function(response) {
        ddoc.grdGroupPermissions.reload(response.List);
    });
};

DDocUi.prototype.DeleteGroupPermission = function () {
    ddoc.Confirm('¿Está seguro que desea eliminar el permiso seleccionado?', function () {
        ddoc.POST('/Admin/DeletePermission', { permissionId: ddoc.permission.Id }, function () {
            ddoc.ShowAlert('Permiso eliminado!', function () {
                ddoc.LoadGroupPermissions();
            });
        });
    });
};