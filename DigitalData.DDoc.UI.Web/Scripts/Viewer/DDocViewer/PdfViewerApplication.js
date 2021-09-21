'use strict';

DDocUi.prototype.InitializeViewer = function() {

	PDFJS.imageResourcesPath = './images/';
 
	this.Constants = {
		DEFAULT_SCALE_DELTA: 1.1,
		MIN_SCALE: 0.25,
		MAX_SCALE: 10.0,
		SCALE_SELECT_CONTAINER_PADDING: 8,
		SCALE_SELECT_PADDING: 22,
		PAGE_NUMBER_LOADING_INDICATOR: 'visiblePageIsLoading',
		DISABLE_AUTO_FETCH_LOADING_BAR_TIMEOUT: 5000,
		CSS_UNITS: 96.0 / 72.0,
		DEFAULT_SCALE: 'auto',
		UNKNOWN_SCALE: 0,
		MAX_AUTO_SCALE: 1.25,
		SCROLLBAR_PADDING: 40,
		VERTICAL_PADDING: 5,
		FIND_SCROLL_OFFSET_TOP: -50,
		FIND_SCROLL_OFFSET_LEFT: -400,
		IGNORE_CURRENT_POSITION_ON_ZOOM: false,
		DEFAULT_CACHE_SIZE: 10,
		CLEANUP_TIMEOUT: 30000,
		TEXT_LAYER_RENDER_DELAY: 200,
		MAX_TEXT_DIVS_TO_RENDER: 100000,
		THUMBNAIL_SCROLL_MARGIN: -19,
		THUMBNAIL_WIDTH: 98,
		THUMBNAIL_CANVAS_BORDER_WIDTH: 1,
		DEFAULT_PREFERENCES: {
			showPreviousViewOnLoad: true,
			defaultZoomValue: '',
			sidebarViewOnLoad: 0,
			enableHandToolOnLoad: false,
			enableWebGL: false,
			disableRange: false,
			disableStream: false,
			disableAutoFetch: false,
			disableFontFace: false,
			disableTextLayer: false,
			useOnlyCssZoom: false
		}
	};

	this.Enums = {
		SidebarView: {
			NONE: 0,
			THUMBS: 1
		},
		FindStates: {
			FIND_FOUND: 0,
			FIND_NOTFOUND: 1,
			FIND_WRAPPED: 2,
			FIND_PENDING: 3
		},
		RenderingStates: {
			INITIAL: 0,
			RUNNING: 1,
			PAUSED: 2,
			FINISHED: 3
		}
	};

	var DDocViewer = this;

	this.CustomStyle = (function() {

		var prefixes = ['ms', 'Moz', 'Webkit', 'O'];
		var _cache = {};

		function customStyle() {}

		customStyle.getProp = function(propName, element) {
			if (arguments.length === 1 && typeof _cache[propName] === 'string') {
				return _cache[propName];
			}

			element = element || document.documentElement;
			var style = element.style, prefixed, uPropName;

			if (typeof style[propName] === 'string') {
				return (_cache[propName] = propName);
			}

			uPropName = propName.charAt(0).toUpperCase() + propName.slice(1);

			for (var i = 0, l = prefixes.length; i < l; i++) {
				prefixed = prefixes[i] + uPropName;
				if (typeof style[prefixed] === 'string') {
					return (_cache[propName] = prefixed);
				}
			}

			return (_cache[propName] = 'undefined');
		};

		customStyle.setProp = function(propName, element, str) {
			var prop = this.getProp(propName);
			if (prop !== 'undefined') {
				element.style[prop] = str;
			}
		};

		return customStyle;
	})();

	(function mozPrintCallbackPolyfillClosure() {
		if ('mozPrintCallback' in document.createElement('canvas')) {
			return;
		}
		HTMLCanvasElement.prototype.mozPrintCallback = undefined;

		var canvases;
		var index;

		var print = window.print;
		window.print = function() {
			if (canvases) {
				console.warn('Ignored window.print() because of a pending print job.');
				return;
			}
			try {
				dispatchEvent('beforeprint');
			} finally {
				canvases = document.querySelectorAll('canvas');
				index = -1;
				next();
			}
		};

		function dispatchEvent(eventType) {
			var event = document.createEvent('CustomEvent');
			event.initCustomEvent(eventType, false, false, 'custom');
			window.dispatchEvent(event);
		}

		function next() {
			if (!canvases) {
				return;
			}

			renderProgress();
			if (++index < canvases.length) {
				var canvas = canvases[index];
				if (typeof canvas.mozPrintCallback === 'function') {
					canvas.mozPrintCallback({
						context: canvas.getContext('2d'),
						abort: abort,
						done: next
					});
				} else {
					next();
				}
			} else {
				renderProgress();
				print.call(window);
				setTimeout(abort, 20);
			}
		}

		function abort() {
			if (canvases) {
				canvases = null;
				renderProgress();
				dispatchEvent('afterprint');
			}
		}

		function renderProgress() {
			var progressContainer = document.getElementById('mozPrintCallback-shim');
			if (canvases) {
				var progress = Math.round(100 * index / canvases.length);
				var progressBar = progressContainer.querySelector('progress');
				var progressPerc = progressContainer.querySelector('.relative-progress');
				progressBar.value = progress;
				progressPerc.textContent = progress + '%';
				progressContainer.removeAttribute('hidden');
				progressContainer.onclick = abort;
			} else {
				progressContainer.setAttribute('hidden', '');
			}
		}

		var hasAttachEvent = !!document.attachEvent;

		window.addEventListener('keydown', function(event) {
			if (event.keyCode === 80 && (event.ctrlKey || event.metaKey) &&
				!event.altKey && (!event.shiftKey || window.chrome || window.opera)) {
				window.print();
				if (hasAttachEvent) {
					return;
				}
				event.preventDefault();
				if (event.stopImmediatePropagation) {
					event.stopImmediatePropagation();
				} else {
					event.stopPropagation();
				}
				return;
			}
			if (event.keyCode === 27 && canvases) {
				abort();
			}
		}, true);
		if (hasAttachEvent) {
			document.attachEvent('onkeydown', function(event) {
				event = event || window.event;
				if (event.keyCode === 80 && event.ctrlKey) {
					event.keyCode = 0;
					return false;
				}
			});
		}

		if ('onbeforeprint' in window) {
			var stopPropagationIfNeeded = function(event) {
				if (event.detail !== 'custom' && event.stopImmediatePropagation) {
					event.stopImmediatePropagation();
				}
			};
			window.addEventListener('beforeprint', stopPropagationIfNeeded, false);
			window.addEventListener('afterprint', stopPropagationIfNeeded, false);
		}
	})();

	this.DownloadManager = (function() {

		function download(blobUrl, filename) {
			var a = document.createElement('a');
			if (a.click) {
				a.href = blobUrl;
				a.target = '_parent';
				if ('download' in a) {
					a.download = filename;
				}
				(document.body || document.documentElement).appendChild(a);
				a.click();
				a.parentNode.removeChild(a);
			} else {
				if (window.top === window &&
					blobUrl.split('#')[0] === window.location.href.split('#')[0]) {
					var padCharacter = blobUrl.indexOf('?') === -1 ? '?' : '&';
					blobUrl = blobUrl.replace(/#|$/, padCharacter + '$&');
				}
				window.open(blobUrl, '_parent');
			}
		}

		function downloadManager() {}

		downloadManager.prototype = {
			downloadUrl: function(url, filename) {
				if (!PDFJS.isValidUrl(url, true)) {
					return;
				}

				download(url + '#pdfjs.action=download', filename);
			},

			downloadData: function(data, filename, contentType) {
				if (navigator.msSaveBlob) {
					return navigator.msSaveBlob(new Blob([data], { type: contentType }),
						filename);
				}

				var blobUrl = PDFJS.createObjectURL(data, contentType);
				download(blobUrl, filename);
			},

			download: function(blob, url, filename) {
				if (!URL) {
					this.downloadUrl(url, filename);
					return;
				}

				if (navigator.msSaveBlob) {
					if (!navigator.msSaveBlob(blob, filename)) {
						this.downloadUrl(url, filename);
					}
					return;
				}

				var blobUrl = URL.createObjectURL(blob);
				download(blobUrl, filename);
			}
		};

		return downloadManager;
	})();

	//this.CustomPanel = (function () {
	//	function pdfCustomPanel(options) {
	//		this.opened = false;
	//		this.panel = options.panel || null;
	//		this.toggleButton = options.toggleButton || null;

	//		var self = this;

	//		this.toggleButton.addEventListener('click', function () {
	//			self.toggle();
	//		});

	//		this.panel.addEventListener('keydown', function (evt) {
	//			switch (evt.keyCode) {
	//				case 27:
	//					self.close();
	//					break;
	//			}
	//		});

	//		this.panel.addEventListener('mouseleave', function () { setTimeout(function() { self.close(); }, 500); });
	//	}

	//	pdfCustomPanel.prototype = {
	//		open: function () {
	//			if (!this.opened) {
	//				this.opened = true;
	//				var panelId = this.panel.id;
	//				var toggleButton = this.toggleButton;

	//				var xhr = new XMLHttpRequest();
	//				xhr.onreadystatechange = function () {
	//					if (xhr.readyState == 4 && xhr.status == 200) {
	//						var panel = document.getElementById(panelId);
	//						panel.innerHTML = xhr.responseText;
	//						toggleButton.classList.add('toggled');
	//						panel.classList.remove('hidden');
	//					}
	//				};
	//				xhr.open('POST', ddoc.GetUrl('/Custom/ViewerCustomPanel'), true);
	//				xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
	//				xhr.send(JSON.stringify(ddocDocument));
	//			}
	//		},

	//		close: function () {
	//			if (!this.opened) {
	//				return;
	//			}
	//			this.opened = false;
	//			this.toggleButton.classList.remove('toggled');
	//			this.panel.classList.add('hidden');
	//		},

	//		toggle: function () {
	//			if (this.opened) {
	//				this.close();
	//			} else {
	//				this.open();
	//			}
	//		}
	//	};
	//	return pdfCustomPanel;
	//})();

	this.PDFFindBar = (function() {
		function pdfFindBar(options) {
			this.opened = false;
			this.bar = options.bar || null;
			this.toggleButton = options.toggleButton || null;
			this.findField = options.findField || null;
			this.highlightAll = options.highlightAllCheckbox || null;
			this.caseSensitive = options.caseSensitiveCheckbox || null;
			this.findMsg = options.findMsg || null;
			this.findStatusIcon = options.findStatusIcon || null;
			this.findPreviousButton = options.findPreviousButton || null;
			this.findNextButton = options.findNextButton || null;
			this.findController = options.findController || null;

			if (this.findController === null) {
				throw new Error('PDFFindBar cannot be used without a PDFFindController instance.');
			}

			var self = this;
			this.toggleButton.addEventListener('click', function() {
				self.toggle();
			});

			this.findField.addEventListener('input', function() {
				self.dispatchEvent('');
			});

			this.bar.addEventListener('keydown', function(evt) {
				switch (evt.keyCode) {
				case 13:
					if (evt.target === self.findField) {
						self.dispatchEvent('again', evt.shiftKey);
					}
					break;
				case 27:
					self.close();
					break;
				}
			});

			this.findPreviousButton.addEventListener('click', function() {
				self.dispatchEvent('again', true);
			});

			this.findNextButton.addEventListener('click', function() {
				self.dispatchEvent('again', false);
			});

			this.highlightAll.addEventListener('click', function() {
				self.dispatchEvent('highlightallchange');
			});

			this.caseSensitive.addEventListener('click', function() {
				self.dispatchEvent('casesensitivitychange');
			});
		}

		pdfFindBar.prototype = {
			
			dispatchEvent: function(type, findPrev) {
				var event = document.createEvent('CustomEvent');
				event.initCustomEvent('find' + type, true, true, {
					query: document.getElementById('findInput').value || this.findField.value,
					caseSensitive: this.caseSensitive.checked,
					highlightAll: this.highlightAll.checked,
					findPrevious: findPrev
				});
				return window.dispatchEvent(event);
			},

			updateUIState: function(state, previous) {
				var notFound = false;
				var findMsg = '';
				var status = '';

				switch (state) {
				case DDocViewer.Enums.FindStates.FIND_FOUND:
					break;

				case DDocViewer.Enums.FindStates.FIND_PENDING:
					status = 'pending';
					break;

				case DDocViewer.Enums.FindStates.FIND_NOTFOUND:
					findMsg = Resources.find_not_found;
					notFound = true;
					break;

				case DDocViewer.Enums.FindStates.FIND_WRAPPED:
					if (previous) {
						findMsg = Resources.find_reached_top;
					} else {
						findMsg = Resources.find_reached_bottom;
					}
					break;
				}

				if (notFound) {
					this.findField.classList.add('notFound');
				} else {
					this.findField.classList.remove('notFound');
				}

				this.findField.setAttribute('data-status', status);
				this.findMsg.textContent = findMsg;
			},

			open: function() {
				if (!this.opened) {
					this.opened = true;
					this.toggleButton.classList.add('toggled');
					this.bar.classList.remove('hidden');
				}
				this.findField.select();
				this.findField.focus();
			},

			close: function() {
				if (!this.opened) {
					return;
				}
				this.opened = false;
				this.toggleButton.classList.remove('toggled');
				this.bar.classList.add('hidden');
				this.findController.active = false;
			},

			toggle: function() {
				if (this.opened) {
					this.close();
				} else {
					this.open();
				}
			}
		};
		return pdfFindBar;
	})();

	this.PDFFindController = (function() {
		function pdfFindController(options) {
			this.startedTextExtraction = false;
			this.extractTextPromises = [];
			this.pendingFindMatches = {};
			this.active = false;
			this.pageContents = [];
			this.pageMatches = [];
			this.selected = {
				pageIdx: -1,
				matchIdx: -1
			};
			this.offset = {
				pageIdx: null,
				matchIdx: null
			};
			this.pagesToSearch = null;
			this.resumePageIdx = null;
			this.state = null;
			this.dirtyMatch = false;
			this.findTimeout = null;
			this.pdfViewer = options.pdfViewer || null;
			this.integratedFind = options.integratedFind || false;
			this.charactersToNormalize = {
				'\u2018': '\'',
				'\u2019': '\'',
				'\u201A': '\'',
				'\u201B': '\'',
				'\u201C': '"',
				'\u201D': '"',
				'\u201E': '"',
				'\u201F': '"',
				'\u00BC': '1/4',
				'\u00BD': '1/2',
				'\u00BE': '3/4',
				'\u00A0': ' '
			};
			this.findBar = options.findBar || null;

			var replace = Object.keys(this.charactersToNormalize).join('');
			this.normalizationRegex = new RegExp('[' + replace + ']', 'g');

			var events = [
				'find',
				'findagain',
				'findhighlightallchange',
				'findcasesensitivitychange'
			];

			this.firstPagePromise = new Promise(function(resolve) {
				this.resolveFirstPage = resolve;
			}.bind(this));
			this.handleEvent = this.handleEvent.bind(this);

			for (var i = 0, len = events.length; i < len; i++) {
				window.addEventListener(events[i], this.handleEvent);
			}
		}

		pdfFindController.prototype = {
			setFindBar: function(findBar) {
				this.findBar = findBar;
			},

			reset: function() {
				this.startedTextExtraction = false;
				this.extractTextPromises = [];
				this.active = false;
			},

			normalize: function(text) {
				var self = this;
				return text.replace(this.normalizationRegex, function(ch) {
					return self.charactersToNormalize[ch];
				});
			},

			calcFindMatch: function(pageIndex) {
				var pageContent = this.normalize(this.pageContents[pageIndex]);
				var query = this.normalize(this.state.query);
				var caseSensitive = this.state.caseSensitive;
				var queryLen = query.length;

				if (queryLen === 0) {
					return;
				}

				if (!caseSensitive) {
					pageContent = pageContent.toLowerCase();
					query = query.toLowerCase();
				}

				var matches = [];
				var matchIdx = -queryLen;
				while (true) {
					matchIdx = pageContent.indexOf(query, matchIdx + queryLen);
					if (matchIdx === -1) {
						break;
					}
					matches.push(matchIdx);
				}
				this.pageMatches[pageIndex] = matches;
				this.updatePage(pageIndex);
				if (this.resumePageIdx === pageIndex) {
					this.resumePageIdx = null;
					this.nextPageMatch();
				}
			},

			extractText: function() {
				if (this.startedTextExtraction) {
					return;
				}
				this.startedTextExtraction = true;

				this.pageContents = [];
				var extractTextPromisesResolves = [];
				var numPages = this.pdfViewer.pagesCount;
				for (var i = 0; i < numPages; i++) {
					this.extractTextPromises.push(new Promise(function(resolve) {
						extractTextPromisesResolves.push(resolve);
					}));
				}

				var self = this;

				function extractPageText(pageIndex) {
					self.pdfViewer.getPageTextContent(pageIndex).then(
						function textContentResolved(textContent) {
							var textItems = textContent.items;
							var str = [];

							for (var i = 0, len = textItems.length; i < len; i++) {
								str.push(textItems[i].str);
							}

							self.pageContents.push(str.join(''));

							extractTextPromisesResolves[pageIndex](pageIndex);
							if ((pageIndex + 1) < self.pdfViewer.pagesCount) {
								extractPageText(pageIndex + 1);
							}
						}
					);
				}

				extractPageText(0);
			},

			handleEvent: function(e) {
				if (this.state === null || e.type !== 'findagain') {
					this.dirtyMatch = true;
				}
				this.state = e.detail;
				this.updateUIState(DDocViewer.Enums.FindStates.FIND_PENDING);

				this.firstPagePromise.then(function() {
					this.extractText();

					clearTimeout(this.findTimeout);
					if (e.type === 'find') {
						this.findTimeout = setTimeout(this.nextMatch.bind(this), 250);
					} else {
						this.nextMatch();
					}
				}.bind(this));
			},

			updatePage: function(index) {
				if (this.selected.pageIdx === index) {
					this.pdfViewer.scrollPageIntoView(index + 1);
				}

				var page = this.pdfViewer.getPageView(index);
				if (page.textLayer) {
					page.textLayer.updateMatches();
				}
			},

			nextMatch: function() {
				var previous = this.state.findPrevious;
				var currentPageIndex = this.pdfViewer.currentPageNumber - 1;
				var numPages = this.pdfViewer.pagesCount;

				this.active = true;

				if (this.dirtyMatch) {
					this.dirtyMatch = false;
					this.selected.pageIdx = this.selected.matchIdx = -1;
					this.offset.pageIdx = currentPageIndex;
					this.offset.matchIdx = null;
					this.hadMatch = false;
					this.resumePageIdx = null;
					this.pageMatches = [];
					var self = this;

					for (var i = 0; i < numPages; i++) {
						this.updatePage(i);

						if (!(i in this.pendingFindMatches)) {
							this.pendingFindMatches[i] = true;
							this.extractTextPromises[i].then(function(pageIdx) {
								delete self.pendingFindMatches[pageIdx];
								self.calcFindMatch(pageIdx);
							});
						}
					}
				}

				if (this.state.query === '') {
					this.updateUIState(DDocViewer.Enums.FindStates.FIND_FOUND);
					return;
				}

				if (this.resumePageIdx) {
					return;
				}

				var offset = this.offset;
				this.pagesToSearch = numPages;
				if (offset.matchIdx !== null) {
					var numPageMatches = this.pageMatches[offset.pageIdx].length;
					if ((!previous && offset.matchIdx + 1 < numPageMatches) ||
					(previous && offset.matchIdx > 0)) {
						this.hadMatch = true;
						offset.matchIdx = (previous ? offset.matchIdx - 1 :
							offset.matchIdx + 1);
						this.updateMatch(true);
						return;
					}
					this.advanceOffsetPage(previous);
				}
				this.nextPageMatch();
			},

			matchesReady: function(matches) {
				var offset = this.offset;
				var numMatches = matches.length;
				var previous = this.state.findPrevious;

				if (numMatches) {
					this.hadMatch = true;
					offset.matchIdx = (previous ? numMatches - 1 : 0);
					this.updateMatch(true);
					return true;
				} else {
					this.advanceOffsetPage(previous);
					if (offset.wrapped) {
						offset.matchIdx = null;
						if (this.pagesToSearch < 0) {
							this.updateMatch(false);
							return true;
						}
					}
					return false;
				}
			},

			updateMatchPosition: function(
				pageIndex, index, elements, beginIdx, endIdx) {
				if (this.selected.matchIdx === index &&
					this.selected.pageIdx === pageIndex) {
					DDocViewer.scrollIntoView(elements[beginIdx], {
						top: DDocViewer.Constants.FIND_SCROLL_OFFSET_TOP,
						left: DDocViewer.Constants.FIND_SCROLL_OFFSET_LEFT
					});
				}
			},

			nextPageMatch: function() {
				if (this.resumePageIdx !== null) {
					console.error('There can only be one pending page.');
				}
				do {
					var pageIdx = this.offset.pageIdx;
					var matches = this.pageMatches[pageIdx];
					if (!matches) {
						this.resumePageIdx = pageIdx;
						break;
					}
				} while (!this.matchesReady(matches));
			},

			advanceOffsetPage: function(previous) {
				var offset = this.offset;
				var numPages = this.extractTextPromises.length;
				offset.pageIdx = (previous ? offset.pageIdx - 1 : offset.pageIdx + 1);
				offset.matchIdx = null;

				this.pagesToSearch--;

				if (offset.pageIdx >= numPages || offset.pageIdx < 0) {
					offset.pageIdx = (previous ? numPages - 1 : 0);
					offset.wrapped = true;
				}
			},

			updateMatch: function(found) {
				var state = DDocViewer.Enums.FindStates.FIND_NOTFOUND;
				var wrapped = this.offset.wrapped;
				this.offset.wrapped = false;

				if (found) {
					var previousPage = this.selected.pageIdx;
					this.selected.pageIdx = this.offset.pageIdx;
					this.selected.matchIdx = this.offset.matchIdx;
					state = (wrapped ? DDocViewer.Enums.FindStates.FIND_WRAPPED : DDocViewer.Enums.FindStates.FIND_FOUND);
					if (previousPage !== -1 && previousPage !== this.selected.pageIdx) {
						this.updatePage(previousPage);
					}
				}

				this.updateUIState(state, this.state.findPrevious);
				if (this.selected.pageIdx !== -1) {
					this.updatePage(this.selected.pageIdx);
				}
			},

			updateUIState: function(state, previous) {
				if (this.integratedFind) {
					FirefoxCom.request('updateFindControlState',
					{ result: state, findPrevious: previous });
					return;
				}
				if (this.findBar === null) {
					throw new Error('PDFFindController is not initialized with a PDFFindBar instance.');
				}
				this.findBar.updateUIState(state, previous);
			}
		};
		return pdfFindController;
	})();

	this.GrabToPan = (function() {

		function grabToPan(options) {
			this.element = options.element;
			this.document = options.element.ownerDocument;
			if (typeof options.ignoreTarget === 'function') {
				this.ignoreTarget = options.ignoreTarget;
			}
			this.onActiveChanged = options.onActiveChanged;

			this.activate = this.activate.bind(this);
			this.deactivate = this.deactivate.bind(this);
			this.toggle = this.toggle.bind(this);
			this._onmousedown = this._onmousedown.bind(this);
			this._onmousemove = this._onmousemove.bind(this);
			this._endPan = this._endPan.bind(this);

			var overlay = this.overlay = document.createElement('div');
			overlay.className = 'grab-to-pan-grabbing';
		}

		grabToPan.prototype = {
			CSS_CLASS_GRAB: 'grab-to-pan-grab',

			activate: function() {
				if (!this.active) {
					this.active = true;
					this.element.addEventListener('mousedown', this._onmousedown, true);
					this.element.classList.add(this.CSS_CLASS_GRAB);
					if (this.onActiveChanged) {
						this.onActiveChanged(true);
					}
				}
			},

			deactivate: function() {
				if (this.active) {
					this.active = false;
					this.element.removeEventListener('mousedown', this._onmousedown, true);
					this._endPan();
					this.element.classList.remove(this.CSS_CLASS_GRAB);
					if (this.onActiveChanged) {
						this.onActiveChanged(false);
					}
				}
			},

			toggle: function() {
				if (this.active) {
					this.deactivate();
				} else {
					this.activate();
				}
			},

			ignoreTarget: function(node) {
				return node[matchesSelector](
					'a[href], a[href] *, input, textarea, button, button *, select, option'
				);
			},

			_onmousedown: function(event) {
				if (event.button !== 0 || this.ignoreTarget(event.target)) {
					return;
				}
				if (event.originalTarget) {
					try {
						event.originalTarget.tagName;
					} catch (e) {
						return;
					}
				}

				this.scrollLeftStart = this.element.scrollLeft;
				this.scrollTopStart = this.element.scrollTop;
				this.clientXStart = event.clientX;
				this.clientYStart = event.clientY;
				this.document.addEventListener('mousemove', this._onmousemove, true);
				this.document.addEventListener('mouseup', this._endPan, true);
				this.element.addEventListener('scroll', this._endPan, true);
				event.preventDefault();
				event.stopPropagation();
				this.document.documentElement.classList.add(this.CSS_CLASS_GRABBING);

				var focusedElement = document.activeElement;
				if (focusedElement && !focusedElement.contains(event.target)) {
					focusedElement.blur();
				}
			},

			_onmousemove: function(event) {
				this.element.removeEventListener('scroll', this._endPan, true);
				if (isLeftMouseReleased(event)) {
					this._endPan();
					return;
				}
				var xDiff = event.clientX - this.clientXStart;
				var yDiff = event.clientY - this.clientYStart;
				this.element.scrollTop = this.scrollTopStart - yDiff;
				this.element.scrollLeft = this.scrollLeftStart - xDiff;
				if (!this.overlay.parentNode) {
					document.body.appendChild(this.overlay);
				}
			},

			_endPan: function() {
				this.element.removeEventListener('scroll', this._endPan, true);
				this.document.removeEventListener('mousemove', this._onmousemove, true);
				this.document.removeEventListener('mouseup', this._endPan, true);
				if (this.overlay.parentNode) {
					this.overlay.parentNode.removeChild(this.overlay);
				}
			}
		};

		var matchesSelector;
		['webkitM', 'mozM', 'msM', 'oM', 'm'].some(function(prefix) {
			var name = prefix + 'atches';
			if (name in document.documentElement) {
				matchesSelector = name;
			}
			name += 'Selector';
			if (name in document.documentElement) {
				matchesSelector = name;
			}
			return matchesSelector;
		});

		var isNotIEorIsIE10plus = !document.documentMode || document.documentMode > 9;
		var chrome = window.chrome;
		var isChrome15OrOpera15plus = chrome && (chrome.webstore || chrome.app);
		var isSafari6plus = /Apple/.test(navigator.vendor) &&
			/Version\/([6-9]\d*|[1-5]\d+)/.test(navigator.userAgent);

		function isLeftMouseReleased(event) {
			if ('buttons' in event && isNotIEorIsIE10plus) {
				return !(event.buttons | 1);
			}
			if (isChrome15OrOpera15plus || isSafari6plus) {
				return event.which === 0;
			}
		}

		return grabToPan;
	})();

	this.PDFRenderingQueue = (function() {
		function pdfRenderingQueue() {
			this.pdfViewer = null;
			this.pdfThumbnailViewer = null;
			this.onIdle = null;

			this.highestPriorityPage = null;
			this.idleTimeout = null;
			this.printing = false;
			this.isThumbnailViewEnabled = false;
		}

		pdfRenderingQueue.prototype = {
			setViewer: function(pdfViewer) {
				this.pdfViewer = pdfViewer;
			},

			setThumbnailViewer:
				function(pdfThumbnailViewer) {
					this.pdfThumbnailViewer = pdfThumbnailViewer;
				},

			isHighestPriority: function(view) {
				return this.highestPriorityPage === view.renderingId;
			},

			renderHighestPriority: function
			(currentlyVisiblePages) {
				if (this.idleTimeout) {
					clearTimeout(this.idleTimeout);
					this.idleTimeout = null;
				}

				if (this.pdfViewer.forceRendering(currentlyVisiblePages)) {
					return;
				}
				if (this.pdfThumbnailViewer && this.isThumbnailViewEnabled) {
					if (this.pdfThumbnailViewer.forceRendering()) {
						return;
					}
				}

				if (this.printing) {
					return;
				}

				if (this.onIdle) {
					this.idleTimeout = setTimeout(this.onIdle.bind(this), DDocViewer.Constants.CLEANUP_TIMEOUT);
				}
			},

			getHighestPriority: function
			(visible, views, scrolledDown) {
				var visibleViews = visible.views;

				var numVisible = visibleViews.length;
				if (numVisible === 0) {
					return false;
				}
				for (var i = 0; i < numVisible; ++i) {
					var view = visibleViews[i].view;
					if (!this.isViewFinished(view)) {
						return view;
					}
				}

				if (scrolledDown) {
					var nextPageIndex = visible.last.id;
					if (views[nextPageIndex] &&
						!this.isViewFinished(views[nextPageIndex])) {
						return views[nextPageIndex];
					}
				} else {
					var previousPageIndex = visible.first.id - 2;
					if (views[previousPageIndex] &&
						!this.isViewFinished(views[previousPageIndex])) {
						return views[previousPageIndex];
					}
				}
				return null;
			},

			isViewFinished: function(view) {
				return view.renderingState === DDocViewer.Enums.RenderingStates.FINISHED;
			},

			renderView: function(view) {
				var state = view.renderingState;
				switch (state) {
				case DDocViewer.Enums.RenderingStates.FINISHED:
					return false;
				case DDocViewer.Enums.RenderingStates.PAUSED:
					this.highestPriorityPage = view.renderingId;
					view.resume();
					break;
				case DDocViewer.Enums.RenderingStates.RUNNING:
					this.highestPriorityPage = view.renderingId;
					break;
				case DDocViewer.Enums.RenderingStates.INITIAL:
					this.highestPriorityPage = view.renderingId;
					var continueRendering = function() {
						this.renderHighestPriority();
					}.bind(this);
					view.draw().then(continueRendering, continueRendering);
					break;
				}
				return true;
			},
		};

		return pdfRenderingQueue;
	})();

	this.PDFPageView = (function() {
		function pdfPageView(options) {
			var container = options.container;
			var id = options.id;
			var scale = options.scale;
			var defaultViewport = options.defaultViewport;
			var renderingQueue = options.renderingQueue;
			var textLayerFactory = options.textLayerFactory;
			var annotationsLayerFactory = options.annotationsLayerFactory;

			this.id = id;
			//ID canva
			this.renderingId = 'page' + id;

			this.rotation = 0;
			this.scale = scale || 1.0;
			this.viewport = defaultViewport;
			this.pdfPageRotate = defaultViewport.rotation;
			this.hasRestrictedScaling = false;

			this.renderingQueue = renderingQueue;
			this.textLayerFactory = textLayerFactory;
			this.annotationsLayerFactory = annotationsLayerFactory;

			this.renderingState = DDocViewer.Enums.RenderingStates.INITIAL;
			this.resume = null;

			this.onBeforeDraw = null;
			this.onAfterDraw = null;

			this.textLayer = null;

			this.zoomLayer = null;

			this.annotationLayer = null;
			//Contenedor
			var div = document.createElement('div');
			div.id = 'pageContainer' + this.id;
			div.className = 'page';
			div.style.width = Math.floor(this.viewport.width) + 'px';
			div.style.height = Math.floor(this.viewport.height) + 'px';
			div.setAttribute('data-page-number', this.id);
			this.div = div;

			container.appendChild(div);
		}

		pdfPageView.prototype = {
			setPdfPage: function(pdfPage) {
				this.pdfPage = pdfPage;
				this.pdfPageRotate = pdfPage.rotate;
				var totalRotation = (this.rotation + this.pdfPageRotate) % 360;
				this.viewport = pdfPage.getViewport(this.scale * DDocViewer.Constants.CSS_UNITS,
					totalRotation);
				this.stats = pdfPage.stats;
				this.reset();
			},

			destroy: function() {
				this.zoomLayer = null;
				this.reset();
				if (this.pdfPage) {
					this.pdfPage.destroy();
				}
			},

			reset: function(keepAnnotations) {
				if (this.renderTask) {
					this.renderTask.cancel();
				}
				this.resume = null;
				this.renderingState = DDocViewer.Enums.RenderingStates.INITIAL;

				var div = this.div;
				div.style.width = Math.floor(this.viewport.width) + 'px';
				div.style.height = Math.floor(this.viewport.height) + 'px';

				var childNodes = div.childNodes;
				var currentZoomLayer = this.zoomLayer || null;
				var currentAnnotationNode = (keepAnnotations && this.annotationLayer &&
					this.annotationLayer.div) || null;
				for (var i = childNodes.length - 1; i >= 0; i--) {
					var node = childNodes[i];
					if (currentZoomLayer === node || currentAnnotationNode === node) {
						continue;
					}
					div.removeChild(node);
				}
				div.removeAttribute('data-loaded');

				if (keepAnnotations) {
					if (this.annotationLayer) {
						this.annotationLayer.hide();
					}
				} else {
					this.annotationLayer = null;
				}

				if (this.canvas) {
					this.canvas.width = 0;
					this.canvas.height = 0;
					delete this.canvas;
				}

				this.loadingIconDiv = document.createElement('div');
				this.loadingIconDiv.className = 'loadingIcon';
				div.appendChild(this.loadingIconDiv);
			},

			update: function(scale, rotation) {
				this.scale = scale || this.scale;

				if (typeof rotation !== 'undefined') {
					this.rotation = rotation;
				}

				var totalRotation = (this.rotation + this.pdfPageRotate) % 360;
				this.viewport = this.viewport.clone({
					scale: this.scale * DDocViewer.Constants.CSS_UNITS,
					rotation: totalRotation
				});

				var isScalingRestricted = false;
				if (this.canvas && PDFJS.maxCanvasPixels > 0) {
					var ctx = this.canvas.getContext('2d');
					var outputScale = DDocViewer.getOutputScale(ctx);
					var pixelsInViewport = this.viewport.width * this.viewport.height;
					var maxScale = Math.sqrt(PDFJS.maxCanvasPixels / pixelsInViewport);
					if (((Math.floor(this.viewport.width) * outputScale.sx) | 0) *
						((Math.floor(this.viewport.height) * outputScale.sy) | 0) >
						PDFJS.maxCanvasPixels) {
						isScalingRestricted = true;
					}
				}

				if (this.canvas &&
				(PDFJS.useOnlyCssZoom ||
				(this.hasRestrictedScaling && isScalingRestricted))) {
					this.cssTransform(this.canvas, true);
					return;
				} else if (this.canvas && !this.zoomLayer) {
					this.zoomLayer = this.canvas.parentNode;
					this.zoomLayer.style.position = 'absolute';
				}
				if (this.zoomLayer) {
					this.cssTransform(this.zoomLayer.firstChild);
				}
				this.reset(true);
			},

			updatePosition: function() {
				if (this.textLayer) {
					this.textLayer.render(DDocViewer.Constants.TEXT_LAYER_RENDER_DELAY);
				}
			},

			cssTransform: function(canvas, redrawAnnotations) {
				var width = this.viewport.width;
				var height = this.viewport.height;
				var div = this.div;
				canvas.style.width = canvas.parentNode.style.width = div.style.width =
					Math.floor(width) + 'px';
				canvas.style.height = canvas.parentNode.style.height = div.style.height =
					Math.floor(height) + 'px';
				var relativeRotation = this.viewport.rotation - canvas._viewport.rotation;
				var absRotation = Math.abs(relativeRotation);
				var scaleX = 1, scaleY = 1;
				if (absRotation === 90 || absRotation === 270) {
					scaleX = height / width;
					scaleY = width / height;
				}
				var cssTransform = 'rotate(' + relativeRotation + 'deg) ' +
					'scale(' + scaleX + ',' + scaleY + ')';
				DDocViewer.CustomStyle.setProp('transform', canvas, cssTransform);

				if (this.textLayer) {
					var textLayerViewport = this.textLayer.viewport;
					var textRelativeRotation = this.viewport.rotation -
						textLayerViewport.rotation;
					var textAbsRotation = Math.abs(textRelativeRotation);
					var scale = width / textLayerViewport.width;
					if (textAbsRotation === 90 || textAbsRotation === 270) {
						scale = width / textLayerViewport.height;
					}
					var textLayerDiv = this.textLayer.textLayerDiv;
					var transX, transY;
					switch (textAbsRotation) {
					case 0:
						transX = transY = 0;
						break;
					case 90:
						transX = 0;
						transY = '-' + textLayerDiv.style.height;
						break;
					case 180:
						transX = '-' + textLayerDiv.style.width;
						transY = '-' + textLayerDiv.style.height;
						break;
					case 270:
						transX = '-' + textLayerDiv.style.width;
						transY = 0;
						break;
					default:
						console.error('Bad rotation value.');
						break;
					}
					DDocViewer.CustomStyle.setProp('transform', textLayerDiv,
						'rotate(' + textAbsRotation + 'deg) ' +
						'scale(' + scale + ', ' + scale + ') ' +
						'translate(' + transX + ', ' + transY + ')');
					DDocViewer.CustomStyle.setProp('transformOrigin', textLayerDiv, '0% 0%');
				}

				if (redrawAnnotations && this.annotationLayer) {
					this.annotationLayer.setupAnnotations(this.viewport);
				}
			},

			get width() {
				return this.viewport.width;
			},

			get height() {
				return this.viewport.height;
			},

			getPagePoint: function(x, y) {
				return this.viewport.convertToPdfPoint(x, y);
			},

			draw: function() {
				if (this.renderingState !== DDocViewer.Enums.RenderingStates.INITIAL) {
					console.error('Must be in new state before drawing');
				}

				this.renderingState = DDocViewer.Enums.RenderingStates.RUNNING;

				var pdfPage = this.pdfPage;
				var viewport = this.viewport;
				var div = this.div;

				var canvasWrapper = document.createElement('div');
				canvasWrapper.style.width = div.style.width;
				canvasWrapper.style.height = div.style.height;
				canvasWrapper.classList.add('canvasWrapper');
				//CREA CANVA
				var canvas = document.createElement('canvas');
				canvas.id = 'page' + this.id;
				canvasWrapper.appendChild(canvas);
				if (this.annotationLayer) {
					div.insertBefore(canvasWrapper, this.annotationLayer.div);
				} else {
					div.appendChild(canvasWrapper);
				}
				this.canvas = canvas;

				var ctx = canvas.getContext('2d');
				var outputScale = DDocViewer.getOutputScale(ctx);

				if (PDFJS.useOnlyCssZoom) {
					var actualSizeViewport = viewport.clone({ scale: DDocViewer.Constants.CSS_UNITS });
					outputScale.sx *= actualSizeViewport.width / viewport.width;
					outputScale.sy *= actualSizeViewport.height / viewport.height;
					outputScale.scaled = true;
				}

				if (PDFJS.maxCanvasPixels > 0) {
					var pixelsInViewport = viewport.width * viewport.height;
					var maxScale = Math.sqrt(PDFJS.maxCanvasPixels / pixelsInViewport);
					if (outputScale.sx > maxScale || outputScale.sy > maxScale) {
						outputScale.sx = maxScale;
						outputScale.sy = maxScale;
						outputScale.scaled = true;
						this.hasRestrictedScaling = true;
					} else {
						this.hasRestrictedScaling = false;
					}
				}

				canvas.width = (Math.floor(viewport.width) * outputScale.sx) | 0;
				canvas.height = (Math.floor(viewport.height) * outputScale.sy) | 0;
				canvas.style.width = Math.floor(viewport.width) + 'px';
				canvas.style.height = Math.floor(viewport.height) + 'px';
				canvas._viewport = viewport;

				//TXTLayer div
				var textLayerDiv = null;
				var textLayer = null;
				if (this.textLayerFactory) {
					textLayerDiv = document.createElement('div');
					textLayerDiv.className = 'textLayer';
					textLayerDiv.id = 'TXTLayer';
					textLayerDiv.style.width = canvas.style.width;
					textLayerDiv.style.height = canvas.style.height;
					if (this.annotationLayer) {
						div.insertBefore(textLayerDiv, this.annotationLayer.div);
					} else {
						div.appendChild(textLayerDiv);
					}

					textLayer = this.textLayerFactory.createTextLayerBuilder(textLayerDiv,
						this.id - 1,
						this.viewport);
				}
				this.textLayer = textLayer;

				if (outputScale.scaled) {
					ctx._transformMatrix = [outputScale.sx, 0, 0, outputScale.sy, 0, 0];
					ctx.scale(outputScale.sx, outputScale.sy);
				}

				var resolveRenderPromise, rejectRenderPromise;
				var promise = new Promise(function(resolve, reject) {
					resolveRenderPromise = resolve;
					rejectRenderPromise = reject;
				});

				var self = this;

				function pageViewDrawCallback(error) {
					if (renderTask === self.renderTask) {
						self.renderTask = null;
					}

					if (error === 'cancelled') {
						rejectRenderPromise(error);
						return;
					}

					self.renderingState = DDocViewer.Enums.RenderingStates.FINISHED;

					if (self.loadingIconDiv) {
						div.removeChild(self.loadingIconDiv);
						delete self.loadingIconDiv;
					}

					if (self.zoomLayer) {
						div.removeChild(self.zoomLayer);
						self.zoomLayer = null;
					}

					self.error = error;
					self.stats = pdfPage.stats;
					if (self.onAfterDraw) {
						self.onAfterDraw();
					}
					var event = document.createEvent('CustomEvent');
					event.initCustomEvent('pagerendered', true, true, {
						pageNumber: self.id
					});
					div.dispatchEvent(event);
					var deprecatedEvent = document.createEvent('CustomEvent');
					deprecatedEvent.initCustomEvent('pagerender', true, true, {
						pageNumber: pdfPage.pageNumber
					});
					div.dispatchEvent(deprecatedEvent);

					if (!error) {
						resolveRenderPromise(undefined);
					} else {
						rejectRenderPromise(error);
					}
				}

				var renderContinueCallback = null;
				if (this.renderingQueue) {
					renderContinueCallback = function(cont) {
						if (!self.renderingQueue.isHighestPriority(self)) {
							self.renderingState = DDocViewer.Enums.RenderingStates.PAUSED;
							self.resume = function() {
								self.renderingState = DDocViewer.Enums.RenderingStates.RUNNING;
								cont();
							};
							return;
						}
						cont();
					};
				}

				var renderContext = {
					canvasContext: ctx,
					viewport: this.viewport,
					continueCallback: renderContinueCallback
				};
				var renderTask = this.renderTask = this.pdfPage.render(renderContext);

				this.renderTask.promise.then(
					function pdfPageRenderCallback() {
						pageViewDrawCallback(null);
						if (textLayer) {
							self.pdfPage.getTextContent().then(
								function textContentResolved(textContent) {
									textLayer.setTextContent(textContent);
									textLayer.render(DDocViewer.Constants.TEXT_LAYER_RENDER_DELAY);
								}
							);
						}
					},
					function pdfPageRenderError(error) {
						pageViewDrawCallback(error);
					}
				);

				if (this.annotationsLayerFactory) {
					if (!this.annotationLayer) {
						this.annotationLayer = this.annotationsLayerFactory.
							createAnnotationsLayerBuilder(div, this.pdfPage);
					}
					this.annotationLayer.setupAnnotations(this.viewport);
				}
				div.setAttribute('data-loaded', true);

				if (self.onBeforeDraw) {
					self.onBeforeDraw();
				}
				return promise;
			},

			beforePrint: function() {
				var pdfPage = this.pdfPage;

				var viewport = pdfPage.getViewport(1);
				var PRINT_OUTPUT_SCALE = 2;
				//CREA CANVA!!
				var canvas = document.createElement('canvas');
				canvas.width = Math.floor(viewport.width) * PRINT_OUTPUT_SCALE;
				canvas.height = Math.floor(viewport.height) * PRINT_OUTPUT_SCALE;
				canvas.style.width = (PRINT_OUTPUT_SCALE * viewport.width) + 'pt';
				canvas.style.height = (PRINT_OUTPUT_SCALE * viewport.height) + 'pt';
				var cssScale = 'scale(' + (1 / PRINT_OUTPUT_SCALE) + ', ' +
				(1 / PRINT_OUTPUT_SCALE) + ')';
				DDocViewer.CustomStyle.setProp('transform', canvas, cssScale);
				DDocViewer.CustomStyle.setProp('transformOrigin', canvas, '0% 0%');
				//CREA DIV CANVA
				var printContainer = document.getElementById('printContainer');
				var canvasWrapper = document.createElement('div');
				canvasWrapper.style.width = viewport.width + 'pt';
				canvasWrapper.style.height = viewport.height + 'pt';
				canvasWrapper.appendChild(canvas);
				canvasWrapper.id = 'CanvaDIV';
				printContainer.appendChild(canvasWrapper);

				canvas.mozPrintCallback = function(obj) {
					var ctx = obj.context;

					ctx.save();
					ctx.fillStyle = 'rgb(255, 255, 255)';
					ctx.fillRect(0, 0, canvas.width, canvas.height);
					ctx.restore();
					ctx._transformMatrix =
					[PRINT_OUTPUT_SCALE, 0, 0, PRINT_OUTPUT_SCALE, 0, 0];
					ctx.scale(PRINT_OUTPUT_SCALE, PRINT_OUTPUT_SCALE);

					var renderContext = {
						canvasContext: ctx,
						viewport: viewport,
						intent: 'print'
					};

					pdfPage.render(renderContext).promise.then(function() {
						obj.done();
					}, function(error) {
						console.error(error);
						if ('abort' in obj) {
							obj.abort();
						} else {
							obj.done();
						}
					});
				};
			},
		};

		return pdfPageView;
	})();

	this.TextLayerBuilder = (function() {
		function textLayerBuilder(options) {
			this.textLayerDiv = options.textLayerDiv;
			this.renderingDone = false;
			this.divContentDone = false;
			this.pageIdx = options.pageIndex;
			this.pageNumber = this.pageIdx + 1;
			this.matches = [];
			this.viewport = options.viewport;
			this.textDivs = [];
			this.findController = options.findController || null;
		}

		textLayerBuilder.prototype = {
			_finishRendering: function() {
				this.renderingDone = true;

				var event = document.createEvent('CustomEvent');
				event.initCustomEvent('textlayerrendered', true, true, {
					pageNumber: this.pageNumber
				});
				this.textLayerDiv.dispatchEvent(event);
			},

			renderLayer: function() {
				var textLayerFrag = document.createDocumentFragment();
				var textDivs = this.textDivs;
				var textDivsLength = textDivs.length;
				var canvas = document.createElement('canvas');
				var ctx = canvas.getContext('2d');

				if (textDivsLength > DDocViewer.Constants.MAX_TEXT_DIVS_TO_RENDER) {
					this._finishRendering();
					return;
				}

				var lastFontSize;
				var lastFontFamily;
				for (var i = 0; i < textDivsLength; i++) {
					var textDiv = textDivs[i];
					if (textDiv.dataset.isWhitespace !== undefined) {
						continue;
					}

					var fontSize = textDiv.style.fontSize;
					var fontFamily = textDiv.style.fontFamily;

					if (fontSize !== lastFontSize || fontFamily !== lastFontFamily) {
						ctx.font = fontSize + ' ' + fontFamily;
						lastFontSize = fontSize;
						lastFontFamily = fontFamily;
					}

					var width = ctx.measureText(textDiv.textContent).width;
					if (width > 0) {
						textLayerFrag.appendChild(textDiv);
						var transform;
						if (textDiv.dataset.canvasWidth !== undefined) {
							var textScale = textDiv.dataset.canvasWidth / width;
							transform = 'scaleX(' + textScale + ')';
						} else {
							transform = '';
						}
						var rotation = textDiv.dataset.angle;
						if (rotation) {
							transform = 'rotate(' + rotation + 'deg) ' + transform;
						}
						if (transform) {
							DDocViewer.CustomStyle.setProp('transform', textDiv, transform);
						}
					}
				}

				this.textLayerDiv.appendChild(textLayerFrag);
				this._finishRendering();
				this.updateMatches();
			},

			render: function(timeout) {
				if (!this.divContentDone || this.renderingDone) {
					return;
				}

				if (this.renderTimer) {
					clearTimeout(this.renderTimer);
					this.renderTimer = null;
				}

				if (!timeout) {
					this.renderLayer();
				} else {
					var self = this;
					this.renderTimer = setTimeout(function() {
						self.renderLayer();
						self.renderTimer = null;
					}, timeout);
				}
			},

			appendText: function(geom, styles) {
				var style = styles[geom.fontName];
				var textDiv = document.createElement('div');
				this.textDivs.push(textDiv);
				if (DDocViewer.isAllWhitespace(geom.str)) {
					textDiv.dataset.isWhitespace = true;
					return;
				}
				var tx = PDFJS.Util.transform(this.viewport.transform, geom.transform);
				var angle = Math.atan2(tx[1], tx[0]);
				if (style.vertical) {
					angle += Math.PI / 2;
				}
				var fontHeight = Math.sqrt((tx[2] * tx[2]) + (tx[3] * tx[3]));
				var fontAscent = fontHeight;
				if (style.ascent) {
					fontAscent = style.ascent * fontAscent;
				} else if (style.descent) {
					fontAscent = (1 + style.descent) * fontAscent;
				}

				var left;
				var top;
				if (angle === 0) {
					left = tx[4];
					top = tx[5] - fontAscent;
				} else {
					left = tx[4] + (fontAscent * Math.sin(angle));
					top = tx[5] - (fontAscent * Math.cos(angle));
				}
				textDiv.style.left = left + 'px';
				textDiv.style.top = top + 'px';
				textDiv.style.fontSize = fontHeight + 'px';
				textDiv.style.fontFamily = style.fontFamily;

				textDiv.textContent = geom.str

				//code to detect some coincidence and mark it
				if (textDiv.textContent == "NOMBRE") {
					textDiv.style.backgroundColor = 'yellow';
                }

				if (angle !== 0) {
					textDiv.dataset.angle = angle * (180 / Math.PI);
				}

				if (textDiv.textContent.length > 1) {
					if (style.vertical) {
						textDiv.dataset.canvasWidth = geom.height * this.viewport.scale;
					} else {
						textDiv.dataset.canvasWidth = geom.width * this.viewport.scale;
					}
				}
			},

			setTextContent: function(textContent) {
				this.textContent = textContent;

				var textItems = textContent.items;
				for (var i = 0, len = textItems.length; i < len; i++) {
					this.appendText(textItems[i], textContent.styles);
				}
				this.divContentDone = true;
			},

			convertMatches: function(matches) {
				var i = 0;
				var iIndex = 0;
				var bidiTexts = this.textContent.items;
				var end = bidiTexts.length - 1;
				var queryLen = (this.findController === null ?
					0 : this.findController.state.query.length);
				var ret = [];

				for (var m = 0, len = matches.length; m < len; m++) {
					var matchIdx = matches[m];

					while (i !== end && matchIdx >= (iIndex + bidiTexts[i].str.length)) {
						iIndex += bidiTexts[i].str.length;
						i++;
					}

					if (i === bidiTexts.length) {
						console.error('Could not find a matching mapping');
					}

					var match = {
						begin: {
							divIdx: i,
							offset: matchIdx - iIndex
						}
					};

					matchIdx += queryLen;

					while (i !== end && matchIdx > (iIndex + bidiTexts[i].str.length)) {
						iIndex += bidiTexts[i].str.length;
						i++;
					}

					match.end = {
						divIdx: i,
						offset: matchIdx - iIndex
					};
					ret.push(match);
				}

				return ret;
			},

			renderMatches: function(matches) {
				if (matches.length === 0) {
					return;
				}

				var bidiTexts = this.textContent.items;
				var textDivs = this.textDivs;
				var prevEnd = null;
				var pageIdx = this.pageIdx;
				var isSelectedPage = (this.findController === null ?
					false : (pageIdx === this.findController.selected.pageIdx));
				var selectedMatchIdx = (this.findController === null ?
					-1 : this.findController.selected.matchIdx);
				var highlightAll = (this.findController === null ?
					false : this.findController.state.highlightAll);
				var infinity = {
					divIdx: -1,
					offset: undefined
				};

				function beginText(begin, className) {
					var divIdx = begin.divIdx;
					textDivs[divIdx].textContent = '';
					appendTextToDiv(divIdx, 0, begin.offset, className);
				}

				function appendTextToDiv(divIdx, fromOffset, toOffset, className) {
					var div = textDivs[divIdx];
					var content = bidiTexts[divIdx].str.substring(fromOffset, toOffset);
					var node = document.createTextNode(content);
					if (className) {
						var span = document.createElement('span');
						span.className = className;
						span.appendChild(node);
						div.appendChild(span);
						return;
					}
					div.appendChild(node);
				}

				var i0 = selectedMatchIdx, i1 = i0 + 1;
				if (highlightAll) {
					i0 = 0;
					i1 = matches.length;
				} else if (!isSelectedPage) {
					return;
				}

				for (var i = i0; i < i1; i++) {
					var match = matches[i];
					var begin = match.begin;
					var end = match.end;
					var isSelected = (isSelectedPage && i === selectedMatchIdx);
					var highlightSuffix = (isSelected ? ' selected' : '');

					if (this.findController) {
						this.findController.updateMatchPosition(pageIdx, i, textDivs,
							begin.divIdx, end.divIdx);
					}

					if (!prevEnd || begin.divIdx !== prevEnd.divIdx) {
						if (prevEnd !== null) {
							appendTextToDiv(prevEnd.divIdx, prevEnd.offset, infinity.offset);
						}
						beginText(begin);
					} else {
						appendTextToDiv(prevEnd.divIdx, prevEnd.offset, begin.offset);
					}

					if (begin.divIdx === end.divIdx) {
						appendTextToDiv(begin.divIdx, begin.offset, end.offset,
							'highlight' + highlightSuffix);
					} else {
						appendTextToDiv(begin.divIdx, begin.offset, infinity.offset,
							'highlight begin' + highlightSuffix);
						for (var n0 = begin.divIdx + 1, n1 = end.divIdx; n0 < n1; n0++) {
							textDivs[n0].className = 'highlight middle' + highlightSuffix;
						}
						beginText(end, 'highlight end' + highlightSuffix);
					}
					prevEnd = end;
				}

				if (prevEnd) {
					appendTextToDiv(prevEnd.divIdx, prevEnd.offset, infinity.offset);
				}
			},

			updateMatches: function() {
				if (!this.renderingDone) {
					return;
				}

				var matches = this.matches;
				var textDivs = this.textDivs;
				var bidiTexts = this.textContent.items;
				var clearedUntilDivIdx = -1;

				for (var i = 0, len = matches.length; i < len; i++) {
					var match = matches[i];
					var begin = Math.max(clearedUntilDivIdx, match.begin.divIdx);
					for (var n = begin, end = match.end.divIdx; n <= end; n++) {
						var div = textDivs[n];
						div.textContent = bidiTexts[n].str;
						div.className = '';
					}
					clearedUntilDivIdx = match.end.divIdx + 1;
				}

				if (this.findController === null || !this.findController.active) {
					return;
				}

				this.matches = this.convertMatches(this.findController === null ?
				[] : (this.findController.pageMatches[this.pageIdx] || []));
				this.renderMatches(this.matches);
			}
		};
		return textLayerBuilder;
	})();

	this.DefaultTextLayerFactory = function() {};

	this.DefaultTextLayerFactory.prototype = {
		createTextLayerBuilder: function(textLayerDiv, pageIndex, viewport) {
			return new DDocViewer.TextLayerBuilder({
				textLayerDiv: textLayerDiv,
				pageIndex: pageIndex,
				viewport: viewport
			});
		}
	};

	this.AnnotationsLayerBuilder = (function() {
		function annotationsLayerBuilder(options) {
			this.pageDiv = options.pageDiv;
			this.pdfPage = options.pdfPage;
			this.linkService = options.linkService;

			this.div = null;
		}

		annotationsLayerBuilder.prototype = {
			setupAnnotations:
				function(viewport) {
					function bindLink(link, dest) {
						link.href = linkService.getDestinationHash(dest);
						link.onclick = function() {
							if (dest) {
								linkService.navigateTo(dest);
							}
							return false;
						};
						if (dest) {
							link.className = 'internalLink';
						}
					}

					function bindNamedAction(link, action) {
						link.href = linkService.getAnchorUrl('');
						link.onclick = function() {
							linkService.executeNamedAction(action);
							return false;
						};
						link.className = 'internalLink';
					}

					var linkService = this.linkService;
					var pdfPage = this.pdfPage;
					var self = this;

					pdfPage.getAnnotations().then(function(annotationsData) {
						viewport = viewport.clone({ dontFlip: true });
						var transform = viewport.transform;
						var transformStr = 'matrix(' + transform.join(',') + ')';
						var data, element, i, ii;

						if (self.div) {
							for (i = 0, ii = annotationsData.length; i < ii; i++) {
								data = annotationsData[i];
								element = self.div.querySelector(
									'[data-annotation-id="' + data.id + '"]');
								if (element) {
									DDocViewer.CustomStyle.setProp('transform', element, transformStr);
								}
							}
							self.div.removeAttribute('hidden');
						} else {
							for (i = 0, ii = annotationsData.length; i < ii; i++) {
								data = annotationsData[i];
								if (!data || !data.hasHtml) {
									continue;
								}

								element = PDFJS.AnnotationUtils.getHtmlElement(data,
									pdfPage.commonObjs);
								element.setAttribute('data-annotation-id', data.id);

								var rect = data.rect;
								var view = pdfPage.view;
								rect = PDFJS.Util.normalizeRect([
									rect[0],
									view[3] - rect[1] + view[1],
									rect[2],
									view[3] - rect[3] + view[1]
								]);
								element.style.left = rect[0] + 'px';
								element.style.top = rect[1] + 'px';
								element.style.position = 'absolute';

								DDocViewer.CustomStyle.setProp('transform', element, transformStr);
								var transformOriginStr = -rect[0] + 'px ' + -rect[1] + 'px';
								DDocViewer.CustomStyle.setProp('transformOrigin', element, transformOriginStr);

								if (data.subtype === 'Link' && !data.url) {
									var link = element.getElementsByTagName('a')[0];
									if (link) {
										if (data.action) {
											bindNamedAction(link, data.action);
										} else {
											bindLink(link, ('dest' in data) ? data.dest : null);
										}
									}
								}

								if (!self.div) {
									var annotationLayerDiv = document.createElement('div');
									annotationLayerDiv.className = 'annotationLayer';
									self.pageDiv.appendChild(annotationLayerDiv);
									self.div = annotationLayerDiv;
								}

								self.div.appendChild(element);
							}
						}
					});
				},

			hide: function() {
				if (!this.div) {
					return;
				}
				this.div.setAttribute('hidden', 'true');
			}
		};
		return annotationsLayerBuilder;
	})();

	this.DefaultAnnotationsLayerFactory = function() {};
	this.DefaultAnnotationsLayerFactory.prototype = {
		createAnnotationsLayerBuilder: function(pageDiv, pdfPage) {
			return new DDocViewer.AnnotationsLayerBuilder({
				pageDiv: pageDiv,
				pdfPage: pdfPage
			});
		}
	};

	this.PDFViewer = (function() {
		function pdfPageViewBuffer(size) {
			var data = [];
			this.push = function(view) {
				var i = data.indexOf(view);
				if (i >= 0) {
					data.splice(i, 1);
				}
				data.push(view);
				if (data.length > size) {
					data.shift().destroy();
				}
			};
			this.resize = function(newSize) {
				size = newSize;
				while (data.length > size) {
					data.shift().destroy();
				}
			};
		}

		function pdfViewer(options) {
			this.container = options.container;
			this.viewer = options.viewer || options.container.firstElementChild;
			this.linkService = options.linkService || new DDocViewer.SimpleLinkService(this);
			this.removePageBorders = options.removePageBorders || false;

			this.defaultRenderingQueue = !options.renderingQueue;
			if (this.defaultRenderingQueue) {
				this.renderingQueue = new DDocViewer.PDFRenderingQueue();
				this.renderingQueue.setViewer(this);
			} else {
				this.renderingQueue = options.renderingQueue;
			}

			this.scroll = DDocViewer.watchScroll(this.container, this._scrollUpdate.bind(this));
			this.updateInProgress = false;
			this._resetView();

			if (this.removePageBorders) {
				this.viewer.classList.add('removePageBorders');
			}
		}

		pdfViewer.prototype = {
			get pagesCount() {
				return this.pages.length;
			},

			getPageView: function(index) {
				return this.pages[index];
			},

			get currentPageNumber() {
				return this._currentPageNumber;
			},

			set currentPageNumber(val) {
				if (!this.pdfDocument) {
					this._currentPageNumber = val;
					return;
				}

				var event = document.createEvent('UIEvents');
				event.initUIEvent('pagechange', true, true, window, 0);
				event.updateInProgress = this.updateInProgress;

				if (!(0 < val && val <= this.pagesCount)) {
					event.pageNumber = this._currentPageNumber;
					event.previousPageNumber = val;
					this.container.dispatchEvent(event);
					return;
				}

				event.previousPageNumber = this._currentPageNumber;
				this._currentPageNumber = val;
				event.pageNumber = val;
				this.container.dispatchEvent(event);
			},

			get currentScale() {
				return this._currentScale;
			},

			set currentScale(val) {
				if (isNaN(val)) {
					throw new Error('Invalid numeric scale');
				}
				if (!this.pdfDocument) {
					this._currentScale = val;
					this._currentScaleValue = val.toString();
					return;
				}
				this._setScale(val, false);
			},

			get currentScaleValue() {
				return this._currentScaleValue;
			},

			set currentScaleValue(val) {
				if (!this.pdfDocument) {
					this._currentScale = isNaN(val) ? DDocViewer.Constants.UNKNOWN_SCALE : val;
					this._currentScaleValue = val;
					return;
				}
				this._setScale(val, false);
			},

			get pagesRotation() {
				return this._pagesRotation;
			},

			set pagesRotation(rotation) {
				this._pagesRotation = rotation;

				for (var i = 0, l = this.pages.length; i < l; i++) {
					var page = this.pages[i];
					page.update(page.scale, rotation);
				}

				this._setScale(this._currentScaleValue, true);
			},

			setDocument: function(pdfDocument) {
				if (this.pdfDocument) {
					this._resetView();
				}

				this.pdfDocument = pdfDocument;
				if (!pdfDocument) {
					return;
				}

				var pagesCount = pdfDocument.numPages;
				var pagesRefMap = this.pagesRefMap = {};
				var self = this;

				var resolvePagesPromise;
				var pagesPromise = new Promise(function(resolve) {
					resolvePagesPromise = resolve;
				});
				this.pagesPromise = pagesPromise;
				pagesPromise.then(function() {
					var event = document.createEvent('CustomEvent');
					event.initCustomEvent('pagesloaded', true, true, {
						pagesCount: pagesCount
					});
					self.container.dispatchEvent(event);
				});

				var isOnePageRenderedResolved = false;
				var resolveOnePageRendered = null;
				var onePageRendered = new Promise(function(resolve) {
					resolveOnePageRendered = resolve;
				});
				this.onePageRendered = onePageRendered;

				var bindOnAfterAndBeforeDraw = function(pageView) {
					pageView.onBeforeDraw = function() {

						self._buffer.push(this);
					};
					pageView.onAfterDraw = function() {
						if (!isOnePageRenderedResolved) {
							isOnePageRenderedResolved = true;
							resolveOnePageRendered();
						}
					};
				};

				var firstPagePromise = pdfDocument.getPage(1);
				this.firstPagePromise = firstPagePromise;

				return firstPagePromise.then(function(pdfPage) {
					var scale = this._currentScale || 1.0;
					var viewport = pdfPage.getViewport(scale * DDocViewer.Constants.CSS_UNITS);
					for (var pageNum = 1; pageNum <= pagesCount; ++pageNum) {
						var textLayerFactory = null;
						if (!PDFJS.disableTextLayer) {
							textLayerFactory = this;
						}
						var pageView = new DDocViewer.PDFPageView({
							container: this.viewer,
							id: pageNum,
							scale: scale,
							defaultViewport: viewport.clone(),
							renderingQueue: this.renderingQueue,
							textLayerFactory: textLayerFactory,
							annotationsLayerFactory: this
						});
						bindOnAfterAndBeforeDraw(pageView);
						this.pages.push(pageView);
					}

					onePageRendered.then(function() {
						if (!PDFJS.disableAutoFetch) {
							var getPagesLeft = pagesCount;
							for (var pageNum = 1; pageNum <= pagesCount; ++pageNum) {
								pdfDocument.getPage(pageNum).then(function(pageNum, pdfPage) {
									var pageView = self.pages[pageNum - 1];
									if (!pageView.pdfPage) {
										pageView.setPdfPage(pdfPage);
									}
									var refStr = pdfPage.ref.num + ' ' + pdfPage.ref.gen + ' R';
									pagesRefMap[refStr] = pageNum;
									getPagesLeft--;
									if (!getPagesLeft) {
										resolvePagesPromise();
									}
								}.bind(null, pageNum));
							}
						} else {
							resolvePagesPromise();
						}
					});

					var event = document.createEvent('CustomEvent');
					event.initCustomEvent('pagesinit', true, true, null);
					self.container.dispatchEvent(event);

					if (this.defaultRenderingQueue) {
						this.update();
					}

					if (this.findController) {
						this.findController.resolveFirstPage();
					}
				}.bind(this));
			},

			_resetView: function() {
				this.pages = [];
				this._currentPageNumber = 1;
				this._currentScale = DDocViewer.Constants.UNKNOWN_SCALE;
				this._currentScaleValue = null;
				this._buffer = new pdfPageViewBuffer(DDocViewer.Constants.DEFAULT_CACHE_SIZE);
				this.location = null;
				this._pagesRotation = 0;
				this._pagesRequests = [];

				var container = this.viewer;
				while (container.hasChildNodes()) {
					container.removeChild(container.lastChild);
				}
			},

			_scrollUpdate: function() {
				if (this.pagesCount === 0) {
					return;
				}
				this.update();
				for (var i = 0, ii = this.pages.length; i < ii; i++) {
					this.pages[i].updatePosition();
				}
			},

			_setScaleDispatchEvent: function(
				newScale, newValue, preset) {
				var event = document.createEvent('UIEvents');
				event.initUIEvent('scalechange', true, true, window, 0);
				event.scale = newScale;
				if (preset) {
					event.presetValue = newValue;
				}
				this.container.dispatchEvent(event);
			},

			_setScaleUpdatePages: function(
				newScale, newValue, noScroll, preset) {
				this._currentScaleValue = newValue;
				if (newScale === this._currentScale) {
					if (preset) {
						this._setScaleDispatchEvent(newScale, newValue, true);
					}
					return;
				}

				for (var i = 0, ii = this.pages.length; i < ii; i++) {
					this.pages[i].update(newScale);
				}
				this._currentScale = newScale;

				if (!noScroll) {
					var page = this._currentPageNumber, dest;
					if (this.location && !DDocViewer.Constants.IGNORE_CURRENT_POSITION_ON_ZOOM) {
						page = this.location.pageNumber;
						dest = [
							null, { name: 'XYZ' }, this.location.left,
							this.location.top, null
						];
					}
					this.scrollPageIntoView(page, dest);
				}

				this._setScaleDispatchEvent(newScale, newValue, preset);
			},

			_setScale: function(value, noScroll) {
				if (value === 'custom') {
					return;
				}
				var scale = parseFloat(value);

				if (scale > 0) {
					this._setScaleUpdatePages(scale, value, noScroll, false);
				} else {
					var currentPage = this.pages[this._currentPageNumber - 1];
					if (!currentPage) {
						return;
					}
					var hPadding = this.removePageBorders ?
						0 : DDocViewer.Constants.SCROLLBAR_PADDING;
					var vPadding = this.removePageBorders ?
						0 : DDocViewer.Constants.VERTICAL_PADDING;
					var pageWidthScale = (this.container.clientWidth - hPadding) /
						currentPage.width * currentPage.scale;
					var pageHeightScale = (this.container.clientHeight - vPadding) /
						currentPage.height * currentPage.scale;
					switch (value) {
					case 'page-actual':
						scale = 1;
						break;
					case 'page-width':
						scale = pageWidthScale;
						break;
					case 'page-height':
						scale = pageHeightScale;
						break;
					case 'page-fit':
						scale = Math.min(pageWidthScale, pageHeightScale);
						break;
					case 'auto':
						var isLandscape = (currentPage.width > currentPage.height);
						var horizontalScale = isLandscape ?
							Math.min(pageHeightScale, pageWidthScale) : pageWidthScale;
						scale = Math.min(DDocViewer.Constants.MAX_AUTO_SCALE, horizontalScale);
						break;
					default:
						console.error('pdfViewSetScale: \'' + value +
							'\' is an unknown zoom value.');
						return;
					}
					this._setScaleUpdatePages(scale, value, noScroll, true);
				}
			},

			scrollPageIntoView: function(pageNumber,
				dest) {
				var pageView = this.pages[pageNumber - 1];

				if (!dest) {
					DDocViewer.scrollIntoView(pageView.div);
					return;
				}

				var x = 0, y = 0;
				var width = 0, height = 0, widthScale, heightScale;
				var changeOrientation = (pageView.rotation % 180 === 0 ? false : true);
				var pageWidth = (changeOrientation ? pageView.height : pageView.width) /
					pageView.scale / DDocViewer.Constants.CSS_UNITS;
				var pageHeight = (changeOrientation ? pageView.width : pageView.height) /
					pageView.scale / DDocViewer.Constants.CSS_UNITS;
				var scale = 0;
				switch (dest[1].name) {
				case 'XYZ':
					x = dest[2];
					y = dest[3];
					scale = dest[4];
					x = x !== null ? x : 0;
					y = y !== null ? y : pageHeight;
					break;
				case 'Fit':
				case 'FitB':
					scale = 'page-fit';
					break;
				case 'FitH':
				case 'FitBH':
					y = dest[2];
					scale = 'page-width';
					break;
				case 'FitV':
				case 'FitBV':
					x = dest[2];
					width = pageWidth;
					height = pageHeight;
					scale = 'page-height';
					break;
				case 'FitR':
					x = dest[2];
					y = dest[3];
					width = dest[4] - x;
					height = dest[5] - y;
					var viewerContainer = this.container;
					var hPadding = this.removePageBorders ? 0 : DDocViewer.Constants.SCROLLBAR_PADDING;
					var vPadding = this.removePageBorders ? 0 : DDocViewer.Constants.VERTICAL_PADDING;

					widthScale = (viewerContainer.clientWidth - hPadding) /
						width / DDocViewer.Constants.CSS_UNITS;
					heightScale = (viewerContainer.clientHeight - vPadding) /
						height / DDocViewer.Constants.CSS_UNITS;
					scale = Math.min(Math.abs(widthScale), Math.abs(heightScale));
					break;
				default:
					return;
				}

				if (scale && scale !== this.currentScale) {
					this.currentScaleValue = scale;
				} else if (this.currentScale === DDocViewer.Constants.UNKNOWN_SCALE) {
					this.currentScaleValue = DDocViewer.Constants.DEFAULT_SCALE;
				}

				if (scale === 'page-fit' && !dest[4]) {
					DDocViewer.scrollIntoView(pageView.div);
					return;
				}

				var boundingRect = [
					pageView.viewport.convertToViewportPoint(x, y),
					pageView.viewport.convertToViewportPoint(x + width, y + height)
				];
				var left = Math.min(boundingRect[0][0], boundingRect[1][0]);
				var top = Math.min(boundingRect[0][1], boundingRect[1][1]);

				DDocViewer.scrollIntoView(pageView.div, { left: left, top: top });
			},

			_updateLocation: function(firstPage) {
				var currentScale = this._currentScale;
				var currentScaleValue = this._currentScaleValue;
				var normalizedScaleValue =
					parseFloat(currentScaleValue) === currentScale ?
						Math.round(currentScale * 10000) / 100 : currentScaleValue;

				var pageNumber = firstPage.id;
				var pdfOpenParams = '#page=' + pageNumber;
				pdfOpenParams += '&zoom=' + normalizedScaleValue;
				var currentPageView = this.pages[pageNumber - 1];
				var container = this.container;
				var topLeft = currentPageView.getPagePoint(
				(container.scrollLeft - firstPage.x),
				(container.scrollTop - firstPage.y));
				var intLeft = Math.round(topLeft[0]);
				var intTop = Math.round(topLeft[1]);
				pdfOpenParams += ',' + intLeft + ',' + intTop;

				this.location = {
					pageNumber: pageNumber,
					scale: normalizedScaleValue,
					top: intTop,
					left: intLeft,
					pdfOpenParams: pdfOpenParams
				};
			},

			update: function() {
				var visible = this._getVisiblePages();
				var visiblePages = visible.views;
				if (visiblePages.length === 0) {
					return;
				}

				this.updateInProgress = true;

				var suggestedCacheSize = Math.max(DDocViewer.Constants.DEFAULT_CACHE_SIZE,
					2 * visiblePages.length + 1);
				this._buffer.resize(suggestedCacheSize);

				this.renderingQueue.renderHighestPriority(visible);

				var currentId = this.currentPageNumber;
				var firstPage = visible.first;

				for (var i = 0, ii = visiblePages.length, stillFullyVisible = false;
					i < ii; ++i) {
					var page = visiblePages[i];

					if (page.percent < 100) {
						break;
					}
					if (page.id === currentId) {
						stillFullyVisible = true;
						break;
					}
				}

				if (!stillFullyVisible) {
					currentId = visiblePages[0].id;
				}

				this.currentPageNumber = currentId;

				this._updateLocation(firstPage);

				this.updateInProgress = false;

				var event = document.createEvent('UIEvents');
				event.initUIEvent('updateviewarea', true, true, window, 0);
				this.container.dispatchEvent(event);
			},

			containsElement: function(element) {
				return this.container.contains(element);
			},

			focus: function() {
				this.container.focus();
			},

			get isHorizontalScrollbarEnabled() {
				return this.container.scrollWidth > this.container.clientWidth;
			},

			_getVisiblePages: function() {
				return DDocViewer.getVisibleElements(this.container, this.pages, true);
			},

			cleanup: function() {
				for (var i = 0, ii = this.pages.length; i < ii; i++) {
					if (this.pages[i] &&
						this.pages[i].renderingState !== DDocViewer.Enums.RenderingStates.FINISHED) {
						this.pages[i].reset();
					}
				}
			},

			_ensurePdfPageLoaded: function(pageView) {
				if (pageView.pdfPage) {
					return Promise.resolve(pageView.pdfPage);
				}
				var pageNumber = pageView.id;
				if (this._pagesRequests[pageNumber]) {
					return this._pagesRequests[pageNumber];
				}
				var promise = this.pdfDocument.getPage(pageNumber).then(
					function(pdfPage) {
						pageView.setPdfPage(pdfPage);
						this._pagesRequests[pageNumber] = null;
						return pdfPage;
					}.bind(this));
				this._pagesRequests[pageNumber] = promise;
				return promise;
			},

			forceRendering: function(currentlyVisiblePages) {
				var visiblePages = currentlyVisiblePages || this._getVisiblePages();
				var pageView = this.renderingQueue.getHighestPriority(visiblePages,
					this.pages,
					this.scroll.down);
				if (pageView) {
					this._ensurePdfPageLoaded(pageView).then(function() {
						this.renderingQueue.renderView(pageView);
					}.bind(this));
					return true;
				}
				return false;
			},

			getPageTextContent: function(pageIndex) {
				return this.pdfDocument.getPage(pageIndex + 1).then(function(page) {
					return page.getTextContent();
				});
			},

			createTextLayerBuilder: function(textLayerDiv, pageIndex, viewport) {
				return new DDocViewer.TextLayerBuilder({
					textLayerDiv: textLayerDiv,
					pageIndex: pageIndex,
					viewport: viewport,
					findController: this.findController
				});
			},

			createAnnotationsLayerBuilder: function(pageDiv, pdfPage) {
				return new DDocViewer.AnnotationsLayerBuilder({
					pageDiv: pageDiv,
					pdfPage: pdfPage,
					linkService: this.linkService
				});
			},

			setFindController: function(findController) {
				this.findController = findController;
			},
		};

		return pdfViewer;
	})();

	this.SimpleLinkService = (function() {
		function simpleLinkService(pdfViewer) {
			this.pdfViewer = pdfViewer;
		}

		simpleLinkService.prototype = {
			get page() {
				return this.pdfViewer.currentPageNumber;
			},
			set page(value) {
				this.pdfViewer.currentPageNumber = value;
			},
			navigateTo: function(dest) {},
			getDestinationHash: function(dest) {
				return '#';
			},
			getAnchorUrl: function(hash) {
				return '#';
			},
			setHash: function(hash) {},
			executeNamedAction: function(action) {},
		};
		return simpleLinkService;
	})();

	this.PDFThumbnailView = (function() {
		function getTempCanvas(width, height) {
			var tempCanvas = DDocViewer.PDFThumbnailView.tempImageCache;
			if (!tempCanvas) {
				tempCanvas = document.createElement('canvas');
				DDocViewer.PDFThumbnailView.tempImageCache = tempCanvas;
			}
			tempCanvas.width = width;
			tempCanvas.height = height;

			var ctx = tempCanvas.getContext('2d');
			ctx.save();
			ctx.fillStyle = 'rgb(255, 255, 255)';
			ctx.fillRect(0, 0, width, height);
			ctx.restore();
			return tempCanvas;
		}

		function pdfThumbnailView(options) {
			var container = options.container;
			var id = options.id;
			var defaultViewport = options.defaultViewport;
			var linkService = options.linkService;
			var renderingQueue = options.renderingQueue;

			this.id = id;
			this.renderingId = 'thumbnail' + id;

			this.pdfPage = null;
			this.rotation = 0;
			this.viewport = defaultViewport;
			this.pdfPageRotate = defaultViewport.rotation;

			this.linkService = linkService;
			this.renderingQueue = renderingQueue;

			this.hasImage = false;
			this.resume = null;
			this.renderingState = DDocViewer.Enums.RenderingStates.INITIAL;

			this.pageWidth = this.viewport.width;
			this.pageHeight = this.viewport.height;
			this.pageRatio = this.pageWidth / this.pageHeight;

			this.canvasWidth = DDocViewer.Constants.THUMBNAIL_WIDTH;
			this.canvasHeight = (this.canvasWidth / this.pageRatio) | 0;
			this.scale = this.canvasWidth / this.pageWidth;

			var anchor = document.createElement('a');
			anchor.href = linkService.getAnchorUrl('#page=' + id);
			anchor.title = Resources.thumb_page_title.format(id);
			anchor.onclick = function() {
				linkService.page = id;
				return false;
			};

			var div = document.createElement('div');
			div.id = 'thumbnailContainer' + id;
			div.className = 'thumbnail';
			this.div = div;

			if (id === 1) {
				div.classList.add('selected');
			}

			var ring = document.createElement('div');
			ring.className = 'thumbnailSelectionRing';
			var borderAdjustment = 2 * DDocViewer.Constants.THUMBNAIL_CANVAS_BORDER_WIDTH;
			ring.style.width = this.canvasWidth + borderAdjustment + 'px';
			ring.style.height = this.canvasHeight + borderAdjustment + 'px';
			this.ring = ring;

			div.appendChild(ring);
			anchor.appendChild(div);
			container.appendChild(anchor);
		}

		pdfThumbnailView.prototype = {
			setPdfPage: function(pdfPage) {
				this.pdfPage = pdfPage;
				this.pdfPageRotate = pdfPage.rotate;
				var totalRotation = (this.rotation + this.pdfPageRotate) % 360;
				this.viewport = pdfPage.getViewport(1, totalRotation);
				this.reset();
			},

			reset: function() {
				if (this.renderTask) {
					this.renderTask.cancel();
				}
				this.hasImage = false;
				this.resume = null;
				this.renderingState = DDocViewer.Enums.RenderingStates.INITIAL;

				this.pageWidth = this.viewport.width;
				this.pageHeight = this.viewport.height;
				this.pageRatio = this.pageWidth / this.pageHeight;

				this.canvasHeight = (this.canvasWidth / this.pageRatio) | 0;
				this.scale = (this.canvasWidth / this.pageWidth);

				this.div.removeAttribute('data-loaded');
				var ring = this.ring;
				var childNodes = ring.childNodes;
				for (var i = childNodes.length - 1; i >= 0; i--) {
					ring.removeChild(childNodes[i]);
				}
				var borderAdjustment = 2 * DDocViewer.Constants.THUMBNAIL_CANVAS_BORDER_WIDTH;
				ring.style.width = this.canvasWidth + borderAdjustment + 'px';
				ring.style.height = this.canvasHeight + borderAdjustment + 'px';

				if (this.canvas) {
					this.canvas.width = 0;
					this.canvas.height = 0;
					delete this.canvas;
				}
			},

			update: function(rotation) {
				if (typeof rotation !== 'undefined') {
					this.rotation = rotation;
				}
				var totalRotation = (this.rotation + this.pdfPageRotate) % 360;
				this.viewport = this.viewport.clone({
					scale: 1,
					rotation: totalRotation
				});
				this.reset();
			},

			_getPageDrawContext: function() {
				var canvas = document.createElement('canvas');
				canvas.id = this.renderingId;

				canvas.width = this.canvasWidth;
				canvas.height = this.canvasHeight;
				canvas.className = 'thumbnailImage';
				canvas.setAttribute('aria-label', Resources.thumb_page_canvas.format(this.id));

				this.canvas = canvas;
				this.div.setAttribute('data-loaded', true);
				this.ring.appendChild(canvas);

				return canvas.getContext('2d');
			},

			draw: function() {
				if (this.renderingState !== DDocViewer.Enums.RenderingStates.INITIAL) {
					console.error('Must be in new state before drawing');
				}
				if (this.hasImage) {
					return Promise.resolve(undefined);
				}
				this.hasImage = true;
				this.renderingState = DDocViewer.Enums.RenderingStates.RUNNING;

				var resolveRenderPromise, rejectRenderPromise;
				var promise = new Promise(function(resolve, reject) {
					resolveRenderPromise = resolve;
					rejectRenderPromise = reject;
				});

				var self = this;

				function thumbnailDrawCallback(error) {
					if (renderTask === self.renderTask) {
						self.renderTask = null;
					}
					if (error === 'cancelled') {
						rejectRenderPromise(error);
						return;
					}
					self.renderingState = DDocViewer.Enums.RenderingStates.FINISHED;

					if (!error) {
						resolveRenderPromise(undefined);
					} else {
						rejectRenderPromise(error);
					}
				}

				var ctx = this._getPageDrawContext();
				var drawViewport = this.viewport.clone({ scale: this.scale });
				var renderContinueCallback = function(cont) {
					if (!self.renderingQueue.isHighestPriority(self)) {
						self.renderingState = DDocViewer.Enums.RenderingStates.PAUSED;
						self.resume = function() {
							self.renderingState = DDocViewer.Enums.RenderingStates.RUNNING;
							cont();
						};
						return;
					}
					cont();
				};

				var renderContext = {
					canvasContext: ctx,
					viewport: drawViewport,
					continueCallback: renderContinueCallback
				};
				var renderTask = this.renderTask = this.pdfPage.render(renderContext);

				renderTask.promise.then(
					function pdfPageRenderCallback() {
						thumbnailDrawCallback(null);
					},
					function pdfPageRenderError(error) {
						thumbnailDrawCallback(error);
					}
				);
				return promise;
			},

			setImage: function(pageView) {
				var img = pageView.canvas;
				if (this.hasImage || !img) {
					return;
				}
				if (!this.pdfPage) {
					this.setPdfPage(pageView.pdfPage);
				}
				this.hasImage = true;
				this.renderingState = DDocViewer.Enums.RenderingStates.FINISHED;

				var ctx = this._getPageDrawContext();
				var canvas = ctx.canvas;

				if (img.width <= 2 * canvas.width) {
					ctx.drawImage(img, 0, 0, img.width, img.height,
						0, 0, canvas.width, canvas.height);
					return;
				}
				var MAX_NUM_SCALING_STEPS = 3;
				var reducedWidth = canvas.width << MAX_NUM_SCALING_STEPS;
				var reducedHeight = canvas.height << MAX_NUM_SCALING_STEPS;
				var reducedImage = getTempCanvas(reducedWidth, reducedHeight);
				var reducedImageCtx = reducedImage.getContext('2d');

				while (reducedWidth > img.width || reducedHeight > img.height) {
					reducedWidth >>= 1;
					reducedHeight >>= 1;
				}
				reducedImageCtx.drawImage(img, 0, 0, img.width, img.height,
					0, 0, reducedWidth, reducedHeight);
				while (reducedWidth > 2 * canvas.width) {
					reducedImageCtx.drawImage(reducedImage,
						0, 0, reducedWidth, reducedHeight,
						0, 0, reducedWidth >> 1, reducedHeight >> 1);
					reducedWidth >>= 1;
					reducedHeight >>= 1;
				}
				ctx.drawImage(reducedImage, 0, 0, reducedWidth, reducedHeight,
					0, 0, canvas.width, canvas.height);
			}
		};

		return pdfThumbnailView;
	})();

	this.PDFThumbnailView.tempImageCache = null;

	this.PDFThumbnailViewer = (function() {
		function pdfThumbnailViewer(options) {
			this.container = options.container;
			this.renderingQueue = options.renderingQueue;
			this.linkService = options.linkService;

			this.scroll = DDocViewer.watchScroll(this.container, this._scrollUpdated.bind(this));
			this._resetView();
		}

		pdfThumbnailViewer.prototype = {
			_scrollUpdated: function() {
				this.renderingQueue.renderHighestPriority();
			},

			getThumbnail: function(index) {
				return this.thumbnails[index];
			},

			_getVisibleThumbs: function() {
				return DDocViewer.getVisibleElements(this.container, this.thumbnails);
			},

			scrollThumbnailIntoView:
				function(page) {
					var selected = document.querySelector('.thumbnail.selected');
					if (selected) {
						selected.classList.remove('selected');
					}
					var thumbnail = document.getElementById('thumbnailContainer' + page);
					if (thumbnail) {
						thumbnail.classList.add('selected');
					}
					var visibleThumbs = this._getVisibleThumbs();
					var numVisibleThumbs = visibleThumbs.views.length;

					if (numVisibleThumbs > 0) {
						var first = visibleThumbs.first.id;
						var last = (numVisibleThumbs > 1 ? visibleThumbs.last.id : first);
						if (page <= first || page >= last) {
							DDocViewer.scrollIntoView(thumbnail, { top: DDocViewer.Constants.THUMBNAIL_SCROLL_MARGIN });
						}
					}
				},

			get pagesRotation() {
				return this._pagesRotation;
			},

			set pagesRotation(rotation) {
				this._pagesRotation = rotation;
				for (var i = 0, l = this.thumbnails.length; i < l; i++) {
					var thumb = this.thumbnails[i];
					thumb.update(rotation);
				}
			},

			cleanup: function() {
				var tempCanvas = DDocViewer.PDFThumbnailView.tempImageCache;
				if (tempCanvas) {
					tempCanvas.width = 0;
					tempCanvas.height = 0;
				}
				DDocViewer.PDFThumbnailView.tempImageCache = null;
			},

			_resetView: function() {
				this.thumbnails = [];
				this._pagesRotation = 0;
				this._pagesRequests = [];
			},

			setDocument: function(pdfDocument) {
				if (this.pdfDocument) {
					var thumbsView = this.container;
					while (thumbsView.hasChildNodes()) {
						thumbsView.removeChild(thumbsView.lastChild);
					}
					this._resetView();
				}

				this.pdfDocument = pdfDocument;
				if (!pdfDocument) {
					return Promise.resolve();
				}

				return pdfDocument.getPage(1).then(function(firstPage) {
					var pagesCount = pdfDocument.numPages;
					var viewport = firstPage.getViewport(1.0);
					for (var pageNum = 1; pageNum <= pagesCount; ++pageNum) {
						var thumbnail = new DDocViewer.PDFThumbnailView({
							container: this.container,
							id: pageNum,
							defaultViewport: viewport.clone(),
							linkService: this.linkService,
							renderingQueue: this.renderingQueue
						});
						this.thumbnails.push(thumbnail);
					}
				}.bind(this));
			},

			_ensurePdfPageLoaded:
				function(thumbView) {
					if (thumbView.pdfPage) {
						return Promise.resolve(thumbView.pdfPage);
					}
					var pageNumber = thumbView.id;
					if (this._pagesRequests[pageNumber]) {
						return this._pagesRequests[pageNumber];
					}
					var promise = this.pdfDocument.getPage(pageNumber).then(
						function(pdfPage) {
							thumbView.setPdfPage(pdfPage);
							this._pagesRequests[pageNumber] = null;
							return pdfPage;
						}.bind(this));
					this._pagesRequests[pageNumber] = promise;
					return promise;
				},

			ensureThumbnailVisible:
				function(page) {
					DDocViewer.scrollIntoView(document.getElementById('thumbnailContainer' + page));
				},

			forceRendering: function() {
				var visibleThumbs = this._getVisibleThumbs();
				var thumbView = this.renderingQueue.getHighestPriority(visibleThumbs,
					this.thumbnails,
					this.scroll.down);
				if (thumbView) {
					this._ensurePdfPageLoaded(thumbView).then(function() {
						this.renderingQueue.renderView(thumbView);
					}.bind(this));
					return true;
				}
				return false;
			}
		};

		return pdfThumbnailViewer;
	})();

	this.Preferences = {
		prefs: Object.create(DDocViewer.Constants.DEFAULT_PREFERENCES),
		isInitializedPromiseResolved: false,
		initializedPromise: null,
		initialize: function() {
			return this.initializedPromise =
				this._readFromStorage(DDocViewer.Constants.DEFAULT_PREFERENCES).then(function(prefObj) {
					this.isInitializedPromiseResolved = true;
					if (prefObj) {
						this.prefs = prefObj;
					}
				}.bind(this));
		},
		_writeToStorage: function(prefObj) {
			return Promise.resolve();
		},
		_readFromStorage: function(prefObj) {
			return Promise.resolve();
		},
		reset: function() {
			return this.initializedPromise.then(function() {
				this.prefs = Object.create(DDocViewer.Constants.DEFAULT_PREFERENCES);
				return this._writeToStorage(DDocViewer.Constants.DEFAULT_PREFERENCES);
			}.bind(this));
		},
		reload: function() {
			return this.initializedPromise.then(function() {
				this._readFromStorage(DDocViewer.Constants.DEFAULT_PREFERENCES).then(function(prefObj) {
					if (prefObj) {
						this.prefs = prefObj;
					}
				}.bind(this));
			}.bind(this));
		},
		set: function(name, value) {
			return this.initializedPromise.then(function() {
				if (DDocViewer.Constants.DEFAULT_PREFERENCES[name] === undefined) {
					throw new Error('preferencesSet: \'' + name + '\' is undefined.');
				} else if (value === undefined) {
					throw new Error('preferencesSet: no value is specified.');
				}
				var valueType = typeof value;
				var defaultType = typeof DDocViewer.Constants.DEFAULT_PREFERENCES[name];

				if (valueType !== defaultType) {
					if (valueType === 'number' && defaultType === 'string') {
						value = value.toString();
					} else {
						throw new Error('Preferences_set: \'' + value + '\' is a \"' +
							valueType + '\", expected \"' + defaultType + '\".');
					}
				} else {
					if (valueType === 'number' && (value | 0) !== value) {
						throw new Error('Preferences_set: \'' + value +
							'\' must be an \"integer\".');
					}
				}
				this.prefs[name] = value;
				return this._writeToStorage(this.prefs);
			}.bind(this));
		},
		get: function(name) {
			return this.initializedPromise.then(function() {
				var defaultValue = DDocViewer.Constants.DEFAULT_PREFERENCES[name];

				if (defaultValue === undefined) {
					throw new Error('preferencesGet: \'' + name + '\' is undefined.');
				} else {
					var prefValue = this.prefs[name];

					if (prefValue !== undefined) {
						return prefValue;
					}
				}
				return defaultValue;
			}.bind(this));
		}
	};

	this.Preferences._writeToStorage = function(prefObj) {
		return new Promise(function(resolve) {
			localStorage.setItem('pdfjs.preferences', JSON.stringify(prefObj));
			resolve();
		});
	};

	this.Preferences._readFromStorage = function(prefObj) {
		return new Promise(function(resolve) {
			var readPrefs = JSON.parse(localStorage.getItem('pdfjs.preferences'));
			resolve(readPrefs);
		});
	};

	this.HandTool = {
		initialize: function(options) {
			var toggleHandTool = options.toggleHandTool;
			this.handTool = new DDocViewer.GrabToPan({
				element: options.container,
				onActiveChanged: function(isActive) {
					if (!toggleHandTool) {
						return;
					}
					if (isActive) {
						toggleHandTool.title = Resources.hand_tool_disable_title;
					} else {
						toggleHandTool.title = Resources.hand_tool_enable_title;
					}
				}
			});
			if (toggleHandTool) {
				toggleHandTool.addEventListener('click', this.toggle.bind(this), false);

				window.addEventListener('localized', function(evt) {
					Preferences.get('enableHandToolOnLoad').then(function resolved(value) {
						if (value) {
							this.handTool.activate();
						}
					}.bind(this), function rejected(reason) {});
				}.bind(this));
			}
		},
		toggle: function() {
			this.handTool.toggle();
		},
	};

	this.OverlayManager = {
		overlays: {},
		active: null,
		register: function(name, callerCloseMethod, canForceClose) {
			return new Promise(function(resolve) {
				var element, container;
				if (!name || !(element = document.getElementById(name)) ||
					!(container = element.parentNode)) {
					throw new Error('Not enough parameters.');
				} else if (this.overlays[name]) {
					throw new Error('The overlay is already registered.');
				}
				this.overlays[name] = {
					element: element,
					container: container,
					callerCloseMethod: (callerCloseMethod || null),
					canForceClose: (canForceClose || false)
				};
				resolve();
			}.bind(this));
		},
		unregister: function(name) {
			return new Promise(function(resolve) {
				if (!this.overlays[name]) {
					throw new Error('The overlay does not exist.');
				} else if (this.active === name) {
					throw new Error('The overlay cannot be removed while it is active.');
				}
				delete this.overlays[name];

				resolve();
			}.bind(this));
		},
		open: function(name) {
			return new Promise(function(resolve) {
				if (!this.overlays[name]) {
					throw new Error('The overlay does not exist.');
				} else if (this.active) {
					if (this.overlays[name].canForceClose) {
						this._closeThroughCaller();
					} else if (this.active === name) {
						throw new Error('The overlay is already active.');
					} else {
						throw new Error('Another overlay is currently active.');
					}
				}
				this.active = name;
				this.overlays[this.active].element.classList.remove('hidden');
				this.overlays[this.active].container.classList.remove('hidden');

				window.addEventListener('keydown', this._keyDown);
				resolve();
			}.bind(this));
		},
		close: function(name) {
			return new Promise(function(resolve) {
				if (!this.overlays[name]) {
					throw new Error('The overlay does not exist.');
				} else if (!this.active) {
					throw new Error('The overlay is currently not active.');
				} else if (this.active !== name) {
					throw new Error('Another overlay is currently active.');
				}
				this.overlays[this.active].container.classList.add('hidden');
				this.overlays[this.active].element.classList.add('hidden');
				this.active = null;

				window.removeEventListener('keydown', this._keyDown);
				resolve();
			}.bind(this));
		},
		_keyDown: function(evt) {
			var self = DDocViewer.OverlayManager;
			if (self.active && evt.keyCode === 27) {
				self._closeThroughCaller();
				evt.preventDefault();
			}
		},
		_closeThroughCaller: function() {
			if (this.overlays[this.active].callerCloseMethod) {
				this.overlays[this.active].callerCloseMethod();
			}
			if (this.active) {
				this.close(this.active);
			}
		}
	};

	this.PDFViewerApplication = {
		initialBookmark: document.location.hash.substring(1),
		initialized: false,
		fellback: false,
		pdfDocument: null,
		sidebarOpen: false,
		printing: false,
		pdfViewer: null,
		pdfThumbnailViewer: null,
		pdfRenderingQueue: null,
		pageRotation: 0,
		updateScaleControls: true,
		isInitialViewSet: false,
		animationStartedPromise: null,
		preferenceSidebarViewOnLoad: DDocViewer.Enums.SidebarView.NONE,
		preferenceShowPreviousViewOnLoad: true,
		preferenceDefaultZoomValue: '',
		isViewerEmbedded: (window.parent !== window),
		url: '',
		initialize: function() {
			var pdfRenderingQueue = new DDocViewer.PDFRenderingQueue();
			pdfRenderingQueue.onIdle = this.cleanup.bind(this);
			this.pdfRenderingQueue = pdfRenderingQueue;

			var container = document.getElementById('viewerContainer');
			var viewer = document.getElementById('viewer');
			this.pdfViewer = new DDocViewer.PDFViewer({
				container: container,
				viewer: viewer,
				renderingQueue: pdfRenderingQueue,
				linkService: this
			});
			pdfRenderingQueue.setViewer(this.pdfViewer);

			var thumbnailContainer = document.getElementById('thumbnailView');
			this.pdfThumbnailViewer = new DDocViewer.PDFThumbnailViewer({
				container: thumbnailContainer,
				renderingQueue: pdfRenderingQueue,
				linkService: this
			});
			pdfRenderingQueue.setThumbnailViewer(this.pdfThumbnailViewer);

			DDocViewer.Preferences.initialize();

			this.findController = new DDocViewer.PDFFindController({
				pdfViewer: this.pdfViewer,
				integratedFind: this.supportsIntegratedFind
			});
			this.pdfViewer.setFindController(this.findController);

			this.findBar = new DDocViewer.PDFFindBar({
				bar: document.getElementById('findbar'),
				toggleButton: document.getElementById('viewFind'),
				findField: document.getElementById('findInput'),
				highlightAllCheckbox: document.getElementById('findHighlightAll'),
				caseSensitiveCheckbox: document.getElementById('findMatchCase'),
				findMsg: document.getElementById('findMsg'),
				findStatusIcon: document.getElementById('findStatusIcon'),
				findPreviousButton: document.getElementById('findPrevious'),
				findNextButton: document.getElementById('findNext'),
				findController: this.findController
			});

			this.findController.setFindBar(this.findBar);

			DDocViewer.HandTool.initialize({
				container: container,
				toggleHandTool: document.getElementById('toggleHandTool')
			});

			if ($('#ShowCustomPanel').val() == 'True') {
				this.customPanel = new DDocUi.CustomPanel({
					panel: document.getElementById('customPanel'),
					toggleButton: document.getElementById('customPanelActivator')
				});
			}

			var self = this;
			var initializedPromise = Promise.all([
				DDocViewer.Preferences.get('enableWebGL').then(function(value) {
					PDFJS.disableWebGL = !value;
				}),
				DDocViewer.Preferences.get('sidebarViewOnLoad').then(function(value) {
					self.preferenceSidebarViewOnLoad = value;
				}),
				DDocViewer.Preferences.get('showPreviousViewOnLoad').then(function(value) {
					self.preferenceShowPreviousViewOnLoad = value;
				}),
				DDocViewer.Preferences.get('defaultZoomValue').then(function(value) {
					self.preferenceDefaultZoomValue = value;
				}),
				DDocViewer.Preferences.get('disableTextLayer').then(function(value) {
					if (PDFJS.disableTextLayer === true) {
						return;
					}
					PDFJS.disableTextLayer = value;
				}),
				DDocViewer.Preferences.get('disableRange').then(function(value) {
					if (PDFJS.disableRange === true) {
						return;
					}
					PDFJS.disableRange = value;
				}),
				DDocViewer.Preferences.get('disableAutoFetch').then(function(value) {
					PDFJS.disableAutoFetch = value;
				}),
				DDocViewer.Preferences.get('disableFontFace').then(function(value) {
					if (PDFJS.disableFontFace === true) {
						return;
					}
					PDFJS.disableFontFace = value;
				}),
				DDocViewer.Preferences.get('useOnlyCssZoom').then(function(value) {
					PDFJS.useOnlyCssZoom = value;
				})
			]).catch(function(reason) {});

			return initializedPromise.then(function() {
				DDocViewer.PDFViewerApplication.initialized = true;
			});
		},
		zoomIn: function(ticks) {
			var newScale = this.pdfViewer.currentScale;
			do {
				newScale = (newScale * DDocViewer.Constants.DEFAULT_SCALE_DELTA).toFixed(2);
				newScale = Math.ceil(newScale * 10) / 10;
				newScale = Math.min(DDocViewer.Constants.MAX_SCALE, newScale);
			} while (--ticks && newScale < DDocViewer.Constants.MAX_SCALE);
			this.setScale(newScale, true);
		},
		zoomOut: function(ticks) {
			var newScale = this.pdfViewer.currentScale;
			do {
				newScale = (newScale / DDocViewer.Constants.DEFAULT_SCALE_DELTA).toFixed(2);
				newScale = Math.floor(newScale * 10) / 10;
				newScale = Math.max(DDocViewer.Constants.MIN_SCALE, newScale);
			} while (--ticks && newScale > DDocViewer.Constants.MIN_SCALE);
			this.setScale(newScale, true);
		},
		get currentScaleValue() {
			return this.pdfViewer.currentScaleValue;
		},
		get pagesCount() {
			return this.pdfDocument.numPages;
		},
		set page(val) {
			this.pdfViewer.currentPageNumber = val;
		},
		get page() {
			return this.pdfViewer.currentPageNumber;
		},
		get supportsPrinting() {
			var canvas = document.createElement('canvas');
			var value = 'mozPrintCallback' in canvas;

			return PDFJS.shadow(this, 'supportsPrinting', value);
		},
		get supportsIntegratedFind() {
			var support = false;

			return PDFJS.shadow(this, 'supportsIntegratedFind', support);
		},
		get supportsDocumentFonts() {
			var support = true;

			return PDFJS.shadow(this, 'supportsDocumentFonts', support);
		},
		get supportsDocumentColors() {
			var support = true;

			return PDFJS.shadow(this, 'supportsDocumentColors', support);
		},
		setTitle: function(title) {
			if (this.isViewerEmbedded) {
				return;
			}
			document.title = title;
		},
		close: function() {
			var errorWrapper = document.getElementById('errorWrapper');
			errorWrapper.setAttribute('hidden', 'true');

			if (!this.pdfDocument) {
				return;
			}

			this.pdfDocument.destroy();
			this.pdfDocument = null;

			this.pdfThumbnailViewer.setDocument(null);
			this.pdfViewer.setDocument(null);
		},
		open: function (pdf, searchQuery, scale) {

			if (this.pdfDocument) {
				DDocViewer.Preferences.reload();
			}
			this.close();

			var self = this;
			self.loading = true;
			self.downloadComplete = false;

			function getDocumentProgress(progressData) {
				self.progress(progressData.loaded / progressData.total);
			}

			PDFJS.getDocument(pdf, null, null, null).then(
				function getDocumentCallback(pdfDocument) {
					self.load(pdfDocument, scale);
					self.loading = false;
					if (searchQuery != '' && searchQuery != null && searchQuery != 'undefined') {
						this.findBar.toggle();
						$('#findInput').val(searchQuery);
					}
				},
				function getDocumentError(exception) {
					var message = exception && exception.message;
					var loadingErrorMessage = Resources.loading_error;

					if (exception instanceof PDFJS.InvalidPDFException) {
						loadingErrorMessage = Resources.invalid_file_error;
					} else if (exception instanceof PDFJS.MissingPDFException) {
						loadingErrorMessage = Resources.missing_file_error;
					} else if (exception instanceof PDFJS.UnexpectedResponseException) {
						loadingErrorMessage = Resources.unexpected_response_error;
					}

					var moreInfo = {
						message: message
					};
					self.error(loadingErrorMessage, moreInfo);
					self.loading = false;
				}
			);
		},
		download: function() {
			function downloadByUrl() {
				DDocViewer.DownloadManager.downloadUrl(url, filename);
			}

			var url = this.url.split('#')[0];
			var filename = DDocViewer.getPDFFileNameFromURL(url);
			var downloadManager = new DDocViewer.DownloadManager();
			downloadManager.onerror = function(err) {
				PDFViewerApplication.error('PDF failed to download.');
			};

			if (!this.pdfDocument) {
				downloadByUrl();
				return;
			}

			if (!this.downloadComplete) {
				downloadByUrl();
				return;
			}

			this.pdfDocument.getData().then(
				function getDataSuccess(data) {
					var blob = PDFJS.createBlob(data, 'application/pdf');
					downloadManager.download(blob, url, filename);
				},
				downloadByUrl
			).then(null, downloadByUrl);
		},
		fallback: function(featureId) {
		},
		navigateTo: function(dest) {
			var destString = '';
			var self = this;

			var goToDestination = function(destRef) {
				self.pendingRefStr = null;
				var pageNumber = destRef instanceof Object ?
					self.pagesRefMap[destRef.num + ' ' + destRef.gen + ' R'] :
					(destRef + 1);
				if (pageNumber) {
					if (pageNumber > self.pagesCount) {
						pageNumber = self.pagesCount;
					}
					self.pdfViewer.scrollPageIntoView(pageNumber, dest);
				} else {
					self.pdfDocument.getPageIndex(destRef).then(function(pageIndex) {
						var pageNum = pageIndex + 1;
						self.pagesRefMap[destRef.num + ' ' + destRef.gen + ' R'] = pageNum;
						goToDestination(destRef);
					});
				}
			};

			var destinationPromise;
			if (typeof dest === 'string') {
				destString = dest;
				destinationPromise = this.pdfDocument.getDestination(dest);
			} else {
				destinationPromise = Promise.resolve(dest);
			}
			destinationPromise.then(function(destination) {
				dest = destination;
				if (!(destination instanceof Array)) {
					return;
				}
				goToDestination(destination[0]);
			});
		},
		executeNamedAction: function(action) {
			switch (action) {
			case 'GoToPage':
				document.getElementById('pageNumber').focus();
				break;

			case 'Find':
				if (!this.supportsIntegratedFind) {
					this.findBar.toggle();
				}
				break;

			case 'NextPage':
				this.page++;
				break;

			case 'PrevPage':
				this.page--;
				break;

			case 'LastPage':
				this.page = this.pagesCount;
				break;

			case 'FirstPage':
				this.page = 1;
				break;

			default:
				break;
			}
		},
		getDestinationHash: function(dest) {
			if (typeof dest === 'string') {
				return this.getAnchorUrl('#' + escape(dest));
			}
			if (dest instanceof Array) {
				var destRef = dest[0];
				var pageNumber = destRef instanceof Object ?
					this.pagesRefMap[destRef.num + ' ' + destRef.gen + ' R'] :
					(destRef + 1);
				if (pageNumber) {
					var pdfOpenParams = this.getAnchorUrl('#page=' + pageNumber);
					var destKind = dest[1];
					if (typeof destKind === 'object' && 'name' in destKind &&
						destKind.name === 'XYZ') {
						var scale = (dest[4] || this.currentScaleValue);
						var scaleNumber = parseFloat(scale);
						if (scaleNumber) {
							scale = scaleNumber * 100;
						}
						pdfOpenParams += '&zoom=' + scale;
						if (dest[2] || dest[3]) {
							pdfOpenParams += ',' + (dest[2] || 0) + ',' + (dest[3] || 0);
						}
					}
					return pdfOpenParams;
				}
			}
			return '';
		},
		getAnchorUrl: function(anchor) {
			return anchor;
		},
		error: function(message, moreInfo) {
			var moreInfoText = Resources.error_version_info.format(PDFJS.version || '?', PDFJS.build || '?') + '\n';
			if (moreInfo) {
				moreInfoText +=
					Resources.error_message.format(moreInfo.message);
				if (moreInfo.stack) {
					moreInfoText += '\n' +
						Resources.error_stack.format(moreInfo.stack);
				} else {
					if (moreInfo.filename) {
						moreInfoText += '\n' +
							Resources.error_file.format(moreInfo.filename);
					}
					if (moreInfo.lineNumber) {
						moreInfoText += '\n' +
							Resources.error_line.format(moreInfo.lineNumber);
					}
				}
			}

			var errorWrapper = document.getElementById('errorWrapper');
			errorWrapper.removeAttribute('hidden');

			var errorMessage = document.getElementById('errorMessage');
			errorMessage.textContent = message;

			var closeButton = document.getElementById('errorClose');
			closeButton.onclick = function() {
				errorWrapper.setAttribute('hidden', 'true');
			};

			var errorMoreInfo = document.getElementById('errorMoreInfo');
			var moreInfoButton = document.getElementById('errorShowMore');
			var lessInfoButton = document.getElementById('errorShowLess');
			moreInfoButton.onclick = function() {
				errorMoreInfo.removeAttribute('hidden');
				moreInfoButton.setAttribute('hidden', 'true');
				lessInfoButton.removeAttribute('hidden');
				errorMoreInfo.style.height = errorMoreInfo.scrollHeight + 'px';
			};
			lessInfoButton.onclick = function() {
				errorMoreInfo.setAttribute('hidden', 'true');
				moreInfoButton.removeAttribute('hidden');
				lessInfoButton.setAttribute('hidden', 'true');
			};
			moreInfoButton.removeAttribute('hidden');
			lessInfoButton.setAttribute('hidden', 'true');
			errorMoreInfo.value = moreInfoText;
		},
		progress: function(level) {
			var percent = Math.round(level * 100);
			if (percent > this.loadingBar.percent || isNaN(percent)) {
				this.loadingBar.percent = percent;
				if (PDFJS.disableAutoFetch && percent) {
					if (this.disableAutoFetchLoadingBarTimeout) {
						clearTimeout(this.disableAutoFetchLoadingBarTimeout);
						this.disableAutoFetchLoadingBarTimeout = null;
					}
					this.loadingBar.show();

					this.disableAutoFetchLoadingBarTimeout = setTimeout(function() {
						this.loadingBar.hide();
						this.disableAutoFetchLoadingBarTimeout = null;
					}.bind(this), DDocViewer.Constants.DISABLE_AUTO_FETCH_LOADING_BAR_TIMEOUT);
				}
			}
		},
		load: function(pdfDocument, scale) {
			var self = this;
			scale = scale || DDocViewer.Constants.UNKNOWN_SCALE;

			this.findController.reset();

			this.pdfDocument = pdfDocument;

			var downloadedPromise = pdfDocument.getDownloadInfo().then(function() {
				self.downloadComplete = true;
			});

			var pagesCount = pdfDocument.numPages;
			document.getElementById('numPages').textContent = Resources.page_of.format(pagesCount);
			document.getElementById('pageNumber').max = pagesCount;

			var id = this.documentFingerprint = pdfDocument.fingerprint;

			var pdfViewer = this.pdfViewer;
			pdfViewer.currentScale = scale;
			pdfViewer.setDocument(pdfDocument);
			var firstPagePromise = pdfViewer.firstPagePromise;
			var pagesPromise = pdfViewer.pagesPromise;
			var onePageRendered = pdfViewer.onePageRendered;

			this.pageRotation = 0;
			this.isInitialViewSet = false;
			this.pagesRefMap = pdfViewer.pagesRefMap;

			this.pdfThumbnailViewer.setDocument(pdfDocument);

			firstPagePromise.then(function(pdfPage) {
				downloadedPromise.then(function() {
					var event = document.createEvent('CustomEvent');
					event.initCustomEvent('documentload', true, true, {});
					window.dispatchEvent(event);
				});

				self.setInitialView(null, scale);
			});

			pagesPromise.then(function() {
				if (self.supportsPrinting) {
					pdfDocument.getJavaScript().then(function(javaScript) {
						if (javaScript.length) {
							console.warn('Warning: JavaScript is not supported');
							self.fallback(PDFJS.UNSUPPORTED_FEATURES.javaScript);
						}
						var regex = /\bprint\s*\(/g;
						for (var i = 0, ii = javaScript.length; i < ii; i++) {
							var js = javaScript[i];
							if (js && regex.test(js)) {
								setTimeout(function() {
									window.print();
								});
								return;
							}
						}
					});
				}
			});

			if (self.preferenceSidebarViewOnLoad === DDocViewer.Enums.SidebarView.THUMBS) {
				Promise.all([firstPagePromise, onePageRendered]).then(function() {
					self.switchSidebarView('thumbs', true);
				});
			}

			pdfDocument.getMetadata().then(function(data) {
				var info = data.info, metadata = data.metadata;
				self.documentInfo = info;
				self.metadata = metadata;

				console.log('PDF ' + pdfDocument.fingerprint + ' [' +
					info.PDFFormatVersion + ' ' + (info.Producer || '-').trim() +
					' / ' + (info.Creator || '-').trim() + ']' +
					' (PDF.js: ' + (PDFJS.version || '-') +
					(!PDFJS.disableWebGL ? ' [WebGL]' : '') + ')');

				var pdfTitle;
				if (metadata && metadata.has('dc:title')) {
					var title = metadata.get('dc:title');
					if (title !== 'Untitled') {
						pdfTitle = title;
					}
				}

				if (!pdfTitle && info && info['Title']) {
					pdfTitle = info['Title'];
				}

				if (pdfTitle) {
					self.setTitle(pdfTitle + ' - ' + document.title);
				}

				if (info.IsAcroFormPresent) {
					console.warn('Warning: AcroForm/XFA is not supported');
					self.fallback(PDFJS.UNSUPPORTED_FEATURES.forms);
				}

			});
		},
		setInitialView: function(initialHash, scale) {
			this.isInitialViewSet = true;

			document.getElementById('pageNumber').value = this.pdfViewer.currentPageNumber = 1;

			if (this.initialBookmark) {
				this.setHash(this.initialBookmark);
				this.initialBookmark = null;
			} else if (scale) {
				this.setScale(scale, true);
				this.page = 1;
			}

			if (this.pdfViewer.currentScale === DDocViewer.Constants.UNKNOWN_SCALE) {
				this.setScale(DDocViewer.Constants.DEFAULT_SCALE, true);
			}
		},
		cleanup: function() {
			this.pdfViewer.cleanup();
			this.pdfThumbnailViewer.cleanup();
			this.pdfDocument.cleanup();
		},
		forceRendering: function() {
			this.pdfRenderingQueue.printing = this.printing;
			this.pdfRenderingQueue.isThumbnailViewEnabled = this.sidebarOpen;
			this.pdfRenderingQueue.renderHighestPriority();
		},
		setHash: function(hash) {
			if (!this.isInitialViewSet) {
				this.initialBookmark = hash;
				return;
			}
			if (!hash) {
				return;
			}

			if (hash.indexOf('=') >= 0) {
				var params = this.parseQueryString(hash);
				if ('nameddest' in params) {
					this.navigateTo(params.nameddest);
					return;
				}
				var pageNumber, dest;
				if ('page' in params) {
					pageNumber = (params.page | 0) || 1;
				}
				if ('zoom' in params) {
					var zoomArgs = params.zoom.split(',');
					var zoomArg = zoomArgs[0];
					var zoomArgNumber = parseFloat(zoomArg);

					if (zoomArg.indexOf('Fit') === -1) {
						dest = [
							null, { name: 'XYZ' },
							zoomArgs.length > 1 ? (zoomArgs[1] | 0) : null,
							zoomArgs.length > 2 ? (zoomArgs[2] | 0) : null,
							(zoomArgNumber ? zoomArgNumber / 100 : zoomArg)
						];
					} else {
						if (zoomArg === 'Fit' || zoomArg === 'FitB') {
							dest = [null, { name: zoomArg }];
						} else if ((zoomArg === 'FitH' || zoomArg === 'FitBH') ||
						(zoomArg === 'FitV' || zoomArg === 'FitBV')) {
							dest = [
								null, { name: zoomArg },
								zoomArgs.length > 1 ? (zoomArgs[1] | 0) : null
							];
						} else if (zoomArg === 'FitR') {
							if (zoomArgs.length !== 5) {
								console.error('pdfViewSetHash: ' +
									'Not enough parameters for \'FitR\'.');
							} else {
								dest = [
									null, { name: zoomArg },
									(zoomArgs[1] | 0), (zoomArgs[2] | 0),
									(zoomArgs[3] | 0), (zoomArgs[4] | 0)
								];
							}
						} else {
							console.error('pdfViewSetHash: \'' + zoomArg +
								'\' is not a valid zoom value.');
						}
					}
				}
				if (dest) {
					this.pdfViewer.scrollPageIntoView(pageNumber || this.page, dest);
				} else if (pageNumber) {
					this.page = pageNumber;
				}
				if ('pagemode' in params) {
					if (params.pagemode === 'thumbs' || params.pagemode === 'bookmarks') {
						this.switchSidebarView(params.pagemode, true);
					} else if (params.pagemode === 'none' && this.sidebarOpen) {
						document.getElementById('sidebarToggle').click();
					}
				}
			} else if (/^\d+$/.test(hash)) {
				this.page = hash;
			} else {
				this.navigateTo(unescape(hash));
			}
		},
		refreshThumbnailViewer: function() {
			var pdfViewer = this.pdfViewer;
			var thumbnailViewer = this.pdfThumbnailViewer;

			var pagesCount = pdfViewer.pagesCount;
			for (var pageIndex = 0; pageIndex < pagesCount; pageIndex++) {
				var pageView = pdfViewer.getPageView(pageIndex);
				if (pageView && pageView.renderingState === DDocViewer.Enums.RenderingStates.FINISHED) {
					var thumbnailView = thumbnailViewer.getThumbnail(pageIndex);
					thumbnailView.setImage(pageView);
				}
			}

			thumbnailViewer.scrollThumbnailIntoView(this.page);
		},
		switchSidebarView: function(view, openSidebar) {
			if (openSidebar && !this.sidebarOpen) {
				document.getElementById('sidebarToggle').click();
			}
			
			var thumbsView = document.getElementById('thumbnailView');
			var dataView = document.getElementById('dataView');

			var thumbsButton = document.getElementById('viewThumbnail');
			var dataButton = document.getElementById('viewData');

			switch (view) {
				case 'thumbs':
					var wasAnotherViewVisible = thumbsView.classList.contains('hidden');

					thumbsButton.classList.add('toggled');
					dataButton.classList.remove('toggled');
					thumbsView.classList.remove('hidden');
					dataView.classList.add('hidden');

					this.forceRendering();

					if (wasAnotherViewVisible) {
						this.pdfThumbnailViewer.ensureThumbnailVisible(this.page);
					}
					break;

				case 'data':
					if (dataButton.disabled) {
						return;
					}
					thumbsButton.classList.remove('toggled');
					dataButton.classList.add('toggled');
					thumbsView.classList.add('hidden');
					dataView.classList.remove('hidden');
					break;
			}
		},
		parseQueryString: function(query) {
			var parts = query.split('&');
			var params = {};
			for (var i = 0, ii = parts.length; i < ii; ++i) {
				var param = parts[i].split('=');
				var key = param[0].toLowerCase();
				var value = param.length > 1 ? param[1] : null;
				params[decodeURIComponent(key)] = decodeURIComponent(value);
			}
			return params;
		},
		beforePrint: function() {
			if (!this.supportsPrinting) {
				var printMessage = Resources.printing_not_supported;
				this.error(printMessage);
				return;
			}

			var alertNotReady = false;
			var i, ii;
			if (!this.pagesCount) {
				alertNotReady = true;
			} else {
				for (i = 0, ii = this.pagesCount; i < ii; ++i) {
					if (!this.pdfViewer.getPageView(i).pdfPage) {
						alertNotReady = true;
						break;
					}
				}
			}
			if (alertNotReady) {
				var notReadyMessage = Resources.printing_not_ready;
				window.alert(notReadyMessage);
				return;
			}

			this.printing = true;
			this.forceRendering();

			var body = document.querySelector('body');
			body.setAttribute('data-mozPrintCallback', true);
			for (i = 0, ii = this.pagesCount; i < ii; ++i) {
				this.pdfViewer.getPageView(i).beforePrint();
			}

		},
		afterPrint: function() {
			var div = document.getElementById('printContainer');
			while (div.hasChildNodes()) {
				div.removeChild(div.lastChild);
			}

			this.printing = false;
			this.forceRendering();
		},
		setScale: function(value, resetAutoSettings) {
			this.updateScaleControls = !!resetAutoSettings;
			this.pdfViewer.currentScaleValue = value;
			this.updateScaleControls = true;
		},
		rotatePages: function(delta) {
			var pageNumber = this.page;
			this.pageRotation = (this.pageRotation + 360 + delta) % 360;
			this.pdfViewer.pagesRotation = this.pageRotation;
			this.pdfThumbnailViewer.pagesRotation = this.pageRotation;

			this.forceRendering();

			this.pdfViewer.scrollPageIntoView(pageNumber);
		}

	};

	window.PDFView = DDocViewer.PDFViewerApplication;

	document.addEventListener('DOMContentLoaded', function(evt) { DDocViewer.pdfWebViewerLoad(evt); }, true);

	document.addEventListener('pagerendered', function(e) {
		var pageNumber = e.detail.pageNumber;
		var pageIndex = pageNumber - 1;
		var pageView = DDocViewer.PDFViewerApplication.pdfViewer.getPageView(pageIndex);

		if (DDocViewer.PDFViewerApplication.sidebarOpen) {
			var thumbnailView = DDocViewer.PDFViewerApplication.pdfThumbnailViewer.
				getThumbnail(pageIndex);
			thumbnailView.setImage(pageView);
		}

		if (pageView.error) {
			DDocViewer.PDFViewerApplication.error(Resources.rendering_error, pageView.error);
		}

		if (pageNumber === DDocViewer.PDFViewerApplication.page) {
			var pageNumberInput = document.getElementById('pageNumber');
			pageNumberInput.classList.remove(DDocViewer.Constants.PAGE_NUMBER_LOADING_INDICATOR);
		}

	}, true);

	document.addEventListener('textlayerrendered', function(e) {
		var pageIndex = e.detail.pageNumber - 1;
		var pageView = DDocViewer.PDFViewerApplication.pdfViewer.getPageView(pageIndex);
		
		//AQUI
		var X1;
		var Y1;
		var X2;
		var Y2;
		
		if (document.getElementById('findInput').value == '' || document.getElementById('findInput').value == null ||
			document.getElementById('findInput').value == 'undefined') {

		}
		else
		{
				document.getElementById('findHighlightAll').checked = false;
				DDocViewer.PDFViewerApplication.findBar.open();
			//TODO: Revisar loop de busqueda de texto
				var cmd = (0) ;
				DDocViewer.PDFViewerApplication.findBar.dispatchEvent('again',
								cmd === 5 || cmd === 12);
		}

		$(function () {

			var im = document.getElementById('page1');
			var W = im.width / 2;
			var H = im.height / 2;

			$('#page1').imgAreaSelect({
				maxWidth: W, maxHeight: H, handles: true
			});
		});

		$(document).ready(function () {
			$('#page1').imgAreaSelect({
				onSelectEnd: function (img, selection) {

					X1 = document.getElementById('x1');
					Y1 = document.getElementById('y1');
					X2 = document.getElementById('x2');
					Y2 = document.getElementById('y2');
					X1.value = selection.x1;
					Y1.value = selection.y1;
					X2.value = selection.x2;
					Y2.value = selection.y2;
				}
			});
		});

	}, true);

	window.addEventListener('updateviewarea', function() {
		if (!DDocViewer.PDFViewerApplication.initialized) {
			return;
		}

		var location = DDocViewer.PDFViewerApplication.pdfViewer.location;

		var href = DDocViewer.PDFViewerApplication.getAnchorUrl(location.pdfOpenParams);
		document.getElementById('viewBookmark').href = href;

		var pageNumberInput = document.getElementById('pageNumber');
		var currentPage =
			DDocViewer.PDFViewerApplication.pdfViewer.getPageView(DDocViewer.PDFViewerApplication.page - 1);

		if (currentPage.renderingState === DDocViewer.Enums.RenderingStates.FINISHED) {
			pageNumberInput.classList.remove(DDocViewer.Constants.PAGE_NUMBER_LOADING_INDICATOR);
		} else {
			pageNumberInput.classList.add(DDocViewer.Constants.PAGE_NUMBER_LOADING_INDICATOR);
		}
	}, true);

	window.addEventListener('resize', function(evt) {
		if (DDocViewer.PDFViewerApplication.initialized &&
		(document.getElementById('pageAutoOption').selected ||
			document.getElementById('pageFitOption').selected ||
			document.getElementById('pageWidthOption').selected)) {
			var selectedScale = document.getElementById('scaleSelect').value;
			DDocViewer.PDFViewerApplication.setScale(selectedScale, false);
		}
		DDocViewer.updateViewarea();
	});

	window.addEventListener('hashchange', function(evt) {
		DDocViewer.PDFViewerApplication.setHash(document.location.hash.substring(1));
	});

	window.addEventListener('change', function(evt) {
		var files = evt.target.files;
		if (!files || files.length === 0) {
			return;
		}
		var file = files[0];

		if (!PDFJS.disableCreateObjectURL &&
			typeof URL !== 'undefined' && URL.createObjectURL) {
			DDocViewer.PDFViewerApplication.open(URL.createObjectURL(file), 0);
		} else {
			var fileReader = new FileReader();
			fileReader.onload = function(evt) {
				var buffer = evt.target.result;
				var uint8Array = new Uint8Array(buffer);
				DDocViewer.PDFViewerApplication.open(uint8Array, 0);
			};
			fileReader.readAsArrayBuffer(file);
		}

		document.getElementById('viewBookmark').setAttribute('hidden', 'true');
		document.getElementById('download').setAttribute('hidden', 'true');
	}, true);

	window.addEventListener('localized', function(evt) {
		DDocViewer.PDFViewerApplication.animationStartedPromise.then(function() {
			var container = document.getElementById('scaleSelectContainer');
			if (container.clientWidth === 0) {
				container.setAttribute('style', 'display: inherit;');
			}
			if (container.clientWidth > 0) {
				var select = document.getElementById('scaleSelect');
				select.setAttribute('style', 'min-width: inherit;');
				var width = select.clientWidth + DDocViewer.Constants.SCALE_SELECT_CONTAINER_PADDING;
				select.setAttribute('style', 'min-width: ' +
				(width + DDocViewer.Constants.SCALE_SELECT_PADDING) + 'px;');
				container.setAttribute('style', 'min-width: ' + width + 'px; ' +
					'max-width: ' + width + 'px;');
			}
		});
	}, true);

	window.addEventListener('scalechange', function(evt) {
		document.getElementById('zoomOut').disabled = (evt.scale === DDocViewer.Constants.MIN_SCALE);
		document.getElementById('zoomIn').disabled = (evt.scale === DDocViewer.Constants.MAX_SCALE);

		var customScaleOption = document.getElementById('customScaleOption');
		customScaleOption.selected = false;

		if (!DDocViewer.PDFViewerApplication.updateScaleControls &&
		(document.getElementById('pageAutoOption').selected ||
			document.getElementById('pageActualOption').selected ||
			document.getElementById('pageFitOption').selected ||
			document.getElementById('pageWidthOption').selected)) {
			DDocViewer.updateViewarea();
			return;
		}

		if (evt.presetValue) {
			DDocViewer.selectScaleOption(evt.presetValue);
			DDocViewer.updateViewarea();
			return;
		}

		var predefinedValueFound = DDocViewer.selectScaleOption('' + evt.scale);
		if (!predefinedValueFound) {
			var customScale = Math.round(evt.scale * 10000) / 100;
			customScaleOption.textContent =
				Resources.page_scale_percent.format(customScale);
			customScaleOption.selected = true;
		}
		DDocViewer.updateViewarea();
	}, true);

	window.addEventListener('pagechange', function(evt) {
		var page = evt.pageNumber;
		if (evt.previousPageNumber !== page) {
			document.getElementById('pageNumber').value = page;
			if (DDocViewer.PDFViewerApplication.sidebarOpen) {
				DDocViewer.PDFViewerApplication.pdfThumbnailViewer.scrollThumbnailIntoView(page);
			}
		}
		var numPages = DDocViewer.PDFViewerApplication.pagesCount;

		document.getElementById('previous').disabled = (page <= 1);
		document.getElementById('next').disabled = (page >= numPages);

		document.getElementById('firstPage').disabled = (page <= 1);
		document.getElementById('lastPage').disabled = (page >= numPages);

		if (evt.updateInProgress) {
			return;
		}
		if (this.loading && page === 1) {
			return;
		}
		DDocViewer.PDFViewerApplication.pdfViewer.scrollPageIntoView(page);
	}, true);

	window.addEventListener('DOMMouseScroll', function(evt) {
		DDocViewer.handleMouseWheel(evt);
	});

	window.addEventListener('mousewheel', function(evt) {
		DDocViewer.handleMouseWheel(evt);
	});

	window.addEventListener('keydown', function(evt) {
		if (DDocViewer.OverlayManager.active) {
			return;
		}

		var handled = false;
		var cmd = (evt.ctrlKey ? 1 : 0) |
		(evt.altKey ? 2 : 0) |
		(evt.shiftKey ? 4 : 0) |
		(evt.metaKey ? 8 : 0);

		var pdfViewer = DDocViewer.PDFViewerApplication.pdfViewer;

		if (cmd === 1 || cmd === 8 || cmd === 5 || cmd === 12) {
			switch (evt.keyCode) {
			case 70:
				if (!DDocViewer.PDFViewerApplication.supportsIntegratedFind) {

					DDocViewer.PDFViewerApplication.findBar.open();

					handled = true;
				}
				break;
			case 71:
				if (!DDocViewer.PDFViewerApplication.supportsIntegratedFind)
				{
					DDocViewer.PDFViewerApplication.findBar.dispatchEvent('again',
						cmd === 5 || cmd === 12);

					handled = true;
				}
				break;
			case 61:
			case 107:
			case 187:
			case 171:

				DDocViewer.PDFViewerApplication.zoomIn();

				handled = true;
				break;
			case 173:
			case 109:
			case 189:
				DDocViewer.PDFViewerApplication.zoomOut();

				handled = true;
				break;
			case 48:
			case 96:
				setTimeout(function() {
					DDocViewer.PDFViewerApplication.setScale(DDocViewer.Constants.DEFAULT_SCALE, true);
				});
				handled = false;

				break;
			}
		}

		if (cmd === 1 || cmd === 8) {
			switch (evt.keyCode) {
			case 83:
				DDocViewer.PDFViewerApplication.download();
				handled = true;
				break;
			}
		}

		if (cmd === 3 || cmd === 10) {
			switch (evt.keyCode) {
			case 80:
				handled = true;
				break;
			case 71:
				document.getElementById('pageNumber').select();
				handled = true;
				break;
			}
		}

		if (handled) {
			evt.preventDefault();
			return;
		}

		var curElement = document.activeElement || document.querySelector(':focus');
		var curElementTagName = curElement && curElement.tagName.toUpperCase();
		if (curElementTagName === 'INPUT' ||
			curElementTagName === 'TEXTAREA' ||
			curElementTagName === 'SELECT') {
			if (evt.keyCode !== 27) {
				return;
			}
		}

		if (cmd === 0) {
			switch (evt.keyCode) {
			case 38:
			case 33:
			case 8:
				if (DDocViewer.PDFViewerApplication.currentScaleValue !== 'page-fit') {
					break;
				}
			case 37:
				if (pdfViewer.isHorizontalScrollbarEnabled) {
					break;
				}
			case 75:
			case 80:
				DDocViewer.PDFViewerApplication.page--;
				handled = true;
				break;
			case 27:
				if (!DDocViewer.PDFViewerApplication.supportsIntegratedFind &&
					DDocViewer.PDFViewerApplication.findBar &&
					DDocViewer.PDFViewerApplication.findBar.opened) {
					DDocViewer.PDFViewerApplication.findBar.close();
					handled = true;
				}
				break;
			case 40:
			case 34:
			case 32:
				if (DDocViewer.PDFViewerApplication.currentScaleValue !== 'page-fit') {
					break;
				}
			case 39:
				if (pdfViewer.isHorizontalScrollbarEnabled) {
					break;
				}
			case 74:
			case 78:
				DDocViewer.PDFViewerApplication.page++;
				handled = true;
				break;

			case 36:
				if (DDocViewer.PDFViewerApplication.page > 1) {
					DDocViewer.PDFViewerApplication.page = 1;
					handled = true;
				}
				break;
			case 35:
				if (DDocViewer.PDFViewerApplication.pdfDocument && DDocViewer.PDFViewerApplication.page < DDocViewer.PDFViewerApplication.pagesCount) {
					DDocViewer.PDFViewerApplication.page = DDocViewer.PDFViewerApplication.pagesCount;
					handled = true;
				}
				break;

			case 72:
				DDocViewer.HandTool.toggle();

				break;
			case 82:
				DDocViewer.PDFViewerApplication.rotatePages(90);
				break;
			}
		}

		if (cmd === 4) {
			switch (evt.keyCode) {
			case 32:
				if (DDocViewer.PDFViewerApplication.currentScaleValue !== 'page-fit') {
					break;
				}
				DDocViewer.PDFViewerApplication.page--;
				handled = true;
				break;

			case 82: // 'r'
				DDocViewer.PDFViewerApplication.rotatePages(-90);
				break;
			}
		}

		if (!handled) {
			if (evt.keyCode >= 33 && evt.keyCode <= 40 &&
				!pdfViewer.containsElement(curElement)) {
				pdfViewer.focus();
			}
			if (evt.keyCode === 32 && curElementTagName !== 'BUTTON' &&
				!pdfViewer.containsElement(curElement)) {
				pdfViewer.focus();
			}
		}

		if (cmd === 2) {
			switch (evt.keyCode) {
			case 37:
				break;
			case 39:
				break;
			}
		}

		if (handled) {
			evt.preventDefault();
		}
	});

	window.addEventListener('beforeprint', function(evt) {
		DDocViewer.PDFViewerApplication.beforePrint();
	});

	window.addEventListener('afterprint', function(evt) {
		DDocViewer.PDFViewerApplication.afterPrint();
	});

	(function() {
		DDocViewer.PDFViewerApplication.animationStartedPromise = new Promise(
			function(resolve) {
				window.requestAnimationFrame(resolve);
			});
	})();

};

DDocUi.prototype.getFileName = function(url) {
	var anchor = url.indexOf('#');
	var query = url.indexOf('?');
	var end = Math.min(
		anchor > 0 ? anchor : url.length,
		query > 0 ? query : url.length);
	return url.substring(url.lastIndexOf('/', end) + 1, end);
};

DDocUi.prototype.getOutputScale = function (ctx) {
	var devicePixelRatio = window.devicePixelRatio || 1;
	var backingStoreRatio = ctx.webkitBackingStorePixelRatio || ctx.mozBackingStorePixelRatio ||
		ctx.msBackingStorePixelRatio || ctx.oBackingStorePixelRatio || ctx.backingStorePixelRatio || 1;
	var pixelRatio = devicePixelRatio / backingStoreRatio;
	return {
		sx: pixelRatio,
		sy: pixelRatio,
		scaled: pixelRatio !== 1
	};
};

DDocUi.prototype.scrollIntoView = function(element, spot) {
	var skipOverflowHiddenElements = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : false;

	var parent = element.offsetParent;
	if (!parent) {
		console.error('offsetParent is not set -- cannot scroll');
		return;
	}
	var offsetY = element.offsetTop + element.clientTop;
	var offsetX = element.offsetLeft + element.clientLeft;
	while (parent.clientHeight === parent.scrollHeight || skipOverflowHiddenElements && getComputedStyle(parent).overflow === 'hidden') {
		if (parent.dataset._scaleY) {
			offsetY /= parent.dataset._scaleY;
			offsetX /= parent.dataset._scaleX;
		}
		offsetY += parent.offsetTop;
		offsetX += parent.offsetLeft;
		parent = parent.offsetParent;
		if (!parent) {
			return;
		}
	}
	if (spot) {
		if (spot.top !== undefined) {
			offsetY += spot.top;
		}
		if (spot.left !== undefined) {
			offsetX += spot.left;
			parent.scrollLeft = offsetX;
		}
	}
	parent.scrollTop = offsetY;
};

DDocUi.prototype.watchScroll = function(viewAreaElement, callback) {
	var debounceScroll = function debounceScroll(evt) {
		if (rAF) {
			return;
		}
		rAF = window.requestAnimationFrame(function viewAreaElementScrolled() {
			rAF = null;
			var currentY = viewAreaElement.scrollTop;
			var lastY = state.lastY;
			if (currentY !== lastY) {
				state.down = currentY > lastY;
			}
			state.lastY = currentY;
			callback(state);
		});
	};
	var state = {
		down: true,
		lastY: viewAreaElement.scrollTop,
		_eventHandler: debounceScroll
	};
	var rAF = null;
	viewAreaElement.addEventListener('scroll', debounceScroll, true);
	return state;
};

DDocUi.prototype.binarySearchFirstItem = function(items, condition) {
	var minIndex = 0;
	var maxIndex = items.length - 1;
	if (items.length === 0 || !condition(items[maxIndex])) {
		return items.length;
	}
	if (condition(items[minIndex])) {
		return minIndex;
	}
	while (minIndex < maxIndex) {
		var currentIndex = minIndex + maxIndex >> 1;
		var currentItem = items[currentIndex];
		if (condition(currentItem)) {
			maxIndex = currentIndex;
		} else {
			minIndex = currentIndex + 1;
		}
	}
	return minIndex;
};

DDocUi.prototype.getVisibleElements = function(scrollEl, views, sortByVisibility) {

	var top = scrollEl.scrollTop, bottom = top + scrollEl.clientHeight;
	var left = scrollEl.scrollLeft, right = left + scrollEl.clientWidth;

	function isElementBottomBelowViewTop(view) {
		var element = view.div;
		var elementBottom =
			element.offsetTop + element.clientTop + element.clientHeight;
		return elementBottom > top;
	}

	var visible = [], view, element;
	var currentHeight, viewHeight, hiddenHeight, percentHeight;
	var currentWidth, viewWidth;
	var firstVisibleElementInd = (views.length === 0) ? 0 :
		ddoc.binarySearchFirstItem(views, isElementBottomBelowViewTop);

	for (var i = firstVisibleElementInd, ii = views.length; i < ii; i++) {
		view = views[i];
		element = view.div;
		currentHeight = element.offsetTop + element.clientTop;
		viewHeight = element.clientHeight;

		if (currentHeight > bottom) {
			break;
		}

		currentWidth = element.offsetLeft + element.clientLeft;
		viewWidth = element.clientWidth;
		if (currentWidth + viewWidth < left || currentWidth > right) {
			continue;
		}
		hiddenHeight = Math.max(0, top - currentHeight) +
			Math.max(0, currentHeight + viewHeight - bottom);
		percentHeight = ((viewHeight - hiddenHeight) * 100 / viewHeight) | 0;

		visible.push({
			id: view.id,
			x: currentWidth,
			y: currentHeight,
			view: view,
			percent: percentHeight
		});
	}

	var first = visible[0];
	var last = visible[visible.length - 1];

	if (sortByVisibility) {
		visible.sort(function(a, b) {
			var pc = a.percent - b.percent;
			if (Math.abs(pc) > 0.001) {
				return -pc;
			}
			return a.id - b.id;
		});
	}
	return { first: first, last: last, views: visible };
};

DDocUi.prototype.getPDFFileNameFromURL = function(url) {

	var reURI = /^(?:([^:]+:)?\/\/[^\/]+)?([^?#]*)(\?[^#]*)?(#.*)?$/;
	var reFilename = /[^\/?#=]+\.pdf\b(?!.*\.pdf\b)/i;
	var splitURI = reURI.exec(url);
	var suggestedFilename = reFilename.exec(splitURI[1]) ||
		reFilename.exec(splitURI[2]) ||
		reFilename.exec(splitURI[3]);
	if (suggestedFilename) {
		suggestedFilename = suggestedFilename[0];
		if (suggestedFilename.indexOf('%') !== -1) {
			try {
				suggestedFilename =
					reFilename.exec(decodeURIComponent(suggestedFilename))[0];
			} catch (e) {
			}
		}
	}
	return suggestedFilename || 'document.pdf';
};

DDocUi.prototype.updateViewarea = function() {
	if (!ddoc.PDFViewerApplication.initialized) {
		return;
	}
	ddoc.PDFViewerApplication.pdfViewer.update();
};

DDocUi.prototype.selectScaleOption = function(value) {
	var options = document.getElementById('scaleSelect').options;
	var predefinedValueFound = false;
	for (var i = 0; i < options.length; i++) {
		var option = options[i];
		if (option.value !== value) {
			option.selected = false;
			continue;
		}
		option.selected = true;
		predefinedValueFound = true;
	}
	return predefinedValueFound;
};

DDocUi.prototype.isAllWhitespace = function(str) {
	return !/\S/.test(str);
};

DDocUi.prototype.handleMouseWheel = function(evt) {
	var MOUSE_WHEEL_DELTA_FACTOR = 40;
	var ticks = (evt.type === 'DOMMouseScroll') ? -evt.detail :
		evt.wheelDelta / MOUSE_WHEEL_DELTA_FACTOR;
	var direction = (ticks < 0) ? 'zoomOut' : 'zoomIn';

	if (evt.ctrlKey || evt.metaKey) {
		evt.preventDefault();
		ddoc.PDFViewerApplication[direction](Math.abs(ticks));
	}
};

DDocUi.prototype.pdfWebViewerLoad = function(evt) {
	this.PDFViewerApplication.initialize().then(function() {
		ddoc.pdfWebViewerInitialized();
	});
};

DDocUi.prototype.pdfWebViewerInitialized = function() {

	var queryString = document.location.search.substring(1);
	var params = ddoc.PDFViewerApplication.parseQueryString(queryString);

	var fileInput = document.createElement('input');
	fileInput.id = 'fileInput';
	fileInput.className = 'fileInput';
	fileInput.setAttribute('type', 'file');
	document.body.appendChild(fileInput);

	if (!window.File || !window.FileReader || !window.FileList || !window.Blob) {

	} else {
		document.getElementById('fileInput').value = null;
	}

	var locale = PDFJS.locale || navigator.language;

	if (!ddoc.PDFViewerApplication.supportsPrinting) {
		document.getElementById('print').classList.add('hidden');
	}

	if (ddoc.PDFViewerApplication.supportsIntegratedFind) {
		document.getElementById('viewFind').classList.add('hidden');
	}

	PDFJS.UnsupportedManager.listen(ddoc.PDFViewerApplication.fallback.bind(ddoc.PDFViewerApplication));

	var mainContainer = document.getElementById('mainContainer');
	var outerContainer = document.getElementById('outerContainer');
	mainContainer.addEventListener('transitionend', function(e) {
		if (e.target === mainContainer) {
			var event = document.createEvent('UIEvents');
			event.initUIEvent('resize', false, false, window, 0);
			window.dispatchEvent(event);
			outerContainer.classList.remove('sidebarMoving');
		}
	}, true);

	document.getElementById('sidebarToggle').addEventListener('click', function() {
		this.classList.toggle('toggled');
		outerContainer.classList.add('sidebarMoving');
		outerContainer.classList.toggle('sidebarOpen');
		ddoc.PDFViewerApplication.sidebarOpen =
			outerContainer.classList.contains('sidebarOpen');
		if (ddoc.PDFViewerApplication.sidebarOpen) {
			ddoc.PDFViewerApplication.refreshThumbnailViewer();
		}
		ddoc.PDFViewerApplication.forceRendering();
	});

	document.getElementById('viewThumbnail').addEventListener('click',
	function () {
		ddoc.PDFViewerApplication.switchSidebarView('thumbs');
	});

	document.getElementById('viewData').addEventListener('click',
	  function () {
		  ddoc.PDFViewerApplication.switchSidebarView('data');
	  });

	document.getElementById('firstPage').addEventListener('click', function(evt) {
		ddoc.PDFViewerApplication.page = 1;
	});


	document.getElementById('previous').addEventListener('click', function() {
		ddoc.PDFViewerApplication.page--;
	});

	document.getElementById('next').addEventListener('click', function() {
		ddoc.PDFViewerApplication.page++;
	});

	document.getElementById('lastPage').addEventListener('click', function(evt) {
		if (ddoc.PDFViewerApplication.pdfDocument) {
			ddoc.PDFViewerApplication.page = ddoc.PDFViewerApplication.pagesCount;
	    }
	});

	document.getElementById('pageNumber').addEventListener('click', function() {
		this.select();
	});

	document.getElementById('pageNumber').addEventListener('change', function() {
		ddoc.PDFViewerApplication.page = (this.value | 0);
		if (this.value !== (this.value | 0).toString()) {
			this.value = ddoc.PDFViewerApplication.page;
		}
	});

	document.getElementById('zoomIn').addEventListener('click', function() {
		ddoc.PDFViewerApplication.zoomIn();
	});

	document.getElementById('zoomOut').addEventListener('click', function() {
		ddoc.PDFViewerApplication.zoomOut();
	});

	document.getElementById('scaleSelect').addEventListener('change', function() {
		ddoc.PDFViewerApplication.setScale(this.value, false);
	});

	document.getElementById('pageRotateCw').addEventListener('click', function(evt) {
		ddoc.PDFViewerApplication.rotatePages(90);
	});

	document.getElementById('pageRotateCcw').addEventListener('click', function(evt) {
		ddoc.PDFViewerApplication.rotatePages(-90);
	});

	document.getElementById('print').addEventListener('click', function(evt) {
		window.print();
	});

	document.getElementById('download').addEventListener('click', function (evt) {

		var continueFlow = true;

		if (ddoc.CustomDocumentDownload && typeof ddoc.CustomDocumentDownload === 'function') {
			continueFlow = ddoc.CustomDocumentDownload();
		}

		if (continueFlow) {
			ddoc.Download();
		}
	});

	if(ddoc.ViewerInitialized && typeof ddoc.ViewerInitialized == 'function')
		ddoc.ViewerInitialized();
};