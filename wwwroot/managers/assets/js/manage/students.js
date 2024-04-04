document.addEventListener('DOMContentLoaded', function () {
    GetContributionForGuest();
});


function GetContributionForGuest() {
	//Retrieve the data from the HTML data attribute
	var contributionDataElement = document.getElementById("contributionGuest");
	var contributionsData = JSON.parse(contributionDataElement.dataset.contributionguest);
	if (contributionsData) {
		var contributionWithoutCommentsDataElement = document.getElementById("contributionWithoutCommentsGuest");
		var contributionWithoutCommentsData = JSON.parse(contributionWithoutCommentsDataElement.dataset.contributionwithoutcommentsguest);

		var contributiomCommentDataElement = document.getElementById("contributionCommentGuest");
		var contributiomCommentData = JSON.parse(contributiomCommentDataElement.dataset.contributioncommentguest);

		var options = {
			series: [{
			data: [contributionWithoutCommentsData[0].quantity, contributiomCommentData[0].quantity],
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
			categories: ['Contributions without comment', 'Contributions have comment'],
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
			  text: 'These contributions have been approved in ' + contributionWithoutCommentsData[0].faculty + '.',
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
  
		  var chart = new ApexCharts(document.querySelector("#chartStudent"), options);
		  chart.render();
	}else{
		document.getElementById('chartStudent').innerHTML = GetGifEmpty(75);
	}
}

function SelectedContributionsYearByStudent(year) {
	var url = '/Students/IndexGuest?task=Contributions&year=' + year;
	window.location.href = url;
}

function SelectedCommentYearByStudent(year) {
	var url = '/Students/IndexGuest?task=CommentContributions&year=' + year;
	window.location.href = url;
}