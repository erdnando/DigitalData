DDocUi.prototype.InitLoginScreen = function() {

    (function setupControls() {
        $('#adTest').hide();
    })();

    (function setupEvents() {
        $('#btnLogin').on('click', function (e) {
            e.preventDefault();
            ddoc.Login();
        });

        $('#adTest').on('click', function (e) {
            e.preventDefault();
            ddoc.AdTest();
        });
    })();
};

DDocUi.prototype.Login = function() {

    var loginData = (function getFormData() {
        var data = {
            user: {
                Username: $('#Username').val(),
                Password: $('#Password').val()
            }
        };
        return data;
    })();

    ddoc.POST('/Security/Login', loginData, function (response) {
        window.location.href = response.Url;
    });
};

DDocUi.prototype.AdTest = function () {

    var loginData = (function getFormData() {
        var data = {
            user: {
                Username: $('#Username').val(),
                Password: $('#Password').val()
            }
        };
        return data;
    })();

    ddoc.POST('/Security/AdTest', loginData, function (response) {
        window.location.href = response.Url;
    });
};

var ddoc = new DDocUi();
ddoc.InitLoginScreen();