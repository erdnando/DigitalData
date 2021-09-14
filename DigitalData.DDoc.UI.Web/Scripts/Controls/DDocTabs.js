DDoc.Controls.Tabs.prototype.init = function(tabs) {
    var doc = document;

    this.activeTab = -1;
    this.tabsArea = doc.createElement('ul');
    this.tabsArea.className = 'tabs-area';
    this.tabsContainer = doc.createElement('div');
    this.tabsContainer.className = 'tabs-container';

    this.tabs.appendChild(this.tabsArea);
    this.tabs.appendChild(this.tabsContainer);
    this.tabs.className = 'tabs-zone';

    var len = tabs.length;

    if (len > 0) {
        for (var index = 0; index < len; index++) {
            this.addTab(tabs[index]);
        }

        $(this.tabsArea).children('li').eq(0).trigger('click');
    }

    $(window).unload(function() {
        $(this.tabsArea).children('li').unbind('click');
    });
};

DDoc.Controls.Tabs.prototype.addTab = function(tab) {
    var self = this;
    var doc = document;
    var tabInfo = $.extend(true, {}, this.defaultData, tab);
    var tab = doc.createElement('li');
    tab.className = 'tab';
    tab.innerHTML = tabInfo.text;
    var tabContent = doc.createElement('div');
    tabContent.className = 'tab-content';

    this.tabsArea.appendChild(tab);
    this.tabsContainer.appendChild(tabContent);

    if (!tabInfo.visible) {
        $(tab).css('display', 'none');
    }

    $(tab).click(function() {
        var index = $(self.tabsArea).children('li').index(this);
        self.activateTab(index);
    });

    var element = doc.getElementById(tabInfo.content);

    if (element) {
        tabContent.appendChild(element);
    }
};

DDoc.Controls.Tabs.prototype.activateTab = function(index) {
    var cancel = false;
    var $tabs = $(this.tabsArea).children('li');

    if (index >= 0 && index <= $tabs.length - 1) {
        this.fire({ type: 'onChangeTab', indexTab: this.activeTab, cancel: cancel });

        $tabs.removeClass('tab-active');
        $tabs.eq(index).addClass('tab-active');
        var $tabsContents = $(this.tabsContainer).children('.tab-content');
        $tabsContents.removeClass('tab-content-active');
        $tabsContents.eq(index).addClass('tab-content-active');
        this.activeTab = index;

        this.fire({ type: 'onTabChanged', indexTab: this.activeTab });
    }
};

DDoc.Controls.Tabs.prototype.hideTab = function(index) {
    var $tabs = $(this.tabsArea).children('li');

    if (index >= 0 && index <= $tabs.length - 1) {
        var $tab = $tabs.eq(index);

        if (this.activeTab == index) {
            if (index == $tabs.length - 1) {
                $tabs.eq(index - 1).trigger('click');
            } else {
                $tabs.eq(index + 1).trigger('click');
            }
        }

        $tab.css('display', 'none');
    }
};

DDoc.Controls.Tabs.prototype.showTab = function(index, activate) {
    var $tabs = $(this.tabsArea).children('li');

    if (index >= 0 && index <= $tabs.length - 1) {
        var $tab = $tabs.eq(index);

        $tab.removeAttr('style');

        if (activate) {
            $tab.trigger('click');
        }
    }
};

DDoc.Controls.Tabs.prototype.getCurrentTab = function() {
    return this.activeTab;
};