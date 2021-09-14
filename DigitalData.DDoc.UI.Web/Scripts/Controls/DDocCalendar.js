DDoc.Controls.Calendar.prototype.init = function () {
    var self = this;
    var doc = document;
    var days = 'Do,Lu,Ma,Mi,Ju,Vi,Sa'.split(',');
    var titles = 'Domingo,Lunes,Martes,Miércoles,Jueves,Viernes,Sábado'.split(',');
    var months = 'Enero,Febrero,Marzo,Abril,Mayo,Junio,Julio,Agosto,Septiembre,Octubre,Noviembre,Diciembre'.split(',');
    var date = new Date();
    var getFisrtDayWeek = function () {
        var date = self.dateInfo.currentDate;
        var time = date.getTime();
        var day = date.getDate();

        date.setTime(parseInt(time - parseInt((day - 1) * self.DAY_MILISEC)));

        return date.getDay() + 1;
    };
    var getLastDayMonth = function () {
        var date = self.dateInfo.currentDate;
        var time = date.getTime();
        var day = date.getDate();

        date.setTime(parseInt(time + parseInt((45 - day) * self.DAY_MILISEC)));

        time = date.getTime();
        day = date.getDate();

        date.setTime(parseInt(time - parseInt(day * self.DAY_MILISEC)));

        return date.getDate();
    };
    var clearTable = function () {
        var $rows = $(self.tdays).find('td');
        $rows.each(function (index, item) {
            $(this).html('')
				   .unbind('click')
				   .removeClass('k-today');
        });
    };
    var setYear = function (value) {
        var month = self.dateInfo.selectedMonth;

        self.dateInfo.selectedYear = value;
        self.dateInfo.currentDate.setFullYear(value, month, 1);

        clearTable();
        loadMonth();
    };
    var setMonth = function (unit) {
        var year = self.dateInfo.selectedYear;
        var month = self.dateInfo.selectedMonth;
        month += unit;

        switch (month) {
            case -1:
                month = 11;
                year -= 1;
                self.dateInfo.currentDate.setFullYear(year, month, 1);
                break;

            case 12:
                month = 0;
                year += 1;
                self.dateInfo.currentDate.setFullYear(year, month, 1);
                break;

            default:
                self.dateInfo.currentDate.setMonth(month, 1);
        }

        self.dateInfo.selectedMonth = month;
        self.dateInfo.selectedYear = year;
    };
    var loadMonth = function () {
        var $cells = $(self.tdays).find('td');
        var dayWeek = getFisrtDayWeek();
        var lastDay = getLastDayMonth();

        for (var day = 1; day <= lastDay; day++) {
            var index = (day - 1) + (dayWeek - 1);

            if (day == self.dateInfo.today &&
				self.dateInfo.selectedMonth == self.dateInfo.month &&
				self.dateInfo.selectedYear == self.dateInfo.year) {
                $($cells[index]).addClass('k-today');
            }

            $($cells[index]).html(day)
						   .click(function () { self.getSelectedDate(this); });
        }
    };
    var goMonth = function (unit, e) {
        clearTable();
        setMonth(unit);
        loadMonth();

        $(self.cboMonth).attr('value', self.dateInfo.selectedMonth + 1);
        $(self.cboYear).attr('value', self.dateInfo.selectedYear);

        if (e) {
            e.stopPropagation();
        }
    };

    var tcontainer = doc.createElement('div');
    tcontainer.className = 'k-table-container';
    var trDays = doc.createElement('tr');
    var thDays = doc.createElement('thead');
    thDays.appendChild(trDays);
    var tbDays = doc.createElement('tbody');
    var prevMonth = doc.createElement('span');
    prevMonth.className = 'k-button k-prev';
    $(prevMonth).click(function (e) { goMonth(-1, e); });
    var nextMonth = doc.createElement('span');
    nextMonth.className = 'k-button k-next';
    $(nextMonth).click(function (e) { goMonth(1, e); });
    var close = doc.createElement('span');
    close.className = 'k-button k-close';
    $(close).click(function (e) { self.close(e); });

    this.DAY_MILISEC = 24 * 60 * 60 * 1000;
    this.cboMonth = doc.createElement('select');
    this.cboMonth.className = 'k-month-selector';
    $(this.cboMonth).click(function (e) { e.stopPropagation(); })
					.change(function () {
					    var unit = this.selectedIndex - self.dateInfo.selectedMonth;

					    if (unit != 0) goMonth(unit);
					});

    this.cboYear = doc.createElement('select');
    this.cboYear.className = 'k-year-selector';
    $(this.cboYear).click(function (e) { e.stopPropagation(); })
				   .change(function () { setYear(parseInt(this.value)); });

    this.tdays = doc.createElement('table');
    this.tdays.className = 'k-table';
    this.tdays.setAttribute('cellpadding', '0');
    this.tdays.setAttribute('cellspacing', '0');
    this.tdays.className = 'k-table-days';
    this.tdays.appendChild(thDays);
    this.tdays.appendChild(tbDays);
    this.isVisible = false;
    this.dateInfo = {
        currentDate: date,
        month: date.getMonth(),
        year: date.getFullYear(),
        today: date.getDate(),
        selectedMonth: date.getMonth(),
        selectedYear: date.getFullYear()
    };

    $.each(months, function (index, item) {
        var option = $('<option></option>').attr('value', index + 1)
										  .text(item);

        if (self.dateInfo.selectedMonth == index) {
            option.attr('selected', 'selected');
        }

        $(self.cboMonth).append(option);
    });

    $.each(days, function (index, item) {
        var cell = doc.createElement('th');
        cell.innerHTML = item;
        cell.setAttribute('title', titles[index]);
        cell.className = (index == 0 || index == 6) ? 'k-day-label k-day-weekend' : 'k-day-label';

        trDays.appendChild(cell);
    });

    for (var week = 1; week < 7; week++) {
        var rowWeek = doc.createElement('tr');

        for (var day = 0, len = days.length; day < len; day++) {
            var cell = doc.createElement('td');
            cell.className = (day == 0 || day == 6) ? 'k-day k-day-weekend' : 'k-day';

            rowWeek.appendChild(cell);
        }

        tbDays.appendChild(rowWeek);
    }

    for (var index = this.defaultData.years.end; index > this.defaultData.years.init; index--) {
        var option = $('<option></option>').attr('value', index)
										  .text(index);
        if (self.dateInfo.selectedYear == index) {
            option.attr('selected', 'selected');
        }

        $(this.cboYear).append(option);
    }

    tcontainer.appendChild(this.tdays)

    var container = doc.createElement('div');
    container.className = 'k-container'
    container.appendChild(prevMonth);
    container.appendChild(this.cboMonth);
    container.appendChild(nextMonth);
    container.appendChild(this.cboYear);
    container.appendChild(close);
    container.appendChild(tcontainer);

    this.calendar.className = 'calendar';
    this.calendar.appendChild(container);

    loadMonth();

    $(document).click(function (e) { self.close(); e.stopPropagation(); });
};

DDoc.Controls.Calendar.prototype.close = function (e) {
    var self = this;

    if (this.isVisible) {
        $(this.calendar).slideUp('slow', function () {
            self.isVisible = false;
            self.fire({ type: 'onClose' });
        });
    }

    if (e) {
        e.stopPropagation();
    }
};

DDoc.Controls.Calendar.prototype.show = function (element) {
    var self = this;
    var field = element;
    var width, height;
    var left, top;
    var hoffset, woffset;

    if (this.isVisible) {
        this.close();
        return;
    }

    if (element) {
        if (typeof element != 'object') {
            field = document.getElementById(element);
        }

        if (!this.defaultData.field) {
            this.defaultData.field = field;
        }

        if (typeof window.innerWidth == 'number') {
            height = window.innerHeight;
            width = window.innerWidth;
        } else {
            height = window.offsetHeight;
            width = window.offsetWidth;
        }

        top = getAbsoluteTop(field);
        left = getAbsoluteLeft(field);

        with (this.calendar) {
            hoffset = offsetHeight;
            woffset = offsetWidth;
            style.top = (top + hoffset > height) ? (top - hoffset + field.offsetHeight) + 'px' : (top + field.offsetHeight) + 'px';
            style.left = (left + woffset > width) ? (0) + 'px' : (left - (field.offsetWidth)) + 'px';
        }
    } else {
        this.calendar.style.top = '0px';
        this.calendar.style.left = '0px';
    }

    $(this.calendar).slideDown('slow', function () { self.isVisible = true; });
};

DDoc.Controls.Calendar.prototype.getSelectedDate = function (cell) {
    var day = parseInt(cell.innerHTML);
    var date = new Date();
    date.setFullYear(this.dateInfo.selectedYear,
					 this.dateInfo.selectedMonth,
					 day);

    this.fire({
        type: 'onSelectDay',
        dateInfo: {
            date: date,
            day: day,
            month: this.dateInfo.selectedMonth + 1,
            year: this.dateInfo.selectedYear
        }
    });

    this.close();
};