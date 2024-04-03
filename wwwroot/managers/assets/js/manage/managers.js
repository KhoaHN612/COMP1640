//onload
document.addEventListener('DOMContentLoaded', function () {
    GetTotalContributionsData();
	GetApprovedContributionData();
	GetRejectedContributionData();
	GetPendingContributionData();
});

function GetContribution() {
    
    var colors = ['#008FFB', '#00E396', '#FEB019', '#FF4560', '#775DD0'];
    var options = {
        series: [{
            data: [21, 22, 10, 28, 16]
        }],
        chart: {
            height: 350,
            type: 'bar',
            toolbar: {
                show: false
            },
            events: {
                click: function (chart, w, e) {
                    console.log(chart, w, e)
                }
            }
        },
        colors: colors,
        plotOptions: {
            bar: {
                columnWidth: '45%',
                distributed: true,
            }
        },
        dataLabels: {
            enabled: false
        },
        legend: {
            show: false
        },
        xaxis: {
            categories: [
                'John Doe',
                'Joe Smith',
                'Jake Williams',
                'Amber',
                'Peter Brown',
            ],
            labels: {
                style: {
                    colors: colors,
                    fontSize: '12px'
                }
            }
        }
    };

    var chart = new ApexCharts(document.querySelector("#chartContributions"), options);
    chart.render();
}

function GetTotalContributionsData() {
	//Retrieve the data from the HTML data attribute
	var contriDataElement = document.getElementById("ContributionsTotalData");
	if (contriDataElement) {
		var contributionsData = JSON.parse(contriDataElement.dataset.total);

		var dataTotalPie = [[], []];

		//Lấy tất cả tên faculty
		for (var i = 0; i < contributionsData.length; i++) {
			var facultyName = contributionsData[i].facultyName;
			if (!dataTotalPie[0].includes(facultyName)) {
				dataTotalPie[0].push(facultyName);
			}
		}

		//Lấy số lượng bài viết theo faculty
		for (var i = 0; i < dataTotalPie[0].length; i++) {
			var total = 0;
			for (var j = 0; j < contributionsData.length; j++) {
				if (dataTotalPie[0][i] == contributionsData[j].facultyName) {
					total += contributionsData[j].totalByMonth;
				}
			}
			dataTotalPie[1].push(total);
		}
		var selectedTotalYear = document.getElementById("yearTotal").value;

		//Get all months name
		var dataTotalColumn = [[], []];
		dataTotalColumn[0] = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
		tempTotalMonth = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

		var currentTotalYear = new Date().getFullYear();
		if (selectedTotalYear == currentTotalYear) {
			var currentMonth = new Date().getMonth();
			dataTotalColumn[0] = dataTotalColumn[0].slice(0, currentMonth + 1);
			tempTotalMonth = tempTotalMonth.slice(0, currentMonth + 1);
		}
		console.log(tempTotalMonth);

		//Lấy số lượng bài viết theo tháng
		for (var i = 0; i < tempTotalMonth.length; i++) {
			var total = 0;
			for (var j = 0; j < contributionsData.length; j++) {
				if (tempTotalMonth[i] == contributionsData[j].month) {
					total += contributionsData[j].totalByMonth;
				}
			}
			dataTotalColumn[1].push(total);
		}

		var arrLabelPie = [];
		for (var i = 0; i < dataTotalPie[0].length; i++) {
			arrLabelPie.push(dataTotalPie[0][i] + ' - ' + dataTotalPie[1][i] + ' articles');
		}
		// CHART TOTAL CONTRIBUTIONS
		var optionsPie = {
			series: dataTotalPie[1],
			chart: {
				type: 'pie',
			},
			labels: arrLabelPie,
			responsive: [{
				breakpoint: 480,
				options: {
					chart: {
						width: '100%'
					},
					legend: {
						position: 'bottom'
					}
				}
			}]
		};

		// Dữ liệu mẫu cho biểu đồ cột
		var optionsColumn = {
			series: [{
				name: 'Inflation',
				data: dataTotalColumn[1]
			}],
			chart: {
				height: 350,
				type: 'bar',
				toolbar: {
					show: false
				}
			},
			plotOptions: {
				bar: {
					borderRadius: 10,
					dataLabels: {
						position: 'top', // top, center, bottom
					},
				}
			},
			dataLabels: {
				enabled: true,
				formatter: function (val) {
					return val + "";
				},
				offsetY: -20,
				style: {
					fontSize: '12px',
					colors: ["#304758"]
				}
			},

			xaxis: {
				categories: dataTotalColumn[0],
				position: 'top',
				axisBorder: {
					show: false
				},
				axisTicks: {
					show: false
				},
				crosshairs: {
					fill: {
						type: 'gradient',
						gradient: {
							colorFrom: '#D8E3F0',
							colorTo: '#BED1E6',
							stops: [0, 100],
							opacityFrom: 0.4,
							opacityTo: 0.5,
						}
					}
				},
				tooltip: {
					enabled: true,
				}
			},
			yaxis: {
				axisBorder: {
					show: false
				},
				axisTicks: {
					show: false,
				},
				labels: {
					show: false,
					formatter: function (val) {
						return val + "";
					}
				}

			}
		};

		// tổng contributions
		var sum = dataTotalPie[1].reduce((a, b) => a + b, 0);
		$('#totalArticles').text(sum + ' articles');

		// Render biểu đồ tròn
		var chartPie = new ApexCharts(document.querySelector("#chartPieContributions"), optionsPie);
		chartPie.render();

		// Render biểu đồ cột
		var chartColumn = new ApexCharts(document.querySelector("#chartColumnContributions"), optionsColumn);
		chartColumn.render();
	}
}


function GetApprovedContributionData() {
	var approvedDataElement = document.getElementById("ContributionsApprovedData");
	if(approvedDataElement){
		var approvedData = JSON.parse(approvedDataElement.dataset.approved);

		var dataApprovedPie = [[], []];
		for (var i = 0; i < approvedData.length; i++) {
			var facultyName = approvedData[i].facultyName;
			if (!dataApprovedPie[0].includes(facultyName)) {
				dataApprovedPie[0].push(facultyName);
			}
		}
	
		for (var i = 0; i < dataApprovedPie[0].length; i++) {
			var total = 0;
			for (var j = 0; j < approvedData.length; j++) {
				if (dataApprovedPie[0][i] == approvedData[j].facultyName) {
					total += approvedData[j].totalByMonth;
				}
			}
			dataApprovedPie[1].push(total);
		}
	
		var selectedApprovedYear = document.getElementById("yearApproved").value;
		var dataApprovedColumn = [[], []];
		dataApprovedColumn[0] = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
		tempApprovedMonth = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
	
		var currentApprovedYear = new Date().getFullYear();
		if (selectedApprovedYear == currentApprovedYear) {
			var currentMonth = new Date().getMonth();
			dataApprovedColumn[0] = dataApprovedColumn[0].slice(0, currentMonth + 1);
			tempApprovedMonth = tempApprovedMonth.slice(0, currentMonth + 1);
		}
	
		for (var i = 0; i < tempApprovedMonth.length; i++) {
			var total = 0;
			for (var j = 0; j < approvedData.length; j++) {
				if (tempApprovedMonth[i] == approvedData[j].month) {
					total += approvedData[j].totalByMonth;
				}
			}
			dataApprovedColumn[1].push(total);
		}
	
		var arrLabelPie = [];
		for (var i = 0; i < dataApprovedPie[0].length; i++) {
			arrLabelPie.push(dataApprovedPie[0][i] + ' - ' + dataApprovedPie[1][i] + ' articles');
		}
	
		var optionsPie = {
			series: dataApprovedPie[1],
			chart: {
				type: 'pie',
			},
			labels: arrLabelPie,
			responsive: [{
				breakpoint: 480,
				options: {
					chart: {
						width: '100%'
					},
					legend: {
						position: 'bottom'
					}
				}
			}]
		};
	
		var optionsColumn = {
			series: [{
				name: 'Inflation',
				data: dataApprovedColumn[1]
			}],
			chart: {
				height: 350,
				type: 'bar',
				toolbar: {
					show: false
				}
			},
			plotOptions: {
				bar: {
					borderRadius: 10,
					dataLabels: {
						position: 'top', // top, center, bottom
					},
				}
			},
			dataLabels: {
				enabled: true,
				formatter: function (val) {
					return val + "";
				},
				offsetY: -20,
				style: {
					fontSize: '12px',
					colors: ["#304758"]
				}
			},
	
			xaxis: {
				categories: dataApprovedColumn[0],
				position: 'top',
				axisBorder: {
					show: false
				},
				axisTicks: {
					show: false
				},
				crosshairs: {
					fill: {
						type: 'gradient',
						gradient: {
							colorFrom: '#D8E3F0',
							colorTo: '#BED1E6',
							stops: [0, 100],
							opacityFrom: 0.4,
							opacityTo: 0.5,
						}
					}
				},
				tooltip: {
					enabled: true,
				}
			},
			yaxis: {
				axisBorder: {
					show: false
				},
				axisTicks: {
					show: false,
				},
				labels: {
					show: false,
					formatter: function (val) {
						return val + "";
					}
				}
	
			}
		};
	
		var sum = dataApprovedPie[1].reduce((a, b) => a + b, 0);
		$('#totalApproved').text(sum + ' articles');
	
		var chartPie = new ApexCharts(document.querySelector("#chartPieApproved"), optionsPie);
		chartPie.render();
	
		var chartColumn = new ApexCharts(document.querySelector("#chartColumnApproved"), optionsColumn);
		chartColumn.render();
	}
}

function GetRejectedContributionData() {
	//Tương tự như GetTotalContributionsData
	var rejectedDataElement = document.getElementById("ContributionsRejectedData");
	if (rejectedDataElement) {
		var rejectedData = JSON.parse(rejectedDataElement.dataset.rejected);

		var dataRejectedPie = [[], []];
		for (var i = 0; i < rejectedData.length; i++) {
			var facultyName = rejectedData[i].facultyName;
			if (!dataRejectedPie[0].includes(facultyName)) {
				dataRejectedPie[0].push(facultyName);
			}
		}

		for (var i = 0; i < dataRejectedPie[0].length; i++) {
			var total = 0;
			for (var j = 0; j < rejectedData.length; j++) {
				if (dataRejectedPie[0][i] == rejectedData[j].facultyName) {
					total += rejectedData[j].totalByMonth;
				}
			}
			dataRejectedPie[1].push(total);
		}

		var selectedRejectedYear = document.getElementById("yearRejected").value;
		var dataRejectedColumn = [[], []];
		dataRejectedColumn[0] = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
		tempRejectedMonth = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

		var currentRejectedYear = new Date().getFullYear();
		if (selectedRejectedYear == currentRejectedYear) {
			var currentMonth = new Date().getMonth();
			dataRejectedColumn[0] = dataRejectedColumn[0].slice(0, currentMonth + 1);
			tempRejectedMonth = tempRejectedMonth.slice(0, currentMonth + 1);
		}

		for (var i = 0; i < tempRejectedMonth.length; i++) {
			var total = 0;
			for (var j = 0; j < rejectedData.length; j++) {
				if (tempRejectedMonth[i] == rejectedData[j].month) {
					total += rejectedData[j].totalByMonth;
				}
			}
			dataRejectedColumn[1].push(total);
		}

		var arrLabelPie = [];
		for (var i = 0; i < dataRejectedPie[0].length; i++) {
			arrLabelPie.push(dataRejectedPie[0][i] + ' - ' + dataRejectedPie[1][i] + ' articles');
		}

		var optionsPie = {
			series: dataRejectedPie[1],
			chart: {
				type: 'pie',
			},
			labels: arrLabelPie,
			responsive: [{
				breakpoint: 480,
				options: {
					chart: {
						height: '100%'
					},
					legend: {
						position: 'bottom'
					}
				}
			}]
		};

		var optionsColumn = {
			series: [{
				name: 'Inflation',
				data: dataRejectedColumn[1]
			}],
			chart: {
				height: 450,
				type: 'bar',
				toolbar: {
					show: false
				}
			},
			plotOptions: {
				bar: {
					borderRadius: 10,
					dataLabels: {
						position: 'top', // top, center, bottom
					},
				}
			},
			dataLabels: {
				enabled: true,
				formatter: function (val) {
					return val + "";
				},
				offsetY: -20,
				style: {
					fontSize: '12px',
					colors: ["#304758"]
				}
			},

			xaxis: {
				categories: dataRejectedColumn[0],
				position: 'top',
				axisBorder: {
					show: false
				},
				axisTicks: {
					show: false
				},
				crosshairs: {
					fill: {
						type: 'gradient',
						gradient: {
							colorFrom: '#D8E3F0',
							colorTo: '#BED1E6',
							stops: [0, 100],
							opacityFrom: 0.4,
							opacityTo: 0.5,
						}
					}
				},
				tooltip: {
					enabled: true,
				}
			},
			yaxis: {
				axisBorder: {
					show: false
				},
				axisTicks: {
					show: false,
				},
				labels: {
					show: false,
					formatter: function (val) {
						return val + "";
					}
				}

			}
		};

		var sum = dataRejectedPie[1].reduce((a, b) => a + b, 0);
		$('#totalRejected').text(sum + ' articles');

		var chartPie = new ApexCharts(document.querySelector("#chartPieRejected"), optionsPie);
		chartPie.render();

		var chartColumn = new ApexCharts(document.querySelector("#chartColumnRejected"), optionsColumn);
		chartColumn.render();
	}
}

function GetPendingContributionData() {
	//Tương tự như GetTotalContributionsData
	var pendingDataElement = document.getElementById("ContributionsPendingData");
	if (pendingDataElement) {
		var pendingData = JSON.parse(pendingDataElement.dataset.pending);

		var dataPendingPie = [[], []];
		for (var i = 0; i < pendingData.length; i++) {
			var facultyName = pendingData[i].facultyName;
			if (!dataPendingPie[0].includes(facultyName)) {
				dataPendingPie[0].push(facultyName);
			}
		}

		for (var i = 0; i < dataPendingPie[0].length; i++) {
			var total = 0;
			for (var j = 0; j < pendingData.length; j++) {
				if (dataPendingPie[0][i] == pendingData[j].facultyName) {
					total += pendingData[j].totalByMonth;
				}
			}
			dataPendingPie[1].push(total);
		}

		var selectedPendingYear = document.getElementById("yearPending").value;
		var dataPendingColumn = [[], []];
		dataPendingColumn[0] = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
		tempPendingMonth = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

		var currentPendingYear = new Date().getFullYear();
		if (selectedPendingYear == currentPendingYear) {
			var currentMonth = new Date().getMonth();
			dataPendingColumn[0] = dataPendingColumn[0].slice(0, currentMonth + 1);
			tempPendingMonth = tempPendingMonth.slice(0, currentMonth + 1);
		}

		for (var i = 0; i < tempPendingMonth.length; i++) {
			var total = 0;
			for (var j = 0; j < pendingData.length; j++) {
				if (tempPendingMonth[i] == pendingData[j].month) {
					total += pendingData[j].totalByMonth;
				}
			}
			dataPendingColumn[1].push(total);
		}

		var arrLabelPie = [];
		for (var i = 0; i < dataPendingPie[0].length; i++) {
			arrLabelPie.push(dataPendingPie[0][i] + ' - ' + dataPendingPie[1][i] + ' articles');
		}

		var optionsPie = {
			series: dataPendingPie[1],
			chart: {
				type: 'pie',
			},
			labels: arrLabelPie,
			responsive: [{
				breakpoint: 480,
				options: {
					chart: {
						width: '100%'
					},
					legend: {
						position: 'bottom'
					}
				}
			}]
		};

		var optionsColumn = {
			series: [{
				name: 'Inflation',
				data: dataPendingColumn[1]
			}],
			chart: {
				height: 350,
				type: 'bar',
				toolbar: {
					show: false
				}
			},
			plotOptions: {
				bar: {
					borderRadius: 10,
					dataLabels: {
						position: 'top', // top, center, bottom
					},
				}
			},
			dataLabels: {
				enabled: true,
				formatter: function (val) {
					return val + "";
				},
				offsetY: -20,
				style: {
					fontSize: '12px',
					colors: ["#304758"]
				}
			},
			xaxis: {
				categories: dataPendingColumn[0],
				position: 'top',
				axisBorder: {
					show: false
				},
				axisTicks: {
					show: false
				},
				crosshairs: {
					fill: {
						type: 'gradient',
						gradient: {
							colorFrom: '#D8E3F0',
							colorTo: '#BED1E6',
							stops: [0, 100],
							opacityFrom: 0.4,
							opacityTo: 0.5,
						}
					}
				},
				tooltip: {
					enabled: true,
				}
			},
			yaxis: {
				axisBorder: {
					show: false
				},
				axisTicks: {
					show: false,
				},
				labels: {
					show: false,
					formatter: function (val) {
						return val + "";
					}
				}

			}
		};

		var sum = dataPendingPie[1].reduce((a, b) => a + b, 0);
		$('#totalPending').text(sum + ' articles');

		var chartPie = new ApexCharts(document.querySelector("#chartPiePending"), optionsPie);
		chartPie.render();

		var chartColumn = new ApexCharts(document.querySelector("#chartColumnPending"), optionsColumn);
		chartColumn.render();
	}
}

function SelectedYearContributions(year) {
	var url = '/Managers/IndexManagers?task=TotalContribution&year=' + year;
	//redirect to url
	window.location.href = url;
}


function SelectedYearApproved(year) {
	var url = '/Managers/IndexManagers?task=ApprovedContribution&year=' + year;
	//redirect to url
	window.location.href = url;
}

function SelectedYearRejected(year) {
	var url = '/Managers/IndexManagers?task=RejectedContribution&year=' + year;
	//redirect to url
	window.location.href = url;
}

function SelectedYearPending(year) {
	var url = '/Managers/IndexManagers?task=PendingContribution&year=' + year;
	//redirect to url
	window.location.href = url;
}