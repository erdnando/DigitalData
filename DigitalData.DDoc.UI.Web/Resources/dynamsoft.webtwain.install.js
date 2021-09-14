
function OnWebTwainNotFoundOnWindowsCallback(ProductName, InstallerUrl, bHTML5, bIE, bSafari, bSSL, strIEVersion) {
    _show_install_dialog(ProductName, InstallerUrl, bHTML5, false, bIE, bSafari, bSSL, strIEVersion);
}

function OnWebTwainNotFoundOnMacCallback(ProductName, InstallerUrl, bHTML5, bIE, bSafari, bSSL, strIEVersion) {
    _show_install_dialog(ProductName, InstallerUrl, bHTML5, true, bIE, bSafari, bSSL, strIEVersion);
}

function _show_install_dialog(ProductName, InstallerUrl, bHTML5, bMac, bIE, bSafari, bSSL, strIEVersion) {

    //var _height = 150, ObjString = [' <div style= "font-size: 14px; ',
    //		'color:black; display:inline-block; margin:auto; margin-left:5px; ',
    //	'margin-top:10px; font-weight: bold;"> EL COMPONENTE DYNAMSOFT WEB TWAIN NO ESTA INSTALADO. </div>',
    //		''];

    //<div class="mainFront">
    //			     <div style="height:100px; width:300px; margin:auto; text-alin:center; background-color:white; 
    //    border:2px solid gray; border-radius:4px; display:inline-block;">
    //				     <label style="Font-family: 'Helvetica Neue', sans-serif; font-size: 12px; 
    //    display:inline-block; margin:auto; margin-left:5px; 
    //						margin-top:25px;"> El componente Dynamsoft Web Twain no esta instalado. </label>
    //				</div>
    //			</div>

    document.getElementById('dwtError').style.display = 'inline-block';
    document.getElementById('uploadImages').style.display = 'none';
}

function OnWebTwainOldPluginNotAllowedCallback(ProductName) {
    var ObjString = [
        '<div class="dwt-box-title">',
        ProductName,
        ' plugin is not allowed to run on this site.</div>',
        '<ul>',
        '<li>Please click "<b>Always run on this site</b>" for the prompt "',
        ProductName,
        ' Plugin needs your permission to run", then <a href="javascript:void(0);" style="color:blue" class="ClosetblCanNotScan">close</a> this dialog OR refresh/restart the browser and try again.</li>',
        '</ul>'];

    Dynamsoft.WebTwainEnv.ShowDialog(392, 227, ObjString.join(''));
}

function OnWebTwainNeedUpgradeCallback(ProductName, InstallerUrl, bHTML5, bMac, bIE, bSafari, bSSL, strIEVersion) {
    var ObjString = ['<div class="dwt-box-title"></div>',
        '<div style="font-size: 15px;">',
        'This page is using a newer version of Dynamic Web TWAIN than your local copy. Please download and upgrade now.',
        '</div>',
        '<a id="dwt-btn-install" target="_blank" href="',
        InstallerUrl,
        '" onclick="Dynamsoft_OnClickInstallButton()"><div class="dwt-button"></div></a>',
        '<div style="text-align:center"><i>* Please manually install it</i></div><p></p>'], _height = 220;

    if (bHTML5) {
        ObjString.push('<div class="dwt-red">Please REFRESH your browser after the upgrade.</div>');
    } else {

        if (bIE) {
            _height = 240;
            ObjString.push('<div class="dwt-red">');
            ObjString.push('Please EXIT Internet Explorer before you install the new version.');
            ObjString.push('</div>');
        }
        else {
            ObjString.push('<div class="dwt-red">Please RESTART your browser after the upgrade.</div>');
        }
    }

    Dynamsoft.WebTwainEnv.ShowDialog(392, _height, ObjString.join(''));
}

function OnWebTwainPreExecuteCallback() {
    Dynamsoft.WebTwainEnv.OnWebTwainPreExecute();
}

function OnWebTwainPostExecuteCallback() {
    Dynamsoft.WebTwainEnv.OnWebTwainPostExecute();
}

