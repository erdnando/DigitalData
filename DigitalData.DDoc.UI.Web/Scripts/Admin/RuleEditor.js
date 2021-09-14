
DDocUi.prototype.NewRule = function () {
    ddoc.rule = { IsNew: true };
    ddoc.CreateModal('/Admin/NewRule', undefined, 'ruleEditor', 'Nueva relación de colecciones', { width: 600 }, ddoc.InitRuleEditor);
};

DDocUi.prototype.EditRule = function() {
    ddoc.CreateModal('/Admin/EditRule', ddoc.rule, 'ruleEditor', 'Editar relación de colecciones', { width: 600 }, ddoc.InitRuleEditor);
};

DDocUi.prototype.InitRuleEditor = function () {

    ddoc.ParentCollections = [];
    ddoc.ChildCollections = [];

    (function setupControls() {
        ddoc.GET('/Collection/GetCollections', undefined, function (response) {
            $.each(response.List, function (i, collection) {
                ddoc.ParentCollections.push(collection);
                var $parentOption = $('<option>').text(collection.Name).val(collection.Id).data('collectionItem', collection);
                $('#ParentId').append($parentOption);
                if (collection.Type != 1) {
                    ddoc.ChildCollections.push(collection);
                    var $childOption = $('<option>').text(collection.Name).val(collection.Id).data('collectionItem', collection);
                    $('#ChildId').append($childOption);
                }
            });

            if (window.hasOwnProperty("ddocRule")) {
                $('#ParentId').val(ddocRule.ParentId);
                ddoc.onCollectionChange($('#ParentId')[0]);
                $('#ChildId').val(ddocRule.ChildId);
                ddoc.onCollectionChange($('#ChildId')[0]);
            }
        });
    })();

    (function setupEvents() {
        function onDialogButtonClick(e) {
            e.preventDefault();
            switch(this.id) {
                case 'saveRule':
                    ddoc.SaveCollectionRule();
                    break;
                case 'cancelRuleEdit':
                    $('#ruleEditor').dialog('destroy');
                    break;
            }
        }

        $('.dialog-button-panel').on('click', 'button', onDialogButtonClick);
        $('#ParentId, #ChildId').on('change', e => ddoc.onCollectionChange(e.target));
    })();
};

DDocUi.prototype.populateParentCollections = function (element, exceptId) {
    $('option', $(element)).remove();
    $.each(ddoc.ParentCollections, function (i, collection) {
        if (collection.Id != exceptId) {
            var $option = $('<option>').text(collection.Name).val(collection.Id).data('collectionItem', collection);
            $(element).append($option);
        }
    });
}

DDocUi.prototype.populateChildCollections = function (element, exceptId) {
    $('option', $(element)).remove();
    $.each(ddoc.ChildCollections, function (i, collection) {
        if (collection.Id != exceptId) {
            var $option = $('<option>').text(collection.Name).val(collection.Id).data('collectionItem', collection);
            $(element).append($option);
        }
    });
}

DDocUi.prototype.onCollectionChange = function (target) {
    var fieldSelect;

    switch (target.id) {
    case 'ParentId':
        $('option', '#ParentField').remove();
        fieldSelect = 'ParentField';
        if ($('option:selected', $(target)).data('collectionItem').Type == 1) {
            $('#ParentField').prop('disabled', true);
            $('option', '#ChildField').remove();
            $('#ChildField').prop('disabled', true);
        } else {
            $('#ParentField').prop('disabled', false);
            $('#ChildField').prop('disabled', false);
        }

        var prevChildValue = $('#ChildId').val();
        this.populateChildCollections($('#ChildId')[0], $('option:selected', $(target)).val());
        $('#ChildId').val(prevChildValue);
        break;
    case 'ChildId':
        $('option', '#ChildField').remove();
        fieldSelect = 'ChildField';

        var prevParentValue = $('#ParentId').val();
        this.populateParentCollections($('#ParentId')[0], $('option:selected', $(target)).val());
        $('#ParentId').val(prevParentValue);
        break;
    }

    if (!$('#' + fieldSelect).is(':disabled')) {
        ddoc.GET('/Collection/GetFields', { collectionId: $('option:selected', $(target)).val() }, function (response) {
            $.each(response.List, function (i, field) {
                var $option = $('<option>').text(field.Name).val(field.Id);
                $('#' + fieldSelect).append($option);
            });
        });
    }

}

DDocUi.prototype.SaveCollectionRule = function() {

    var rule = (function getFormData() {
        var data = {};

        $.each($('#frmRuleEdit').serializeArray(), function (i, field) {
            data[field.name] = field.value;
        });

        if (data.IsNew === 'True') {
            data.IsNew = true;

        } else {
            data.IsNew = false;
        }

        return data;
    })();

    ddoc.POST('/Admin/SaveCollectionRule', rule, function() {
        ddoc.ShowAlert('Relación de colecciones guardada correctamente.', function () {
            $('#ruleEditor').dialog('destroy');
            ddoc.LoadCollectionRules();
            ddoc.LoadCollections();
        });
    });
};