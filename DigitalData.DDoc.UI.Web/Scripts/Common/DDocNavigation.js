DDocUi.prototype.InitializeNavigation = function () {

	$('.current-user', '.main-menu').click(function () {
		$('.main-sub-menu').toggle();
	});

	$('.main-sub-menu').on('mouseleave', function () { $(this).delay(300).fadeOut(500); });

	$('.main-sub-menu').on('click', 'li', function () {
		switch (this.id) {
			case 'changeMyPassword':
				ddoc.user = ddoc.GetUsername();
				ddoc.ChangePassword();
				break;
		}
	});

	if ($('#tree').length == 1) {
		$('#tree').fancytree({
			extensions: ['filter'],
			filter: {
				mode: 'hide'
			},
			activate: function (e, data) {
				if (ddoc.Navigate) {
					var collectionId = data.node.key.substr(data.node.key.lastIndexOf('/') + 1);
					window.open(ddoc.GetUrl('/Navigation/Navigate/' + collectionId), 'workspace');
				} else {
					ddoc.Navigate = true;
				}
			},
			select: function (e, data) {
				var collectionId = data.node.key;
				var grid = $('.work-space')[0].contentWindow.jQuery('.grid-container.' + collectionId);
				if (data.node.selected) {
					grid.show();
				} else {
					grid.hide();
				}
			},
			source: {
				url: ddoc.GetUrl('/Navigation/NavigationTree'),
				cache: false
			}
		});

		$('.fancytree-container').addClass('fancytree-connectors');

		ddoc.Navigate = false;
		ddoc.NavigationTree = $('#tree').fancytree('getTree');
	}
}

var ddoc = new DDocUi();
ddoc.InitializeNavigation();