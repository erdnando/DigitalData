DDocUi.prototype.InitFilterSearchGrid = function (contentType, collectionFields) {
    function onGridAction(e) {
        switch (e.command) {
            case 'edit':
                switch (contentType) {
                    case DDocUi.Enums.CollectionType.Document:
                        ddoc.document = e.dataRow;
                        ddoc.EditDocument(ddoc.grdDocumentResults);
                        break;
                    case DDocUi.Enums.CollectionType.Folder:
                        ddoc.folder = e.dataRow;
                        ddoc.EditFolder(ddoc.grdFolderResults);
                        break;
                }
                break;
            case 'send':
                ddoc.ShowAlert('¡Funcionalidad en construcción! ¡Vuelva más tarde!');
                break;
            default:
                if (typeof ddoc.CustomCommands === 'function') {
                    ddoc.CustomCommands(e.command);
                }
                break;
        }
    }

    var columns = [];

    switch (contentType) {
        case DDocUi.Enums.CollectionType.Folder:
            columns.push({ width: 24, field: 'Id', cellType: { type: DDoc.Constants.cellType.ICONLINK, url: ddoc.GetUrl('/Navigation/OpenFolder/{0}'), target: '_self', cssClass: { normal: 'imgFolderView' }, tooltip: 'Ver folder' } });
            columns.push({ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'edit', cssClass: { normal: 'imgFolderEdit' }, tooltip: 'Ver propiedades' } });
            break;
        case DDocUi.Enums.CollectionType.Document:
            columns.push({ width: 24, field: 'Id', cellType: { type: DDoc.Constants.cellType.THUMBNAILLINK, url: ddoc.GetUrl('/Viewer/OpenViewer/{0}'), target: ddoc.ViewerAnchorTarget, cssClass: { normal: 'imgThumb' }, tooltip: 'Ver documento' }, visible: false });
            columns.push({ width: 24, field: 'Id', cellType: { type: DDoc.Constants.cellType.ICONLINK, url: ddoc.GetUrl('/Viewer/OpenViewer/{0}'), target: ddoc.ViewerAnchorTarget, cssClass: { normal: 'imgDocumentView' }, tooltip: 'Ver documento' } });
            columns.push({ width: 24, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'edit', cssClass: { normal: 'imgDocumentEdit' }, tooltip: 'Ver propiedades' } });

            if (typeof ddoc.ListCustomActions === 'function') {
                var customActions = ddoc.ListCustomActions(ddoc.CollectionId);
                $.each(customActions, function (i, item) {
                    columns.push(item);
                });
            }
    }

    $.each(collectionFields, function (i, field) {
        if (!field.Hidden) {
            var column = {
                text: field.Name,
                fieldId: 'Data[' + i + '].Id',
                field: 'Data[' + i + '].Value',
                sort: { sortable: false },
                disableHider: false,
                tileVisibility: i <= 5,
                visible: i <= 8,
                hidden: field.Hidden,
                tooltip: true,
                columnType: field.Type
            };
            if (field.Type == 4) {
                column.Type = DDoc.Constants.columnType.NUMBER;
                column.currency = true;
            }
            columns.push(column);
        }
    });

    columns.push({ text: 'Fecha de Creación', field: 'CreationDate', sort: { sortable: false }, disableHider: false, tileVisibility: true, visible: true, hidden: false, tooltip: true, columnType: DDoc.Constants.columnType.DATE });

    ddoc.grdFilterResults.init(columns);
    ddoc.grdFilterResults.addListener('onAction', onGridAction);
    ddoc.grdFilterResults.addListener('onGetPage', function (e) { ddoc.GetFilterResults(e.page, e.pageSize); });

    if (typeof ddoc.CustomGridEvents === 'function') {
        ddoc.CustomGridEvents(ddoc.grdFilterResults);
    }
};


DDocUi.prototype.InitTextSearchGrid = function () {

    function onGridAction(e) {
        ddoc.document = e.dataRow;
        switch (e.command) {
            case 'edit':
                ddoc.EditDocument(ddoc.grdTextResults);
                break;
            case 'send':
                ddoc.ShowAlert('¡Funcionalidad en construcción! ¡Vuelva más tarde!');
                break;
        }
    }

    var colType = DDoc.Constants.columnType;

    var columns = [
        { width: 20, field: 'Id', cellType: { type: DDoc.Constants.cellType.THUMBNAILLINK, url: ddoc.GetUrl('/Viewer/OpenViewer/{0}'), target: ddoc.ViewerAnchorTarget, cssClass: { normal: 'imgThumb' }, tooltip: 'Ver documento' }, visible: false },
        { width: 20, field: 'Id', cellType: { type: DDoc.Constants.cellType.ICONLINK, url: ddoc.GetUrl('/Viewer/OpenViewer/{0}'), target: ddoc.ViewerAnchorTarget, cssClass: { normal: 'imgDocumentView' }, tooltip: 'Ver documento' } },
        { width: 20, cellType: { type: DDoc.Constants.cellType.ACTION, cmd: 'edit', cssClass: { normal: 'imgDocumentEdit' }, tooltip: 'Ver propiedades' } }
    ];

    if ('customActions' in window)
        $.each(customActions, function (i, item) {
            columns.push({
                width: 24,
                field: 'Id',
                cellType: {
                    type: item.CellType,
                    url: ddoc.GetUrl('/' + item.Controller + '/' + item.Action + '/{0}'),
                    cmd: item.Command,
                    cssClass: { normal: item.ImageClass },
                    tooltip: item.Tooltip
                }
            });
        });

    columns.push({ width: 150, text: 'Colección', field: 'Name', tooltip: false, columnType: colType.TEXT, sort: { sortable: true, sorted: 1 }, disableHider: true, tileVisibility: true, visible: true });
    columns.push({ width: 400, text: 'Datos', field: 'Data[0].Value', tooltip: false, columnType: colType.TEXT, sort: { sortable: false, sorted: 0 }, disableHider: true, tileVisibility: true, visible: true });
    columns.push({ width: 400, text: 'Sinopsis', field: 'Data[1].Value', tooltip: false, columnType: colType.TEXT, sort: { sortable: false, sorted: 0 }, disableHider: true, tileVisibility: true, visible: true });

    function onGetPage(e) { ddoc.GetTextResults(e.page, e.pageSize); }

    ddoc.grdTextResults.init(columns);
    ddoc.grdTextResults.addListener('onAction', onGridAction);
    ddoc.grdTextResults.addListener('onGetPage', onGetPage);
};

DDocUi.prototype.InitAdvancedSearchScreen = function () {

    ddoc.tabindex = 6
    ddoc.ViewerAnchorTarget = $('#ViewerAnchorTarget').val();
    parent.ddoc.Navigate = true;
    ddoc.NavigationTree = parent.$('#tree').fancytree('getTree');
    ddoc.NavigationTree.clearFilter();
    ddoc.NavigationTree.visit(function (node) { node.unselectable = false });
    ddoc.NavigationTree.visit(function (node) { node.setActive(false); });
    ddoc.NavigationTree.visit(function (node) { node.setSelected(false); });

    (function setupEvents() {
        function onCollectionChange() {
            var self = this;
            $(self).removeClass('field-error');
            $('.save-filters', '#frmFilterSearch').parent().hide();
            var collectionId = $(self).val();
            ddoc.GET('/Collection/GetCollection', { collectionId: collectionId }, function (response) {
                ddoc.Collection = response.ObjectResult;
                parent.ddoc.Navigate = false;
                ddoc.NavigationTree.getNodeByKey(collectionId).setActive();
                ddoc.LoadFields();
                $('.clauseComparison').prop('disabled', true).find('option').remove().end();
                $('.clauseField').prop('disabled', false);
                $('.clauseValue').val('').prop('disabled', true);
                $('.between-separator').hide();
                $('.clauseValue2').val('').prop('disabled', true).css('display', 'none');
                $(self).focus();
            });
        }

        function onFieldChange() {
            $('.save-filters', '#frmFilterSearch').parent().hide();
            var $clause = $(this).parent().parent();
            var $selectedOption = $(this).find('option:selected');

            if ($clause.data('controls')) {
                $.each($clause.data('controls'), function (i, dtPicker) { dtPicker.destroy(); });
                $clause.removeData('controls');
            }
            $('.clauseComparison', $clause).prop('disabled', true).find('option').remove();
            $('.clauseValue', $clause).val('').prop('disabled', true).show();
            $('.between-separator', $clause).hide();
            $('.clauseValue2', $clause).val('').prop('disabled', true).css('display', 'none');

            if ($selectedOption.val() !== '') {
                $('.clauseComparison', $clause).prop('disabled', false);
                var field = $selectedOption.data('fieldItem');
                ddoc.LoadComparisons(field, $clause);
                if (field.Type == 3) {
                    $('.clauseValue', $clause).val('').hide();
                }
            }
        }

        function onComparisonChange() {
            $('.save-filters', '#frmFilterSearch').parent().hide();
            var $clause = $(this).parent().parent();
            var $selectedOption = $(this).find('option:selected');

            var field = $('option:selected', $('.clauseField', $clause)).data('fieldItem');
            var fieldType = field.Type;

            $('.clauseValue', $clause).val('').prop('disabled', true);
            $('.between-separator', $clause).hide();
            $('.clauseValue2', $clause).val('').prop('disabled', true).hide();

            var controls = $clause.data('controls');

            if (controls) {
                if (controls[1]) {
                    $clause.data('controls')[1].destroy();
                    $clause.data('controls').length = 1;
                }
            }

            if ($selectedOption.val() !== '') {
                if (fieldType !== 3) {
                    if (fieldType !== 2) {
                        $('.clauseValue', $clause).val('').prop('disabled', false);
                    } else {
                        if (!$('.clauseValue', $clause).hasClass('datePicker')) {
                            $clause.removeData('controls');
                            $clause.data('controls', []);
                            $clause.data('controls').push(new DDoc.Controls.DatePicker($('.clauseValue', $clause)[0]));
                            $clause.data('controls')[0].init();
                        }
                    }

                    if ($selectedOption.text().indexOf('entre') !== -1) {
                        $('.between-separator', $clause).show();
                        $('.clauseValue2', $clause).val('').prop('disabled', false).show();

                        if (fieldType === 2 && !$('.clauseValue2', $clause).hasClass('datePicker')) {
                            $clause.data('controls').push(new DDoc.Controls.DatePicker($('.clauseValue2', $clause)[0]));
                            $clause.data('controls')[1].init();
                        }
                    }
                }
            }
        }

        function onValueKeyDown(e) {
            $('.save-filters', '#frmFilterSearch').parent().hide();
            if (e.which == 13) {
                e.preventDefault();
                if (validateFilters()) {
                    ddoc.ExecuteFilterSearch();
                }
            }
        }

        function onDatePickerChange() {
            $(this).removeClass('field-error');
            $('.save-filters', '#frmFilterSearch').parent().hide();
            var $clause = $(this).parent().parent();
            var $this = $(this);

            if ($('.clauseValue2', $clause).length) {

                var startDate, startDateString, endDate, endDateString;

                switch (true) {
                    case $this.hasClass('clauseValue'):
                        var value2 = $('.clauseValue2', $clause).val();
                        if (value2 !== '') {
                            startDateString = $this.val().split('/');
                            startDate = new Date(startDateString[2], startDateString[1] - 1, startDateString[0]);
                            endDateString = value2.split('/');
                            endDate = new Date(endDateString[2], endDateString[1] - 1, endDateString[0]);
                            if (startDate >= endDate) {
                                ddoc.ShowAlert('Debe elegir una fecha inicial menor a la fecha final', $this.val(''));
                            }
                        }
                        break;
                    case $this.hasClass('clauseValue2'):
                        var value1 = $('.clauseValue', $clause).val();
                        if (value1 !== '') {
                            endDateString = $this.val().split('/');
                            endDate = new Date(endDateString[2], endDateString[1] - 1, endDateString[0]);
                            startDateString = value1.split('/');
                            startDate = new Date(startDateString[2], startDateString[1] - 1, startDateString[0]);
                            if (startDate >= endDate) {
                                ddoc.ShowAlert('Debe elegir una fecha final mayor a la fecha inicial', $this.val(''));
                            }
                        }
                        break;
                }
            }
        }

        function validateFilters() {
            var validFilters = true;

            var $collection = $('#CollectionId', '#frmFilterSearch');

            if ($collection.val() === '' || $collection.val() == null) {
                $collection.addClass('field-error');
                validFilters = false;
                $('.save-filters', '#frmFilterSearch').parent().hide();
            }

            $('input[type=hidden]', '#frmFilterSearch').remove();

            if (validFilters) {
                $('.group').each(function (i, group) {

                    var $group = $(group);

                    var $clauseGroup1 = $('<input type="hidden" name="ClauseGroups[' + i + '].ClausesOperator">')
                        .val($('.clausesOperator', $group).val());
                    var $clauseGroup2 = $('<input type="hidden" name="ClauseGroups[' + i + '].GroupOperator">')
                        .val($('.groupOperator', $group).val());

                    $('#frmFilterSearch').append($clauseGroup1, $clauseGroup2);

                    $('.clause', $group).each(function (j, clause) {

                        var $clause = $(clause);

                        var hiddenFields = $();

                        var $clauseField = $('.clauseField', $clause);

                        if ($clauseField.val() === '' || $clauseField.val() == null) {
                            $clauseField.addClass('field-error');
                            validFilters = false;
                            $('.save-filters', '#frmFilterSearch').parent().hide();
                            return 'continue';
                        } else {
                            hiddenFields =
                                hiddenFields.add(
                                    $('<input type="hidden" name="ClauseGroups[' + i + '].Clauses[' + j +
                                        '].FieldId">').val($clauseField.val()));
                            hiddenFields =
                                hiddenFields.add(
                                    $('<input type="hidden" name="ClauseGroups[' + i + '].Clauses[' + j +
                                        '].Type">').val($('option:selected', $clauseField).data('fieldItem').Type));
                        }

                        var $clauseComparison = $('.clauseComparison', $clause);
                        if ($clauseComparison.val() === '' || $clauseComparison.val() == null) {
                            $clauseComparison.addClass('field-error');
                            validFilters = false;
                            $('.save-filters', '#frmFilterSearch').parent().hide();
                            return 'continue';
                        } else {
                            hiddenFields =
                                hiddenFields.add(
                                    $('<input type="hidden" name="ClauseGroups[' + i + '].Clauses[' + j +
                                        '].Comparison">').val($clauseComparison.val()));
                        }

                        var fieldType = $('option:selected', $clauseField).data('fieldItem').Type;

                        var $clauseValue = $('.clauseValue', $clause);
                        if ($clauseValue.val() === '' && fieldType !== 3) {
                            $clauseValue.addClass('field-error');
                            validFilters = false;
                            $('.save-filters', '#frmFilterSearch').parent().hide();
                            return 'continue';
                        } else {
                            hiddenFields =
                                hiddenFields.add(
                                    $('<input type="hidden" name="ClauseGroups[' + i + '].Clauses[' + j +
                                        '].Value">').val($clauseValue.val()));
                        }

                        var $clauseValue2 = $('.clauseValue2', $clause);

                        if (!$clauseValue2.is(':hidden')) {
                            if ($clauseValue2.val() === '') {
                                $clauseValue2.addClass('field-error');
                                validFilters = false;
                                $('.save-filters', '#frmFilterSearch').parent().hide();
                                return 'continue';
                            } else {
                                hiddenFields =
                                    hiddenFields.add(
                                        $('<input type="hidden" name="ClauseGroups[' + i + '].Clauses[' + j +
                                            '].Value2">').val($clauseValue2.val()));
                            }
                        }

                        $('#frmFilterSearch').append(hiddenFields);
                    });
                });

                if (!validFilters) {
                    return false;
                } else {
                    ddoc.SearchParameters = {};

                    ddoc.SearchParameters = (function getFormData() {
                        var data = {};
                        $.each($('#frmFilterSearch').serializeArray(), function (i, field) {
                            data[field.name] = field.value;
                        });
                        return data;
                    })();
                    return true;
                }
            }
        }

        function onTextCollectionChange() {
            var collectionId = $(this).val();
            ddoc.GET('/Collection/GetCollection', { collectionId: collectionId }, function (response) {
                ddoc.Collection = response.ObjectResult;
                parent.ddoc.Navigate = false;
                ddoc.NavigationTree.getNodeByKey(collectionId).setActive();
                ddoc.GET('/Collection/GetFields', { collectionId: ddoc.Collection.Id }, function (response) {
                    response.List.push({ Name: 'Fecha de Creación', Id: 99999, Type: 2 });
                    $('#SortResultsBy', '#frmTextSearch').children().remove();
                    $('#SortDirection', '#frmTextSearch').prop('disabled', false);
                    $('#SortResultsBy', '#frmTextSearch').prop('disabled', false);
                    $.each(response.List, function (i, field) {
                        var $option = $('<option>').text(field.Name).val(field.Id);
                        $option.data('fieldItem', field);
                        if (field.Id != 99999) {
                            $('#SortResultsBy', '#frmTextSearch')
                                .append($('<option>').text(field.Name).val('C' + field.Id));
                        } else {
                            $('#SortResultsBy', '#frmTextSearch').append($('<option>').text(field.Name).val(null));
                        }
                    });
                    ddoc.Collection.Fields = response.List.slice(0, response.List.length - 1);
                });
            });
        }

        function validateSearchType() {
            var searchType = $('#SearchType').val();
            if (searchType == '' || searchType == null) {
                ddoc.ShowAlert('Debe definir el tipo de búsqueda');
                return false;
            }

            ddoc.TextQuery = $('#TextQuery').val();
            ddoc.CollectionId = $('#CollectionId', '#frmTextSearch').val();
            ddoc.SearchType = $('#SearchType').val();
            ddoc.SortResultsBy = $('#SortResultsBy', '#frmTextSearch').val();
            ddoc.SortDirection = $('#SortDirection', '#frmTextSearch').val();

            return true;
        }

        $('.load-filters', '#frmFilterSearch').on('click', function () {
            $('#filterDefinitionFile').click();
        });

        $('#filterDefinitionFile').on('change', function (e) {
            var file = e.target.files[0];
            var formData = new FormData();
            formData.append('searchFilters', file);
            $.ajax({
                url: ddoc.GetUrl('/Search/LoadSearchFilters'),
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (response) {
                    ddoc.LoadFilters(response.ObjectResult);
                }
            });
        });

        $('#CollectionId', '#frmFilterSearch').on('change', onCollectionChange);
        $('.group-container').on('focus', 'input[type="text"]', function () { $(this).removeClass('field-error'); });
        $('.group-container').on('focus', 'select', function () { $(this).removeClass('field-error'); });
        $('.group-container').on('change', '.clauseField', onFieldChange);
        $('.group-container').on('change', '.clauseComparison', onComparisonChange);
        $('.group-container').on('keydown', '.clauseValue', onValueKeyDown);
        $('.group-container').on('keydown', '.clauseValue2', onValueKeyDown);
        $('.group-container').on('change', '.clausesOperator', function () { $('.save-filters', '#frmFilterSearch').parent().hide(); });
        $('.group-container').on('change', '.groupOperator', function () { $('.save-filters', '#frmFilterSearch').parent().hide(); });
        $('.group-container').on('change', '.datePicker', onDatePickerChange);
        $('#CollectionId', '#frmTextSearch').on('change', onTextCollectionChange);


        $('#frmFilterSearch').submit(function (e) {
            e.preventDefault();
            if (validateFilters()) {
                ddoc.ExecuteFilterSearch();
            }
        });
        $('#frmTextSearch').submit(function (e) {
            e.preventDefault();
            if (validateSearchType()) {
                ddoc.ExecuteTextSearch();
            }
        });

        $('.group-container').on('click', '.add-group', ddoc.AddClauseGroup);
        $('.group-container').on('click', '.remove-group', ddoc.RemoveClauseGroup);
        $('.group-container').on('click', '.add-clause', ddoc.AddClause);
        $('.group-container').on('click', '.remove-clause', ddoc.RemoveClause);
        $('.group-container').on('click', '.save-filters', function (e) { ddoc.SaveFilters(e) });

        $('#exportResults').on('click', ddoc.PrintSearchResults);

        $('#exportMassiveDocument').on('click', ddoc.MassiveDocuments);

        $('#filterSearch').prev().on('click', function () {
            if ($('#textSearch').is(':visible'))
                $('#textSearch').hide('blind', function () {
                    $('#filterSearch').show('blind', function () { });
                });
        });
        $('#textSearch').prev().on('click', function () {
            if ($('#filterSearch').is(':visible'))
                $('#filterSearch').hide('blind', function () {
                    $('#textSearch').show('blind', function () { });
                });
        });

        $('.style-changer').click(function () {
            ddoc.SetActiveStyleSheet($(this).attr('id'));
            $(this).siblings().removeClass('toggle-on');
            $(this).addClass('toggle-on');
            ddoc.grdFilterResults.autoHeight();
        });
    })();

    (function setupControls() {
        if ($('#CollectionId', '#frmFilterSearch').val() != "") {
            var collectionId = $('#CollectionId', '#frmFilterSearch').val();
            ddoc.GET('/Collection/GetCollection', { collectionId: collectionId }, function (response) {
                ddoc.Collection = response.ObjectResult;
                parent.ddoc.Navigate = false;
                ddoc.NavigationTree.getNodeByKey(collectionId).setActive();
                ddoc.LoadFields();
                $('.clauseField').prop('disabled', false);
            });
        }
    })();
};

DDocUi.prototype.LoadFields = function ($clause) {

    var $clauseField = $clause ? $('.clauseField', $clause) : $('.clauseField');

    var $defaultOption = $('<option>').text('Seleccione un campo...').val('');
    $clauseField.find('option').remove().end().append($defaultOption);

    $clauseField.addClass('loading');
    ddoc.GET('/Collection/GetFields', { collectionId: ddoc.Collection.Id }, function (response) {
        response.List.push({ Name: 'Fecha de Creación', Id: 99999, Type: 2 });
        $('#SortResultsBy', '#frmFilterSearch').children().remove();
        $('#SortDirection', '#frmFilterSearch').prop('disabled', false);
        $('#SortResultsBy', '#frmFilterSearch').prop('disabled', false);
        $.each(response.List, function (i, field) {
            if (!field.Hidden) {
                var $option = $('<option>').text(field.Name).val(field.Id);
                $option.data('fieldItem', field);
                $clauseField.append($option);
                if (field.Id != 99999) {
                    $('#SortResultsBy', '#frmFilterSearch').append($('<option>').text(field.Name).val('C' + field.Id));
                } else {
                    $('#SortResultsBy', '#frmFilterSearch').append($('<option>').text(field.Name).val(null));
                }
            }
        });
        ddoc.Collection.Fields = response.List.slice(0, response.List.length - 1);
        $clauseField.removeClass('loading');
    });
};

DDocUi.prototype.LoadComparisons = function (field, $clause) {
    var $clauseOperation = $('.clauseComparison', $clause);

    var $defaultOption = $('<option>').text('Seleccione una operación...').val('');
    $clauseOperation.find('option').remove().end().append($defaultOption);

    $clauseOperation.addClass('loading');

    var comparisons = [];

    switch (field.Type) {
        case DDoc.Constants.columnType.TEXT:
            comparisons = textComparisons;
            break;
        case DDoc.Constants.columnType.NUMBER:
        case DDoc.Constants.columnType.MONEY:
            comparisons = numericComparisons;
            break;
        case DDoc.Constants.columnType.DATE:
            comparisons = dateComparisons;
            break;
        case DDoc.Constants.columnType.BOOLEAN:
            comparisons = booleanComparisons;
            break;
    }

    $.each(comparisons, function (i, item) {
        var $option = $('<option>').text(item.Value).val(item.Key);
        $clauseOperation.append($option);
    });
    if (field.AllowedValues) {
        $clauseOperation.val(1);
        $('.clauseValue', $clause).prop('disabled', false);
        var $allowedValues = $('<datalist id="allowedValues">');
        $.each(field.AllowedValues, function (i, item) {
            $allowedValues.append($('<option>').val(item));
        });
        $('.clauseValue', $clause).after($allowedValues);
        $('.clauseValue', $clause).attr('list', 'allowedValues');
    } else {
        if ($('datalist', $clause).length > 0)
            $('datalist', $clause).remove();
    }
    $clauseOperation.removeClass('loading');
};

DDocUi.prototype.AddClauseGroup = function () {
    var $newGroup = $('<div>').addClass('group').attr('data-groupItem', {});
    $.ajax({
        url: ddoc.GetUrl('/Search/SearchClauseGroup?tabindex=' + ddoc.tabindex),
        data: undefined,
        dataType: 'html',
        success: function (response) {
            $newGroup.html(response);
            $('.group-controls').before($newGroup);
            var collectionId = $('#CollectionId', '#frmFilterSearch').val();
            if (collectionId !== '') {
                ddoc.LoadFields($newGroup);
                $('.clauseField', $newGroup).prop('disabled', false);
            }
            ddoc.tabindex = ddoc.tabindex + 4;
        }
    });
};

DDocUi.prototype.RemoveClauseGroup = function () {
    $(this).parent().parent().remove();
};

DDocUi.prototype.AddClause = function () {
    var $clause = $(this).parent().parent();
    var $newClause = $('<div>').addClass('clause').attr('data-clauseItem', {});
    $.ajax({
        url: ddoc.GetUrl('/Search/NewSearchClause?tabindex=' + ddoc.tabindex),
        data: undefined,
        dataType: 'html',
        success: function (response) {
            $newClause.html(response);
            $clause.after($newClause);
            var collectionId = $('#CollectionId', '#frmFilterSearch').val();
            if (collectionId !== '') {
                ddoc.LoadFields($newClause);
                $('.clauseField', $newClause).prop('disabled', false);
            }
            ddoc.tabindex = ddoc.tabindex + 4;
        }
    });
};

DDocUi.prototype.RemoveClause = function () {
    $(this).parent().parent().remove();
};

DDocUi.prototype.SaveFilters = function () {
    var searchParameters = {
        ClauseGroups: [],
        CollectionId: $('#CollectionId', '#frmFilterSearch').val(),
        SortResultsBy: $('#SortResultsBy', '#frmFilterSearch').val(),
        SortDirection: $('#SortDirection', '#frmFilterSearch').val()
    };

    $('.group').each(function (i, groupElement) {
        var $group = $(groupElement);
        var group = {
            Clauses: [],
            ClausesOperator: $('.clausesOperator', $group).val(),
            GroupOperator: $('.groupOperator', $group).val()
        };
        $('.clause', $group).each(function (j, clauseElement) {
            var $clause = $(clauseElement);
            var $clauseField = $('.clauseField', $clause);
            var $clauseComparison = $('.clauseComparison', $clause);
            var $clauseValue = $('.clauseValue', $clause);
            var $clauseValue2 = $('.clauseValue2', $clause);
            var clause = {
                Comparison: $clauseComparison.val(),
                FieldId: $clauseField.val(),
                Type: $('option:selected', $clauseField).data('fieldItem').Type,
                Value: $clauseValue.val(),
                Value2: $clauseValue2.val()
            };
            group.Clauses.push(clause);
        });
        searchParameters.ClauseGroups.push(group);
    });

    ddoc.POST('/Search/SaveSearchFilters', { searchParameters: searchParameters }, function (response) {
        window.location.href = ddoc.GetUrl('/api/files/downloadSearchFilters/' + response.TextResult + '?user=' + ddoc.GetUsername());
    });
};

DDocUi.prototype.LoadFilters = function (searchParameters) {
    $('.save-filters', '#frmFilterSearch').parent().hide();
    $('#CollectionId', '#frmFilterSearch').removeClass('field-error');
    $('#CollectionId', '#frmFilterSearch').val(searchParameters.CollectionId);
    var collectionFields = [];

    function addFields($clauseField) {
        $clauseField.children().remove();
        $.each(collectionFields, function (i, field) {
            var $option = $('<option>').text(field.Name).val(field.Id);
            $option.data('fieldItem', field);
            $clauseField.append($option);
            if (field.Id != 99999) {
                $('#SortResultsBy', '#frmFilterSearch').append($('<option>').text(field.Name).val('C' + field.Id));
            } else {
                $('#SortResultsBy', '#frmFilterSearch').append($('<option>').text(field.Name).val(null));
            }
        });
    }

    ddoc.GET('/Collection/GetCollection', { collectionId: searchParameters.CollectionId }, function (response) {
        ddoc.Collection = response.ObjectResult;
        ddoc.GET('/Collection/GetFields', { collectionId: searchParameters.CollectionId },
            function (response) {
                response.List.push({ Name: 'Fecha de Creación', Id: 99999, Type: 2 });
                collectionFields = response.List;
                $.each(searchParameters.ClauseGroups, function (i, group) {
                    var $group = $('.group', '.group-container').eq(i);
                    if ($group.length == 0) {
                        $group = $('<div class="group">');
                        $group.html(ddoc.SearchClauseGroup);
                        $('.group-controls').before($group);
                        addFields($('.clauseField', $group));
                    }

                    $('.groupOperator', $group).val(group.GroupOperator).trigger('change');
                    $('.clausesOperator', $group).val(group.ClausesOperator).trigger('change');
                    $.each(group.Clauses, function (j, clause) {
                        var $clause = $('.clause', $('.group-clauses', $group)).eq(j);
                        if ($clause.length == 0) {
                            $clause = $('<div class="clause">').html(ddoc.NewSearchClause());
                            $('.group-clauses', $group).append($clause);
                        }

                        addFields($('.clauseField', $clause));

                        $('.clauseField', $clause).prop('disabled', false);
                        $('.clauseField', $clause).val(clause.FieldId);

                        var $selectedField = $('.clauseField', $clause).find('option:selected');

                        if ($clause.data('controls')) {
                            $.each($clause.data('controls'), function (i, dtPicker) { dtPicker.destroy(); });
                            $clause.removeData('controls');
                        }
                        $('.clauseComparison', $clause).prop('disabled', true).find('option').remove();
                        $('.clauseValue', $clause).val('').prop('disabled', true).show();
                        $('.between-separator', $clause).hide();
                        $('.clauseValue2', $clause).val('').prop('disabled', true).css('display', 'none');

                        if ($selectedField.val() !== '') {
                            $('.clauseComparison', $clause).prop('disabled', false);
                            var field = $selectedField.data('fieldItem');
                            ddoc.LoadComparisons(field, $clause);
                            if (field.Type == 3) {
                                $('.clauseValue', $clause).val('').hide();
                            }
                        }

                        $('.clauseComparison', $clause).val(clause.Comparison);

                        var $selectedComparison = $('.clauseComparison', $clause).find('option:selected');

                        var field = $('option:selected', $('.clauseField', $clause)).data('fieldItem');
                        var fieldType = field.Type;

                        $('.clauseValue', $clause).val('').prop('disabled', true);
                        $('.between-separator', $clause).hide();
                        $('.clauseValue2', $clause).val('').prop('disabled', true).hide();

                        var controls = $clause.data('controls');

                        if (controls) {
                            if (controls[1]) {
                                $clause.data('controls')[1].destroy();
                                $clause.data('controls').length = 1;
                            }
                        }

                        if ($selectedComparison.val() !== '') {
                            if (fieldType !== 3) {
                                if (fieldType !== 2) {
                                    $('.clauseValue', $clause).val('').prop('disabled', false);
                                } else {
                                    if (!$('.clauseValue', $clause).hasClass('datePicker')) {
                                        $clause.removeData('controls');
                                        $clause.data('controls', []);
                                        $clause.data('controls').push(new DDoc.Controls.DatePicker($('.clauseValue', $clause)[0]));
                                        $clause.data('controls')[0].init();
                                    }
                                }

                                if ($selectedComparison.text().indexOf('entre') !== -1) {
                                    $('.between-separator', $clause).show();
                                    $('.clauseValue2', $clause).val('').show();

                                    if (fieldType === 2 && !$('.clauseValue2', $clause).hasClass('datePicker')) {
                                        $clause.data('controls').push(new DDoc.Controls.DatePicker($('.clauseValue2', $clause)[0]));
                                        $clause.data('controls')[1].init();
                                    }
                                }
                            }
                        }
                        $('.clauseValue', $clause).val(clause.Value).trigger('keydown');
                        $('.clauseValue2', $clause).val(clause.Value2).trigger('keydown');

                        $('#SortDirection', '#frmFilterSearch').prop('disabled', false);
                        $('#SortResultsBy', '#frmFilterSearch').prop('disabled', false);

                        $.each(collectionFields, function (i, field) {
                            var $option = $('<option>').text(field.Name).val(field.Id);
                            $option.data('fieldItem', field);
                            if (field.Id != 99999) {
                                $('#SortResultsBy', '#frmTextSearch')
                                    .append($('<option>').text(field.Name).val('C' + field.Id));
                            } else {
                                $('#SortResultsBy', '#frmTextSearch').append($('<option>').text(field.Name).val(null));
                            }
                        });

                        $('#SortResultsBy', '#frmFilterSearch').val(searchParameters.SortResultsBy);
                        $('#SortDirection', '#frmFilterSearch').val(searchParameters.SortDirection);

                        ddoc.Collection.Fields = response.List.slice(0, response.List.length - 1);
                    });
                });
            });
    });
};

DDocUi.prototype.ExecuteFilterSearch = function () {
    $('.grid-container', '.filter-search-results').remove();
    $('.filter-search-results').show();
    $('.menu-title-content', '.filter-search-results').text(ddoc.Collection.Name);
    $('.filter-search-results').append($('<div class="grid-container filterResults"><div id="grdFilterResults"><div class="grid-title-label"></div></div></div>'));
    ddoc.grdFilterResults = new DDoc.Controls.Grid('grdFilterResults', { height: 362, showFilter: false, showTools: true, pager: { show: true, pageSize: 10, onServer: true }, thumbnailSrc: ddoc.GetUrl('/api/files/getThumbnail/{0}/115/150') });
    ddoc.grdFilterResults.CollectionId = ddoc.Collection.Id;
    ddoc.InitFilterSearchGrid(ddoc.Collection.Type, ddoc.Collection.Fields);

    ddoc.GetFilterResults(ddoc.grdFilterResults.currentPage, ddoc.grdFilterResults.pageSize);
};

DDocUi.prototype.GetFilterResults = function (page, pageSize) {
    ddoc.grdFilterResults.showLoader();
    switch (ddoc.Collection.Type) {
        case DDocUi.Enums.CollectionType.Folder:
            ddoc.POST('/Search/SearchFolders', { searchParams: ddoc.SearchParameters, page: page, pageSize: pageSize }, function (response) {
                if (response.Total === 0) {
                    $('.filter-search-results').hide();
                    ddoc.ShowAlert('No hay resultados para esta búsqueda');
                } else {
                    ddoc.grdFilterResults.reload(response.List);
                    ddoc.grdFilterResults.setPager(page, Math.ceil(response.Total / pageSize));
                    $('.style-changer').hide();
                    $('span', '.grid-tools').before('Resultados de búsqueda: ' + response.Total + ' elemento(s) encontrado(s).');
                }
                $('.save-filters', '#frmFilterSearch').parent().show();
            });
            break;
        case DDocUi.Enums.CollectionType.Document:
            ddoc.POST('/Search/SearchDocuments', { searchParams: ddoc.SearchParameters, page: page, pageSize: pageSize }, function (response) {
                if (response.Total === 0) {
                    $('.filter-search-results').hide();
                    ddoc.ShowAlert('No hay resultados para esta búsqueda');
                } else {
                    ddoc.grdFilterResults.reload(response.List);
                    ddoc.grdFilterResults.setPager(page, Math.ceil(response.Total / pageSize));
                    ddoc.grdFilterResults.loadThumbnails('Pages[0].Id');
                    $('.style-changer').show();
                    $('span', '.grid-tools').before('Resultados de búsqueda: ' + response.Total + ' elemento(s) encontrado(s).');
                }
                $('.save-filters', '#frmFilterSearch').parent().show();
            });
            break;
    }
};

DDocUi.prototype.ExecuteTextSearch = function () {
    $('.grid-container', '.text-search-results').remove();
    $('.text-search-results').append($('<div class="grid-container textResults"><div id="grdTextResults"></div></div>'));
    ddoc.grdTextResults = new DDoc.Controls.Grid('grdTextResults', { showTools: false, pager: { show: true, sizes: [500], pageSize: 500 } });
    ddoc.InitTextSearchGrid();

    ddoc.GetTextResults();
};

DDocUi.prototype.GetTextResults = function () {
    ddoc.grdTextResults.showLoader();

    ddoc.GET('/Search/SearchText', { CollectionId: ddoc.CollectionId, TextQuery: ddoc.TextQuery, SearchType: ddoc.SearchType, SortResultsBy: ddoc.SortResultsBy, SortDirection: ddoc.SortDirection }, function (response) {
        ddoc.grdTextResults.reload(response.List);
        ddoc.grdTextResults.setPager(ddoc.grdTextResults.currentPage, Math.ceil(response.Total / ddoc.grdTextResults.pageSize));
    });
};

DDocUi.prototype.PrintSearchResults = function () {
    ddoc.grdFilterResults.showLoader();
    switch (ddoc.Collection.Type) {
        case DDocUi.Enums.CollectionType.Folder:
            ddoc.POST('/Search/PrintFolderSearchResults', { searchParams: ddoc.SearchParameters }, function (response) {
                ddoc.grdFilterResults.hideLoader();
                window.location.href = ddoc.GetUrl('/api/files/downloadSearchResults/' + response.TextResult + '?user=' + ddoc.GetUsername());
            });
            break;
        case DDocUi.Enums.CollectionType.Document:
            ddoc.POST('/Search/PrintDocumentSearchResults', { searchParams: ddoc.SearchParameters }, function (response) {
                ddoc.grdFilterResults.hideLoader();
                window.location.href = ddoc.GetUrl('/api/files/downloadSearchResults/' + response.TextResult + '?user=' + ddoc.GetUsername());
            });
            break;
    }
}

DDocUi.prototype.MassiveDocuments = function () {
    ddoc.grdFilterResults.showLoader();

    ddoc.POST('/Search/DocumentMassDownload', { searchParams: ddoc.SearchParameters }, function (response) {
        ddoc.grdFilterResults.hideLoader();
        window.location.href = ddoc.GetUrl('/api/files/downloadMassDocuments/' + response.TextResult + '?user=' + ddoc.GetUsername());
    });
}

DDocUi.prototype.NewSearchClause = function (tabindex) {
    return '<div class="actions-column">' +
        '<span class="image-button add-clause"></span></div>' +
        '<div class="actions-column"><span class="image-button remove-clause"></span></div>' +
        '<div class="field-column"><select class="clauseField" disabled="disabled"></select></div>' +
        '<div class="comparison-column"><select class="clauseComparison" disabled="disabled"></select></div>' +
        '<div class="value-column"><input class="clauseValue" disabled="disabled" placeholder="Valor" type="text" /></div>' +
        '<div class="value-between-column"><span class="between-separator" style="display: none"> y </span></div>' +
        '<div class="value-column"><input class="clauseValue2" disabled="disabled" placeholder="Valor" style="display: none" type="text" />' +
        '</div>'
}

DDocUi.prototype.SearchClauseGroup = '' +
    '<div class="group-header">' +
    '<span class="image-button remove-group"></span>' +
    '<select class="groupOperator">' +
    '<option value="0">Y los elementos que cumplan con</option>' +
    '<option value="1">O los elementos que cumplan con</option>' +
    '</select>' +
    '<select class="clausesOperator"> ' +
    '<option value = "0" > Todas las siguientes condiciones</option> ' +
    '<option value = "1" > Cualquiera de las siguientes condiciones</option> ' +
    '</select> ' +
    '</div> ' +
    '<div class="group-clauses"> ' +
    '<div class="clause"> ' +
    '<div class="actions-column"><span class="image-button add-clause"></span></div> ' +
    '<div class="actions-column"></div> ' +
    '<div class="field-column"><select class="clauseField" disabled="disabled"></select></div> ' +
    '<div class="comparison-column"> <select class="clauseComparison" disabled="disabled"></select></div> ' +
    '<div class="value-column"><input class="clauseValue" disabled="disabled" type="text" placeholder="Valor"/></div> ' +
    '<div class="value-between-column"> <span class="between-separator" style="display: none"> y </span></div> ' +
    '<div class="value-column"><input class="clauseValue2" disabled="disabled" style="display: none" type="text" placeholder="Valor"/></div> ' +
    '</div> ' +
    '</div>';

var ddoc = new DDocUi();
ddoc.InitAdvancedSearchScreen();
$('#CollecionId').focus();