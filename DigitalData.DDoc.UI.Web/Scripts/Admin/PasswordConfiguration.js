
DDocUi.prototype.EditPasswordConfiguration = function() {
    ddoc.CreateModal('/Admin/EditPasswordConfiguration', undefined, 'passwordConfigurationEditor', 'Configuración de contraseña', { width: 500 }, ddoc.InitConfigurationEditor);
};

DDocUi.prototype.InitConfigurationEditor = function () {

    (function setupControls() {
        if ($('#EnablePasswordHistory').is(':checked')) {
            $('#PastPasswordCheck').prop('disabled', false);
        }
        else {
            $('#PastPasswordCheck').prop('disabled', true);
        }
        if ($('#EnablePasswordExpiration').is(':checked')) {
            $('#InactivityDays').prop('disabled', false);
            $('#ExpirationDays').prop('disabled', false);
            $('#DaysForWarning').prop('disabled', false);
        }
        else {
            $('#InactivityDays').prop('disabled', true);
            $('#ExpirationDays').prop('disabled', true);
            $('#DaysForWarning').prop('disabled', true);
        }
    })();

    (function setupEvents() {
        function onFormButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'cancel':
                    $('#passwordConfigurationEditor').dialog('destroy');
                    break;
                case 'saveConfiguration':
                    ddoc.SaveConfiguration();
                    break;
            }
        }

        function onCheckboxToggle() {
            switch (this.id) {
                case 'EnablePasswordHistory':
                    if ($(this).is(':checked')) {
                        $('#PastPasswordCheck').prop('disabled', false);
                    }
                    else {
                        $('#PastPasswordCheck').prop('disabled', true);
                    }
                    break;
                case 'EnablePasswordExpiration':
                    if ($(this).is(':checked')) {
                        $('#InactivityDays').prop('disabled', false);
                        $('#ExpirationDays').prop('disabled', false);
                        $('#DaysForWarning').prop('disabled', false);
                    }
                    else {
                        $('#InactivityDays').prop('disabled', true);
                        $('#ExpirationDays').prop('disabled', true);
                        $('#DaysForWarning').prop('disabled', true);
                    }
                    break;
            }
        }

        $('#frmConfiguration').on('click', 'button', onFormButtonClick);
        $('input[type="checkbox"]').on('change', onCheckboxToggle);
    })();

};

DDocUi.prototype.SaveConfiguration = function() {

    var configuration = (function getFormData() {
        var data = {};
        $.each($('#frmConfiguration').serializeArray(), function(i, field) {
            data[field.name] = field.value;
        });
        $('input[type="checkbox"]', '#frmConfiguration').each(function(i, checkbox) {
            if ($(checkbox).is(':checked'))
                data[checkbox.id] = true;
            else {
                data[checkbox.id] = false;
            }
        });
        return data;
    })();

    ddoc.POST('/Admin/SaveConfiguration', configuration, function() {
        ddoc.ShowAlert('Configuración guardada con éxito!', function() {
            $('#passwordConfigurationEditor').dialog('destroy');
        });
    });
};