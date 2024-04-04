//onload
document.addEventListener('DOMContentLoaded', function () {
	GetContribution();
	GetContributionsWithoutComment();
});

function GetContributionsWithoutComment() {
	var contributionsData = JSON.parse(document.getElementById("ContributionsWithoutCommentData").dataset.withoutcomment);
	if (contributionsData.length == 0) {
		contributionsData[0] = { quantity: 0 };
	}	
	
	var contributionsOver14DaysData = JSON.parse(document.getElementById("ContributionsOver14DaysData").dataset.over14days);
	if (contributionsOver14DaysData.length == 0) {
		contributionsOver14DaysData[0] = { quantity: 0 };
	}

	var options = {
		series: [{
		data: [contributionsData[0].quantity, contributionsOver14DaysData[0].quantity],
		name: 'Quantity',
	  }],
		chart: {
		type: 'bar',
		height: 380,
		toolbar: {
			show: false
		}
	  },
	  plotOptions: {
		bar: {
		  barHeight: '100%',
		  distributed: true,
		  horizontal: true,
		  dataLabels: {
			position: 'bottom'
		  },
		}
	  },
	  colors: ['#f48024', '#69d2e7'],
	  dataLabels: {
		enabled: true,
		textAnchor: 'start',
		style: {
		  colors: ['#fff']
		},
		formatter: function (val, opt) {
		  return opt.w.globals.labels[opt.dataPointIndex] + ":  " + val
		},
		offsetX: 0,
		dropShadow: {
		  enabled: true
		}
	  },
	  stroke: {
		width: 1,
		colors: ['#fff']
	  },
	  xaxis: {
		categories: ['Contributions without comment', 'Contributions over 14 Days'],
	  },
	  yaxis: {
		labels: {
		  show: false
		}
	  },
	  title: {
		  text: 'Visualize contributions',
		  align: 'center',
		  floating: true
	  },
	  subtitle: {
		  text: 'These contributions have been approved but have not been commented on yet.',
		  align: 'center',
	  },
	  tooltip: {
		theme: 'dark',
		x: {
		  show: false
		},
		y: {
		  title: {
			formatter: function () {
			  return ''
			}
		  }
		}
	  }
	  };
	  
	var totalContributionsData = JSON.parse(document.getElementById("TotalContributions").dataset.totalcontributions);
	if (totalContributionsData[0].quantity > 0) {
		document.getElementById("withoutCOmmentCount").innerHTML = totalContributionsData[0].quantity + " articles";
	}	  

	var chart = new ApexCharts(document.querySelector("#chartCoordinators1"), options);
	chart.render();
}

function GetContribution() {
	var contributionsData = JSON.parse(document.getElementById("ContributionsTotalData").dataset.total);
	var approvedContributionsData = JSON.parse(document.getElementById("ContributionsApprovedData").dataset.approved);
	var publishedContributionsData = JSON.parse(document.getElementById("ContributionsPublishedData").dataset.published);
	var rejectedContributionsData = JSON.parse(document.getElementById("ContributionsRejectedData").dataset.rejected);
	var pendingContributionsData = JSON.parse(document.getElementById("ContributionsPendingData").dataset.pending);
	
	var data = [[],[]];
	data[0].push(contributionsData);
	data[1].push("All Contributions");
	data[0].push(approvedContributionsData);
	data[1].push("Approved Contributions");
	data[0].push(publishedContributionsData);
	data[1].push("Published Contributions");
	data[0].push(rejectedContributionsData);
	data[1].push("Rejected Contributions");
	data[0].push(pendingContributionsData);
	data[1].push("Pending Contributions");
	
    var colors = ['#008FFB', '#00E396', '#FEB019', '#FF4560', '#775DD0'];
    var options = {
        series: [{
            data: data[0],
			name: 'Quantity',
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
            categories: data[1],
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

function GetGifEmpty(size) {
	var width = 'w-' + size;
	return `
    <div class="empty d-flex flex-column align-items-center position-relative">
        <img src=".././gif/empty.gif" class="img-fluid ` + width + `" alt="browser" />
        <p class="position-absolute start-50 translate-middle mt-2" style="top: 85%;">No data here!</p>
    </div>`;
}

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

			if (formatDate == currentDate.toISOString().slice(0, 10)) {
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
	var url = '/Managers/IndexCoordinators?task=TotalContribution&year=' + year;
	window.location.href = url;
}

function SelectedYearContributors(year) {
	var url = '/Managers/IndexCoordinators?task=ContributionUser&year=' + year;
	window.location.href = url;
}
