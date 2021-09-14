
DDocUi.prototype.EditFilenameTemplate = function() {
    ddoc.CreateModal('/Admin/EditCollectionFilenameTemplate', undefined, 'filenameTemplateEditor', 'Editar plantilla de nombre de archivo para descarga', { width: 600 }, ddoc.InitFilenameTemplateEditor);
};

DDocUi.prototype.InitFilenameTemplateEditor = function() {

    (function setupControls() {
        function setupGrid() {
            function onGridAction(e) {
                ddoc.tempmplateField = e.dataRow;
                switch (e.command) {
                    case 'addField':
                        ddoc.InsertAtCaret('Template', '' + ddoc.tempmplateField.Id);
                        break;
                }
            };

            ddoc.grdTemplateFields = new DDoc.Controls.Grid('grdTemplateFields', { showTools: false, pager: { show: false } });

            var colType = DDoc.Constants.columnType;
            var columns = [
                { width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'addField', cssClass: { normal: 'imgAddTemplateField' }, tooltip: 'Agregar a plantilla' } },
                { text: 'Id', field: 'Id', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Nombre', field: 'Name', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Tipo', field: 'TypeString', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true },
                { text: 'Longitud', field: 'TypeLength', columnType: colType.TEXT, sort: { sortable: true, sorted: 0 }, disableHider: true, visible: true, tooltip: true }
            ];

            ddoc.grdTemplateFields.init(columns);
            ddoc.grdTemplateFields.addListener('onAction', onGridAction);
        }

        $('#Template').val($('#FileDownloadTemplate', '#collectionEditor').val());

        setupGrid();
    })();

    (function setupEvents() {
        function saveTemplate() {
            $('#FileDownloadTemplate', '#collectionEditor').val($('#Template').val());
            $('#filenameTemplateEditor').dialog('destroy');
        }

        function onDialogButtonClick(e) {
            e.preventDefault();
            switch(this.id) {
                case 'saveTemplate':
                    saveTemplate();
                    break;
                case 'cancelEditTemplate':
                    $('#filenameTemplateEditor').dialog('destroy');
                    break;
            }
        }

        function onTemplateKeydown(e) {
            if (e.which == 13) {
                e.preventDefault();
                saveTemplate();
            }
        }

        $('.dialog-button-panel').on('click', 'button', onDialogButtonClick);
        $('#Template').on('keydown', onTemplateKeydown);
    })();

    ddoc.LoadTemplateFields();
};

DDocUi.prototype.LoadTemplateFields = function() {

    ddoc.grdTemplateFields.showLoader();

    ddoc.GET('/Collection/GetFields', { collectionId: ddoc.collection.Id }, function (response) {
        ddoc.grdTemplateFields.reload(response.List);
    });
};

DDocUi.prototype.InsertAtCaret = function (areaId, text) {
    var $template = $('#Template');

    var scrollPos = $template[0].scrollTop;
    var caretPos = $template[0].selectionStart;

    var front = $template.val().substring(0, caretPos);
    var back = $template.val().substring($template[0].selectionEnd, $template.val().length);

    $template.val(front + '{' + text + '}' + back);

    caretPos = caretPos + text.length + 2;

    $template[0].selectionStart = caretPos;
    $template[0].selectionEnd = caretPos;
    $template.focus();
    $template[0].scrollTop = scrollPos;
};