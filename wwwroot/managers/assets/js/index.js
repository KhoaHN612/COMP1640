function GetTotalContributionsData() {
	//Retrieve the data from the HTML data attribute
	var contriDataElement = document.getElementById("ContributionsTotalData");
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

	// CHART TOTAL CONTRIBUTIONS
	var optionsPie = {
		series: dataTotalPie[1],
		chart: {
			width: 380,
			type: 'pie',
		},
		labels: dataTotalPie[0],
		responsive: [{
			breakpoint: 480,
			options: {
				chart: {
					width: 800
				},
				legend: {
					position: 'bottom'
				}
			}
		}]
	};

	// Dữ liệu mẫu cho biểu đồ cột
	var optionsColumn =  {
		series: [{
		name: 'Inflation',
		data: dataTotalColumn[1]
	}],
		chart: {
		height: 350,
		type: 'bar',
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

function GetApprovedContributionData() {
	//Tương tự như GetTotalContributionsData
	var approvedDataElement = document.getElementById("ContributionsApprovedData");
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

	var optionsPie = {
		series: dataApprovedPie[1],
		chart: {
			width: 380,
			type: 'pie',
		},
		labels: dataApprovedPie[0],
		responsive: [{
			breakpoint: 480,
			options: {
				chart: {
					width: 800
				},
				legend: {
					position: 'bottom'
				}
			}
		}]
	};

	var optionsColumn =  {
		series: [{
		name: 'Inflation',
		data: dataApprovedColumn[1]
	}],
		chart: {
		height: 350,
		type: 'bar',
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

function GetRejectedContributionData() {
	//Tương tự như GetTotalContributionsData
	var rejectedDataElement = document.getElementById("ContributionsRejectedData");
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

	var optionsPie = {
		series: dataRejectedPie[1],
		chart: {
			width
			: 380,
			type: 'pie',
		},	
		labels: dataRejectedPie[0],
		responsive: [{
			breakpoint: 480,
			options: {
				chart: {
					height: 800
				},
				legend: {
					position: 'bottom'
				}
			}
		}]
	};

	var optionsColumn =  {
		series: [{
		name: 'Inflation',
		data: dataRejectedColumn[1]
	}],
		chart: {
		height: 350,
		type: 'bar',
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

function GetPendingContributionData() {
	//Tương tự như GetTotalContributionsData
	var pendingDataElement = document.getElementById("ContributionsPendingData");
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

	var optionsPie = {
		series: dataPendingPie[1],
		chart: {
			width: 380,
			type: 'pie',
		},
		labels: dataPendingPie[0],
		responsive: [{
			breakpoint: 480,
			options: {
				chart: {
					width: 800
				},
				legend: {
					position: 'bottom'
				}
			}
		}]
	};

	var optionsColumn =  {
		series: [{
		name: 'Inflation',
		data: dataPendingColumn[1]
	}],
		chart: {
		height: 350,
		type: 'bar',
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


//sk onload
$(window).on('load', function () {
	
	//======================================================= MANAGER START =======================================================
	GetTotalContributionsData();
	GetApprovedContributionData();
	GetRejectedContributionData();
	GetPendingContributionData();
	//======================================================= MANAGER END =======================================================


	//=======================================================COORDINATOR DASHBOARD=======================================================
	// chart 1
	// Retrieve the data from the HTML data attribute
	var totalContributionDataElement = document.getElementById("totalContributionData");
	var totalContributionData = JSON.parse(totalContributionDataElement.dataset.totalcontribution);

	//Sum of totalContributionData by totalContributionData
	var sumContribution = totalContributionData.reduce((a, b) => a + b.total, 0);
	document.getElementById('totalContributions').innerText = sumContribution + ' articles';

	// total Accepted
	var totalAcceptedDataElement = document.getElementById("totalAcceptedData");
	var totalAcceptedData = JSON.parse(totalAcceptedDataElement.dataset.totalaccepted);

	var sumAccepted = totalAcceptedData.reduce((a, b) => a + b.total, 0);
	document.getElementById('totalAccepted').innerText = sumAccepted + ' articles';

	// total Rejected
	var totalRejectedDataElement = document.getElementById("totalRejectedData");
	var totalRejectedData = JSON.parse(totalRejectedDataElement.dataset.totalrejected);

	var sumRejected = totalRejectedData.reduce((a, b) => a + b.total, 0);
	document.getElementById('totalRejected').innerText = sumRejected + ' articles';

	// total Pending
	var totalPendingDataElement = document.getElementById("totalPendingData");
	var totalPendingData = JSON.parse(totalPendingDataElement.dataset.totalpending);

	var sumPending = totalPendingData.reduce((a, b) => a + b.total, 0);
	document.getElementById('totalPending').innerText = sumPending + ' articles';	



	//=======================================================COORDINATORS CHART=======================================================
	//Retrieve the data from the HTML data attribute
	var contributionDataElement = document.getElementById("totalContributionsData");
	var contributionsData = JSON.parse(contributionDataElement.dataset.totalcontributions);

	var totalContributionDataElement = document.getElementById("contributionWithoutCommentsData");
	var totalContributionData = JSON.parse(totalContributionDataElement.dataset.contributionwithoutcomments);

	var totalContributionAfter14DaysDataElement = document.getElementById("contributionWithoutCommentsAfter14DaysData");
	var totalContributionAfter14DaysData = JSON.parse(totalContributionAfter14DaysDataElement.dataset.contributionwithoutcommentsafter14days);
	
	//Lấy tất tháng hiện tại
	const today = new Date();
	const startOfYear = new Date(today.getFullYear(), 0, 1);

	const formattedStartDate = `${startOfYear.getFullYear()}-${(startOfYear.getMonth() + 1).toString().padStart(2, '0')}-${startOfYear.getDate().toString().padStart(2, '0')}`;
	const formattedDate = new Date().toISOString().slice(0, 10);

	//Lấy dữ liệu từ totalContributionData và totalContributionAfter14DaysData
	var data = GetData(formattedStartDate, formattedDate, contributionsData, totalContributionData, totalContributionAfter14DaysData);
	
	console.log(data);

	var arrLabelDate = data[0];
	var arrData1 = data[1];
	var arrData2 = data[2];
	var arrData3 = data[3];

	var options = {
		series: [{
		name: 'All Contributions',
		type: 'column',
		data: arrData1
	  }, {
		name: 'Contribution Without Comments',
		type: 'area',
		data: arrData2
	  }, {
		name: 'Contributions After 14 Days Without Comments',
		type: 'line',
		data: arrData3
	  }],
		chart: {
		height: 350,
		type: 'line',
		stacked: false,
	  },
	  stroke: {
		width: [0, 2, 5],
		curve: 'smooth'
	  },
	  plotOptions: {
		bar: {
		  columnWidth: '50%'
		}
	  },
	  
	  fill: {
		opacity: [0.85, 0.25, 1],
		gradient: {
		  inverseColors: false,
		  shade: 'light',
		  type: "vertical",
		  opacityFrom: 0.85,
		  opacityTo: 0.55,
		  stops: [0, 100, 100, 100]
		}
	  },
	  labels: arrLabelDate,
	  markers: {
		size: 0
	  },
	  xaxis: {
		type: 'datetime'
	  },
	  yaxis: {
		title: {
		  text: 'Points',
		},
		min: 0
	  },
	  tooltip: {
		shared: true,
		intersect: false,
		y: {
		  formatter: function (y) {
			if (typeof y !== "undefined") {
			  return y.toFixed(0) + " points";
			}
			return y;
	  
		  }
		}
	  }
	  };

	var chart = new ApexCharts(document.querySelector("#chartCoordinators1"), options);
	chart.render();
});

function GetData(startDate, endDate, contributionsData, totalContributionData, totalContributionAfter14DaysData) {
	const dates = [];
	var totalcontributions = [];
	var totalContributionWithoutComment = [];
	var totalContributionAfter14Days = [];
	let currentDate = new Date(startDate);

	// Lặp cho đến khi currentDate lớn hơn endDate
	while (currentDate <= new Date(endDate)) {
		dates.push(currentDate.toISOString().slice(0, 10));
		//Duyệt qua vòng lặp mà không có ngày nào trong totalContributionData thì gán giá trị 0
		let total = 0;
		let total1 = 0;
		let total2 = 0;

		for (var i = 0; i < contributionsData.length; i++) {
			var formatDate = new Date(contributionsData[i].date + 'Z').toISOString().slice(0, 10);

			if (formatDate == currentDate.toISOString().slice(0, 10)) {
				total = contributionsData[i].quantity;
				break;
			}
		}

		for (var i = 0; i < totalContributionData.length; i++) {
			var formatDate = new Date(totalContributionData[i].date + 'Z').toISOString().slice(0, 10);

			if (formatDate == currentDate.toISOString().slice(0, 10)){
				total1 = totalContributionData[i].quantity;
				break;
			}
		}

		for (var i = 0; i < totalContributionAfter14DaysData.length; i++) {
			var formatDate = new Date(totalContributionAfter14DaysData[i].date + 'Z').toISOString().slice(0, 10);

			if (formatDate == currentDate.toISOString().slice(0, 10)) {
				total2 = totalContributionAfter14DaysData[i].quantity;
				break;
			}
		}
		
		totalcontributions.push(total);
		totalContributionAfter14Days.push(total2);
		totalContributionWithoutComment.push(total1);

		currentDate.setDate(currentDate.getDate() + 1);
	}
	
	return [dates, totalcontributions, totalContributionWithoutComment, totalContributionAfter14Days];
}

function SelectedYearInCoordinators(year) {
	var url = '/Managers/IndexCooridinators?task=TotalContribution&year=' + year;

	//redirect to url
	window.location.href = url;
}

function SelectedYearUser(year) {
	var url = '/Managers?task=ContributionUser&year=' + year;
	//redirect to url
	window.location.href = url;
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

function GetContributionByYear(year){
	var url = '/Managers?task=ContributionFaculty&year=' + year;
	//redirect to url
	window.location.href = url;
}

$(function () {
	"use strict";

	//=======================================================ADMIN DASHBOARD=======================================================
	// chart 5
	var arrLabel = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

	var arrData = new Array(12).fill(0);
	var contributionItems = document.querySelectorAll('.contribution-item');

	for (var i = 0; i < contributionItems.length; i++) {
		var contributionItem = contributionItems[i];
		var month = parseInt(contributionItem.getAttribute('data-month'));
		var total = contributionItem.getAttribute('data-total');
		arrData[month - 1] = total;
	}

	var selectedYear = document.getElementById("year").value;
	var currentYear = new Date().getFullYear();
	if (selectedYear == currentYear) {
		var currentMonth = new Date().getMonth();
		arrLabel = arrLabel.slice(0, currentMonth + 1);
		arrData = arrData.slice(0, currentMonth + 1);
	}

	//Total Articles by sum of arrData
	var sum = arrData.reduce((a, b) => parseInt(a) + parseInt(b), 0);
	document.getElementById('totalArticles').innerText = sum + ' articles';
	

	var options = {
		series: [{
			name: "Articles",
			data: arrData
		}],
		chart: {
			type: "area",
			// width: 130,
			stacked: true,
			height: 280,
			toolbar: {
				show: !1
			},
			zoom: {
				enabled: !1
			},
			dropShadow: {
				enabled: 0,
				top: 3,
				left: 14,
				blur: 4,
				opacity: .12,
				color: "#3461ff"
			},
			sparkline: {
				enabled: !1
			}
		},
		markers: {
			size: 0,
			colors: ["#3461ff"],
			strokeColors: "#fff",
			strokeWidth: 2,
			hover: {
				size: 7
			}
		},
		grid: {
			row: {
				colors: ["transparent", "transparent"],
				opacity: .2
			},
			borderColor: "#f1f1f1"
		},
		plotOptions: {
			bar: {
				horizontal: !1,
				columnWidth: "25%",
				//endingShape: "rounded"
			}
		},
		dataLabels: {
			enabled: !1
		},
		stroke: {
			show: !0,
			width: [2.5],
			//colors: ["#3461ff"],
			curve: "smooth"
		},
		fill: {
			type: 'gradient',
			gradient: {
				shade: 'light',
				type: 'vertical',
				shadeIntensity: 0.5,
				gradientToColors: ['#3461ff'],
				inverseColors: false,
				opacityFrom: 0.5,
				opacityTo: 0.1,
				// stops: [0, 100]
			}
		},
		colors: ["#3461ff"],
		xaxis: {
			categories: arrLabel
		},
		responsive: [
			{
				breakpoint: 1000,
				options: {
					chart: {
						type: "area",
						// width: 130,
						stacked: true,
					}
				}
			}
		],
		legend: {
			show: false
		},
		tooltip: {
			theme: "dark"
		}
	};

	var chart = new ApexCharts(document.querySelector("#chart5"), options);
	chart.render();
 
	// chart6
	var lstLabels = [];
	var lstValues = [];

	var spans = document.querySelectorAll('.lstContributions');
	spans.forEach(function (span) {
		var faculty = span.nextElementSibling.innerText; // Lấy nội dung của phần tử span kế tiếp
		var cleanedFaculty = faculty.replace(' -', '').trim();
		var totalArticles = span.nextElementSibling.nextElementSibling.innerText; // Lấy nội dung của phần tử span sau phần tử kế tiếp
		var cleanedTotalArticles = totalArticles.replace(' articles', '').trim();

		lstLabels.push(cleanedFaculty);
		lstValues.push(cleanedTotalArticles);
	});

	var lstPercent = lstValues.map(function (value) {
		return (value / lstValues.reduce((a, b) => parseInt(a) + parseInt(b), 0) * 100).toFixed(2);
	});

	var backgroundColors = lstLabels.map(function () {
		return randomColor();
	});

	spans.forEach(function (span, index) {
		// Thiết lập màu sắc cho biểu tượng bằng cách thêm class có chứa màu sắc từ mảng backgroundColors
		span.classList.add('bi-square-fill');
		span.style.color = backgroundColors[index % backgroundColors.length]; // Sử dụng màu sắc từ mảng backgroundColors, vòng lại nếu cần
	});
	
	//đặt mặc định value và percent
	document.getElementById('falcuty').innerText = lstLabels[0];
	document.getElementById('percent').innerText = lstPercent[0] + '%';

	var chart = new Chart(document.getElementById('chart6'), {
		type: 'doughnut',
		data: {
			labels: lstLabels,
			datasets: [{
				backgroundColor: backgroundColors,
				data: lstValues
			}]
		},
		options: {
			maintainAspectRatio: false,
			cutoutPercentage: 85,
			responsive: true,
			legend: {
				display: false
			},
			onHover: function(event, elements) {
				if (elements.length > 0) {
					var index = elements[0]._index; // Lấy chỉ mục của phần tử được nhấp
					var label = this.data.labels[index]; // Lấy nhãn tương ứng với chỉ mục
					//check index in lstLabels after that display percent at this index
					document.getElementById('falcuty').innerText = lstLabels[index];
					document.getElementById('percent').innerText = lstPercent[index] + '%';
				}
			}
		}
	});

	// sum value of lstValues and display it
	// var sum = lstValues.reduce((a, b) => parseInt(a) + parseInt(b), 0);
	// document.getElementById('totalArticles').innerText = sum + ' articles';
	//End ADMIN DASHBOARD


	// chart 2
	var options = {
		series: [{
			name: "Total Views",
			data: [400, 555, 257, 640, 460, 671, 350]
		}],
		chart: {
			type: "bar",
			//width: 100%,
			height: 40,
			toolbar: {
				show: !1
			},
			zoom: {
				enabled: !1
			},
			dropShadow: {
				enabled: 0,
				top: 3,
				left: 14,
				blur: 4,
				opacity: .12,
				color: "#3461ff"
			},
			sparkline: {
				enabled: !0
			}
		},
		markers: {
			size: 0,
			colors: ["#3461ff"],
			strokeColors: "#fff",
			strokeWidth: 2,
			hover: {
				size: 7
			}
		},
		plotOptions: {
			bar: {
				horizontal: !1,
				columnWidth: "35%",
				endingShape: "rounded"
			}
		},
		dataLabels: {
			enabled: !1
		},
		stroke: {
			show: !0,
			width: 2.5,
			curve: "smooth"
		},
		colors: ["#3461ff"],
		xaxis: {
			categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
		},
		fill: {
			opacity: 1
		},
		tooltip: {
			theme: "dark",
			fixed: {
				enabled: !1
			},
			x: {
				show: !1
			},
			y: {
				title: {
					formatter: function (e) {
						return ""
					}
				}
			},
			marker: {
				show: !1
			}
		}
	};

	var chart = new ApexCharts(document.querySelector("#chart2"), options);
	chart.render();



	// chart 3
	var options = {
		series: [{
			name: "Revenue",
			data: [240, 160, 555, 257, 671, 414]
		}],
		chart: {
			type: "line",
			//width: 100%,
			height: 40,
			toolbar: {
				show: !1
			},
			zoom: {
				enabled: !1
			},
			dropShadow: {
				enabled: 0,
				top: 3,
				left: 14,
				blur: 4,
				opacity: .12,
				color: "#3461ff"
			},
			sparkline: {
				enabled: !0
			}
		},
		markers: {
			size: 0,
			colors: ["#3461ff"],
			strokeColors: "#fff",
			strokeWidth: 2,
			hover: {
				size: 7
			}
		},
		plotOptions: {
			bar: {
				horizontal: !1,
				columnWidth: "35%",
				endingShape: "rounded"
			}
		},
		dataLabels: {
			enabled: !1
		},
		stroke: {
			show: !0,
			width: 2.5,
			curve: "smooth"
		},
		colors: ["#3461ff"],
		xaxis: {
			categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
		},
		fill: {
			opacity: 1
		},
		tooltip: {
			theme: "dark",
			fixed: {
				enabled: !1
			},
			x: {
				show: !1
			},
			y: {
				title: {
					formatter: function (e) {
						return ""
					}
				}
			},
			marker: {
				show: !1
			}
		}
	};

	var chart = new ApexCharts(document.querySelector("#chart3"), options);
	chart.render();



	


	var arrCategoriesDate = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
	var arrDataChart1 = new Array(12).fill(0);
	for (var i = 0; i < totalContributionData.length; i++) {
		var month = totalContributionData[i].month;
		var total = totalContributionData[i].total;
		arrDataChart1[month - 1] = total;
	}

	//Check if the year is the current year, then only display until the current month
	var currentYear = new Date().getFullYear();
	var currentMonth = new Date().getMonth();
	if (totalContributionData[0].year == currentYear) {
		arrCategoriesDate = arrCategoriesDate.slice(0, currentMonth + 1);
		arrDataChart1 = arrDataChart1.slice(0, currentMonth + 1);
	}

	var options = {
		series: [{
			name: "Total Orders",
			data: arrDataChart1
		}],
		chart: {
			type: "line",
			//width: 100%,
			height: 40,
			toolbar: {
				show: !1
			},
			zoom: {
				enabled: !1
			},
			dropShadow: {
				enabled: 0,
				top: 3,
				left: 14,
				blur: 4,
				opacity: .12,
				color: "#3461ff"
			},
			sparkline: {
				enabled: !0
			}
		},
		markers: {
			size: 0,
			colors: ["#3461ff"],
			strokeColors: "#fff",
			strokeWidth: 2,
			hover: {
				size: 7
			}
		},
		plotOptions: {
			bar: {
				horizontal: !1,
				columnWidth: "35%",
				endingShape: "rounded"
			}
		},
		dataLabels: {
			enabled: !1
		},
		stroke: {
			show: !0,
			width: 2.5,
			curve: "smooth"
		},
		colors: ["#3461ff"],
		xaxis: {
			categories: arrCategoriesDate
		},
		fill: {
			opacity: 1
		},
		tooltip: {
			theme: "dark",
			fixed: {
				enabled: !1
			},
			x: {
				show: !1
			},
			y: {
				title: {
					formatter: function (e) {
						return ""
					}
				}
			},
			marker: {
				show: !1
			}
		}
	};

	var chart = new ApexCharts(document.querySelector("#chart1"), options);
	chart.render();

	// chart 4
	var options = {
		series: [{
			name: "Customers",
			data: [400, 555, 257, 640, 460, 671, 350]
		}],
		chart: {
			type: "bar",
			//width: 100%,
			height: 40,
			toolbar: {
				show: !1
			},
			zoom: {
				enabled: !1
			},
			dropShadow: {
				enabled: 0,
				top: 3,
				left: 14,
				blur: 4,
				opacity: .12,
				color: "#3461ff"
			},
			sparkline: {
				enabled: !0
			}
		},
		markers: {
			size: 0,
			colors: ["#3461ff"],
			strokeColors: "#fff",
			strokeWidth: 2,
			hover: {
				size: 7
			}
		},
		plotOptions: {
			bar: {
				horizontal: !1,
				columnWidth: "35%",
				endingShape: "rounded"
			}
		},
		dataLabels: {
			enabled: !1
		},
		stroke: {
			show: !0,
			width: 2.5,
			curve: "smooth"
		},
		colors: ["#3461ff"],
		xaxis: {
			categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
		},
		fill: {
			opacity: 1
		},
		tooltip: {
			theme: "dark",
			fixed: {
				enabled: !1
			},
			x: {
				show: !1
			},
			y: {
				title: {
					formatter: function (e) {
						return ""
					}
				}
			},
			marker: {
				show: !1
			}
		}
	};

	var chart = new ApexCharts(document.querySelector("#chart4"), options);
	chart.render();
	//COORDINATORS END

	

	// chart 7

	var options = {
		chart: {
			height: 300,
			type: 'radialBar',
			toolbar: {
				show: false
			}
		},
		plotOptions: {
			radialBar: {
				//startAngle: -135,
				//endAngle: 225,
				hollow: {
					margin: 0,
					size: '80%',
					background: 'transparent',
					image: undefined,
					imageOffsetX: 0,
					imageOffsetY: 0,
					position: 'front',
					dropShadow: {
						enabled: true,
						top: 3,
						left: 0,
						blur: 4,
						color: 'rgba(0, 169, 255, 0.85)',
						opacity: 0.65
					}
				},
				track: {
					background: '#e8edff',
					strokeWidth: '67%',
					margin: 0, // margin is in pixels
					dropShadow: {
						enabled: 0,
						top: -3,
						left: 0,
						blur: 4,
						color: 'rgba(0, 169, 255, 0.85)',
						opacity: 0.65
					}
				},
				dataLabels: {
					showOn: 'always',
					name: {
						offsetY: -20,
						show: true,
						color: '#212529',
						fontSize: '16px'
					},
					value: {
						formatter: function (val) {
							return val + "%";
						},
						color: '#212529',
						fontSize: '35px',
						show: true,
						offsetY: 10,
					}
				}
			}
		},
		fill: {
			type: 'gradient',
			gradient: {
				shade: 'light',
				type: 'horizontal',
				shadeIntensity: 0.5,
				gradientToColors: ['#3461ff'],
				inverseColors: false,
				opacityFrom: 1,
				opacityTo: 1,
				stops: [0, 100]
			}
		},
		colors: ["#3461ff"],
		series: [78],
		stroke: {
			lineCap: 'round',
			//dashArray: 4
		},
		labels: ['Total Traffic'],
		responsive: [
			{
				breakpoint: 1281,
				options: {
					chart: {
						height: 280,
					}
				}
			}
		],

	}

	var chart = new ApexCharts(
		document.querySelector("#chart7"),
		options
	);

	chart.render();



	// chart 8

	var options = {
		series: [{
			name: "Comments",
			data: [0, 160, 671, 414, 555, 257, 901, 613, 727, 414, 555, 0]
		}],
		chart: {
			type: "area",
			//width: 130,
			height: 55,
			toolbar: {
				show: !1
			},
			zoom: {
				enabled: !1
			},
			dropShadow: {
				enabled: 0,
				top: 3,
				left: 14,
				blur: 4,
				opacity: .12,
				color: "#e72e2e"
			},
			sparkline: {
				enabled: !0
			}
		},
		markers: {
			size: 0,
			colors: ["#3461ff"],
			strokeColors: "#fff",
			strokeWidth: 2,
			hover: {
				size: 7
			}
		},
		plotOptions: {
			bar: {
				horizontal: !1,
				columnWidth: "35%",
				endingShape: "rounded"
			}
		},
		dataLabels: {
			enabled: !1
		},
		stroke: {
			show: !0,
			width: 2.5,
			curve: "smooth"
		},
		fill: {
			type: 'gradient',
			gradient: {
				shade: 'light',
				type: 'vertical',
				shadeIntensity: 0.5,
				gradientToColors: ['#3461ff'],
				inverseColors: false,
				opacityFrom: 0.6,
				opacityTo: 0.1,
				//stops: [0, 100]
			}
		},
		colors: ["#3461ff"],
		xaxis: {
			categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
		},
		tooltip: {
			theme: "dark",
			fixed: {
				enabled: !1
			},
			x: {
				show: !1
			},
			y: {
				title: {
					formatter: function (e) {
						return ""
					}
				}
			},
			marker: {
				show: !1
			}
		}
	};

	var chart = new ApexCharts(document.querySelector("#chart8"), options);
	chart.render();



	// chart 9

	var options = {
		series: [{
			name: "Comments",
			data: [0, 160, 671, 414, 555, 257, 901, 613, 727, 414, 555, 0]
		}],
		chart: {
			type: "area",
			//width: 130,
			height: 55,
			toolbar: {
				show: !1
			},
			zoom: {
				enabled: !1
			},
			dropShadow: {
				enabled: 0,
				top: 3,
				left: 14,
				blur: 4,
				opacity: .12,
				color: "#e72e2e"
			},
			sparkline: {
				enabled: !0
			}
		},
		markers: {
			size: 0,
			colors: ["#3461ff"],
			strokeColors: "#fff",
			strokeWidth: 2,
			hover: {
				size: 7
			}
		},
		plotOptions: {
			bar: {
				horizontal: !1,
				columnWidth: "35%",
				endingShape: "rounded"
			}
		},
		dataLabels: {
			enabled: !1
		},
		stroke: {
			show: !0,
			width: 2.5,
			curve: "smooth"
		},
		fill: {
			type: 'gradient',
			gradient: {
				shade: 'light',
				type: 'vertical',
				shadeIntensity: 0.5,
				gradientToColors: ['#3461ff'],
				inverseColors: false,
				opacityFrom: 0.6,
				opacityTo: 0.1,
				//stops: [0, 100]
			}
		},
		colors: ["#3461ff"],
		xaxis: {
			categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
		},
		tooltip: {
			theme: "dark",
			fixed: {
				enabled: !1
			},
			x: {
				show: !1
			},
			y: {
				title: {
					formatter: function (e) {
						return ""
					}
				}
			},
			marker: {
				show: !1
			}
		}
	};

	var chart = new ApexCharts(document.querySelector("#chart9"), options);
	chart.render();




	// chart 10

	var options = {
		series: [{
			name: "Comments",
			data: [0, 160, 671, 414, 555, 257, 901, 613, 727, 414, 555, 0]
		}],
		chart: {
			type: "area",
			//width: 130,
			height: 55,
			toolbar: {
				show: !1
			},
			zoom: {
				enabled: !1
			},
			dropShadow: {
				enabled: 0,
				top: 3,
				left: 14,
				blur: 4,
				opacity: .12,
				color: "#e72e2e"
			},
			sparkline: {
				enabled: !0
			}
		},
		markers: {
			size: 0,
			colors: ["#3461ff"],
			strokeColors: "#fff",
			strokeWidth: 2,
			hover: {
				size: 7
			}
		},
		plotOptions: {
			bar: {
				horizontal: !1,
				columnWidth: "35%",
				endingShape: "rounded"
			}
		},
		dataLabels: {
			enabled: !1
		},
		stroke: {
			show: !0,
			width: 2.5,
			curve: "smooth"
		},
		fill: {
			type: 'gradient',
			gradient: {
				shade: 'light',
				type: 'vertical',
				shadeIntensity: 0.5,
				gradientToColors: ['#3461ff'],
				inverseColors: false,
				opacityFrom: 0.6,
				opacityTo: 0.1,
				//stops: [0, 100]
			}
		},
		colors: ["#3461ff"],
		xaxis: {
			categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
		},
		tooltip: {
			theme: "dark",
			fixed: {
				enabled: !1
			},
			x: {
				show: !1
			},
			y: {
				title: {
					formatter: function (e) {
						return ""
					}
				}
			},
			marker: {
				show: !1
			}
		}
	};

	var chart = new ApexCharts(document.querySelector("#chart10"), options);
	chart.render();
});



//Random Colors without duplicates
function randomColors() {
	var letters = '0123456789ABCDEF'.split('');
	var color = '#';
	for (var i = 0; i < 6; i++) {
		color += letters[Math.floor(Math.random() * 16)];
	}
	return color;
}
var usedColors = [];
function randomColor() {
	var color = randomColors();
	if (usedColors.indexOf(color) > -1) {
		return randomColor();
	}
	usedColors.push(color);
	return color;
}
//End Random Colors without duplicates

// Set color for bi bi-square-fill by backgroundColor in chart6
var icons = document.querySelectorAll('.bi-square-fill');
for (var i = 0; i < icons.length; i++) {
	icons[i].style.backgroundColor = randomColor();
}

//total totalContributors
document.addEventListener("DOMContentLoaded", function() {
    var totalContributions = 0;

    var contributionCells = document.querySelectorAll("#qtyContribution .totalContributionCell");
    contributionCells.forEach(function(cell) {
        if (!isNaN(parseInt(cell.textContent))) {
            totalContributions += parseInt(cell.textContent);
        }
    });

    document.getElementById("totalContributors").textContent = totalContributions + " contributors";
});