DDocUi.prototype.RenderReport = function (reportData) {
	var self = this;

	var colors = [];
	for (var i = 0; i < 36; i++) {
		colors.push(ddoc.GetRandomColor());
	}

	function buildDateLabels(labelData) {
		var dateLabels = new Array();
		for (var i = 0; i < labelData.length; i++) {
			var timeString = labelData[i].toUTCString().replace('GMT', '');
			var time = new Date(timeString);
			dateLabels.push(
				('0' + time.getDate()).slice(-2) + '/' +
				('0' + (time.getMonth() + 1)).slice(-2) + '/' +
				time.getFullYear());
		}
		return dateLabels;
	}

	var labels = buildDateLabels(reportData[0]);
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

	var totalCount = 0;

	this.Chart = new Chart($('#report')[0].getContext('2d'), {
		type: 'bar',
		data: chartData,
		options: {
			responsive: true,
			title: {
				display: true,
				fontColor: 'black',
				text: 'Reporte Diario de Migraciones'
			},
			legend: { position: 'bottom' },
			legendCallback: function() {
				var legendHtml = [];
				legendHtml.push('<tr>');
				legendHtml.push('<th colspan="2">Fechas</th>');
				legendHtml.push('<th>' + $('#CollectionPage option:selected').text() + '</th>');
				legendHtml.push('</tr>');
				for (var i = 0; i < reportData[0].length; i++) {
					if (values[i] !== 0) {
						legendHtml.push('<tr>');
						legendHtml.push('<td class="legend-color" style="background-color:' + colors[i] + '">&nbsp;</td>');
						legendHtml.push('<td class="legend-label">' + labels[i] + '</td>');
						legendHtml.push('<td class="legend-value">' + values[i] + '</td>');
						legendHtml.push('</tr>');
						totalCount += values[i];
					}
				}
				legendHtml.push('<tr>');
				legendHtml.push('<td>&nbsp;</td>');
				legendHtml.push('<td class="legend-label"> TOTAL: </td>');
				legendHtml.push('<td class="legend-value">' + totalCount + '</td>');
				legendHtml.push('</tr>');
				return legendHtml.join('');
			},
			scales: {
				xAxes: [{ gridLines: { display: false }, display: true, scaleLabel: { display: false, labelString: '' }, ticks: { fontSize: 8 } }],
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

	$('#lblFromDate').html($('#startDate').val());
	$('#lblToDate').html($('#endDate').val());
	$('#legendTable').html(self.Chart.generateLegend());
	$('.report-container').show();
	ddoc.HideLoader();
};

DDocUi.prototype.ValidateFilters = function () {
	var fromDate = $('#startDate').val();
	var toDate = $('#endDate').val();
	if (fromDate == '' || toDate == '') {
		alert('Ingrese un rango de fechas correcto.');
		return false;
	} else if (toDate < fromDate) {
		alert('La fecha final debe ser mayor a la inicial.');
		return false;
	}
	return true;
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

DDocUi.prototype.InitDailyReport = function () {
	(function setupEvents () {
		$('#btnGetData').click(function (e) {
			e.preventDefault();
			if (ddoc.ValidateFilters()) {
				ddoc.ShowLoader();
				ddoc.POST('/Reports/GetDailyReport', {
					startDate: $('#startDate').val(),
					endDate: $('#endDate').val(),
					reportType: parseInt($('#CollectionPage').val())
				}, function (response) {
					ddoc.RenderReport(response.List);
				});
			}
		});
	})();

	$('input,select').eq(0).focus();
};

var ddoc = new DDocUi();
ddoc.InitDailyReport();
