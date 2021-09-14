DDoc.Controls.Accordion.prototype.init = function(items) {
    for (var index = 0, len = items.length; index < len; index++) {
        this.addItem(items[index]);
    }

    this.items = items;
    this.accordion.className = 'accordion';
};

DDoc.Controls.Accordion.prototype.addItem = function(item) {
    var self = this;
    var doc = document;
    var settings = $.extend(true, {}, this.defaultItemData, item);
    var handler = doc.createElement('span');
    handler.className = 'acc-item-handler expanded';
    $(handler).click(function() {
        $(this).parent()
            .next()
            .slideToggle('normal', function () { self.fire({ type: 'onChangeState', key: settings.key }); });

        if ($(this).hasClass('expanded')) {
            $(this).removeClass('expanded');
        } else {
            $(this).addClass('expanded');
        }
    });

    var title;

    if (typeof settings.title != 'object') {
        if ($('#' + settings.title).length) {
            title = document.getElementById(settings.title);
            $(title).addClass('acc-title');
            title.appendChild(handler);
        } else {
            title = doc.createElement('p');
            title.innerHTML = settings.title;
            title.className = 'acc-title';
            title.appendChild(handler);
        }
    } else {
        title = settings.title;
    }

    var accItemContent = doc.createElement('div');
    accItemContent.className = 'acc-item-content';
    var accItem = doc.createElement('div');
    accItem.className = 'acc-item';
    accItem.appendChild(title);
    accItem.appendChild(accItemContent);

    if (settings.content && settings.content != '') {
        var content = doc.getElementById(settings.content);

        accItemContent.appendChild(content);

        if (settings.collapsed) {
            $(accItemContent).css('display', 'none');
        }
    }

    this.accordion.appendChild(accItem);
};

DDoc.Controls.Accordion.prototype.collapse = function(key) {
    var self = this;
    var itemIndex = -1;

    for (var index = 0, len = this.items.length; index < len; index++) {
        if (this.items[index].key == key) {
            itemIndex = index;
            break;
        }
    }

    if (itemIndex >= 0) {
        $(this.accordion).find('.acc-item-content')
            .eq(itemIndex)
            .slideUp('normal', function() {
                self.fire({ type: 'onCollapse', key: key });
            })
            .end().end()
            .find('.acc-item-handler')
            .eq(itemIndex)
            .removeClass('expanded');
    }
};

DDoc.Controls.Accordion.prototype.expand = function(key) {
    var self = this;
    var itemIndex = -1;

    for (var index = 0, len = this.items.length; index < len; index++) {
        if (this.items[index].key == key) {
            itemIndex = index;
            break;
        }
    }

    if (itemIndex >= 0) {
        $(this.accordion).find('.acc-item-content')
            .eq(itemIndex)
            .slideDown('normal', function() {
                self.fire({ type: 'onExpand', key: key });
            })
            .end().end()
            .find('.acc-item-handler')
            .eq(itemIndex)
            .addClass('expanded');
    }
};