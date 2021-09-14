
DDocUi.prototype.InitPermissionManagement = function() {

    (function setupControls() {
        function setupGrid() {
            function onGridAction(e) {
                ddoc.permission = e.dataRow;
                switch (e.command) {
                    case 'editPermission':
                        ddoc.EditPermission(0, 0);
                        break;
                    case 'deletePermission':
                        ddoc.DeletePermission();
                        break;
                }
            };

            ddoc.grdPermissions = new DDoc.Controls.Grid('grdPermissions', { defaultFilter: true, showColumnMenu: false, pager: { show: true } });

            var colType = DDoc.Constants.columnType;

            var columns = [
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'editPermission', cssClass: { normal: 'imgPermissionEdit' }, tooltip: 'Editar permiso' } },
                { text: 'Grupo de usuarios', field: 'UserGroupName', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Grupo de seguridad', field: 'SecurityGroupName', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Ver', field: 'ReadPermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Modificar', field: 'WritePermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Comentar', field: 'CommentPermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Imprimir', field: 'PrintPermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Exportar', field: 'ExportPermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Eliminar', field: 'DeletePermission', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'deletePermission', cssClass: { normal: 'imgPermissionDelete' }, tooltip: 'Eliminar permiso' } }
            ];

            ddoc.grdPermissions.init(columns);
            ddoc.grdPermissions.addListener('onAction', onGridAction);
        }

        setupGrid();
    })();

    (function setupEvents() {
        function onMenuBarOptionClick() {
            switch (this.id) {
                case 'addPermission':
                    ddoc.NewPermission(0, 0);
                    break;
            }
        }

        $('.menu-bar').on('click', 'li', onMenuBarOptionClick);
    })();

    ddoc.LoadAllPermissions();
}

DDocUi.prototype.LoadAllPermissions = function() {

    ddoc.grdPermissions.showLoader();

    ddoc.GET('/Admin/GetAllPermissions', undefined, function(response) {
        ddoc.grdPermissions.reload(response.List);
    });
};

DDocUi.prototype.DeletePermission = function () {
    ddoc.Confirm('¿Está seguro que desea eliminar el permiso seleccionado?', function () {
        ddoc.POST('/Admin/DeletePermission', { permissionId: ddoc.permission.Id }, function () {
            ddoc.ShowAlert('Permiso eliminado!', function () {
                ddoc.LoadAllPermissions();
            });
        });
    });
};

var ddoc = new DDocUi();
ddoc.InitPermissionManagement();