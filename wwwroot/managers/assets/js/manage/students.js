document.addEventListener('DOMContentLoaded', function () {
    GetContributionForGuest();
    GetContribution();
});


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

		
	}else{
		document.getElementById('chartStudent').innerHTML = GetGifEmpty(75);
	}
}


