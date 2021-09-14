DDocUi.prototype.NewUser = function() {
    ddoc.user = { IsNew: true };
    ddoc.CreateModal('/Admin/NewUser', undefined, 'userEditor', 'Nuevo Usuario', { width: 700 }, ddoc.InitUserEditor);
};

DDocUi.prototype.EditUser = function() {
    ddoc.CreateModal('/Admin/EditUser', ddoc.user, 'userEditor', 'Editar usuario', { width: 700 }, ddoc.InitUserEditor);
};

DDocUi.prototype.InitUserEditor = function() {

    (function setupControls() {
        $('.password-icon').hide();
        $('.password-confirmation-icon').hide();
    })();

    (function setupEvents() {
        function onMenuBarOptionClick() {
            switch (this.id) {
                case 'editProfile':
                    ddoc.EditUserProfile();
                    break;
                case 'changePassword':
                    ddoc.ChangePassword(true);
                    break;
            }
        };

        function onDialogButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'saveUser':
                    ddoc.SaveUser();
                    break;
                case 'cancel':
                    ddoc.user = undefined;
                    $('#userEditor').dialog('destroy');
                    break;
                case 'nextScreen':
                    ddoc.EditUserProfile();
                    break;
            }
        }

        $('#Password').keyup(ddoc.ValidatePasswordStrength);
        $('#PasswordConfirmation').keyup(ddoc.ValidatePasswordConfirmation);

        $('.menu-bar', '#userEditor').on('click', 'li', onMenuBarOptionClick);
        $('#frmUserEdit').on('click', 'button', onDialogButtonClick);
    })();
};

DDocUi.prototype.SaveUser = function() {

    var user = (function getFormData() {
        var data = {};

        $.each($('#frmUserEdit').serializeArray(), function(i, field) {
            data[field.name] = field.value;
        });

        data.Profile = ddoc.user.Profile;

        if (data.IsNew === 'True') {
            data.IsNew = true;
            
        } else {
            data.IsNew = false;
        }

        return data;
    })();

    ddoc.POST('/Admin/SaveUser', user, function() {
        ddoc.ShowAlert('Usuario guardado con éxito!', function() {
            ddoc.user = undefined;
            $('#userEditor').dialog('destroy');
            ddoc.LoadUsers();
        });
    });
};