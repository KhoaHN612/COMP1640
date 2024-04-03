//onload
document.addEventListener('DOMContentLoaded', function () {
    GetPageVisitData("desc");
    GetContributionByFaculty();
    GetBrowserByYear(new Date().getFullYear());
    GetTotalContribution();
});

function GetGifEmpty(size) {
	var width = 'w-' + size;
	return `
    <div class="empty d-flex flex-column align-items-center position-relative">
        <img src=".././gif/empty.gif" class="img-fluid ` + width + `" alt="browser" />
        <p class="position-absolute start-50 translate-middle mt-2" style="top: 85%;">No data here!</p>
    </div>`;
}

function GetPageVisitData(action) {
	if (action == "desc") {
		document.getElementById("sort-by").innerHTML = '<i class="bi bi-sort-up" onclick="GetPageVisitData(\'asc\')"></i>';
	} else {	
		document.getElementById("sort-by").innerHTML = '<i class="bi bi-sort-down" onclick="GetPageVisitData(\'desc\')"></i>';
	}

	//Method POST sent desc to server
	fetch('/WebAccessLog/PageVisitData', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		body: JSON.stringify(action)
	})
		.then(response => response.json())
		.then(data => {
			
			if (data.length > 0) {
				//Get data from server
				var arrData = [];

				// Lặp qua mỗi phần tử trong object data
				for (var key in data) {
					// Kiểm tra xem phần tử hiện tại có thuộc tính visitCount không
					if (data[key].visitCount !== undefined) {
						// Nếu có, thêm một đối tượng mới vào mảng cleanedData
						arrData.push({
							x: data[key].pageName,
							y: data[key].visitCount
						});
					}
				}

				//Create chart column
				var options = {
					series: [{
						name: "sales",
						data: arrData
					}],
					chart: {
						type: 'bar',
						height: 380,
						toolbar: {
							show: false
						}
					},
					xaxis: {
						type: 'category',
						group: {
							style: {
								fontSize: '10px',
								fontWeight: 700
							}
						}
					},
					tooltip: {
						x: {
							formatter: function (val) {
								return "Q" + dayjs(val).quarter() + " " + dayjs(val).format("YYYY")
							}
						}
					},
				};

				var chart = new ApexCharts(document.querySelector("#chartPageVisit"), options);
				chart.render();
			} else {
				//If no data, display gif empty
				document.getElementById("chartPageVisit").innerHTML = GetGifEmpty(50);
			}
		})
		.catch(error => {
			console.error('Error fetching data:', error);
		}
		);
}

function GetContributionByFaculty() {
	// Retrieve the data from the HTML data attribute
	var contributionDataElement = document.getElementById("contributionFaculty");
	if (contributionDataElement) {
		var contributionsData = JSON.parse(contributionDataElement.dataset.contributionfaculty);
		if (contributionsData[0].faculty != null) {
			var arrDataChart6 = [[], []];

			for (var i = 0; i < contributionsData.length; i++) {
				arrDataChart6[0].push(contributionsData[i].faculty);
				arrDataChart6[1].push(contributionsData[i].total);
			}

			//Background color
			var backgroundColors = [];
			for (var i = 0; i < arrDataChart6[0].length; i++) {
				backgroundColors.push(getRandomColor());
			}

			//Get percent
			var sum = arrDataChart6[1].reduce((a, b) => a + b, 0);
			var lstPercent = [];
			for (var i = 0; i < arrDataChart6[1].length; i++) {
				lstPercent.push(((arrDataChart6[1][i] / sum) * 100).toFixed(2));
			}

			//Lấy phần tử đầu tiên hiển thị phần trăm và tên faculty
			document.getElementById('falcuty').innerText = arrDataChart6[0][0];
			document.getElementById('percent').innerText = lstPercent[0] + '%';
			
			var chart = new Chart(document.getElementById('chart6'), {
				type: 'doughnut',
				data: {
					labels: arrDataChart6[0],
					datasets: [{
						label: "Device Users",
						backgroundColor: backgroundColors,
						data: arrDataChart6[1]
					}]
				},
				options: {
					maintainAspectRatio: false,
					cutoutPercentage: 85,
					responsive: true,
					legend: {
						display: true,
						position: 'right'
					},
					onHover: function (event, chartElement) {
						if (chartElement.length > 0) {
							var index = chartElement[0]._index; // Lấy chỉ mục của phần tử được nhấp
							//check index in lstLabels after that display percent at this index
							document.getElementById('falcuty').innerText = arrDataChart6[0][index];
							document.getElementById('percent').innerText = lstPercent[index] + '%';
						}
					}
				},
				with: 500,
				height: 500
			});

			// Display chart 
			chart.render();	
		}else{
			document.getElementById('PieChartPercent').innerHTML = GetGifEmpty(75);
		}
	}
}

function GetBrowserByYear(year) {
	fetch('/WebAccessLog/AddVisitLog', {
		method: 'POST',
		headers: {
			'Content-Type': 'application/json',
		},
		body: JSON.stringify(year),
	})
		.then(response => response.json())
		.then(data => {
			if (Object.entries(data).length > 1) {
				document.getElementById("yearBrowser").value = Object.values(data)[0];

				var options = {
					series: Object.values(data).slice(1),
					chart: {
						width: 460,
						height: 500,
						type: 'pie'
					},
					labels: Object.keys(data).slice(1),
					plotOptions: {
						pie: {
							dataLabels: {
								formatter: function (val, opts) {
									return val + " views";
								}
							}
						}
					},
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
				document.getElementById("chartBrowser").innerHTML = "";
				var chart = new ApexCharts(document.querySelector("#chartBrowser"), options);
				chart.render();
			} else {
				document.getElementById("yearBrowser").value = Object.values(data)[0];

				var resizeTriggers = document.getElementsByClassName("resize-triggers");
				while (resizeTriggers.length > 0) {
					resizeTriggers[0].parentNode.removeChild(resizeTriggers[0]);
				}

				document.getElementById("chartBrowser").innerHTML = GetGifEmpty(75);
			}
		})
		.catch(error => {
			console.error('Error fetching data:', error);
		});
}

function GetTotalContribution() {
    var contributionItems = document.querySelectorAll('.contribution-item');
	if (contributionItems[0].getAttribute('data-month') != 0) {
		var arrLabel = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

		var arrData = new Array(12).fill(0);

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
							toolbar: {
								show: false
							}
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
	}else{
		document.getElementById('chart5').innerHTML = GetGifEmpty(50);
	}

	document.getElementById("totalContributors").textContent = document.querySelectorAll('.countContributors').length + " contributors";
}

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
