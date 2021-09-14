
DDocUi.prototype.InitSecurityManagement = function() {

    (function setupControls() {
        function setupGrids() {
            function onGridAction(e) {
                ddoc.group = e.dataRow;
                switch (e.command) {
                    case 'editGroup':
                        ddoc.EditDdocGroup();
                        break;
                    case 'viewGroupPermissions':
                        ddoc.ViewDdocGroupPermissions();
                        break;
                    case 'deleteGroup':
                        ddoc.DeleteDdocGroup();
                        break;
                }
            };

            ddoc.grdUserGroups = new DDoc.Controls.Grid('grdUserGroups', { title: 'Grupos de Usuarios', defaultFilter: true, showColumnMenu: false, pager: { show: false } });
            ddoc.grdSecurityGroups = new DDoc.Controls.Grid('grdSecurityGroups', { title: 'Grupos de Seguridad', defaultFilter: true, showColumnMenu: false, pager: { show: false } });

            var colType = DDoc.Constants.columnType;

            var ntColumns = [
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'editGroup', cssClass: { normal: 'imgUserGroupEdit' }, tooltip: 'Editar grupo' } },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'viewGroupPermissions', cssClass: { normal: 'imgUserGroupPermissions' }, tooltip: 'Ver permisos' } },
                { width: 100, text: 'Id', field: 'Id', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Nombre', field: 'Name', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'deleteGroup', cssClass: { normal: 'imgUserGroupDelete' }, tooltip: 'Eliminar grupo' } }
            ];

            var securityColumns = [
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'editGroup', cssClass: { normal: 'imgSecurityGroupEdit' }, tooltip: 'Editar grupo' } },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'viewGroupPermissions', cssClass: { normal: 'imgSecurityGroupPermissions' }, tooltip: 'Ver permisos' } },
                { width: 100, text: 'Id', field: 'Id', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Nombre', field: 'Name', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'deleteGroup', cssClass: { normal: 'imgSecurityGroupDelete' }, tooltip: 'Eliminar grupo' } }
            ];

            ddoc.grdUserGroups.init(ntColumns);
            ddoc.grdSecurityGroups.init(securityColumns);
            ddoc.grdUserGroups.addListener('onAction', onGridAction);
            ddoc.grdSecurityGroups.addListener('onAction', onGridAction);
        }

        setupGrids();
    })();

    (function setupEvents() {
        function onMenuBarOptionClick() {
            switch (this.id) {
                case 'newUserGroup':
                    ddoc.NewDdocGroup(1);
                    break;
                case 'newSecurityGroup':
                    ddoc.NewDdocGroup(2);
                    break;
            }
        }

        $('.menu-bar').on('click', 'li', onMenuBarOptionClick);
    })();

    ddoc.LoadSecurityGroups();
    ddoc.LoadUserGroups();
};

DDocUi.prototype.LoadUserGroups = function () {
    ddoc.grdUserGroups.showLoader();

    ddoc.GET('/Admin/GetDdocGroups', { groupType: 1 }, function (response) {
        ddoc.grdUserGroups.reload(response.List);
    });
};

DDocUi.prototype.LoadSecurityGroups = function() {
    ddoc.grdSecurityGroups.showLoader();

    ddoc.GET('/Admin/GetDdocGroups', { groupType: 2 }, function(response) {
        ddoc.grdSecurityGroups.reload(response.List);
    });
};

DDocUi.prototype.DeleteDdocGroup = function () {
    ddoc.Confirm('¿Está seguro que desea eliminar este grupo de ' + (ddoc.group.Type == 1 ? 'usuarios' : 'seguridad') + '?', function () {
        ddoc.POST('/Admin/DeleteDdocGroup', { groupId: ddoc.group.Id, groupType: ddoc.group.Type }, function () {
            ddoc.ShowAlert('Grupo de ' + (ddoc.group.Type == 1 ? 'usuarios' : 'seguridad') + ' eliminado!', function () {
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
    });
};

var ddoc = new DDocUi();
ddoc.InitSecurityManagement();