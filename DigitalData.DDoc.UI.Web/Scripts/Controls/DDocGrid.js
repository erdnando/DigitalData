DDoc.Controls.Grid.prototype.init = function(columns, pageSize) {
	var self = this;
	var doc = document;
	var cons = DDoc.Constants.cellType;
	var createTable = function(cssClass) {
		var table = doc.createElement('table');
		table.setAttribute('cellpadding', '0');
		table.setAttribute('cellspacing', '0');
		table.setAttribute('table-layout', 'fixed');
		table.className = cssClass;

		if (doc.documentMode == 7) {
			table.style.borderCollapse = 'collapse';
		}

		return table;
	};
	var showMenu = function(e) {
		var pos = $(self.menuColumns).position();

		if (pos.top > 0) {
			$(self.menuColumns).slideUp('normal', function() { $(this).css({ 'top': '-4000px', 'left': '-4000px', 'display': 'block' }) });
		} else {
			var width, height;
			var left, top;
			var trigger = $(self.header).find('.columns-menu').get(0);

			$(self.menuColumns).find('input[type="checkbox"]')
				.each(function() {
					var column = self.tcontent.rows[0].cells[parseInt(this.value)];
					this.checked = (!$(column).hasClass('grid-hidden-col') && column.scrollWidth > 0);
				});

			if (typeof window.innerWidth == 'number') {
				height = window.innerHeight;
				width = window.innerWidth;
			} else {
				height = window.offsetHeight;
				width = window.offsetWidth;
			}

			top = getAbsoluteTop(trigger);
			left = getAbsoluteLeft(trigger);

			with (self.menuColumns) {
				hoffset = offsetHeight;
				woffset = offsetWidth;
				style.top = (top + hoffset > height) ? (top - hoffset + trigger.offsetHeight) + 'px' : (top + trigger.offsetHeight) + 'px';
				style.left = (left + woffset > width) ? (left - woffset) + 'px' : (left - (trigger.offsetWidth)) + 'px';
				style.display = 'none';
			}

			$(self.menuColumns).slideDown('normal');
		}

		e.stopPropagation();
	};
	var checkAllCells = function(check) {
		var checked = check.checked;
		var index = check.parentNode.cellIndex;

		for (var indexRow = 1, len = self.tcontent.rows.length; indexRow < len; indexRow++) {
			var cell = self.tcontent.rows[indexRow].cells[index];

			cell.firstChild.checked = checked;
		}

		self.fire({ type: 'onCheckAllCells', col: index, checked: checked });
	};
	var cursorChange = function(e) {
		var pos = (e.layerX) ? e.layerX : e.clientX;
		var cell = e.target;
		var width = self.grid.offsetLeft + cell.offsetLeft + cell.offsetWidth;
		var cols = self.columns.length - 1;

		cell.style.cursor = (pos > (width - 4) && cell.cellIndex < cols) ? 'E-resize' : 'default';
	};
	var doResizeColumn = function(e, cell, initWidth, xpos) {
		var newWidth = initWidth + (e.clientX - xpos);
		var index = cell.cellIndex;

		cell.style.cursor = 'E-resize';
		cell.style.width = newWidth + 'px';

		self.tcontent.rows[0].cells[index].style.width = newWidth + 'px';

		for (var indexCol = 0, len = self.tcontent.rows.length; indexCol < len; indexCol++) {
			self.tcontent.rows[indexCol].cells[index].style.width = newWidth + 'px';
		}
	};
	var stopColResize = function(e) {
		$(document.body).unbind('mousemove mouseup');

		e.stopPropagation();
	};
	var startColResize = function(e) {
		var cell = e.target;

		if (cell.style.cursor != 'default') {
			var xpos = e.clientX;
			var initWidth = cell.offsetWidth;
			var width = self.tcontent.offsetWidth;

			$(document.body).mousemove(function(e) { doResizeColumn(e, cell, initWidth, xpos, width); })
				.mouseup(function(e) { stopColResize(e); });
		}

		e.stopPropagation();
	};
	var changePageSize = function(newSize) {
		self.fire({ type: 'onChangePageSize', oldSize: self.pageSize, newSize: newSize });
		self.pageSize = newSize;
		self.currentPage = 0;
		self.getPage(1);
	};
	var gotoPage = function(index) {
		if (index >= 1 && index <= self.pages) {
			self.currentPage = index;
			self.getPage(-1);
		}
	};

	var menuTrigger = doc.createElement('span');
	menuTrigger.className = 'grid-cell-control columns-menu';
	menuTrigger.setAttribute('title', 'Mostrar u ocultar columnas');
	$(menuTrigger).click(function(e) { showMenu(e); });
	var frmTools = doc.createElement('form');
	frmTools.className = 'grid-tools';

	if (self.defaultData.showFilter) {
		var spanSeek = doc.createElement('span');
		spanSeek.className = 'grid-seek-icon';
		var textSeek = doc.createElement('input');
		textSeek.className = 'text-seeker';
		textSeek.setAttribute('type', 'text');
		textSeek.setAttribute('placeholder', 'Filtrar resultados');
		if (self.defaultData.defaultFilter) {
			$(textSeek).keyup(function(event) {
				window.clearTimeout(this.delay);
				this.delay = window.setTimeout(function() {
					self.seek(event.target.value);
				}, 100);
			});
		} else {
			$(textSeek).on('keyup', function(event) {
				window.clearTimeout(this.delay);
				this.delay = window.setTimeout(function() {
					self.fire({ type: 'onTextSeek', text: event.target.value });
				}, 500);
			});
		}
		frmTools.appendChild(spanSeek);
		frmTools.appendChild(textSeek);
	}

	if (self.defaultData.title) {
		var spanTitle = doc.createElement('span');
		spanTitle.className = 'grid-title-label';
		spanTitle.innerHTML = self.defaultData.title;
		frmTools.appendChild(spanTitle);
	}

	if (self.defaultData.showColumnMenu) {
		frmTools.appendChild(menuTrigger);
	}

	this.cacheRows = undefined;
	this.selectedRow = -1;
	this.pages = 0;
	this.pageSize = this.defaultData.pager.pageSize;
	this.currentPage = this.defaultData.pager.currentPage;
	this.menuColumns = doc.createElement('ul');
	this.menuColumns.className = 'menu-columns no-select';
	this.columns = columns;
	this.header = doc.createElement('div');
	this.header.className = 'grid-header';
	this.footer = doc.createElement('div');
	this.footer.className = 'grid-footer';
	this.content = doc.createElement('div');
	this.content.className = 'grid-content';

	var footRow = doc.createElement('tr');
	var footRowLayout = doc.createElement('tr');
	var contentRowLayout = doc.createElement('tr');
	var headRow = doc.createElement('tr');

	var contentBody = doc.createElement('tbody');
	contentBody.appendChild(contentRowLayout);
	contentBody.appendChild(headRow);
	$(contentBody).addClass('no-data');

	var skipSpan = false;
	var skipCounter = 0;
	var span = 0;

	for (var index = 0, len = columns.length; index < len; index++) {
		var column = $.extend(true, {}, this.columnDefault, columns[index]);

		if (column.hidden)
			continue;

		var contentLayoutCell = doc.createElement('th');
		var cssClass = (column.sort.sortable) ? (column.sort.sorted == 1) ? ' column-sorted-asc' :
		(column.sort.sorted == -1) ? ' column-sorted-desc' : '' : '';

		if (span == 0 && column.colSpan > 1) {
			span = column.colSpan;
			skipCounter = 1;
		}

		if (!skipSpan) {
			var text = doc.createElement('p');
			text.className = 'text-cell';
			text.innerHTML = column.text;
			text.setAttribute('title', column.text);
			var cellContainer = doc.createElement('div');
			cellContainer.className = 'cell-content';
			cellContainer.appendChild(text);
			var cell = doc.createElement('td');
			cell.className = 'grid-column';
			cell.appendChild(cellContainer);

			$(cell).mousemove(function(e) { cursorChange(e); })
				.mousedown(function(e) { startColResize(e); });

			if (column.cellType.type == cons.NORMAL) {
				if (!column.disableHider) {
					var cellHider = doc.createElement('span');
					cellHider.className = 'cell-hider';
					cellHider.setAttribute('rel', index);
					cellHider.setAttribute('title', 'Click aquí para ocultar la columna');
					$(cellHider).click(function(e) {
						self.hiddeColumn(parseInt($(this).attr('rel')));
						var pos = $(self.menuColumns).position();

						if (pos.top > 0) {
							$(self.menuColumns).slideUp('normal', function() {
								$(this).css({
									'top': '-1000px',
									'left': '-4000px',
									'display': 'block'
								});
							});
						}
						e.stopPropagation();
					});

					cellContainer.appendChild(cellHider);

					var chkItem = doc.createElement('input');
					chkItem.setAttribute('type', 'checkbox');
					chkItem.value = index;
					chkItem.checked = column.visible;
					$(chkItem).click(function(e) {
						if (this.checked) {
							self.showColumn(this.value);
						} else {
							self.hiddeColumn(this.value);
						}
						e.stopPropagation();
					});
					var lbl = doc.createElement('label');
					lbl.className = 'label-option-menu';
					lbl.innerHTML = column.text;
					$(lbl).click(function(e) { e.stopPropagation(); });
					$(lbl.firstChild).before(chkItem);
					var item = doc.createElement('li');
					item.appendChild(lbl);
					this.menuColumns.appendChild(item);
				}

				if (column.sort.sortable) {
					var cellSort = doc.createElement('span');
					cellSort.className = 'grid-cell-control sort-cell-flag' + cssClass;
					cellSort.setAttribute('title', 'Click aquí para ordenar los datos');
					$(cellSort).click(function() {
						if (self.defaultData.serverSortable) {
							self.serverSort($(this).parents('.grid-column').get(0));
						} else {
							self.sort($(this).parents('.grid-column').get(0));
						}
					});

					cellContainer.appendChild(cellSort);

					$(cell).data('cellHeaderData', { type: column.sort.sorted, col: column });
				} else {
					$(cellContainer).addClass('cell-without-order');
				}
			} else {
				$(cellContainer).addClass('cell-control');
			}

			if (span > 1) {
				cell.setAttribute('colspan', span);
				skipSpan = true;
			}

			headRow.appendChild(cell);
		} else {
			skipCounter++;
		}

		if (column.cellType.type == cons.CHECKBOX) {
			var check = doc.createElement('input');
			check.setAttribute('type', 'checkbox');
			check.className = 'cell-check';
			cell.insertBefore(check, cell.firstChild);
			$(check).click(function(event) { checkAllCells(this, event); });
		}

		if (column.width > 0) {
			var width = column.width.toString();
			contentLayoutCell.style.width = width + 'px';
		}

		if (!column.visible) {
			$(cell).addClass('grid-hidden-col');
			$(contentLayoutCell).addClass('grid-hidden-col');
		}

		contentRowLayout.appendChild(contentLayoutCell);

		if (skipSpan) {
			skipSpan = (skipCounter == span) ? false : skipSpan;

			if (!skipSpan) {
				span = 0;
				skipCounter = 0;
			}
		}
	}

	if (this.defaultData.pager.show) {
		var first = doc.createElement('span');

		first.className = 'grid-button-pager grid-button-first grid-button-disabled';
		$(first).click(function() { self.getPage(1, this); });
		var prev = doc.createElement('span');

		prev.className = 'grid-button-pager grid-button-prev grid-button-disabled';
		$(prev).click(function() { self.getPage(2, this); });
		var next = doc.createElement('span');

		next.className = 'grid-button-pager grid-button-next grid-button-disabled';
		$(next).click(function() { self.getPage(3, this); });
		var last = doc.createElement('span');

		last.className = 'grid-button-pager grid-button-last grid-button-disabled';
		$(last).click(function() { self.getPage(4, this); });
		var textPage = doc.createElement('input');
		textPage.setAttribute('type', 'text');
		textPage.className = 'grid-pager-page';
		$(textPage).focus(function() {
			this.select();
		}).keypress(function(event) {
			var key = event.which;
			var charKey = String.fromCharCode(key);

			if (event.which == 13) {
				gotoPage(parseInt(this.value));
				return true;
			}

			if (event.ctrlKey || event.altKey || key < 32) {
				return true;
			}

			if ('0123456789'.indexOf(charKey) > -1) {
				return true;
			}
		});
		var lblPage = doc.createElement('span');
		lblPage.className = 'grid-page-label';
		lblPage.innerHTML = 'Registros por página:';
		var pageSizeSelector = doc.createElement('select');
		pageSizeSelector.className = 'grid-pager-sizer';
		$(pageSizeSelector).change(function() { changePageSize(parseInt($(this).val())); });
		var pagerControls = doc.createElement('div');
		pagerControls.className = 'grid-pager-controls';
		pagerControls.appendChild(first);
		pagerControls.appendChild(prev);
		pagerControls.appendChild(textPage);
		pagerControls.appendChild(next);
		pagerControls.appendChild(last);
		pagerControls.appendChild(lblPage);
		pagerControls.appendChild(pageSizeSelector);

		this.pager = doc.createElement('div');
		this.pager.className = 'grid-pager';
		this.pager.appendChild(pagerControls);
		this.footer.appendChild(this.pager);

		$.each(this.defaultData.pager.sizes, function(index, item) {
			var $item = $('<option></option>').text(item).val(item);

			$(pageSizeSelector).append($item);

			if (self.defaultData.pager.pageSize == item) {
				$item.attr('selected', 'selected');
			}
		});
	}

	if (this.defaultData.showTools) {
		this.header.appendChild(frmTools);
	}

	this.tcontent = createTable('grid-table-content');
	this.tcontent.appendChild(contentBody);
	this.content.appendChild(this.tcontent);

	this.grid.appendChild(this.header);
	this.grid.appendChild(this.content);
	this.grid.appendChild(this.footer);
	$(this.grid).addClass('grid');

	doc.body.appendChild(this.menuColumns);

	this.scroller = function() {
		self.header.scrollLeft = self.content.scrollLeft;
	};
	this.content.onscroll = function() { self.scroller(); };
	this.autoHeight();

	$(doc.body).click(function() {
		var pos = $(self.menuColumns).position();

		if (pos.top > 0) {
			$(self.menuColumns).slideUp('normal', function() { $(this).css({ 'top': '-1000px', 'left': '-4000px', 'display': 'block' }) });
		}
	});
};

DDoc.Controls.Grid.prototype.hiddeColumn = function(index) {
	for (var indexRow = 0, len = this.tcontent.rows.length; indexRow < len; indexRow++) {
		$(this.tcontent.rows[indexRow].cells[index]).addClass('grid-hidden-col');
	}
};

DDoc.Controls.Grid.prototype.showColumn = function(index) {
	for (var indexRow = 0, len = this.tcontent.rows.length; indexRow < len; indexRow++) {
		$(this.tcontent.rows[indexRow].cells[index]).removeClass('grid-hidden-col');
	}
};

DDoc.Controls.Grid.prototype.hiddenRow = function(key) {
	$('#' + key).addClass('grid-hidden-row');
	this.getPage(0);
};

DDoc.Controls.Grid.prototype.showRow = function(key) {
	$('#' + key).removeClass('grid-hidden-row');
	this.getPage(0);
};

DDoc.Controls.Grid.prototype.loadRows = function(rows) {
	var tbody = this.tcontent.firstChild;

    if (!rows || rows.length == 0) {
		$(tbody).addClass('no-data');
		this.hideLoader(this.defaultData.loaderClass);
		this.fire({ type: 'onLoadComplete', rows: 0 });
		this.autoHeight();
		return this;
	}

	$(tbody).removeClass('no-data');

	for (var indexRow = 0, len = rows.length; indexRow < len; indexRow++) {
        var dataRow = rows[indexRow];
        this.loadRow(dataRow);
    }

	if (!this.defaultData.pager.onServer) {
		if (this.defaultData.pager.show) {
			this.getPage(0);
		} else {
			this.autoHeight();
		}
	}

	this.selectedRow = -1;
	this.hideLoader(this.defaultData.loaderClass);
	this.fire({ type: 'onLoadComplete', rows: rows.length });
};

DDoc.Controls.Grid.prototype.loadRow = function (dataRow) {
    var self = this;
    var doc = document;
    var cons = DDoc.Constants.cellType;
    var tbody = this.tcontent.firstChild;
    var formatCurrency = function (n) {
        return '$ ' + n.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    };
    var filterCurrency = function (e, decimalPlaces) {
        var code = e.which;
        var key = String.fromCharCode(code);
        var field = e.target;
        var isControlKey = (code == 0 || code == 8 || code == 37 || code == 39 || code == 46);
        var valid = (key == '.') ? field.value.indexOf('.') == -1 :
            (!isControlKey) ? /[0-9]|\./.test(key) : !/[°|!\"#$%&\/()\'=?¿¡\[\]\*]/.test(key);
        var valueParts = field.value.split('.');
        var len = (field.setSelectionRange) ? (field.selectionEnd - field.selectionStart) : document.selection.createRange().text.length;

        if (valid && !isControlKey && valueParts.length > 1 && len == 0) {
            valid = valueParts[1].length < decimalPlaces;
        }

        return valid;
    };

	var key = '';
    var isDetailHandler = false;
    var trow = doc.createElement('tr');
    var $trow = $(trow);
    $trow.data('dataRow', dataRow)
        .click(function(e) { self.selectRow(this, e); })
        .dblclick(function() {
			self.fire({ type: 'onRowDoubleClick', rowData: dataRow });
        });

    for (var indexCol = 0, lenCols = this.columns.length; indexCol < lenCols; indexCol++) {
        var isHidden = $(this.tcontent.rows[0].cells[indexCol]).hasClass('grid-hidden-col');
        var cell = doc.createElement('td');
        var column = $.extend(true, {}, this.columnDefault, this.columns[indexCol]);

        if (column.hidden)
            continue;

        var value = 'dataRow.' + column.field;
        var cellValue = (column.field && column.field.length > 0) ? eval(value) : '';

        cellValue = (cellValue == undefined || cellValue == null) ? '' : cellValue;
        this.columns[indexCol].Id = eval('dataRow.' + column.fieldId);

        var pagetarget = '';

        key += (column.iskey) ? cellValue : '';

        if (!(cellValue instanceof Date)) {
            if (!isNaN(cellValue) && cellValue != '' && cellValue != null && cellValue !== true && cellValue !== false) {
                cellValue = parseFloat(cellValue);
            }
        }

        switch (true) {
            case (typeof cellValue === 'string'):

                if (cellValue.substring(0, 3) == '01D') {
                    pagetarget += cellValue.substring(10, cellValue.length);
                    cellValue = cellValue.substring(0, 10);
                }

                if (cellValue === 'True') {
                    cellValue = 'Si';
                }
                if (cellValue === 'False') {
                    cellValue = 'No';
                }
                break;

            case (typeof cellValue === 'boolean' && column.cellType.type == cons.NORMAL):
                cellValue = (cellValue) ? 'Si' : 'No';
                break;

            case (typeof cellValue === 'number'):
                cellValue = (column.currency) ? formatCurrency(cellValue) : cellValue;
                break;

            case (typeof cellValue == 'object' && typeof cellValue.getDate == 'function'):
                cellValue = cellValue.format(column.formatDate);
                break;
        }

        if (column.columnType == DDoc.Constants.columnType.NUMBER) {
            $(cell).addClass('grid-numeric-cell');
        }

        if (column.fieldValueReplacement && $.isFunction(column.fieldValueReplacement)) {
            cellValue = column.fieldValueReplacement.call(this, dataRow, cellValue);
        }

        var $labelSpan = $('<span>').addClass('cell-label');
        var columnTitle = this.columns[indexCol].text;

        cell.innerHTML = cellValue;

        if (columnTitle) {
            $labelSpan.html(columnTitle + ': ');
            cell.insertBefore($labelSpan[0], cell.firstChild);
        }

        switch (column.cellType.type) {
            case cons.NORMAL:
                if (this.defaultData.detail.field != '' && !isDetailHandler) {
                    var rowHandler = doc.createElement('span');
                    rowHandler.className = 'grid-cell-control grid-handler-detail';
                    rowHandler.setAttribute('title', 'Mostrar detalle');
                    $(rowHandler).click(function () {
                        var $this = $(this);
                        var row = this.parentNode.parentNode;

                        $this.toggleClass('grid-detail-expanded');

                        if ($this.hasClass('grid-detail-expanded')) {
                            $this.attr('title', 'Ocultar detalle');
                        } else {
                            $this.attr('title', 'Mostrar detalle');
                        }

                        $(row).next('[rel="' + row.id + '"]').toggle('fast', function () {
                            self.autoHeight();
                        });
                    });
                    $(cell.firstChild).before(rowHandler);
                    isDetailHandler = true;

                }

                $(cell).addClass('cell-data');

                if (!column.tileVisibility) {
                    $(cell).addClass('hide-on-tiles');
                }

                break;

            case cons.CHECKBOX:
                var check = doc.createElement('input');
                check.className = 'cell-check';
                check.setAttribute('type', 'checkbox');
                check.checked = cellValue;
                $(check).click(function (event) { self.checkCell(this, event); });
                cell.innerHTML = '';
                cell.appendChild(check);
                break;

            case cons.RADIO:
                var radio = doc.createElement('input');
                radio.className = 'cell-radio';
                radio.setAttribute('type', 'radio');
                radio.setAttribute('name', 'GridRadio');
                radio.setAttribute('value', cellValue);
                $(radio).click(function (event) { self.checkRadio(this, event); });
                cell.innerHTML = '';
                cell.appendChild(radio);
                break;

			case cons.ACTION:
                var cssClass;
				if (typeof column.cellType.cssClass == 'function') {
                    cssClass = column.cellType.cssClass(cellValue);
                } else {
					cssClass = (column.cellType.cssClass && typeof cellValue === 'boolean') ? (cellValue) ? column.cellType.cssClass.active : column.cellType.cssClass.inactive : (column.cellType.cssClass) ? column.cellType.cssClass.normal : '';
				}
                var tooltip;
				if (typeof column.cellType.tooltip == 'function') {
                    tooltip = column.cellType.cssClass(cellValue);
                } else {
                    tooltip = ($.isFunction(column.cellType.tooltip)) ? column.cellType.tooltip.call(this, cellValue) : column.cellType.tooltip;
                }
                var action = doc.createElement('span');
                action.className = 'grid-action';
                action.setAttribute('title', tooltip);
                action.className += (' ' + cssClass);
                $(action).click(function (event) { self.execAction(this, event); });
                cell.innerHTML = '';
                cell.appendChild(action);

                break;

            case cons.THUMBNAIL:
                var cssClass = (column.cellType.cssClass &&
                    typeof cellValue === 'boolean') ? (cellValue) ? column.cellType.cssClass.active
                        : column.cellType.cssClass.inactive
                    : (column.cellType.cssClass) ? column.cellType.cssClass.normal : '';
                var tooltip = ($.isFunction(column.cellType.tooltip)) ? column.cellType.tooltip.call(this, cellValue) : column.cellType.tooltip;
                var thumbnail = doc.createElement('div');
                thumbnail.className = 'grid-thumbnail';
                thumbnail.setAttribute('title', tooltip);
                thumbnail.className += (' ' + cssClass);
                $(thumbnail).click(function (event) { self.execAction(this, event); });

                var loadingImage = document.createElement('span');
                loadingImage.className = 'grid-thumbnail-loading';

                $(thumbnail).append(loadingImage);

                cell.innerHTML = '';
                cell.appendChild(thumbnail);

                break;

            case cons.THUMBNAILLINK:
                var link = doc.createElement('a');
                //link 
                var url = column.cellType.url.format(cellValue) + pagetarget;
                link.setAttribute('href', url);

                if (column.cellType.target && typeof column.cellType.target == 'string') {
                    link.setAttribute('target', column.cellType.target);
                }

                var cssClass = (column.cellType.cssClass &&
                    typeof cellValue === 'boolean') ? (cellValue) ? column.cellType.cssClass.active
                        : column.cellType.cssClass.inactive
                    : (column.cellType.cssClass) ? column.cellType.cssClass.normal : '';
                var tooltip = ($.isFunction(column.cellType.tooltip)) ? column.cellType.tooltip.call(this, cellValue) : column.cellType.tooltip;

                var thumbnail = doc.createElement('div');
                thumbnail.className = 'grid-thumbnail';
                thumbnail.setAttribute('title', tooltip);
                thumbnail.className += (' ' + cssClass);
                $(thumbnail).click(function (event) { self.execAction(this, event); });

                var loadingImage = document.createElement('span');
                loadingImage.className = 'grid-thumbnail-loading';

                $(thumbnail).append(loadingImage);

                link.setAttribute('title', tooltip);

                link.innerHTML = thumbnail.outerHTML;

                cell.innerHTML = '';
                cell.appendChild(link);

                break;

            case cons.BUTTON:
                var button = doc.createElement('input');
                button.setAttribute('type', 'button');
                button.setAttribute('value', (column.cellType.text) ? column.cellType.text : '');
                $(button).click(function (e) { self.execActionButton(this, e); });
                cell.innerHTML = '';
                cell.appendChild(button);

                break;

            case cons.COMBO:
                var cbo = doc.createElement('select');
                cbo.className = 'grid-data-combo';
                $(cbo).data('cboData', column);

                $.each(column.cellType.combo.items, function (index, item) {
                    var value = eval('this.' + column.cellType.combo.fieldValue);
                    var text = eval('this.' + column.cellType.combo.fieldText);

                    var element = document.createElement('option');
                    element.value = value;
                    element.innerHTML = text;
                    element.setAttribute('title', text);
                    cbo.appendChild(element);

                    if (index == 0) {
                        eval('dataRow.' + column.cellType.combo.fieldSet + ' = "' + value + '"');
                    }

                    if (text == cellValue || value == cellValue) {
                        element.setAttribute('selected', 'selected');
                        eval('dataRow.' + column.cellType.combo.fieldSet + ' = "' + value + '"');
                    }
                });

                $(cbo).change(function () {
                    var option = this.options[this.selectedIndex];
                    var value = option.value;
                    var data = $(this.parentNode.parentNode).data('dataRow');
                    var column = $(this).data('cboData');

                    eval('data.' + column.cellType.combo.fieldSet + ' = "' + value + '"');
                });

                if (!column.cellType.initStateEnabled) {
                    cbo.setAttribute('disabled', 'disabled');
                }

                cell.innerHTML = '';
                cell.appendChild(cbo);
                break;

            case cons.TEXTBOX:
                var txt = doc.createElement('input');
                txt.setAttribute('type', 'text');
                txt.className = 'grid-text-field';
                txt.value = cellValue;
                $(txt).data('txtField', { column: column, index: indexCol })
                    .keyup(function () {
                        var cell = this.parentNode;
                        var row = cell.parentNode;
                        var col = cell.cellIndex;
                        self.fire({ type: 'onKeyUp', textbox: this, value: this.value.replace(/[,$\s]/g, ''), dataRow: $(row).data('dataRow'), cell: cell, row: row, col: col });
                    })
                    .bind('blur', function (e) {
                        var txtData = $(this).data('txtField');
                        var fieldName = txtData.column.field;
                        var data = $(this.parentNode.parentNode).data('dataRow');
                        var valueField = this.value;

                        if (txtData.column.currency) {
                            this.value = (this.value != '') ? this.value : '0';

                            valueField = parseFloat(this.value.replace(/[,$\s]/g, ''));
                            eval('data.' + fieldName + ' = ' + valueField);
                            this.value = formatCurrency(valueField);
                        } else {
                            eval('data.' + fieldName + ' = "' + this.value + '"');
                        }

                        var eventArgs = { type: 'onEditedField', value: valueField, dataRow: data, textbox: this, column: txtData.index, cancel: false };
                        self.fire(eventArgs);

                        if (eventArgs.cancel) {
                            e.preventDefault();
                            e.stopPropagation();

                            return false;
                        }
                    });

                if (column.columnType == DDoc.Constants.columnType.NUMBER) {
                    $(txt).addClass('grid-numeric-cell')
                        .keypress(function (e) { return filterCurrency(e, 2); })
                        .focus(function (e) { this.value = this.value.replace(/[,$\s]/g, ''); })
                        .click(function (e) {
                            var cell = this.parentNode;
                            var row = cell.parentNode;
                            self.fire({ type: 'onClick', textbox: this, value: this.value.replace(/[,$\s]/g, ''), dataRow: $(row).data('dataRow'), cell: cell, row: row, col: cell.cellIndex });
                        });
                }

                if (!column.cellType.initStateEnabled) {
                    txt.setAttribute('disabled', 'disabled');
                }

                cell.innerHTML = '';
                cell.appendChild(txt);
                break;

            case cons.TEXTLINK:
                var link = doc.createElement('a');
                var url = column.cellType.url.format(cellValue);
                link.setAttribute('href', url);

                if (column.cellType.target && typeof column.cellType.target == 'string') {
                    link.setAttribute('target', column.cellType.target);
                }
                link.innerHTML = cellValue;
                cell.innerHTML = '';
                cell.appendChild(link);
                break;

            case cons.ICONLINK:
                var link = doc.createElement('a');
                var url = column.cellType.url.format(cellValue) + pagetarget;;
                link.setAttribute('href', url);

                if (column.cellType.target && typeof column.cellType.target == 'string') {
                    link.setAttribute('target', column.cellType.target);
                }

                var cssClass = (column.cellType.cssClass &&
                    typeof cellValue === 'boolean') ? (cellValue) ? column.cellType.cssClass.active
                        : column.cellType.cssClass.inactive
                    : (column.cellType.cssClass) ? column.cellType.cssClass.normal : '';
                var tooltip = ($.isFunction(column.cellType.tooltip)) ? column.cellType.tooltip.call(this, cellValue) : column.cellType.tooltip;

                var icon = doc.createElement('span');
                icon.className = 'grid-action';
                icon.className += (' ' + cssClass);
                $(icon).click(function (event) { self.execAction(this, event); });

                link.setAttribute('title', tooltip);

                var wrap = document.createElement('div');
                wrap.appendChild(icon);

                link.innerHTML = wrap.innerHTML;

                cell.innerHTML = '';
                cell.appendChild(link);
                break;

        }

        if (column.tooltip) {


            var valcell = cellValue;
            if (typeof valcell == 'string') {
                valcell = replaceAll(valcell, '<mark>', '');
                valcell = replaceAll(valcell, '</mark>', '');
                valcell = replaceAll(valcell, '<strong>', '');
                valcell = replaceAll(valcell, '</strong>', '');
                valcell = replaceAll(valcell, '</br>', '\n');
                valcell = replaceAll(valcell, '<i>', '');
                valcell = replaceAll(valcell, '</i>', '');
            }

            cell.setAttribute('title', valcell);
        }

        if (!column.visible || isHidden) {
            $(cell).addClass('grid-hidden-col');
        }

        trow.appendChild(cell);
    }

    if (key.length > 0) trow.id = key;

    tbody.appendChild(trow);

    this.fire({ type: 'onRowLoaded', row: trow, dataRow: dataRow });

    if (this.defaultData.detail.field != '') {
        this.loadDetail(tbody, doc, trow, dataRow);
    }
}

function replaceAll(string, find, replace) {
	return string.replace(new RegExp(escapeRegExp(find), 'g'), replace);
}

function escapeRegExp(string) {
	return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, '\\$1');
}

DDoc.Controls.Grid.prototype.loadThumbnails = function(thumbnailIdDataProperty) {

	var self = this;
	self.thumbnailLoadQueue = $.jqmq({
		delay: -1,
		batch: 1,
		callback: function(item) {
			var thumbnailId = eval('$(item).closest(\'tr\').data(\'dataRow\').' + thumbnailIdDataProperty);

			var thumbnail = new Image();
			thumbnail.onload = function() {
				$(item).find('.grid-thumbnail-loading').remove();
				var thbImage = $('<img>').attr('id', thumbnailId);
				$(item).append(thbImage);
				$('#' + thumbnailId).attr('src', thumbnail.src);
				self.thumbnailLoadQueue.next();
			};
			thumbnail.onerror = function() {
				$(item).find('.grid-thumbnail-loading').remove();
				var errorThb = $('<img>').attr('id', thumbnailId);
				$(item).append(errorThb);
				$('#' + thumbnailId).attr('src', thumbnail.src);
				self.thumbnailLoadQueue.next();
			};
			thumbnail.src = self.defaultData.thumbnailSrc.format(thumbnailId);
		}
	});

	$('.imgThumb', $(self.content)).jqmqAddEach(self.thumbnailLoadQueue);
};

DDoc.Controls.Grid.prototype.showLoader = function(loaderClass) {

	var contentArea = this.content;

	var loader = document.createElement('div');
	loader.className = this.defaultData.loaderClass;
	$('.grid-table-content', contentArea).hide();
	$(contentArea).prepend(loader);

};

DDoc.Controls.Grid.prototype.hideLoader = function (loaderClass) {
	$('.grid-table-content', this.content).show();
	loaderClass = this.defaultData.loaderClass
	$('.' + loaderClass, this.content).remove();
};

DDoc.Controls.Grid.prototype.loadDetail = function(tbody, doc, parent, dataRow) {
	var cons = DDoc.Constants.cellType;
	var rows = eval('dataRow.' + this.defaultData.detail.field);
	var createTable = function(cssClass) {
		var table = doc.createElement('table');
		table.setAttribute('cellpadding', '0');
		table.setAttribute('cellspacing', '0');
		table.setAttribute('table-layout', 'fixed');
		table.className = cssClass;

		if (doc.documentMode == 7) {
			table.style.borderCollapse = 'collapse';
		}

		return table;
	};

	if (rows && rows.length > 0) {
		var trh = doc.createElement('tr');
		var tdhead = doc.createElement('thead');
		tdhead.appendChild(trh);
		var tdbody = doc.createElement('tbody');
		var tblDetail = createTable('grid-table-detail');
		tblDetail.appendChild(tdhead);
		tblDetail.appendChild(tdbody);
		var td = doc.createElement('td');
		td.className = 'grid-detail-cell';
		td.appendChild(tblDetail);
		var tr = doc.createElement('tr');
		tr.appendChild(td);
		tr.style.display = 'none';
		tr.setAttribute('rel', parent.id);

		td.setAttribute('colspan', this.columns.length);
		for (var index = 0, len = this.defaultData.detail.columns.length; index < len; index++) {
			var column = this.defaultData.detail.columns[index];
			var th = doc.createElement('th');
			th.innerHTML = column.text;
			trh.appendChild(th);
		}

		for (var index = 0, lenCols = rows.length; index < lenCols; index++) {
			var data = rows[index];
			var row = doc.createElement('tr');

			for (var indexCol = 0, len = this.defaultData.detail.columns.length; indexCol < len; indexCol++) {
				var column = $.extend(true, {}, this.columnDefault, this.defaultData.detail.columns[indexCol]);
				var value = eval('data.' + column.field);
				var cell = doc.createElement('td');
				cell.innerHTML = (value) ? value : '';

				switch (column.cellType.type) {
					/*case cons.CHECKBOX:
						var check = doc.createElement('input');
							check.className = 'cell-check';
							check.setAttribute('type', 'checkbox');
							check.checked = cellValue;
							$(check).click(function(event){ self.checkCell(this, event); });
						cell.innerHTML = '';
						cell.appendChild(check);
						break;

					case cons.RADIO:
						var radio = doc.createElement('input');
							radio.className = 'cell-radio';
							radio.setAttribute('type', 'radio');
							radio.setAttribute('name', 'GridRadio');
							radio.setAttribute('value', cellValue);
							$(radio).click(function(event){ self.checkRadio(this, event); });
						cell.innerHTML = '';
						cell.appendChild(radio);
						break;*/
					case cons.ACTION:
						var cssClass = (column.cellType.cssClass &&
								typeof cellValue === 'boolean') ? (cellValue) ? column.cellType.cssClass.active
							: column.cellType.cssClass.inactive
							: (column.cellType.cssClass) ? column.cellType.cssClass.normal : '';
						var action = doc.createElement('span');
						action.className = 'grid-action';
						action.setAttribute('title', column.cellType.tooltip);
						action.className += (' ' + cssClass);
						$(action).click(function(event) { self.execAction(this, event); });
						cell.innerHTML = '';
						cell.appendChild(action);

						break;

					/*					case cons.BUTTON:
														var button = doc.createElement('input');
															button.setAttribute('type', 'button');
															button.setAttribute('value', (column.cellType.text) ? column.cellType.text: '');
															$(button).click(function(e){ self.execActionButton(this, e); });
														cell.innerHTML = '';
														cell.appendChild(button);
	
														break;
	
													case cons.COMBO:
														var cbo = doc.createElement('select');
															cbo.className = 'grid-data-combo';
	
														$.each(column.cellType.combo.items, function (index, item) {
															var value = eval('this.' + column.cellType.combo.fieldValue);
															var text = eval('this.' + column.cellType.combo.fieldText);
	
															var element = document.createElement('option');
																element.value = value;
																element.innerHTML = text;
																element.setAttribute('title', text);
															cbo.appendChild(element);
	
															if (text == cellValue){
																element.setAttribute('selected', 'selected');
															}
														});
	
														$(cbo).change(function () {
															var option = this.options[this.selectedIndex];
															var value = option.value;
															var data = $(cell.parentNode).data('dataRow');
	
															eval('data.' + column.cellType.combo.fieldSet + ' = "' + value + '"');
														});
	
														cell.innerHTML = '';
														cell.appendChild(cbo);
														break;
	
													case cons.TEXTBOX:
														var txt = doc.createElement('input');
															txt.setAttribute('type', 'text');
															txt.className = 'grid-text-field';
															$(txt).data('txtField', column)
																  .bind('blur change', function() {
																	var txtData = $(this).data('txtField');
																	var fieldName = txtData.field;
																	var data = $(this.parentNode.parentNode).data('dataRow');
	
																	if (txtData.currency && this.value != ''){
																		var valueField = parseFloat(this.value.replace(/[,$]/g, ''));
																		eval('data.' + fieldName + ' = ' + valueField );
																		this.value = formatCurrency(valueField);
																	}else{
																		eval('data.' + fieldName + ' = "' + this.value + '"');
																	}
																});
														cell.innerHTML = '';
														cell.appendChild(txt);
														break;
	
													case cons.LINK:
														var link = doc.createElement('a');
															link.setAttribute('href', column.cellType.url + cellValue);
															link.innerHTML = cellValue;
														cell.innerHTML = '';
														cell.appendChild(link);*/
				}

				if (column.tooltip) {
					cell.setAttribute('title', cellValue);
				}

				row.appendChild(cell);
			}
			tdbody.appendChild(row);
		}

		tbody.appendChild(tr);
	}
};

DDoc.Controls.Grid.prototype.getPage = function(position, sender) {
	var self = this;
	var $pager = $(this.pager);
	var pageSize = this.pageSize;
	var newPage;

	var paginate = function() {
		var exit = false;
		var index = 2;
		var element = 1;
		var rows = self.tcontent.rows;
		var offset = (newPage - 1) * pageSize + 1;
		var limit = offset + pageSize - 1;
		var len = rows.length;

		self.pages = Math.ceil((len - 2) / pageSize);

		if (newPage > self.pages) {
			self.currentPage = self.pages;
			newPage = self.currentPage;
			offset = (newPage - 1) * pageSize + 1;
			limit = offset + pageSize - 1;
		}

		while (index < len) {
			var row = rows[index];

			if (!$(row).hasClass('grid-hidden-row')) {
				row.style.display = (element < offset || element > limit) ? 'none' : '';
				element++;
			}
			index++;
		}

		$pager.find('.grid-pager-page').val(newPage + '/' + self.pages);

		if (newPage > 1 && self.pages > 1) {
			$pager.find('.grid-button-first').removeClass('grid-button-disabled');
			$pager.find('.grid-button-prev').removeClass('grid-button-disabled');
		} else {
			$pager.find('.grid-button-first').addClass('grid-button-disabled');
			$pager.find('.grid-button-prev').addClass('grid-button-disabled');
		}

		if (newPage < self.pages && self.pages > 1) {
			$pager.find('.grid-button-next').removeClass('grid-button-disabled');
			$pager.find('.grid-button-last').removeClass('grid-button-disabled');
		} else {
			$pager.find('.grid-button-next').addClass('grid-button-disabled');
			$pager.find('.grid-button-last').addClass('grid-button-disabled');
		}

		self.cacheRows = undefined;
		self.autoHeight();
	};

	if (sender && $(sender).hasClass('grid-button-disabled')) {
		return this.currentPage;
	}

	$pager.find('.grid-button-pager').addClass('grid-button-disabled');

	switch (position) {
		case -1:
			newPage = this.currentPage;
			break;
		case 0:
			newPage = (this.currentPage === 0) ? 1 : this.currentPage;
			break;
		case 1:
			newPage = 1;
			break;
		case 2:
			newPage = (this.currentPage === 1) ? 1 : (this.currentPage - 1);
			break;
		case 3:
			newPage = (this.currentPage !== this.pages) ? (this.currentPage + 1) : this.pages;
			break;
		case 4:
			newPage = this.pages;
			break;
	}

	if (newPage !== this.currentPage || !sender) {
		this.fire({ type: 'onChangePage', oldPage: this.currentPage, newPage: newPage });

		if (this.defaultData.pager.onServer) {
			var e = { type: 'onGetPage', page: newPage, pageSize: pageSize };
			if (this.sortBy) {
				$.extend(e, { sortBy: self.sortBy, sortDirection: self.sortDirection });
			}
			this.fire(e);
		} else {
			paginate();
		}

		this.currentPage = newPage;
	}
};

DDoc.Controls.Grid.prototype.setPager = function(page, totalPages) {
	totalPages = totalPages > 0 ? totalPages : 1;
	var $pager = $(this.pager);
	this.pages = totalPages;
	if (page > totalPages) {
		this.currentPage = totalPages;
		page = this.currentPage;
	}
	$pager.find('.grid-pager-page').val(page + '/' + totalPages);
	if (page > 1 && totalPages > 1) {
		$pager.find('.grid-button-first').removeClass('grid-button-disabled');
		$pager.find('.grid-button-prev').removeClass('grid-button-disabled');
	}
	if (page < totalPages && totalPages > 1) {
		$pager.find('.grid-button-next').removeClass('grid-button-disabled');
		$pager.find('.grid-button-last').removeClass('grid-button-disabled');
	}
};

DDoc.Controls.Grid.prototype.reload = function(rows) {
	this.cacheRows = undefined;

	$(this.tcontent.rows).each(function(index) {
		if (index > 1) {
			$(this).remove();
		}
	});

	this.loadRows(rows);
};

DDoc.Controls.Grid.prototype.checkCell = function(check, event) {
	var row = check.parentNode.parentNode;
	var col = check.parentNode.cellIndex;
	var dataRow = $(row).data('dataRow');
	var itemsChecked = $(this.tcontent).find('td:nth-child(' + (col + 1) + ') :checked').length;

	eval('dataRow.' + this.columns[col].field + ' = ' + check.checked);

	var e = { type: 'onChecked', checked: check.checked, htmlRow: row, dataRow: dataRow, col: col, checkedRows: itemsChecked, cancel: false };

	if (itemsChecked == 0 || itemsChecked == (this.tcontent.rows.length - 1)) {
		var headerCheck = $(this.tcontent.rows[1].cells[col]).find('.cell-check').get(0);
		headerCheck.checked = (itemsChecked == 0) ? false : true;
	}

	this.fire(e);

	if (e.cancel) {
		check.checked = !check.checked;
	}

	if (event) {
		event.stopPropagation();
	}
};

DDoc.Controls.Grid.prototype.unCheckAll = function() {
	$(this.tcontent).find('td :checked')
		.each(function(index, item) {
			item.checked = false;
		});
};

DDoc.Controls.Grid.prototype.checkRadio = function(radio, event) {
	var row = radio.parentNode.parentNode;
	var dataRow = $(row).data('dataRow');

	this.fire({ type: 'onRadioChecked', value: radio.value, htmlRow: row, dataRow: dataRow });

	if (event) {
		event.stopPropagation();
	}
};

DDoc.Controls.Grid.prototype.execAction = function(action, event) {
	var row = action.parentNode.parentNode;
	var indexCol = action.parentNode.cellIndex;
	var column = $.extend(true, {}, this.columnDefault, this.columns[indexCol]);

	this.fire({ type: 'onAction', row: row.rowIndex, column: indexCol, command: column.cellType.cmd, dataRow: $(row).data('dataRow'), key: row.id });

	if (event) {
		event.stopPropagation();
	}
};

DDoc.Controls.Grid.prototype.execActionButton = function(button, event) {
	var row = button.parentNode.parentNode;

	this.fire({ type: 'onButtonClick', row: row.rowIndex, column: button.parentNode.cellIndex, dataRow: $(row).data('dataRow'), key: row.id });

	if (event) {
		event.stopPropagation();
	}
};

DDoc.Controls.Grid.prototype.onClickCell = function(indexCell, indexRow) {
};

DDoc.Controls.Grid.prototype.selectRow = function(htmlRow, event) {
	if (this.defaultData.showSelectedRow) {
		$(this.tcontent.rows[this.selectedRow]).removeClass('row-selected');
		$(htmlRow).addClass('row-selected');
	}

	this.selectedRow = htmlRow.rowIndex;
	this.fire({ type: 'onSelectRow', grid: this, row: htmlRow, dataRow: $(htmlRow).data('dataRow'), index: this.selectRow, key: htmlRow.id });

	if (event) {
		event.stopPropagation();
	}
};

DDoc.Controls.Grid.prototype.rowDelete = function(index) {
	if (index && index > 0) {
		var dataRow = $(this.tcontent.rows[index]).data('dataRow');
		this.tcontent.deleteRow(index);

		if (this.tcontent.rows.length == 1) {
			$(this.tcontent.firstChild).addClass('no-data');
		}

		this.fire({ type: 'onRowRemoved', dataRow: dataRow, rows: this.tcontent.rows.length - 1 });

		if (this.defaultData.pager.show) {
			this.getPage(0);
		}
	}
};

DDoc.Controls.Grid.prototype.rowCount = function() {
	return (this.tcontent.rows.length - 1);
};

DDoc.Controls.Grid.prototype.getRows = function() {
	var rows = [];
	var htmlRows = $(this.tcontent.rows);

	for (var index = 1, len = htmlRows.length; index < len; index++) {
		var row = htmlRows[index];

		rows.push({ row: row, dataRow: $(row).data('dataRow') });
	}

	return rows;
};

DDoc.Controls.Grid.prototype.getCheckedRows = function() {
	var self = this;
	var rows = [];
	var $rows = $(this.tcontent).find('td input:checked');
	$rows.each(function(index, item) {
		var row = item.parentNode.parentNode;

		rows.push({ row: row, dataRow: $(row).data('dataRow') });
	});

	return rows;
};

DDoc.Controls.Grid.prototype.updateRow = function(index, data) {
	var self = this;

	if (index && typeof index == 'number' && data && typeof data == 'object') {
		var cons = DDoc.Constants.cellType;
		var doc = document;
		var row = this.tcontent.rows[index];
		var $row = $(row);
		var dataRow = $row.data('dataRow');

		$row.removeData('dataRow')
			.data('dataRow', data);

		for (var indexCol = 0, len = this.columns.length; indexCol < len; indexCol++) {
			var cell = row.cells[indexCol];
			var column = $.extend(true, {}, this.columnDefault, this.columns[indexCol]);
			var value = 'data.' + column.field;
			var cellValue = (column.field && column.field.length > 0) ? eval(value) : '';
			cellValue = (cellValue == undefined || cellValue == null) ? '' : cellValue;

			switch (true) {
				case (typeof cellValue === 'string'):
					if (cellValue.indexOf('/Date(') != -1) {
						var date = eval('new ' + cellValue.replace(/\//g, ''));
						cellValue = date.format(column.formatDate);
					}

					break;

				case (typeof cellValue === 'boolean' && column.cellType.type == cons.NORMAL):
					cellValue = (cellValue) ? 'SI' : 'NO';
					break;

				case (typeof cellValue === 'number'):
					cellValue = (column.currency) ? formatCurrency(cellValue) : cellValue;
					break;
			}

			if (column.columnType == DDoc.Constants.columnType.NUMBER) {
				$(cell).addClass('grid-numeric-cell');
			}

			if (column.fieldValueReplacement && $.isFunction(column.fieldValueReplacement)) {
				cellValue = column.fieldValueReplacement.call(this, dataRow, cellValue);
			}

			cell.innerHTML = cellValue;

			switch (column.cellType.type) {
				case cons.CHECKBOX:
					var check = doc.createElement('input');
					check.className = 'cell-check';
					check.setAttribute('type', 'checkbox');
					check.checked = cellValue;
					$(check).click(function(event) { self.checkCell(this, event); });
					cell.innerHTML = '';
					cell.appendChild(check);
					break;

				case cons.RADIO:
					var radio = doc.createElement('input');
					radio.className = 'cell-radio';
					radio.setAttribute('type', 'radio');
					radio.setAttribute('name', 'GridRadio');
					radio.setAttribute('value', cellValue);
					$(radio).click(function(event) { self.checkRadio(this, event); });
					cell.innerHTML = '';
					cell.appendChild(radio);
					break;

				case cons.ACTION:
					var cssClass = (column.cellType.cssClass &&
							typeof cellValue === 'boolean') ? (cellValue) ? column.cellType.cssClass.active
						: column.cellType.cssClass.inactive
						: (column.cellType.cssClass) ? column.cellType.cssClass.normal : '';
					var tooltip = ($.isFunction(column.cellType.tooltip)) ? column.cellType.tooltip.call(this, cellValue) : column.cellType.tooltip;
					var action = doc.createElement('span');
					action.className = 'grid-action';
					action.setAttribute('title', tooltip);
					action.className += (' ' + cssClass);
					$(action).click(function(event) { self.execAction(this, event); });
					cell.innerHTML = '';
					cell.appendChild(action);

					break;

				case cons.BUTTON:
					var button = doc.createElement('input');
					button.setAttribute('type', 'button');
					button.setAttribute('value', (column.cellType.text) ? column.cellType.text : '');
					$(button).click(function(e) { self.execActionButton(this, e); });

					cell.innerHTML = '';
					cell.appendChild(button);

					break;

				case cons.COMBO:
					var cbo = doc.createElement('select');
					cbo.className = 'grid-data-combo';
					$(cbo).data('cboData', column);

					$.each(column.cellType.combo.items, function(index, item) {
						var value = eval('this.' + column.cellType.combo.fieldValue);
						var text = eval('this.' + column.cellType.combo.fieldText);

						var element = document.createElement('option');
						element.value = value;
						element.innerHTML = text;
						element.setAttribute('title', text);
						cbo.appendChild(element);

						if (index == 0) {
							eval('dataRow.' + column.cellType.combo.fieldSet + ' = "' + value + '"');
						}
						if (text == cellValue || value == cellValue) {
							element.setAttribute('selected', 'selected');
							eval('dataRow.' + column.cellType.combo.fieldSet + ' = "' + value + '"');
						}
					});

					$(cbo).change(function() {
						var option = this.options[this.selectedIndex];
						var value = option.value;
						var data = $(cell.parentNode).data('dataRow');
						var column = $(this).data('cboData');

						eval('data.' + column.cellType.combo.fieldSet + ' = "' + value + '"');
					});

					if (!column.cellType.initStateEnabled) {
						cbo.setAttribute('disabled', 'disabled');
					}

					cell.innerHTML = '';
					cell.appendChild(cbo);
					break;

				case cons.TEXTBOX:
					var txt = doc.createElement('input');
					txt.setAttribute('type', 'text');
					txt.className = 'grid-text-field';
					txt.value = cellValue;
					$(txt).data('txtField', { column: column, index: indexCol })
						.bind('blur', function(e) {
							var txtData = $(this).data('txtField');
							var fieldName = txtData.column.field;
							var data = $(this.parentNode.parentNode).data('dataRow');
							var valueField = this.value;

							if (txtData.column.currency) {
								this.value = (this.value != '') ? this.value : '0';

								valueField = parseFloat(this.value.replace(/[,$\s]/g, ''));
								eval('data.' + fieldName + ' = ' + valueField);
								this.value = formatCurrency(valueField);
							} else {
								eval('data.' + fieldName + ' = "' + this.value + '"');
							}

							var eventArgs = { type: 'onEditedField', value: valueField, dataRow: data, textbox: this, column: txtData.index, cancel: false };
							self.fire(eventArgs);

							if (eventArgs.cancel) {
								e.preventDefault();
								e.stopPropagation();

								return false;
							}
						});

					if (column.columnType == DDoc.Constants.columnType.NUMBER) {
						$(txt).addClass('grid-numeric-cell')
							.keypress(function(e) { return filterCurrency(e); })
							.focus(function(e) { this.value = this.value.replace(/[,$\s]/g, ''); });
					}

					if (!column.cellType.initStateEnabled) {
						txt.setAttribute('disabled', 'disabled');
					}

					cell.innerHTML = '';
					cell.appendChild(txt);
					break;

				case cons.TEXTLINK:
					var link = doc.createElement('a');
					link.setAttribute('href', column.cellType.url + cellValue);
					link.innerHTML = cellValue;
					cell.innerHTML = '';
					cell.appendChild(link);
			}

			if (column.tooltip) {
				cell.setAttribute('title', cellValue);
			}
		}
	}
};

DDoc.Controls.Grid.prototype.setItemsCombo = function(items, index) {
	if (items && items.length > 0) {
		var cons = DDoc.Constants.cellType;
		var len = this.columns.length;
		var column = undefined;

		if (index == undefined || index < 0 || index > len) {
			for (var index = 0, len = this.columns.length; index < len; index++) {
				column = $.extend(true, {}, this.columnDefault, this.columns[index]);

				if (column.cellType.type == cons.COMBO) {
					break;
				}
			}
		}

		this.columns[index].cellType.combo.items = items;
	}
};

DDoc.Controls.Grid.prototype.setValueCombo = function(value, column) {
	if (value != undefined && value != null && column && typeof column == 'number' && column >= 0 && column < this.columns.length) {
		for (var index = 1, len = this.tcontent.rows.length; index < len; index++) {
			var row = this.tcontent.rows[index];
			var $cbo = $(row.cells[column]).find('.grid-data-combo');

			if ($cbo) {
				var $options = $cbo.find('option');
				$options.removeAttr('selected');
				$options.each(function() {
					var $option = $(this);

					if (this.value == value) {
						$option.attr('selected', 'selected');
						$cbo.trigger('change');
					}
				});
			}
		}
	}
};

DDoc.Controls.Grid.prototype.seek = function(text) {
	var words = text.toLowerCase().split(' ');

	if (!this.cacheRows) {
		var $rows = $(this.tcontent).find('tr');

		this.cacheRows = $rows.slice(1);
	}

	this.cacheRows.each(function() {
		var hasWord = true;
		var value = this.innerHTML;
		value = value.replace(new RegExp('<[^<]+\>', 'g'), '');
		value = $.trim(value.toLowerCase());

		for (var index = 0, len = words.length; index < len; index++) {
			if (value.indexOf(words[index]) == -1) {
				hasWord = false;
				break;
			}
		}

		this.style.display = (hasWord) ? '' : 'none';
	});
};

DDoc.Controls.Grid.prototype.serverSort = function(cell) {
	var self = this;
	var sortOrder = 0;

	if (!cell) {
		for (var index = 0, len = this.columns.length; index < len; index++) {
			var column = $.extend(true, {}, this.columnDefault, this.columns[index]);

			if (column.sort.sortable && column.sort.sorted != 0) {
				cell = this.tcontent.rows[1].cells[index];
				sortOrder = 1;
				break;
			}
		}
	}

	var dataCol = $(cell).data('cellHeaderData');
	var sortType = (sortOrder) ? sortOrder : (dataCol.type == 0 || dataCol.type == -1) ? 1 : -1;

	$('.sort-cell-flag').removeClass('column-sorted-asc column-sorted-desc');
	$(cell).find('.sort-cell-flag').addClass((sortType == 1) ? 'column-sorted-asc' : 'column-sorted-desc');

	dataCol.type = sortType;

	$.each(this.columns, function(i, column) {
		if (column.field == dataCol.col.field) {
			self.sortBy = 'C' + column.Id;
			return false;
		}
	});

	this.sortDirection = sortType;

	if (this.defaultData.pager.show) {
		this.currentPage = 1;
		this.setPager(1, this.pages);
		this.getPage(0);
	}
};

DDoc.Controls.Grid.prototype.sort = function(cell) {
	var self = this;
	var sortKey = '';
	var sortOrder = 0;

	if (!cell) {
		for (var index = 0, len = this.columns.length; index < len; index++) {
			var column = $.extend(true, {}, this.columnDefault, this.columns[index]);

			if (column.sort.sortable && column.sort.sorted != 0) {
				cell = this.tcontent.rows[1].cells[index];
				sortOrder = 1;
				break;
			}
		}
	}

	var dataCol = $(cell).data('cellHeaderData');
	var sortType = (sortOrder) ? sortOrder : (dataCol.type == 0 || dataCol.type == -1) ? 1 : -1;
	var cons = DDoc.Constants.columnType;
	var rows = $(this.tcontent.rows).get();
	var headLayout = rows[0];

	$('.sort-cell-flag').removeClass('column-sorted-asc column-sorted-desc');
	$(cell).find('.sort-cell-flag').addClass((sortType == 1) ? 'column-sorted-asc' : 'column-sorted-desc');

	rows.splice(0, 1);

	$.each(rows, function(index, row) {
		var $col = $(row).children('td').eq(cell.cellIndex);
		var columnType = dataCol.col.columnType;

		switch (columnType) {
			case cons.TEXT:
				sortKey = $col.text().toLowerCase();
				break;

			case cons.NUMBER:
				sortKey = $col.text().replace(/[$,]/g, '');
				sortKey = parseFloat(sortKey);
				sortKey = isNaN(sortKey) ? 0 : sortKey;
				break;

			case cons.DATE:
				break;
		}

		row.sortKey = sortKey;
	});

	rows.sort(function(a, b) {
		if (a.sortKey < b.sortKey) return -sortType;
		if (a.sortKey > b.sortKey) return sortType;
		return 0;
	});

	dataCol.type = sortType;

	$(this.tcontent.firstChild).append(headLayout);
	$.each(rows, function(index, row) {
		$(self.tcontent.firstChild).append(row);
		row.sortKey = null;
	});

	if (this.defaultData.pager.show) {
		this.getPage(0);
	}
};

DDoc.Controls.Grid.prototype.autoHeight = function(height) {
	var isVertScroll = false;

	if (height) {
		this.grid.style.height = height + 'px';
		this.content.style.height = (this.grid.clientHeight - (this.header.clientHeight + this.footer.clientHeight)) + 'px';
	}

	if (this.defaultData.height > 0) {
		height = this.defaultData.height;
		this.grid.style.height = height + 'px';
		this.content.style.height = (this.grid.clientHeight - (this.header.clientHeight + this.footer.clientHeight)) + 'px';
	}

	if (this.defaultData.maxHeight > 0) {
		var maxHeight = this.defaultData.maxHeight;
		maxHeight -= (this.header.clientHeight + this.footer.clientHeight);
		var tableHeight = $(this.tcontent).height();
		var rows = this.tcontent.rows.length - 1;
		var rowHeight = (rows == 0) ? 37 : parseFloat(tableHeight / ((this.defaultData.pager.show && this.pageSize < rows) ? this.pageSize : rows));
		height = (rows == 0) ? 64 : tableHeight;

		if (this.defaultData.pager.show) {
			var maxHeight = this.defaultData.maxHeight;
			maxHeight -= (this.header.clientHeight + this.footer.clientHeight);

			height = Math.min((this.pageSize >= rows) ? height : (this.pageSize * rowHeight), maxHeight);

			this.content.style.height = height + 'px';
			this.grid.style.height = height + this.header.clientHeight + this.footer.clientHeight + 'px';
		} else {
			height = (rows <= 2) ? (2 * rowHeight) : Math.min(height, maxHeight);
			this.content.style.height = height + 'px';
			this.grid.style.height = height + this.header.clientHeight + this.footer.clientHeight + 'px';
		}
	}

	isVertScroll = this.content.scrollHeight > this.content.offsetHeight;

	if (isVertScroll) {
		$(this.tcontent).addClass('header-space-scroll');
		this.content.style.height = (this.grid.clientHeight - (this.header.clientHeight + this.footer.clientHeight)) + 'px';
	} else {
		$(this.tcontent).removeClass('header-space-scroll');
		this.content.style.height = (this.grid.clientHeight - (this.header.clientHeight + this.footer.clientHeight)) + 'px';
	}

	this.fire({ type: 'onResize' });
};

DDoc.Controls.Grid.prototype.setActionLink = function(columnIndex, url, data) {
	var self = this;
	var rows = self.getRows();

	$.each(rows, function(index, item) {

		var $spanLink = $('span', $(item.row.cells[columnIndex]));

		var link = document.createElement('a');

		var linkUrl = ddoc.GetUrl(url);

		if (data.constructor === Array) {
			$.each(data, function(i, field) {
				linkUrl += '/' + item.dataRow[field];
			});
		} else {
			linkUrl += '/' + item.dataRow[data];
		}

		link.setAttribute('href', linkUrl);

		$(link).append($spanLink[0].outerHTML);
		$(item.row.cells[columnIndex]).html(link);
	});
};