DDocUi.prototype.ChangePassword = function(bypassCurrentPassword) {
    ddoc.CreateModal('/Admin/ChangeUserPassword' + (bypassCurrentPassword ? '?bypassCurrentPassword=' + bypassCurrentPassword : ''), ddoc.user, 'passwordEditor', 'Cambiar Contraseña', { width: 700 }, ddoc.InitPasswordEditor);
};

DDocUi.prototype.InitPasswordEditor = function() {

    (function setupControls() {
        $('.password-icon').hide();
        $('.password-confirmation-icon').hide();
    })();

    (function setupEvents() {
        function onDialogButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'savePassword':
                    ddoc.UpdatePassword();
                    break;
                case 'cancel':
                    $('#passwordEditor').dialog('destroy');
                    break;
            }
        }

        $('#Password').keyup(ddoc.ValidatePasswordStrength);
        $('#PasswordConfirmation').keyup(ddoc.ValidatePasswordConfirmation);

        $('#frmPasswordChange').on('click', 'button', onDialogButtonClick);
    })();
};

DDocUi.prototype.ValidatePasswordStrength = function() {
    var strength = 1;

    $('.password-icon').hide();

    ddoc.ColorizeControl($(this), strength);

    var password = $(this).val();

    if (password.length > 0) {
        if (password.length >= parseInt($('#MinPassLength').val()))
            strength++;
        if (/\d/.test(password))
            strength++;
        if (/[A-Z]/.test(password))
            strength++;
        if (/.[!,@,#,$,%,&,/,?,*,+,-,_,(,)]/.test(password))
            strength++;

        ddoc.ColorizeControl($(this), strength);

        if (strength > 4) {
            $('.password-icon').show();
        }
    } else
        ddoc.ColorizeControl($(this), 0);

    $('#PasswordConfirmation').val('').css('background-color', 'rgb(255, 255, 255)');
    $('.password-confirmation-icon').hide();
};

DDocUi.prototype.ValidatePasswordConfirmation = function() {

    $('.password-confirmation-icon').hide();

    var confirmation = 2;

    var password = $(this).val();

    if (password.length > 0) {
        if (password.length > 1) {
            if ($(this).val() == $('#Password').val()) {
                confirmation = 5;
                $('.password-confirmation-icon').show();
            }
        }
        ddoc.ColorizeControl($(this), confirmation);
    } else {
        ddoc.ColorizeControl($(this), 0);
    }
};

DDocUi.prototype.ColorizeControl = function($element, value) {
    switch (value) {
        case 0:
            $element.css('background-color', 'rgb(255, 255, 255)');
            break;
        case 1:
            $element.css('background-color', 'rgba(255, 0, 0, 0.7)');
            break;
        case 2:
            $element.css('background-color', 'rgba(255, 127, 0, 0.7)');
            break;
        case 3:
            $element.css('background-color', 'rgba(255, 255, 0, 0.7)');
            break;
        case 4:
            $element.css('background-color', 'rgba(127, 255, 0, 0.7)');
            break;
        case 5:
            $element.css('background-color', 'rgba(0, 255, 0, 0.7)');
            break;
    }
};

DDocUi.prototype.UpdatePassword = function() {

    var user = (function getFormData() {
        var data = { Username: ddoc.user.Username };
        $.each($('#frmPasswordChange').serializeArray(), function(i, field) {
            data[field.name] = field.value;
        });
        return data;
    })();

    ddoc.POST('/Admin/UpdateUserPassword', user, function() {
        ddoc.ShowAlert('Contraseña modificada con éxito!', function() {
            $('#passwordEditor').dialog('destroy');
        });
    });
};