DDocUi.prototype.RenderReport = function (reportData) {
	var self = this;

	var colors = [];
	for (var i = 0; i < 36; i++) {
		colors.push(ddoc.GetRandomColor());
	}

	var labels = reportData[0];
	var values = reportData[1];
	var chartData = {
		labels: labels,
		datasets: [{
			label: 'Total de ' + $('#CollectionPage option:selected').text(),
			data: values,
			fill: false,
			backgroundColor: colors,
			borderWidth: 1
		}]
	};

	$('#report').remove();
	$('#reportTitle').after($('<canvas id="report">'));

	this.Chart = new Chart($('#report')[0].getContext('2d'), {
		type: 'bar',
		data: chartData,
		options: {
			responsive: true,
			title: {
				display: true,
				fontColor: 'black',
				text: 'Reporte Total de Migraciones'
			},
			legend: { position: 'bottom' },
			legendCallback: function () {
				var legendHtml = [];
				legendHtml.push('<tr>');
				legendHtml.push('<th colspan="2">Colección</th>');
				legendHtml.push('<th>' + $('#CollectionPage option:selected').text() + '</th>');
				legendHtml.push('</tr>');
				for (var i = 0; i < reportData[0].length; i++) {
					legendHtml.push('<tr>');
					legendHtml.push('<td class="legend-color" style="background-color:' + colors[i] + '">&nbsp;</td>');
					legendHtml.push('<td class="legend-label">' + labels[i] + '</td>');
					legendHtml.push('<td class="legend-value">' + values[i] + '</td>');
					legendHtml.push('</tr>');
				}
				return legendHtml.join('');
			},
			scales: {
				xAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' }, ticks: { fontSize: 0 } }],
				yAxes: [{
					gridLines: { display: true }, display: true, scaleLabel: { display: false, labelString: '' }, ticks: {
						min: 0,
						callback: function (value) {
							if (Math.floor(value) === value) {
								return value;
							}
						}
					}
				}]
			}
		}
	});

	$('#totalsLabel').text(chartData.datasets[0].label);
	var itemTotals = 0;
	for (var i = 0; i < values.length; i++) {
		itemTotals += parseInt(values[i]);
	}
	$('#totalsValue').text(itemTotals);
	$('#legendTable').html(self.Chart.generateLegend());
	$('.report-container').show();
	ddoc.HideLoader();
};

DDocUi.prototype.ShowLoader = function () {
	var loader = document.createElement('div');
	loader.className = 'report-loader';
	$('.report-container').before(loader);
	$('.report-container').hide();
};

DDocUi.prototype.HideLoader = function () {
	$('.report-loader').remove();
};

DDocUi.prototype.InitTotalReport = function () {
	(function setupEvents () {
		$('#btnGetData').click(function (e) {
			e.preventDefault();
			ddoc.ShowLoader();
			ddoc.POST('/Reports/GetTotalReport', {
				typeReport: parseInt($('#CollectionPage').val())
			}, function (response) {
				ddoc.RenderReport(response.List);
			});
		});
    })();

    $('input,select').eq(0).focus();
}

var ddoc = new DDocUi();
ddoc.InitTotalReport();