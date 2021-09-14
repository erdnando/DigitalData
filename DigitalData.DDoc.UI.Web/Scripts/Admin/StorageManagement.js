
DDocUi.prototype.InitStorageManagement = function() {

    (function setupControls() {
        function setupGrids() {
            function onServerGridAction(e) {
                ddoc.fileServer = e.dataRow;
                switch (e.command) {
                    case 'edit':
                        ddoc.EditFileServer();
                        break;
                    case 'delete':
                        ddoc.DeleteFileServer();
                        break;
                }
            };

            function onWarehouseGridAction(e) {
                ddoc.warehouse = e.dataRow;
                switch (e.command) {
                    case 'edit':
                        ddoc.EditWarehouse();
                        break;
                    case 'delete':
                        ddoc.DeleteWarehouse();
                        break;
                }
            };

            ddoc.grdServers = new DDoc.Controls.Grid('grdServers', { title: 'Servidores de archivos', showFilter: false, showColumnMenu: false, pager: { show: false } });
            ddoc.grdWarehouses = new DDoc.Controls.Grid('grdWarehouses', { title: 'Almacenes DDOC', showFilter: false, showColumnMenu: false, pager: { show: false } });

            var colType = DDoc.Constants.columnType;

            var serverColumns = [
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'edit', cssClass: { normal: 'imgServerEdit' }, tooltip: 'Editar servidor' } },
                { text: 'Id', field: 'Id', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Nombre', field: 'Name', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Url', field: 'Url', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'delete', cssClass: { normal: 'imgServerDelete' }, tooltip: 'Eliminar servidor' } }
            ];

            var warehouseColumns = [
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'edit', cssClass: { normal: 'imgWarehouseEdit' }, tooltip: 'Editar almacén' } },
                { text: 'Id', field: 'Id', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Nombre', field: 'Name', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Servidor', field: 'ServerId', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Raíz del almacén', field: 'ActivePath', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'delete', cssClass: { normal: 'imgWarehouseDelete' }, tooltip: 'Eliminar almacén' } }
            ];

            ddoc.grdServers.init(serverColumns);
            ddoc.grdWarehouses.init(warehouseColumns);
            ddoc.grdServers.addListener('onAction', onServerGridAction);
            ddoc.grdWarehouses.addListener('onAction', onWarehouseGridAction);
        }

        setupGrids();
    })();

    (function setupEvents() {
        function onMenuBarOptionClick() {
            switch (this.id) {
                case 'newFileServer':
                    ddoc.NewFileServer();
                    break;
                case 'newWarehouse':
                    ddoc.NewWarehouse();
                    break;
            }
        }

        $('.menu-bar').on('click', 'li', onMenuBarOptionClick);
    })();

    ddoc.LoadFileServers();
    ddoc.LoadWarehouses();
};

DDocUi.prototype.LoadFileServers = function() {

    ddoc.grdServers.showLoader();

    ddoc.GET('/Admin/GetFileServers', undefined, function(response) {
        ddoc.grdServers.reload(response.List);
    });
};

DDocUi.prototype.DeleteFileServer = function () {
    ddoc.Confirm('¿Está seguro que desea eliminar este servidor?', function () {
        ddoc.POST('/Admin/DeleteFileServer', { fileServerId: ddoc.fileServer.Id }, function () {
            ddoc.ShowAlert('Servidor eliminado!', function () {
                ddoc.LoadFileServers();
            });
        });
    });
};

DDocUi.prototype.LoadWarehouses = function() {

    ddoc.grdWarehouses.showLoader();

    ddoc.GET('/Admin/GetWarehouses', undefined, function(response) {
        ddoc.grdWarehouses.reload(response.List);
    });
};

DDocUi.prototype.DeleteWarehouse = function () {
    ddoc.Confirm('¿Está seguro que desea eliminar este almacén?', function() {
        ddoc.POST('/Admin/DeleteWarehouse', { warehouseId: ddoc.warehouse.Id }, function() {
            ddoc.ShowAlert('Almacén eliminado!', function() {
                ddoc.LoadWarehouses();
            });
        });
    });
};

var ddoc = new DDocUi();
ddoc.InitStorageManagement();