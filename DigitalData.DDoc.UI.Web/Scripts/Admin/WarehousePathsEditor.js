DDocUi.prototype.EditWarehousePaths = function() {
    ddoc.CreateModal('/Admin/EditWarehousePaths', ddoc.warehouse, 'warehousePathsEditor', 'Editar rutas', { width: 700 }, ddoc.InitWarehousePathsEditor);
};

DDocUi.prototype.InitWarehousePathsEditor = function() {

    (function setupControls() {
        if (ddoc.warehouse.Paths && ddoc.warehouse.Paths.length > 0) {
            $('option', '#Paths').each(function(i, path) {
                if ($(path).val() == ddoc.warehouse.ActivePathId)
                    $('option', '#Paths').eq(i).addClass('selected-option');
            });
        }
    })();

    (function setupEvents() {
        function initPathEditor() {
            (function setupEvents() {
                function onPathEditorButtonClick(e) {
                    e.preventDefault();
                    switch (this.id) {
                        case 'savePath':
                            ddoc.SaveWarehousePath();
                            $('#pathEditor').dialog('destroy');
                            break;
                        case 'cancelPathEdit':
                            $('#pathEditor').dialog('destroy');
                            break;
                    }
                }

                $('.dialog-button-panel', '#pathEditor').on('click', 'button', onPathEditorButtonClick);
            })();
        }

        function onListboxActionClick() {
            switch (this.id) {
                case 'addPath':
                    ddoc.CreateModal('/Admin/NewPath', undefined, 'pathEditor', 'Agregar nueva ruta', { width: 500 }, initPathEditor);
                    break;
                case 'updatePath':
                    ddoc.CreateModal('/Admin/EditPath', { pathId: parseInt($('option:selected', '#Paths').val()) }, 'pathEditor', 'Editar ruta', { width: 500 }, initPathEditor);
                    break;
                case 'removePath':
                    ddoc.DeleteWarehousePath();
                    break;
                case 'setActivePath':
                    ddoc.UpdateWarehouseActivePath();
                    break;
            }
        }

        function onPathsEditorButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'closePathsEditor':
                    $('#warehousePathsEditor').dialog('destroy');
            }
        }

        $('.listbox-actions').on('click', '.listbox-action', onListboxActionClick);
        $('.dialog-button-panel', '#warehousePathsEditor').on('click', 'button', onPathsEditorButtonClick);
    })();

    ddoc.LoadWarehousePaths();
};
DDocUi.prototype.LoadWarehousePaths = function() {

    $('#Paths').find('option').remove();

    ddoc.GET('/Admin/GetWarehousePaths', { warehouseId: ddoc.warehouse.Id }, function(response) {
        $.each(response.List, function(i, path) {
            $('#Paths').append($('<option value="' + path.Id + '" ' + (path.Active ? 'class="selected-option"' : '') + '>' + path.RootPath + '</option>').data('pathItem', path));
        });
    });
};

DDocUi.prototype.SaveWarehousePath = function () {

    var path = (function getFormData() {
        var data = {};
        $.each($('#frmPathEdit').serializeArray(), function (i, field) {
            data[field.name] = field.value;
        });
        if (data.IsNew === 'True') {
            data.IsNew = true;
        } else {
            data.IsNew = false;
        }
        return data;
    })();

    ddoc.POST('/Admin/SaveWarehousePath', { warehouseId: ddoc.warehouse.Id, path: path }, function () {
        ddoc.ShowAlert('Ruta guardada!', function() {
            ddoc.LoadWarehousePaths();
        });
    });
};

DDocUi.prototype.DeleteWarehousePath = function() {
    ddoc.Confirm('¿Está seguro de eliminar la ruta seleccionada?', function() {
        ddoc.POST('/Admin/DeleteWarehousePath', { pathId: parseInt($('option:selected', '#Paths').val()) }, function() {
            ddoc.ShowAlert('Ruta eliminada correctamente!', function() {
                ddoc.LoadWarehousePaths();
                if ($('option', '#Paths').length == 0) {
                    $('#warehouseEditor').dialog('destroy');
                    $('#warehousePathsEditor').dialog('destroy');
                    ddoc.LoadWarehouses();
                }
            });
        });
    });
};

DDocUi.prototype.UpdateWarehouseActivePath = function() {
    ddoc.POST('/Admin/SetWarehouseActivePath', { warehouseId: ddoc.warehouse.Id, pathId: parseInt($('option:selected', '#Paths').val()) }, function() {
        ddoc.ShowAlert('Se seleccionó una nueva ruta activa para el almacén. Los documentos nuevos seran almacenados en la nueva ruta.', function() {
            $('#warehouseEditor').dialog('destroy');
            $('#warehousePathsEditor').dialog('destroy');
            ddoc.LoadWarehousePaths();
            ddoc.LoadWarehouses();
        });
    });
};