DDoc.Controls.DatePicker.prototype.init = function () {
    var self = this;
    var doc = document;
    var setFocus = function (event) {
        self.datePicker.focus();
    };
    var showCalendar = function (e) {
        if (!self.disabled) {
            self.calendar.show(self.datePicker);
        }

        if (e) {
            e.stopPropagation();
        }
    };
    var loadDate = function (event) {
        var dateValue = undefined;
        var padLeft = function (item, length, character) {
            return new Array(length - item.length + 1).join(character || ' ') + item;
        };
        var day = padLeft(event.dateInfo.day.toString(), 2, '0');
        var month = padLeft(event.dateInfo.month.toString(), 2, '0');
        var year = event.dateInfo.year;

        switch (self.defaultData.format) {
            case 'DMY':
                dateValue = day + '/' + month + '/' + year;
                break;

            case 'MDY':
                dateValue = month + '/' + day + '/' + year;
                break;

            case 'YMD':
                dateValue = year + '/' + month + '/' + day;
                break;
        }

        self.fire({ type: 'onSelectDate', dateInfo: { date: event.dateInfo.date, day: day, month: month, year: year } });
        self.datePicker.value = dateValue;
        $(self.datePicker).trigger('change');
    };
    var button = doc.createElement('span');
    button.className = 'dtp-button';
    $(button).click(function (e) { showCalendar(e); });

    this.calendar = new DDoc.Controls.Calendar('', { field: this.datePicker });
    this.calendar.addListener('onClose', setFocus);
    this.calendar.addListener('onSelectDay', loadDate);
    this.calendar.init();
    this.disabled = false;
    this.datePicker.className += ' datePicker';
    this.datePicker.parentNode.appendChild(button);
};

DDoc.Controls.DatePicker.prototype.destroy = function () {
    var self = this;
    var doc = document;

    $(this.datePicker).removeAttr('disabled');
    $(this.calendar.calendar).remove();
    this.calendar = undefined;
    $(this.datePicker).removeClass('datePicker').next().remove();
    this.datePicker = undefined;
};

/**
 * Asigna una fecha al control
 * @param {Object} date objeto Date
 */
DDoc.Controls.DatePicker.prototype.setDate = function (date) {
    var dateValue = undefined;
    var padLeft = function (item, length, character) {
        return new Array(length - item.length + 1).join(character || ' ') + item;
    };
    var day = padLeft(date.getDate().toString(), 2, '0');
    var month = padLeft((date.getMonth() + 1).toString(), 2, '0');
    var year = date.getFullYear();

    switch (this.defaultData.format) {
        case 'DMY':
            dateValue = day + '/' + month + '/' + year;
            break;

        case 'MDY':
            dateValue = month + '/' + day + '/' + year;
            break;

        case 'YMD':
            dateValue = year + '/' + month + '/' + day;
            break;
    }

    this.datePicker.value = dateValue;
};