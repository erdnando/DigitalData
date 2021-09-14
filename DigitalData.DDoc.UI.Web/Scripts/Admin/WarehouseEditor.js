DDocUi.prototype.NewWarehouse = function() {
    ddoc.warehouse = { IsNew: true };
    ddoc.CreateModal('/Admin/NewWarehouse', undefined, 'warehouseEditor', 'Nuevo almacén', { width: 700 }, ddoc.InitWarehouseEditor);
};

DDocUi.prototype.EditWarehouse = function() {
    ddoc.CreateModal('/Admin/EditWarehouse', ddoc.warehouse, 'warehouseEditor', 'Editar almacén', { width: 700 }, ddoc.InitWarehouseEditor);
};

DDocUi.prototype.InitWarehouseEditor = function() {

    (function setupEvents() {
        function onMenuBarOptionClick() {
            switch (this.id) {
                case 'editPaths':
                    ddoc.EditWarehousePaths();
                    break;
            }
        }

        function onDialogButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'saveWarehouse':
                    ddoc.SaveWarehouse();
                    break;
                case 'cancel':
                    ddoc.warehouse = undefined;
                    $('#warehouseEditor').dialog('destroy');
                    break;
            }
        }

        $('.menu-bar').on('click', 'li', onMenuBarOptionClick);
        $('#frmWarehouseEdit').on('click', 'button', onDialogButtonClick);
    })();
};

DDocUi.prototype.SaveWarehouse = function() {

    var warehouse = (function getFormData() {
        var data = {};
        $.each($('#frmWarehouseEdit').serializeArray(), function(i, field) {
            data[field.name] = field.value;
        });
        if (data.IsNew === 'True') {
            data.IsNew = true;
        } else {
            data.IsNew = false;
        }
        return data;
    })();

    ddoc.POST('/Admin/SaveWarehouse', warehouse, function() {
        ddoc.ShowAlert('Almacén guardado con éxito!', function() {
            ddoc.warehouse = undefined;
            $('#warehouseEditor').dialog('destroy');
            ddoc.LoadWarehouses();
        });
    });
};