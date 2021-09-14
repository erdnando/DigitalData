function object(o) {
    function f() {
    }

    f.prototype = o;
    return new f();
}

function inheritPrototype(subtype, superType) {
    var prototype = object(superType.prototype);

    prototype.constructor = subtype;
    subtype.prototype = prototype;
}

function EventObject() {
    this.listeners = {};
}

EventObject.prototype = {
    constructor: EventObject,
    addListener: function(type, listener) {
        if (typeof this.listeners[type] == 'undefined') {
            this.listeners[type] = [];
        }
        this.listeners[type].push(listener);
    },
    removeListener: function(type, listener) {
        if (this.listeners[type] instanceof Array) {
            var listeners = this.listeners[type];
            for (var item = 0, len = listeners.length; item <= len; item++) {
                if (listeners[item] === listener) {
                    break;
                }
            }
            listeners.splice(item, 1);
        }
    },

    fire: function(event) {
        if (!event.target) {
            event.target = this;
        }
        if (this.listeners[event.type] instanceof Array) {
            var listeners = this.listeners[event.type];
            for (var item = 0, len = listeners.length; item < len; item++) {
                listeners[item](event);
            }
        }
    }
};

var DDoc = {
    Constants: {
        columnType: { TEXT: 0, NUMBER: 1, DATE: 2, BOOLEAN: 3, MONEY: 4 },
        cellType: { NORMAL: 0, CHECKBOX: 1, RADIO: 2, TEXTBOX: 3, COMBO: 4, BUTTON: 5, IMAGE: 6, ACTION: 7, TEXTLINK: 8, ICONLINK: 9, THUMBNAIL : 10, THUMBNAILLINK : 11 },
        buttonType: { CUSTOM: 0, OK: 1, CANCEL: 2 },
        windowIcon: { NONE: 0, OK: 1, WARNING: 2, ERROR: 3, QUESTION: 4 }
    },
    Controls: {
        DatePicker: function (element, options) {
            if (typeof element != 'object') {
                this.datePicker = document.getElementById(element);
            } else {
                this.datePicker = element;
            }

            this.defaultData = $.extend(true, { format: 'DMY' }, options);

            EventObject.call(this);
        },
        Calendar: function (element, options) {
            var doc = document;

            switch (true) {
                case (!element):
                    this.calendar = doc.createElement('div');
                    doc.body.appendChild(this.calendar);
                    break;

                case (typeof element != 'object'):
                    this.calendar = doc.getElementById(element);
                    break;

                default:
                    this.calendar = element;
            }

            this.defaultData = $.extend(true, { height: 18, width: 216, field: undefined, years: { init: 1936, end: 2040 } }, options);

            EventObject.call(this);
        },
        Tabs: function(element) {
            if (typeof element != 'object') {
                this.tabs = document.getElementById(element);
            } else {
                this.tabs = element;
            }

            this.defaultData = {
                text: '',
                content: undefined,
                visible: true
            };

            EventObject.call(this);
		},
        Grid: function(element, options) {
            if (typeof element != 'object') {
                this.grid = document.getElementById(element);
            } else {
                this.grid = element;
            }

            this.defaultData = $.extend(true, {
                title: undefined,
                height: 486,
                maxHeight: 0,
                showTools: true,
                showFilter: true,
                defaultFilter: true,
                showColumnMenu : true,
                loaderClass: 'grid-loader',
                thumbnailSrc : '',
                striped: true,
                showSelectedRow: true,
                serverSortable: true,
                pager: {
                    show: false,
                    onServer: false,
                    sizes: [10, 20, 50, 100, 150, 200],
                    pageSize: 20,
                    currentPage: 1,
                    pages: 0
                },
                detail: {
                    field: '',
                    columns: []
                }
            }, options);

            this.columnDefault = {
                iskey: false,
                text: '',
                width: 0,
                field: '',
                fieldValueReplacement: undefined,
                tooltip: true,
                hidden: false,
                visible: true,
                currency: false,
                formatDate: 'default',
                columnType: DDoc.Constants.columnType.TEXT,
                colSpan: 0,
                sort: {
                    sortable: false,
                    sortKey: '',
                    sorted: 0
                },
                disableHider : false,
                tileVisibility: false,
                cellType: {
                    type: DDoc.Constants.cellType.NORMAL,
                    combo: {
                        items: [],
                        fieldValue: '',
                        fieldText: '',
                        fieldSet: ''
                    },
                    text: '',
                    cmd: '',
                    url: '',
                    initStateEnabled: true,
                    mask: { format: '', input: '' },
                    cssClass: {
                        active: '',
                        inactive: '',
                        normal: ''
                    },
                    tooltip: ''
                }
            };

            EventObject.call(this);
		},
        Accordion: function(element, options) {
            if (typeof element != 'object') {
                this.accordion = document.getElementById(element);
            } else {
                this.accordion = element;
            }

            this.defaultData = $.extend(true, { linked: false }, options);
            this.defaultItemData = {
                key: '',
                title: '',
                collapsed: false,
                content: ''
            };

            EventObject.call(this);
        }
    }
};

inheritPrototype(DDoc.Controls.Tabs, EventObject);
inheritPrototype(DDoc.Controls.Grid, EventObject);
inheritPrototype(DDoc.Controls.Accordion, EventObject);
inheritPrototype(DDoc.Controls.DatePicker, EventObject);
inheritPrototype(DDoc.Controls.Calendar, EventObject);

function getAbsoluteTop(object) { return getOffsetRect(object).top; }

function getAbsoluteLeft(object) { return getOffsetRect(object).left; }

function getOffset(object) { return getOffsetRect(object); }

function getOffsetRect(object) {
    var box = object.getBoundingClientRect();
    var body = document.body;
    var element = document.documentElement;
    var scrollTop = window.pageYOffset || element.scrollTop || body.scrollTop;
    var scrollLeft = window.pageXOffset || element.scrollLeft || body.scrollLeft;
    var clientTop = element.clientTop || body.clientTop || 0;
    var clientLeft = element.clientLeft || body.clientLeft || 0;
    var top = box.top + scrollTop - clientTop;
    var left = box.left + scrollLeft - clientLeft;

    return { top: Math.round(top), left: Math.round(left) };
}

var dateFormat = function() {
    var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
        timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
        timezoneClip = /[^-+\dA-Z]/g,
        pad = function(val, len) {
            val = String(val);
            len = len || 2;
            while (val.length < len) val = '0' + val;
            return val;
        };

    // Regexes and supporting functions are cached through closure
    return function(date, mask, utc) {
        var dF = dateFormat;

        // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
        if (arguments.length == 1 && Object.prototype.toString.call(date) == '[object String]' && !/\d/.test(date)) {
            mask = date;
            date = undefined;
        }

        // Passing date through Date applies Date.parse, if necessary
        date = date ? new Date(date) : new Date;
        if (isNaN(date)) throw SyntaxError('invalid date');

        mask = String(dF.masks[mask] || mask || dF.masks['default']);

        // Allow setting the utc argument via the mask
        if (mask.slice(0, 4) == 'UTC:') {
            mask = mask.slice(4);
            utc = true;
        }

        var _ = utc ? 'getUTC' : 'get',
            d = date[_ + 'Date'](),
            D = date[_ + 'Day'](),
            m = date[_ + 'Month'](),
            y = date[_ + 'FullYear'](),
            H = date[_ + 'Hours'](),
            M = date[_ + 'Minutes'](),
            s = date[_ + 'Seconds'](),
            L = date[_ + 'Milliseconds'](),
            o = utc ? 0 : date.getTimezoneOffset(),
            flags = {
                d: d,
                dd: pad(d),
                ddd: dF.i17n.dayNames[D],
                dddd: dF.i17n.dayNames[D + 7],
                m: m + 1,
                mm: pad(m + 1),
                mmm: dF.i17n.monthNames[m],
                mmmm: dF.i17n.monthNames[m + 12],
                yy: String(y).slice(2),
                yyyy: y,
                h: H % 12 || 12,
                hh: pad(H % 12 || 12),
                H: H,
                HH: pad(H),
                M: M,
                MM: pad(M),
                s: s,
                ss: pad(s),
                l: pad(L, 3),
                L: pad(L > 99 ? Math.round(L / 10) : L),
                t: H < 12 ? 'a' : 'p',
                tt: H < 12 ? 'am' : 'pm',
                T: H < 12 ? 'A' : 'P',
                TT: H < 12 ? 'AM' : 'PM',
                Z: utc ? 'UTC' : (String(date).match(timezone) || ['']).pop().replace(timezoneClip, ''),
                o: (o > 0 ? '-' : '+') + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
                S: ['th', 'st', 'nd', 'rd'][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
            };

        return mask.replace(token, function($0) {
            return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
        });
    };
}();

// Some common format strings
dateFormat.masks = {
    "default": 'dd/mm/yyyy HH:MM:ss',
    olddefault: 'ddd mmm dd yyyy HH:MM:ss',
    shortDate: 'm/d/yy',
    mediumDate: 'mmm d, yyyy',
    longDate: 'mmmm d, yyyy',
    fullDate: 'dddd, mmmm d, yyyy',
    shortTime: 'h:MM TT',
    mediumTime: 'h:MM:ss TT',
    longTime: 'h:MM:ss TT Z',
    isoDate: 'yyyy-mm-dd',
    isoTime: 'HH:MM:ss',
    isoDateTime: 'yyyy-mm-dd\'T\'HH:MM:ss',
    isoUtcDateTime: 'UTC:yyyy-mm-dd\'T\'HH:MM:ss\'Z\''
};

// Internationalization strings
dateFormat.i18n = {
    dayNames: [
        'Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat',
        'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'
    ],
    monthNames: [
        'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec',
        'January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'
    ]
};

dateFormat.i17n = {
    dayNames: [
        'Dom', 'Lun', 'Mar', 'Mie', 'Jue', 'Vie', 'Sab',
        'Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'
    ],
    monthNames: [
        'Enn', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic',
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ]
};

// For convenience...
Date.prototype.format = function(mask, utc) {
    return dateFormat(this, mask, utc);
};

// First, checks if it isn't implemented yet.
if (!String.prototype.format) {
    String.prototype.format = function () {
        var args = arguments;
        return this.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };
}