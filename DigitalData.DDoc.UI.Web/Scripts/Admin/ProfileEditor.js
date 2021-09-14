DDocUi.prototype.EditUserProfile = function() {
    ddoc.CreateModal('/Admin/EditUserProfile', ddoc.user, 'profileEditor', 'Editar perfil de usuario', { width: 580 }, ddoc.InitUserProfileEditor);
};

DDocUi.prototype.InitUserProfileEditor = function() {

    (function setupEvents() {
        function onFormButtonClick(e) {
            e.preventDefault();
            switch (this.id) {
                case 'saveProfile':
                    var user = { Username: ddoc.user.Username };
                    user.Profile = [];
                    $('li', '#userProfile').each(function(i, item) {
                        user.Profile.push({ Id: $(item).attr('id') });
                    });
                    ddoc.POST('/Admin/UpdateUserProfile', user, function() {
                        ddoc.ShowAlert('Perfiles guardados correctamente!', function() {
                            $('#profileEditor').dialog('destroy');
                        });
                    });
                    break;
                case 'saveUser':
                    ddoc.user.Profile = [];
                    $('li', '#userProfile').each(function(i, item) {
                        ddoc.user.Profile.push({ Id: $(item).attr('id') });
                    });
                    $('#profileEditor').dialog('destroy');
                    ddoc.SaveUser();
                    break;
                case 'prevScreen':
                case 'cancel':
                    $('#profileEditor').dialog('destroy');
                    break;
            }
        }

        function onDrop(e) {
            e.preventDefault();
            var groupId = e.originalEvent.dataTransfer.getData('text');
            e.originalEvent.dataTransfer.dropEffect = 'move';
            $(this).append($('#' + groupId));
        };

        function onDragStart(e) {
            e.originalEvent.dataTransfer.setData('text', e.target.id);
            e.originalEvent.dataTransfer.effectAllowed = 'move';
        };

        function onDragOver(e) {
            e.preventDefault();
        };

        $('#frmUserProfile').on('click', 'button', onFormButtonClick);

        $('.group-selector').on('drop', onDrop).on('dragover', onDragOver);
        $('#groupList, #userProfile').on('dragstart', 'li', onDragStart);
    })();

    ddoc.LoadGroups();
};

DDocUi.prototype.LoadGroups = function() {

    ddoc.GET('/Admin/GetDDocGroups', { groupType: 1 }, function(response) {
        $.each(response.List, function(i, profile) {
            $('#groupList').append($('<li>').append($('<span class="group-icon">')).append($('<label>').html(profile.Name)).attr('id', profile.Id).attr('draggable', 'true'));
        });
        ddoc.POST('/Admin/GetUserProfile', ddoc.user, function(resp) {
            $.each(resp.List, function(i, userProfile) {
                $('#userProfile').append($('li[id="' + userProfile.Id + '"]', '#groupList'));
            });
        });
    });
};