
DDocUi.prototype.EditCollectionRules = function() {
    ddoc.CreateModal('/Admin/RulesList', undefined, 'rulesList', 'Relaciones de colecciones', { width: 700 }, ddoc.InitRulesList);
};

DDocUi.prototype.InitRulesList = function() {
    (function setupControls() {
        function setupGrid() {
            function onGridAction(e) {
                ddoc.rule = e.dataRow;
                switch (e.command) {
                    case 'editRule':
                        ddoc.EditRule();
                        break;
                    case 'deleteRule':
                        ddoc.DeleteCollectionRule();
                        break;
                }
            };

            ddoc.grdCollectionRules = new DDoc.Controls.Grid('grdCollectionRules', { height: 300, defaultFilter: true, showColumnMenu: false, pager: { show: false } });

            var colType = DDoc.Constants.columnType;
            var columns = [
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'editRule', cssClass: { normal: 'imgEditRule' }, tooltip: 'Editar relación' } },
                { text: 'Padre', field: 'ParentName', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Hijo', field: 'ChildName', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Campo Padre', field: 'ParentFieldName', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Campo Hijo', field: 'ChildFieldName', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'deleteRule', cssClass: { normal: 'imgDeleteRule' }, tooltip: 'Eliminar relación' } }
            ];

            ddoc.grdCollectionRules.init(columns);
            ddoc.grdCollectionRules.addListener('onAction', onGridAction);
        }

        setupGrid();
        $('#rulesList').dialog('option', 'position', { my: 'center', at: 'center', of: window });
    })();

    (function setupEvents() {
        function onMenuBarOptionClick() {
            switch (this.id) {
                case 'addRule':
                    ddoc.NewRule();
                    break;
            }
        }

        function onDialogButtonClick() {
            switch(this.id) {
                case 'closeRulesList':
                    $('#rulesList').dialog('destroy');
                    break;
            }
        }

        $('.menu-bar').on('click', 'li', onMenuBarOptionClick);
        $('.dialog-button-panel').on('click', 'button', onDialogButtonClick);
    })();

    ddoc.LoadCollectionRules();
};

DDocUi.prototype.LoadCollectionRules = function () {

    ddoc.grdCollectionRules.showLoader();

    ddoc.GET('/Admin/GetCollectionRules', undefined, function(response) {
        ddoc.grdCollectionRules.reload(response.List);
    });
};

DDocUi.prototype.DeleteCollectionRule = function() {
    ddoc.Confirm('¿Está seguro que desea eliminar esta relación entre colecciones?', function() {
        ddoc.POST('/Admin/DeleteCollectionRule', { ruleId: ddoc.rule.Id }, function () {
            ddoc.LoadCollectionRules();
            ddoc.LoadCollections();
        });
    });
};