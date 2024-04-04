
//sk onload
$(window).on('load', function () {
	//======================================================= MANAGER DASHBOARD =======================================================
	
	//======================================================= MANAGER END =======================================================


	//=======================================================COORDINATOR DASHBOARD=======================================================
	
	//=======================================================COORDINATORS END=======================================================


	//=======================================================ADMIN DASHBOARD=======================================================
	
	//=======================================================ADMIN END=======================================================

	//=======================================================GUEST DASHBOARD=======================================================
	GetContributionForGuest();
	//=======================================================GUEST END=======================================================
});



function GetGifEmpty(size) {
	var width = 'w-' + size;
	return `
    <div class="empty d-flex flex-column align-items-center position-relative">
        <img src=".././gif/empty.gif" class="img-fluid ` + width + `" alt="browser" />
        <p class="position-absolute start-50 translate-middle mt-2" style="top: 85%;">No data here!</p>
    </div>`;
}



function GetContributionForGuest() {
	//Retrieve the data from the HTML data attribute
	var contributionDataElement = document.getElementById("contributionGuest");
	var contributionsData = JSON.parse(contributionDataElement.dataset.contributionguest);
	if (contributionsData) {
		var contributionWithoutCommentsDataElement = document.getElementById("contributionWithoutCommentsGuest");
		var contributionWithoutCommentsData = JSON.parse(contributionWithoutCommentsDataElement.dataset.contributionwithoutcommentsguest);
		console.log(contributionWithoutCommentsData);

		var contributiomCommentDataElement = document.getElementById("contributionCommentGuest");
		var contributiomCommentData = JSON.parse(contributiomCommentDataElement.dataset.contributioncommentguest);
		console.log(contributiomCommentData);

		var arrDataChartGuest = [[], [], [], []];
		//get month name from January to December
		var month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
		// get to current month
		var currentMonth = new Date().getMonth();
		month = month.slice(0, currentMonth + 1);

		var totalArticles = RecursiveCleanData(contributionsData, contributionsData.length - 1);
		var totalWithoutComments = RecursiveCleanData(contributionWithoutCommentsData, contributionWithoutCommentsData.length - 1);
		var totalComment = RecursiveCleanData(contributiomCommentData, contributiomCommentData.length - 1);

		var arrDataChartGuest = FillData(month, totalArticles, totalWithoutComments, totalComment);

		var options = {
			series: [{
				name: 'Without Comments',
				type: 'column',
				data: arrDataChartGuest[1]
			}, {
				name: 'Comments',
				type: 'column',
				data: arrDataChartGuest[2]
			}, {
				name: 'Total Articles',
				type: 'line',
				data: arrDataChartGuest[0]
			}],
			chart: {
				height: 350,
				type: 'line',
				stacked: false
			},
			dataLabels: {
				enabled: false
			},
			stroke: {
				width: [1, 1, 4]
			},
			xaxis: {
				categories: arrDataChartGuest[3],
			},
			yaxis: [
				{
					axisTicks: {
						show: true,
					},
					axisBorder: {
						show: true,
						color: '#008FFB'
					},
					labels: {
						style: {
							colors: '#008FFB',
						}
					},
					title: {
						text: "Approved articles have no comments",
						style: {
							color: '#008FFB',
						}
					},
					tooltip: {
						enabled: true
					}
				},
				{
					seriesName: 'Without Comments',
					opposite: true,
					axisTicks: {
						show: true,
					},
					axisBorder: {
						show: true,
						color: '#00E396',
						formatter: function (value) {
							return value + " articles";
						}
					},
					labels: {
						style: {
							colors: '#00E396',
						}
					},
					title: {
						text: "Approved article with comments",
						style: {
							color: '#00E396',
						}
					},
				},
				{
					seriesName: 'Comments',
					opposite: true,
					axisTicks: {
						show: true,
					},
					axisBorder: {
						show: true,
						color: '#FEB019'
					},
					labels: {
						style: {
							colors: '#FEB019',
						},
					},
					title: {
						text: "Total articles in this year",
						style: {
							color: '#FEB019',
						}
					}
				},
			],
			tooltip: {
				fixed: {
					enabled: true,
					position: 'topLeft', // topRight, topLeft, bottomRight, bottomLeft
					offsetY: 30,
					offsetX: 60
				},
			},
			legend: {
				horizontalAlign: 'center',
				offsetX: 40
			}
		};

		var chart = new ApexCharts(document.querySelector("#chartStudent"), options);
		chart.render();
	}else{
		document.getElementById('chartStudent').innerHTML = GetGifEmpty(75);
	}
}

function RecursiveCleanData(contributionsData, i) {
	if (i < 0) {
		return contributionsData;
	}

	var month = new Date(contributionsData[i].date).getMonth() + 1;
	if (i > 0) {
		var previousMonth = new Date(contributionsData[i - 1].date).getMonth() + 1;
		if (month == previousMonth) {
			contributionsData[i - 1].quantity += contributionsData[i].quantity;
			contributionsData.splice(i, 1);
		}
	}
	return RecursiveCleanData(contributionsData, i - 1);
}

function FillData(month, totalArticles, totalWithoutComments, totalComment) {
	var arrDataChartGuest = [[], [], [], []];

	for (var i = 0; i < month.length; i++) {
		arrDataChartGuest[3].push(month[i]);
		arrDataChartGuest[0].push(0);
		arrDataChartGuest[1].push(0);
		arrDataChartGuest[2].push(0);

		for (let j = 0; j < totalArticles.length; j++) {
			console.log(j);

			if ((i + 1) == new Date(totalArticles[j].date).getMonth() + 1) {
				arrDataChartGuest[0][i] = totalArticles[j].quantity;
			}

			if (totalWithoutComments.length > j) {
				if ((i + 1) == new Date(totalWithoutComments[j].date).getMonth() + 1) {
					arrDataChartGuest[1][i] = totalWithoutComments[j].quantity;
				}
			}

			if (totalComment.length > j) {
				if ((i + 1) == new Date(totalComment[j].date).getMonth() + 1) {
					console.log(totalComment[j]);
					arrDataChartGuest[2][i] = totalComment[j].quantity;
				}
			}
		}
	}

	console.log(arrDataChartGuest);

	return arrDataChartGuest;
}






function SelectedYearInCoordinators(year) {
	var url = '/Managers/IndexCoordinators?task=TotalContribution&year=' + year;

	//redirect to url
	window.location.href = url;
}



$(function () {
	"use strict";

	//=======================================================ADMIN DASHBOARD=======================================================
	// chart 5
	
	

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

function getRandomColor() {
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
document.addEventListener("DOMContentLoaded", function () {
	var totalContributions = 0;

	var contributionCells = document.querySelectorAll("#qtyContribution .totalContributionCell");
	contributionCells.forEach(function (cell) {
		if (!isNaN(parseInt(cell.textContent))) {
			totalContributions += parseInt(cell.textContent);
		}
	});

	document.getElementById("totalContributors").textContent = totalContributions + " contributors";
});

