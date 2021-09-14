DDocUi.prototype.ScannerUpload = function (pageId, sequence, grid) {
    $('#newPagePrompt').dialog('destroy');
    $('#replacePagePrompt').dialog('destroy');

    ddoc.CreateModal('/Document/Scan' + (ddoc.StandaloneWindow ? '?apiToken=' + ddoc.ApiToken : ''), { document: ddoc.document }, 'documentAddScannerPage', 'Agregar Páginas', { width: 1000 }, function () {
        ddoc.InitScannerUpload(pageId, sequence, grid);
    });
};

DDocUi.prototype.InitScannerUpload = function (pageId, sequence, grid) {

    (function setupControls() {
        ddoc.InitScannerStuff();

        ddoc.ScannedFiles = new FormData();
    })();

    (function setupEvents() {
        function onDialogButtonClick() {
            switch (this.id) {
                case 'cancelAddPages':
					if(ddoc.Twain.RemoveAllImages)
						ddoc.Twain.RemoveAllImages();
                    $('#documentAddScannerPage').dialog('destroy');
                    break;
                case 'uploadImages':
                    ddoc.UploadScannedImages(pageId, sequence);
                    break;
            }
        }

        $('#scan').click(function () { ddoc.AcquireImage(); });
        $('#cropImage').click(function () { ddoc.CropImage(); });
        $('#rotateLeft').click(function () { ddoc.Twain.RotateLeft(ddoc.Twain.CurrentImageIndexInBuffer); });
        $('#rotateRight').click(function () { ddoc.Twain.RotateRight(ddoc.Twain.CurrentImageIndexInBuffer); });
        $('#mirrorImage').click(function () { if (ddoc.CheckIfImagesInBuffer()) ddoc.Twain.Mirror(ddoc.Twain.CurrentImageIndexInBuffer); });
        $('#flipImage').click(function () { if (ddoc.CheckIfImagesInBuffer()) ddoc.Twain.Flip(ddoc.Twain.CurrentImageIndexInBuffer); });
        $('#removeCurrentImage').click(function () { ddoc.RemoveCurrentImage(); });
        $('#removeAllImages').click(function () { ddoc.RemoveAllImages(); });
        $('.imgTypeSelect').click(function () { ddoc.SelectCompression(); });
        $('#pageActualOptionreviewMode').change(function () { ddoc.SetPreviewMode(); });
        $('#navigateFirstImage').click(function () { ddoc.NavigateFirstImage(); });
        $('#navigatePreImage').click(function () { ddoc.NavigatePrevImage(); });
        $('#navigateNextImage').click(function () { ddoc.NavigateNextImage(); });
        $('#navigateLastImage').click(function () { ddoc.NavigateLastImage(); });

        $('.dialog-button-panel').on('click', 'button', onDialogButtonClick);
    })();
};

DDocUi.prototype.InitScannerStuff = function () {
    document.onscroll = ddoc.OnMouseScroll;
    var previewMode = document.getElementById('previewMode');
    previewMode.options.length = 0;
    previewMode.options.add(new Option('1X1', 0));
    previewMode.options.add(new Option('2X2', 1));
    previewMode.options.add(new Option('3X3', 2));
    previewMode.options.add(new Option('4X4', 3));
    previewMode.options.add(new Option('5X5', 4));
    previewMode.selectedIndex = 0;
    var resolution = document.getElementById('resolution');
    resolution.options.value = 300;
    var colorType = document.getElementById('colortype');
    colorType.options.value = 2;
    Dynamsoft.WebTwainEnv.RegisterEvent('OnWebTwainReady', ddoc.OnTwainReady); // Register OnWebTwainReady event. This event fires as soon as Dynamic Web TWAIN is initialized and ready to be used
    ddoc.Twain = {};
};


DDocUi.prototype.OnTwainReady = function () {
    ddoc.Twain = Dynamsoft.WebTwainEnv.GetWebTwain('dwtcontrolContainer'); // Get the Dynamic Web TWAIN object that is embeded in the div with id 'dwtcontrolContainer'
    if (ddoc.Twain) {
        var count = ddoc.Twain.SourceCount; // Populate how many sources are installed in the system
        for (var i = 0; i < count; i++)
            document.getElementById('source').options.add(new Option(ddoc.Twain.GetSourceNameItems(i), i)); // Add the sources in a drop-down list
    }
    ddoc._iLeft = 0;
    ddoc._iTop = 0;
    ddoc._iRight = 0;
    ddoc._iBottom = 0;
    ddoc.Twain.RegisterEvent('OnPostTransfer', function () { ddoc.UpdatePageInfo(); });
    ddoc.Twain.RegisterEvent('OnPostLoad', function () { ddoc.UpdatePageInfo(); });
    ddoc.Twain.RegisterEvent('OnPostAllTransfers', function () { ddoc.UpdatePageInfo(); });
    ddoc.Twain.RegisterEvent('OnMouseClick', function () { ddoc.UpdatePageInfo(); });
    ddoc.Twain.RegisterEvent('OnImageAreaSelected', ddoc.OnImageAreaSelected);
    ddoc.Twain.RegisterEvent('OnImageAreaDeSelected', ddoc.OnImageAreaDeselected);
    ddoc.Twain.RegisterEvent('OnTopImageInTheViewChanged', ddoc.OnTopImageInTheViewChanged);
    ddoc.UpdatePageInfo();
}

DDocUi.prototype.AcquireImage = function () {

    ddoc.Twain.SelectSourceByIndex(document.getElementById('source').selectedIndex);
    ddoc.Twain.CloseSource();
    ddoc.Twain.OpenSource();

    //no debe mostrar la interfaz :D :D
    ddoc.Twain.IfShowUI = true;

    //seleccionamos el tipo de Pixel  :|
    ddoc.Twain.PixelType = document.getElementById('colortype').value;

    //resolucion a 300dpi :D
    ddoc.Twain.Resolution = document.getElementById('resolution').value;

    ddoc.Twain.IfFeederEnabled = false;

    //seleccionamos solo una cara :v
    ddoc.Twain.IfDuplexEnabled = document.getElementById('Duplex').checked;

    ddoc.Twain.IfDisableSourceAfterAcquire = true; // Scanner source will be disabled/closed automatically after the scan.
    ddoc.Twain.AcquireImage();

    if (document.getElementById('colortype').value == 0) {
        document.getElementById('imgTypejpeg').checked = false;
        document.getElementById('imgTypepdf').checked = false;
        document.getElementById('imgTypetiff').checked = true;
        document.getElementById('imgTypejpeg').disabled = true;
    } else {
        document.getElementById('imgTypejpeg').disabled = false;
    }
}

DDocUi.prototype.CropImage = function () {
    if (!ddoc.CheckIfImagesInBuffer()) {
        return;
    }
    if (ddoc._iLeft != 0 || ddoc._iTop != 0 || ddoc._iRight != 0 || ddoc._iBottom != 0) {
        ddoc.Twain.Crop(
            ddoc.Twain.CurrentImageIndexInBuffer,
            ddoc._iLeft, ddoc._iTop, ddoc._iRight, ddoc._iBottom
        );
        ddoc._iLeft = 0;
        ddoc._iTop = 0;
        ddoc._iRight = 0;
        ddoc._iBottom = 0;
        return;
    } else {
        this.ShowAlert('debe seleccionar el área a recortar');
    }
}


DDocUi.prototype.CheckIfImagesInBuffer = function () {
    if (ddoc.Twain.HowManyImagesInBuffer == 0) {
        return false;
    } else
        return true;
}

DDocUi.prototype.UpdatePageInfo = function () {
    try {
		if (ddoc.Twain) {
			ddoc.Twain.CloseSource();
		}
        document.getElementById('DW_TotalImage').value = ddoc.Twain.HowManyImagesInBuffer;
        document.getElementById('DW_CurrentImage').value = ddoc.Twain.CurrentImageIndexInBuffer + 1;
    } catch (err) {
    }
}

// Botones de navegación

DDocUi.prototype.NavigateFirstImage = function () {
    if (!ddoc.CheckIfImagesInBuffer()) {
        return;
    }
    ddoc.Twain.CurrentImageIndexInBuffer = 0;
    ddoc.UpdatePageInfo();
}

DDocUi.prototype.NavigatePrevImage = function () {
    if (!ddoc.CheckIfImagesInBuffer()) {
        return;
    } else if (ddoc.Twain.CurrentImageIndexInBuffer == 0) {
        return;
    }
    ddoc.Twain.CurrentImageIndexInBuffer = ddoc.Twain.CurrentImageIndexInBuffer - 1;
    ddoc.UpdatePageInfo();
}

DDocUi.prototype.NavigateNextImage = function () {
    if (!ddoc.CheckIfImagesInBuffer()) {
        return;
    } else if (ddoc.Twain.CurrentImageIndexInBuffer == ddoc.Twain.HowManyImagesInBuffer - 1) {
        return;
    }
    ddoc.Twain.CurrentImageIndexInBuffer = ddoc.Twain.CurrentImageIndexInBuffer + 1;
    ddoc.UpdatePageInfo();
}

DDocUi.prototype.NavigateLastImage = function () {
    if (!ddoc.CheckIfImagesInBuffer()) {
        return;
    }
    ddoc.Twain.CurrentImageIndexInBuffer = ddoc.Twain.HowManyImagesInBuffer - 1;
    ddoc.UpdatePageInfo();
}

DDocUi.prototype.RemoveCurrentImage = function () {
    if (!ddoc.CheckIfImagesInBuffer()) {
        return;
    }
    ddoc.Twain.RemoveAllSelectedImages();
    if (ddoc.Twain.HowManyImagesInBuffer == 0) {
        document.getElementById('DW_TotalImage').value = ddoc.Twain.HowManyImagesInBuffer;
        document.getElementById('DW_CurrentImage').value = '';
        return;
    } else {
        ddoc.UpdatePageInfo();
    }
}

DDocUi.prototype.RemoveAllImages = function () {
    if (!ddoc.CheckIfImagesInBuffer()) {
        return;
    }
    ddoc.Twain.RemoveAllImages();
    document.getElementById('DW_TotalImage').value = '0';
    document.getElementById('DW_CurrentImage').value = '';
}

DDocUi.prototype.SetPreviewMode = function () {
    var varNum = parseInt(document.getElementById('previewMode').selectedIndex + 1);
    var btnCrop = document.getElementById('cropImage');
    if (btnCrop) {

        if (varNum > 1) {
            btnCrop.onclick = function () { };
        } else {
            btnCrop.onclick = function () { ddoc.CropImage(); };
        }
    }

    ddoc.Twain.SetViewMode(varNum, varNum);
    if (Dynamsoft.Lib.env.bMac) {
        return;
    } else if (document.getElementById('previewMode').selectedIndex != 0) {
        ddoc.Twain.MouseShape = true;
    } else {

        ddoc.Twain.MouseShape = false;
    }
}

DDocUi.prototype.OnImageAreaSelected = function (index, left, top, right, bottom) {
    ddoc._iLeft = left;
    ddoc._iTop = top;
    ddoc._iRight = right;
    ddoc._iBottom = bottom;
}

DDocUi.prototype.OnImageAreaDeselected = function () {
    ddoc._iLeft = 0;
    ddoc._iTop = 0;
    ddoc._iRight = 0;
    ddoc._iBottom = 0;
}

DDocUi.prototype.OnTopImageInTheViewChanged = function (index) {
    ddoc._iLeft = 0;
    ddoc._iTop = 0;
    ddoc._iRight = 0;
    ddoc._iBottom = 0;
    ddoc.Twain.CurrentImageIndexInBuffer = index;
    ddoc.UpdatePageInfo();
}

DDocUi.prototype.SelectCompression = function () {

    var vCompOutput = document.getElementById('OutComp');
    vCompOutput.options.length = 0;

    if (document.getElementById('imgTypejpeg').checked == true) {
        vCompOutput.options.add(new Option('Automatico', 0));
    } else if (document.getElementById('imgTypetiff').checked == true) {
        vCompOutput.options.add(new Option('Automatico', 0));
        vCompOutput.options.add(new Option('Ninguno', 1));
        vCompOutput.options.add(new Option('CCITT RLE', 2));
        vCompOutput.options.add(new Option('CCITT T.4 (TIFF 6)', 3));
        vCompOutput.options.add(new Option('CCITT Group 4 (FAX4)', 4));
        vCompOutput.options.add(new Option('CCITT T.6 (TIFF 6)', 4));
        vCompOutput.options.add(new Option('LZW', 5));
        vCompOutput.options.add(new Option('JPEG', 7));
        vCompOutput.options.value = 5;

    } else if (document.getElementById('imgTypepdf').checked == true) {
        vCompOutput.options.add(new Option('Automatico', 0));
        vCompOutput.options.add(new Option('CCITT Group 3 (FAX3)', 1));
        vCompOutput.options.add(new Option('CCITT Group 4 (FAX4)', 2));
        vCompOutput.options.add(new Option('LZW', 3));
        vCompOutput.options.add(new Option('CCITT RLE', 4));
        vCompOutput.options.add(new Option('JPEG', 5));
    }
}

DDocUi.prototype.UploadScannedImages = function (pageId, sequence) {

    if (ddoc.Twain) {
        if (ddoc.Twain.HowManyImagesInBuffer == 0)
            return;

        var url = ddoc.GetUrl('/Document/ScannerUpload?username=' + ddoc.GetUsername() + '&documentId=' + ddoc.document.Id + (pageId ? '&pageId=' + pageId : '') + (sequence ? '&sequence=' + sequence : '') + (ddoc.StandaloneWindow ? '&apiToken=' + ddoc.ApiToken : ''));
        function onUploadSuccess() {
            $('#documentAddScannerPage').dialog('destroy');
            if (!ddoc.document.IsNew) {
                ddoc.ShowAlert('Páginas agregadas!');
            }
            if (ddoc.document.IsNew) {
                ddoc.LoadNewDocumentPages();
            }
            else {
                if (typeof ddoc.LoadDocumentPages == 'function') {
                    ddoc.LoadDocumentPages();
                }
                if (typeof ddoc.LoadDocuments == 'function') {
                    ddoc.LoadDocuments();
                }
            }

        }

        function onUploadFailure(errorCode, errorString, sHttpResponse) {
            alert(errorString + sHttpResponse);
        }

        ddoc.Twain.IfSSL = false;
        ddoc.Twain.HTTPPort = 80;//location.port == '' ? 80 : location.port;

        switch (true) {
            case ddoc.Twain.HowManyImagesInBuffer == 1:

                break;
            case ddoc.Twain.HowManyImagesInBuffer > 1:

                break;
        }

        if (document.getElementById('imgTypejpeg').checked == true) {
            ddoc.Twain.HTTPUploadThroughPost(location.hostname, ddoc.Twain.CurrentImageIndexInBuffer, url, pageId + '.JPG', onUploadSuccess, onUploadFailure);
        } else if (document.getElementById('imgTypetiff').checked == true) {
            ddoc.Twain.TIFFCompressionType = document.getElementById('OutComp').value;
            ddoc.Twain.HTTPUploadAllThroughPostAsMultiPageTIFF(location.hostname, url, pageId + '.TIF', onUploadSuccess, onUploadFailure);
        } else if (document.getElementById('imgTypepdf').checked == true) {
            ddoc.Twain.PDFCompressionType = document.getElementById('OutComp').value;
            ddoc.Twain.HTTPUploadAllThroughPostAsPDF(location.hostname, url, pageId + '.PDF', onUploadSuccess, onUploadFailure);
        }
    }
}

DDocUi.prototype.OnMouseScroll = function (e) {
    var mousewheelevt = (/Firefox/i.test(navigator.userAgent)) ? 'DOMMouseScroll' : 'mousewheel';

    function navigateImages(event) {
        var evt = window.event || event;
        var delta = evt.detail ? evt.detail * (-120) : evt.wheelDelta;
        if (delta < 0)
            ddoc.NavigateNextImage();
        else if (delta > 0)
            ddoc.NavigatePrevImage();
    }

    if (document.attachEvent)
        document.attachEvent('on' + mousewheelevt, navigateImages);
    else if (document.addEventListener);
    document.addEventListener(mousewheelevt, navigateImages, false);
}

function stopWheel(evt) {
    if (!evt) { /* IE7, IE8, Chrome, Safari */
        evt = window.event;
    }
    if (evt.preventDefault) { /* Chrome, Safari, Firefox */
        var ret = evt.preventDefault();
    }
    evt.returnValue = false; /* IE7, IE8 */
}

$('.dtw').hover(function () {
    $(document).bind('mousewheel DOMMouseScroll', function (event) {
        stopWheel(event);
    });
}, function () {
    $(document).unbind('mousewheel DOMMouseScroll');
});