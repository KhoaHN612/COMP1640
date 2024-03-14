//sk onload
$(window).on('load', function () {
	//COORDINATORS START
	// chart 1
	
	// Retrieve the data from the HTML data attribute
	var totalContributionDataElement = document.getElementById("totalContributionData");
	var totalContributionData = JSON.parse(totalContributionDataElement.dataset.totalcontribution);

	console.log(totalContributionData);

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
});

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

function SelectedYear(year) {
    var url = '/Managers?task=ContributionYear&year=' + year;
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

	// Admin Chart
	
	// chart 5
	//Get Viewbag["ContributionYear"] 
	//Nếu năm được chọn là năm hiện tại thì chỉ hiển thị đến tháng hiện tại
	//Nếu năm được chọn là năm khác thì hiển thị đến tháng 12
	var arrLabel = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

	var arrData = new Array(12).fill(0);
	var contributionItems = document.querySelectorAll('.contribution-item');

	for (var i = 0; i < contributionItems.length; i++) {
		var contributionItem = contributionItems[i];
		var month = parseInt(contributionItem.getAttribute('data-month'));
		var total = contributionItem.getAttribute('data-total');

		//Kiểm tra nếu 
		arrData[month - 1] = total;
	}

	//Kiểm tra nếu năm được chọn là năm hiện tại thì chỉ hiển thị đến tháng hiện tại
	//Nếu năm được chọn là năm khác thì hiển thị đến tháng 12
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
	// khai báo mảng chứa nhãn và giá trị
	var lstLabels = [];
	var lstValues = [];

	// Lấy tất cả các phần tử span có class là 'lstContributions'
	var spans = document.querySelectorAll('.lstContributions');

	// Duyệt qua từng phần tử span và lấy nội dung của chúng
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

	// Tạo mảng màu ngẫu nhiên
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



	// // chart 11

	// var options = {
	// 	series: [{
	// 		name: "New Visitors",
	// 		data: [640, 560, 871, 614, 755, 457, 650]
	// 	}, {
	// 		name: "Old Visitors",
	// 		data: [440, 360, 671, 414, 555, 257, 450]
	// 	}],
	// 	chart: {
	// 		foreColor: '#9a9797',
	// 		type: "bar",
	// 		//width: 130,
	// 		stacked: true,
	// 		height: 280,
	// 		toolbar: {
	// 			show: !1
	// 		},
	// 		zoom: {
	// 			enabled: !1
	// 		},
	// 		dropShadow: {
	// 			enabled: 0,
	// 			top: 3,
	// 			left: 15,
	// 			blur: 4,
	// 			opacity: .12,
	// 			color: "#3461ff"
	// 		},
	// 		sparkline: {
	// 			enabled: !1
	// 		}
	// 	},
	// 	markers: {
	// 		size: 0,
	// 		colors: ["#3461ff", "#c1cfff"],
	// 		strokeColors: "#fff",
	// 		strokeWidth: 2,
	// 		hover: {
	// 			size: 7
	// 		}
	// 	},
	// 	plotOptions: {
	// 		bar: {
	// 			horizontal: !1,
	// 			columnWidth: "35%",
	// 			//endingShape: "rounded"
	// 		}
	// 	},
	// 	dataLabels: {
	// 		enabled: !1
	// 	},
	// 	legend: {
	// 		show: false,
	// 	},
	// 	stroke: {
	// 		show: !0,
	// 		width: 0,
	// 		curve: "smooth"
	// 	},
	// 	colors: ["#3461ff", "#c1cfff"],
	// 	xaxis: {
	// 		categories: ["Mo", "Tu", "We", "Th", "Fr", "Sa", "Su"]
	// 	},
	// 	tooltip: {
	// 		theme: "dark",
	// 		fixed: {
	// 			enabled: !1
	// 		},
	// 		x: {
	// 			show: !1
	// 		},
	// 		y: {
	// 			title: {
	// 				formatter: function (e) {
	// 					return ""
	// 				}
	// 			}
	// 		},
	// 		marker: {
	// 			show: !1
	// 		}
	// 	}
	// };

	// var chart = new ApexCharts(document.querySelector("#chart11"), options);
	// chart.render();




	// worl map

	jQuery('#geographic-map').vectorMap(
		{
			map: 'world_mill_en',
			backgroundColor: 'transparent',
			borderColor: '#818181',
			borderOpacity: 0.25,
			borderWidth: 1,
			zoomOnScroll: false,
			color: '#009efb',
			regionStyle: {
				initial: {
					fill: '#3461ff'
				}
			},
			markerStyle: {
				initial: {
					r: 9,
					'fill': '#fff',
					'fill-opacity': 1,
					'stroke': '#000',
					'stroke-width': 5,
					'stroke-opacity': 0.4
				},
			},
			enableZoom: true,
			hoverColor: '#009efb',
			markers: [{
				latLng: [21.00, 78.00],
				name: 'Lorem Ipsum Dollar'

			}],
			hoverOpacity: null,
			normalizeFunction: 'linear',
			scaleColors: ['#b6d6ff', '#005ace'],
			selectedColor: '#c9dfaf',
			selectedRegions: [],
			showTooltip: true,
		});
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


//Chart coordinators

//Retrieve the data from the HTML data attribute
// var totalContributionDataElement = document.getElementById("contributionWithoutCommentsData");
// var totalContributionData = JSON.parse(totalContributionDataElement.dataset.contributionWithoutComments);

// console.log(totalContributionData);

// var options = {
// 	series: [{
// 	name: 'Contributions without comments',
// 	data: arrWithoutCommentsData
//   }, {
// 	name: 'Contributions without comments (14 days+)',
// 	data: arrWithoutComments14DaysData
//   }],
// 	chart: {
// 	height: 350,
// 	type: 'area'
//   },
//   dataLabels: {
// 	enabled: false
//   },
//   stroke: {
// 	curve: 'smooth'
//   },
//   xaxis: {
// 	type: 'datetime',
// 	categories: arrCategoriesDays
//   },
//   tooltip: {
// 	x: {
// 	  format: 'dd/MM/yy HH:mm'
// 	},
//   },
//   };
// var chart = new ApexCharts(document.querySelector("#chartCoordinators1"), options);
// chart.render();




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