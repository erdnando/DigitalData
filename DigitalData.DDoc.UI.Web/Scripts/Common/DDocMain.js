if (!String.prototype.includes) {
	String.prototype.includes = function (search, start) {
		'use strict';
		if (typeof start !== 'number') {
			start = 0;
		}

		if (start + search.length > this.length) {
			return false;
		} else {
			return this.indexOf(search, start) !== -1;
		}
	};
}

if (!Array.prototype.groupBy) {
	Array.prototype.groupBy = function (key) {
		return this.reduce(function (rv, x) {
			(rv[x[key]] = rv[x[key]] || []).push(x);
			return rv;
		}, {});
	};
}

if (!Array.prototype.find) {
	Object.defineProperty(Array.prototype, 'find', {
		value: function (predicate) {
			// 1. Let O be ? ToObject(this value).
			if (this == null) {
				throw new TypeError('"this" is null or not defined');
			}

			var o = Object(this);

			// 2. Let len be ? ToLength(? Get(O, "length")).
			var len = o.length >>> 0;

			// 3. If IsCallable(predicate) is false, throw a TypeError exception.
			if (typeof predicate !== 'function') {
				throw new TypeError('predicate must be a function');
			}

			// 4. If thisArg was supplied, let T be thisArg; else let T be undefined.
			var thisArg = arguments[1];

			// 5. Let k be 0.
			var k = 0;

			// 6. Repeat, while k < len
			while (k < len) {
				// a. Let Pk be ! ToString(k).
				// b. Let kValue be ? Get(O, Pk).
				// c. Let testResult be ToBoolean(? Call(predicate, T, « kValue, k, O »)).
				// d. If testResult is true, return kValue.
				var kValue = o[k];
				if (predicate.call(thisArg, kValue, k, o)) {
					return kValue;
				}
				// e. Increase k by 1.
				k++;
			}

			// 7. Return undefined.
			return undefined;
		},
		configurable: true,
		writable: true
	});
}

if (!Array.prototype.filter) {
	Array.prototype.filter = function (func, thisArg) {
		'use strict';
		if (!((typeof func === 'Function' || typeof func === 'function') && this))
			throw new TypeError();

		var len = this.length >>> 0,
			res = new Array(len), // preallocate array
			t = this, c = 0, i = -1;
		if (thisArg === undefined) {
			while (++i !== len) {
				// checks to see if the key was set
				if (i in this) {
					if (func(t[i], i, t)) {
						res[c++] = t[i];
					}
				}
			}
		}
		else {
			while (++i !== len) {
				// checks to see if the key was set
				if (i in this) {
					if (func.call(thisArg, t[i], i, t)) {
						res[c++] = t[i];
					}
				}
			}
		}

		res.length = c; // shrink down array to proper size
		return res;
	};
}

if (!Array.prototype.map) {

	Array.prototype.map = function (callback/*, thisArg*/) {

		var T, A, k;

		if (this == null) {
			throw new TypeError('this is null or not defined');
		}

		// 1. Let O be the result of calling ToObject passing the |this| 
		//    value as the argument.
		var O = Object(this);

		// 2. Let lenValue be the result of calling the Get internal 
		//    method of O with the argument "length".
		// 3. Let len be ToUint32(lenValue).
		var len = O.length >>> 0;

		// 4. If IsCallable(callback) is false, throw a TypeError exception.
		// See: http://es5.github.com/#x9.11
		if (typeof callback !== 'function') {
			throw new TypeError(callback + ' is not a function');
		}

		// 5. If thisArg was supplied, let T be thisArg; else let T be undefined.
		if (arguments.length > 1) {
			T = arguments[1];
		}

		// 6. Let A be a new array created as if by the expression new Array(len) 
		//    where Array is the standard built-in constructor with that name and 
		//    len is the value of len.
		A = new Array(len);

		// 7. Let k be 0
		k = 0;

		// 8. Repeat, while k < len
		while (k < len) {

			var kValue, mappedValue;

			// a. Let Pk be ToString(k).
			//   This is implicit for LHS operands of the in operator
			// b. Let kPresent be the result of calling the HasProperty internal 
			//    method of O with argument Pk.
			//   This step can be combined with c
			// c. If kPresent is true, then
			if (k in O) {

				// i. Let kValue be the result of calling the Get internal 
				//    method of O with argument Pk.
				kValue = O[k];

				// ii. Let mappedValue be the result of calling the Call internal 
				//     method of callback with T as the this value and argument 
				//     list containing kValue, k, and O.
				mappedValue = callback.call(T, kValue, k, O);

				// iii. Call the DefineOwnProperty internal method of A with arguments
				// Pk, Property Descriptor
				// { Value: mappedValue,
				//   Writable: true,
				//   Enumerable: true,
				//   Configurable: true },
				// and false.

				// In browsers that support Object.defineProperty, use the following:
				// Object.defineProperty(A, k, {
				//   value: mappedValue,
				//   writable: true,
				//   enumerable: true,
				//   configurable: true
				// });

				// For best browser support, use the following:
				A[k] = mappedValue;
			}
			// d. Increase k by 1.
			k++;
		}

		// 9. return A
		return A;
	};
}

if (!Array.prototype.findIndex) {
	Object.defineProperty(Array.prototype, 'findIndex', {
		value: function (predicate) {
			// 1. Let O be ? ToObject(this value).
			if (this == null) {
				throw new TypeError('"this" is null or not defined');
			}

			var o = Object(this);

			// 2. Let len be ? ToLength(? Get(O, "length")).
			var len = o.length >>> 0;

			// 3. If IsCallable(predicate) is false, throw a TypeError exception.
			if (typeof predicate !== 'function') {
				throw new TypeError('predicate must be a function');
			}

			// 4. If thisArg was supplied, let T be thisArg; else let T be undefined.
			var thisArg = arguments[1];

			// 5. Let k be 0.
			var k = 0;

			// 6. Repeat, while k < len
			while (k < len) {
				// a. Let Pk be ! ToString(k).
				// b. Let kValue be ? Get(O, Pk).
				// c. Let testResult be ToBoolean(? Call(predicate, T, « kValue, k, O »)).
				// d. If testResult is true, return k.
				var kValue = o[k];
				if (predicate.call(thisArg, kValue, k, o)) {
					return k;
				}
				// e. Increase k by 1.
				k++;
			}

			// 7. Return -1.
			return -1;
		},
		configurable: true,
		writable: true
	});
}

$(window).bind('beforeunload', function cancelThumbnailQueue () {
	if (window.ddoc != undefined) {
		if (ddoc.thumbnailLoadQueue) {
			ddoc.thumbnailLoadQueue.clear();
			if (ddoc.currentXhr) {
				ddoc.currentXhr.abort();
			}
		}
		if (ddoc.grids) {
			$.each(ddoc.grids, function (i, grid) {
				if (grid.thumbnailLoadQueue) {
					grid.thumbnailLoadQueue.clear();
					if (ddoc.currentXhr) {
						ddoc.currentXhr.abort();
					}
				}
			});
		}
	}
});

$(document).ready(function () {

	$(document)
		.ajaxError(function (event, jqXhr, ajaxSettings) {
			switch (jqXhr.status) {
				case 0: // Request cancelado
				case 200:
					if (jqXhr.responseText != '') {
						ddoc.ShowAlert(jqXhr.responseText);
					}
					break;
				case 303: // Sesión expirada
				case 401: // Ataque CSRF
					try {
						window.location = ddoc.GetUrl('/Error/Unauthorized');
					}
					catch (e) {
						ddoc.ShowAlert('Error irrecuperable');
					}
					break;
				case 403:
					ddoc.ShowAlert(jqXhr.responseText);
					break;
				case 404:
					ddoc.ShowAlert('Error 404: ' + /<b> Requested URL: <\/b>([A-Za-z\/]+)<br>/.exec(jqXhr.responseText)[1]);
					break;
				case 500:
					ddoc.ShowAlert('Error Interno. Contacte al Administrador.');
					break;
				default:
					ddoc.ShowAlert('Se sucito un error, intente de nuevo! - Info: ' + jqXhr.statusText);
			}
		})
		.ajaxSend(function () {
			if (window.ddoc) {
				if (ddoc.thumbnailLoadQueue) {
					ddoc.thumbnailLoadQueue.pause();
				}
				if (ddoc.grids) {
					$.each(ddoc.grids, function (i, grid) {
						if (grid.thumbnailLoadQueue) {
							grid.thumbnailLoadQueue.pause();
						}
					});
				}
			}
		})
		.ajaxComplete(function () {
			if (window.ddoc) {
				if (ddoc.thumbnailLoadQueue) {
					ddoc.thumbnailLoadQueue.start();
				}
				if (ddoc.grids) {
					$.each(ddoc.grids, function (i, grid) {
						if (grid.thumbnailLoadQueue) {
							grid.thumbnailLoadQueue.start();
						}
					});
				}
			}
		});
});

if (typeof DDocUi === 'undefined') {
	var DDocUi = function () {
		if (DDocUi.instance) {
			return DDocUi.instance;
		}
		DDocUi.instance = this;
		this.prototype = {};
	};
}

DDocUi.Enums = {
	CollectionType: {
		Root: 0,
		Collection: 1,
		Folder: 2,
		Document: 3,
	}
};

DDocUi.prototype.SetActiveStyleSheet = function (title) {
	var i, a;
	for (i = 0; (a = document.getElementsByTagName('link')[i]); i++) {
		if (a.getAttribute('rel').indexOf('style') != -1 && a.getAttribute('title')) {
			a.disabled = true;
			if (a.getAttribute('title') == title) {
				a.disabled = false;
			}
		}
	}
};

DDocUi.prototype.GetActiveStyleSheet = function () {
	var i, a;
	for (i = 0; (a = document.getElementsByTagName('link')[i]); i++) {
		if (a.getAttribute('rel').indexOf('style') != -1 && a.getAttribute('title') && !a.disabled) {
			return a.getAttribute('title');
		}
	}
	return null;
};

DDocUi.prototype.GetPreferredStyleSheet = function () {
	var i, a;
	for (i = 0; (a = document.getElementsByTagName('link')[i]); i++) {
		if (a.getAttribute('rel').indexOf('style') != -1
			&& a.getAttribute('rel').indexOf('alt') == -1
			&& a.getAttribute('title')
		) {
			return a.getAttribute('title');
		}
	}
	return null;
};

DDocUi.prototype.CreateCookie = function (name, value, days) {
	var expires;
	if (days) {
		var date = new Date();
		date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
		expires = '; expires=' + date.toGMTString();
	} else {
		expires = '';
	}
	document.cookie = name + '=' + value + expires + '; path=/';
};

DDocUi.prototype.ReadCookie = function (name) {
	var nameEq = name + '=';
	var ca = document.cookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') {
			c = c.substring(1, c.length);
		}
		if (c.indexOf(nameEq) == 0) {
			return c.substring(nameEq.length, c.length);
		}
	}
	return null;
};

/**
 * @Description Función que muestra una alerta en pantalla
 * @param {string} message: Mensaje a mostrar en la alerta
 * @param {function} callback: Función a ejecutar al cerrar la alerta
 */
DDocUi.prototype.ShowAlert = function (message, callback) {

	function closeAlert () {
		$('#alertDialog').dialog('destroy');
		if (callback && typeof callback == 'function') {
			callback();
		}
	}

	var $alertDialog = $('<div></div>').attr('id', 'alertDialog').attr('title', 'DDOC');

	var alertParameters = {
		resizable: false,
		modal: true,
		width: 300,
		close: closeAlert,
		dialogClass: 'modal-dialog alert no-select no-close'
	};

	var alertContent = $('<div>').addClass('alert-contents').text(message);
	var buttonPanel = $('<div>').addClass('alert-button-panel').append($('<button>').text('Aceptar'));

	$alertDialog.on('click', 'button', closeAlert);

	$alertDialog.append(alertContent, buttonPanel).dialog(alertParameters);
};

/**
 * @Description Función que muestra un dialogo de Aceptar/Cancelar para una acción determinada
 * @param {string} message: Mensaje para mostrar en el dialogo de confirmación
 * @param {function} okCallback: Función que se ejecutará al dar click en el botón Aceptar
 * @param {function} cancelCallback: Función que se ejecutará al dar click en el botón Cancelar
 * @param {function} closeCallback: Función que se ejecutará al cerrarse el dialogo de confirmación
 */
DDocUi.prototype.Confirm = function (message, okCallback, cancelCallback, closeCallback) {

	function closeConfirm () {
		$('#okCancelDialog').dialog('destroy');
		if (closeCallback && typeof closeCallback == 'function') {
			closeCallback();
		}
	}

	function onConfirmButtonClick () {
		switch (this.id) {
			case 'accept':
				if (okCallback && typeof okCallback == 'function') {
					okCallback();
				}
				break;
			case 'cancel':
				if (cancelCallback && typeof cancelCallback == 'function') {
					cancelCallback();
				}
				break;
		}
		closeConfirm();
	}

	var $confirmDialog = $('<div></div>').attr('id', 'okCancelDialog').attr('title', 'DDOC');

	var confirmParameters = {
		resizable: false,
		modal: true,
		width: 300,
		close: closeConfirm,
		dialogClass: 'modal-dialog alert no-select no-close'
	};

	var confirmContent = $('<div>').addClass('alert-contents').text(message);
	var buttonPanel = $('<div>').addClass('alert-button-panel').append($('<button id="accept">').text('Aceptar')).append($('<button id="cancel">').text('Cancelar'));

	$confirmDialog.on('click', 'button', onConfirmButtonClick);

	$confirmDialog.append(confirmContent, buttonPanel).dialog(confirmParameters);
};

/**
 * @Description Función que muestra un dialogo modal en pantalla
 * @param {string} id: Id del diálogo modal
 * @param {html} content: Contenido del diálogo modal
 * @param {string} title: Título para el diálogo modal
 * @param {object} parameters: Objeto con parámetros para el diálogo:
 *                  resizable {bool}: Si el dialogo se puede redimensioanr por el usuario
 *                  modal {bool}: Si el dialogo se mostrará como Modal (bloquea UI)
 *                  width {int}: Ancho del dialogo
 *                  height {int}: Altura del dialogo
 *                  position {Array}: Posición top relativa a la ventana del browser
 *                  dialogClass {string}: Clase CSS para aplicar al díalogo modal
 */
DDocUi.prototype.ShowModal = function (id, content, title, parameters) {

	var $modalDialog = $('<div>').attr('id', id).attr('title', title);

	var modalParameters = {
		resizable: false,
		draggable: false,
		modal: true,
		width: 800,
		position: { my: 'center', at: 'center', of: window },
		dialogClass: 'modal-dialog no-select no-close'
	};

	if (!parameters) {
		modalParameters.dialogClass += ' whole-screen';
		modalParameters.width = undefined;
		modalParameters.height = undefined;
	} else {
		$.extend(modalParameters, parameters);
	}

	$modalDialog.html(content);
	$modalDialog.dialog(modalParameters);
};

/**
 * @Description Función que muestra un dialogo modal en pantalla
 * @param {string} viewUrl: Url de vista parcial
 * @param {object} data: Datos para el dialogo modal
 * @param {string} modalId: Id del diálogo modal
 * @param {string} modalTitle: Título del diálogo modal
 * @param {object} parameters: Objeto con parámetros para el diálogo
 * @param {function} modalInitializer: Función a ejecutar tras construir el diálogo modal
 */
DDocUi.prototype.CreateModal = function (viewUrl, data, modalId, modalTitle, parameters, modalInitializer) {

	$.ajax({
		type: 'POST',
		url: ddoc.GetUrl(viewUrl),
		dataType: 'html',
		data: data ? JSON.stringify(data) : undefined,
		contentType: 'application/json; charset=utf-8',
		success: function (view) {

			var $modalDialog = $('<div>').attr('id', modalId).attr('title', modalTitle);

			var modalParameters = {
				resizable: false,
				draggable: true,
				modal: true,
				position: { my: 'center', at: 'center', of: window },
				dialogClass: 'modal-dialog no-select no-close'
			};

			if (!parameters) {
				modalParameters.dialogClass += ' whole-screen';
				modalParameters.width = undefined;
				modalParameters.height = undefined;
			} else {
				$.extend(modalParameters, parameters);
			}

			$modalDialog.html(view);
			$modalDialog.dialog(modalParameters);

			if (modalInitializer && typeof modalInitializer == 'function') {
				modalInitializer();
			}
		}
	});
};

DDocUi.prototype.GetUsername = function() {
	if (this.username == undefined) {
		this.username = $('#Username').val();
	}
	return this.username;
}

DDocUi.prototype.GetUrl = function (url) {
	console.log("open url.....");
	if (this.urlBase == undefined) {
		this.urlBase = $('#UrlBase').val();
	}
	var path = (this.urlBase.length > 1) ? this.urlBase : '';
	return path + url;
};

DDocUi.prototype.GET = function (url, data, successCallback, errorCallback) {
	ddoc.AjaxRequest('GET', url, data, successCallback, undefined, errorCallback);
};

DDocUi.prototype.POST = function (url, data, successCallback, sendRawData, errorCallback) {
	ddoc.AjaxRequest('POST', url, data, successCallback, sendRawData, errorCallback);
};

DDocUi.prototype.SGET = function (url, data, successCallback, errorCallback) {
	ddoc.AjaxRequest('GET', url, data, successCallback, undefined, errorCallback, true);
};

DDocUi.prototype.SPOST = function (url, data, successCallback, sendRawData, errorCallback) {
	ddoc.AjaxRequest('POST', url, data, successCallback, sendRawData, errorCallback, true);
};

DDocUi.prototype.EGET = function (url, data, successCallback, errorCallback) {
	ddoc.AjaxRequest('GET', url, data, successCallback, undefined, errorCallback, false, true);
};

DDocUi.prototype.EPOST = function (url, data, successCallback, sendRawData, errorCallback) {
	ddoc.AjaxRequest('POST', url, data, successCallback, sendRawData, errorCallback, false, true);
};

DDocUi.prototype.AjaxRequest = function (requestType, url, data, successCallback, sendRawData, errorCallback, antiCSRF, externalRequest) {

	var requestData = undefined;

	var ajaxOptions = {};

	if (data) {
		switch (requestType) {
			case 'GET':
				requestData = data;
				break;
			case 'POST':
				if (sendRawData) {
					$.extend(ajaxOptions, {
						processData: false,
						contentType: false
					});
					requestData = data;
				} else {
					$.extend(ajaxOptions, {
						dataType: 'json',
						contentType: 'application/json; charset=utf-8'
					});
					requestData = JSON.stringify(data);
				}
				break;
		}
	}

	var finalUrl = '';

	switch (externalRequest) {
		case true:
			finalUrl = url;
			break;
		case undefined:
		case false:
			finalUrl = ddoc.GetUrl(url);
			break;
	}

	ajaxOptions = $.extend(ajaxOptions, {
		url: finalUrl,
		type: requestType,
		data: requestData,
		success: function (response) {
			switch (response.Result) {
				case 0:
					ddoc.ShowAlert(response.Message, function () {
						if (errorCallback && typeof errorCallback == 'function') {
							errorCallback(response);
						};
					});
					break;
				case 1:
					if (response.List) {
						response.List.forEach(function (item) {
							for (let property in item) {
								if (item.hasOwnProperty(property)
									&& typeof item[property] == 'string'
									&& item[property].includes('/Date(')) {
									item[property] = new Date(parseInt(item[property].replace('/Date(', '').replace(')/', ''), 10));
								}
							}
						});
					}

					if (response.ObjectResult) {
						for (let property in response.ObjectResult) {
							if (response.ObjectResult.hasOwnProperty(property)
								&& typeof response.ObjectResult[property] == 'string'
								&& response.ObjectResult[property].includes('/Date(')) {
								response.ObjectResult[property] = new Date(parseInt(response.ObjectResult[property].replace('/Date(', '').replace(')/', ''), 10));;
							}
						}
					}

					if (successCallback && typeof successCallback == 'function') {
						successCallback(response);
					}
					break;
			}
		}
	});

	if (antiCSRF) {
		$.extend(ajaxOptions, {
			headers: {
				'AntiForgeryToken': $('#AntiForgeryToken').val()
			}
		});
	}

	$.ajax(ajaxOptions);
};

DDocUi.prototype.onWindowLoad = function () {
	if (ddoc.CollectionType == DDocUi.Enums.CollectionType.Document) {
		var cookie = ddoc.ReadCookie('__ddocresultstyle');
		var title = cookie ? cookie : ddoc.GetPreferredStyleSheet();
		ddoc.SetActiveStyleSheet(title);
		$('#' + title).addClass('toggle-on');
	} else {
		ddoc.SetActiveStyleSheet('list');
	}
};

DDocUi.prototype.onWindowUnload = function () {
	if (ddoc.CollectionType == DDocUi.Enums.CollectionType.Document) {
		var title = ddoc.GetActiveStyleSheet();
		ddoc.CreateCookie('__ddocresultstyle', title, 365);
	}
};

DDocUi.prototype.GetRandomColor = function getRandomColor () {
	var letters = '789ABCD';
	var color = '#';
	for (var i = 0; i < 6; i++) {
		color += letters[Math.floor(Math.random() * 6)];
	}
	return color;
};

DDocUi.CustomPanel = (function () {
	function customPanel (options) {
		this.opened = false;
		this.panel = options.panel || document.getElementById('customPanel');
		this.toggleButton = options.toggleButton || document.getElementById('customPanelActivator');

		var self = this;

		this.toggleButton.addEventListener('click', function () {
			self.toggle();
		});

		this.panel.addEventListener('keydown', function (evt) {
			switch (evt.keyCode) {
				case 27:
					self.close();
					break;
			}
		});

		this.panel.addEventListener('mouseleave', function () { setTimeout(function () { self.close(); }, 500); });
	}

	customPanel.prototype = {
		open: function () {
			if (!this.opened) {
				this.opened = true;
				this.toggleButton.classList.add('toggled');
				this.panel.classList.remove('hidden');
			}
		},

		close: function () {
			if (!this.opened) {
				return;
			}
			this.opened = false;
			this.toggleButton.classList.remove('toggled');
			this.panel.classList.add('hidden');
		},

		toggle: function () {
			if (this.opened) {
				this.close();
			} else {
				this.open();
			}
		}
	};

	return customPanel;
})();

DDocUi.prototype.FormatCurrency = function (n) {
	return '$ ' + n.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
};