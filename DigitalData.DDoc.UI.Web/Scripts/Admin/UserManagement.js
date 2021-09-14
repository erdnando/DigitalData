
DDocUi.prototype.InitUserManagement = function() {

    (function setupControls() {
        function setupGrid() {
            function onGridAction(e) {
                ddoc.user = e.dataRow;
                switch (e.command) {
                    case 'edit':
                        ddoc.EditUser();
                        break;
                    case 'unlock':
                        ddoc.UnlockUser();
                        break;
                    case 'delete':
                        ddoc.DeleteUser();
                        break;
                }
            };

            ddoc.grdUsers = new DDoc.Controls.Grid('grdUsers', { title: 'Usuarios', defaultFilter: true, showColumnMenu: false, pager: { show: true } });

            var colType = DDoc.Constants.columnType;
            var columns = [
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'edit', cssClass: { normal: 'imgEdit' }, tooltip: 'Editar usuario' } },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'unlock', cssClass: { normal: 'imgUnlock' }, tooltip: 'Desbloquear contraseña' } },
                { text: 'Usuario', field: 'Username', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Nombre', field: 'Name', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Email', field: 'Email', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'delete', cssClass: { normal: 'imgDelete' }, tooltip: 'Eliminar usuario' } }
            ];

            ddoc.grdUsers.init(columns);
            ddoc.grdUsers.addListener('onAction', onGridAction);
        }

        setupGrid();
    })();

    (function setupEvents() {
        function onMenuBarOptionClick() {
            switch (this.id) {
                case 'newUser':
                    ddoc.NewUser();
                    break;
                case 'editPasswordConfiguration':
                    ddoc.EditPasswordConfiguration();
                    break;
            }
        }

        $('.menu-bar').on('click', 'li', onMenuBarOptionClick);
    })();

    ddoc.LoadUsers();
};

DDocUi.prototype.LoadUsers = function() {

    ddoc.grdUsers.showLoader();

    ddoc.GET('/Admin/GetUsers', undefined, function(response) {
        ddoc.grdUsers.reload(response.List);
    });
};

DDocUi.prototype.UnlockUser = function () {
    ddoc.Confirm('¿Esta seguro que desea desbloqear la contraseña de este usuario?', function () {
        ddoc.POST('/Admin/UnlockUser', { username: ddoc.user.Username }, function () {
            ddoc.ShowAlert('Usuario desbloqueado con éxito!', function () {
                ddoc.LoadUsers();
            });
        });
    });
};

DDocUi.prototype.DeleteUser = function () {
    ddoc.Confirm('¿Está seguro que desea eliminar este usuario?', function () {
        ddoc.POST('/Admin/DeleteUser', { username: ddoc.user.Username }, function () {
            ddoc.ShowAlert('Usuario eliminado con éxito!', function () {
                ddoc.LoadUsers();
            });
        });
    });
};

var ddoc = new DDocUi();
ddoc.InitUserManagement();