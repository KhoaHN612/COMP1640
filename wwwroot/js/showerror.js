$(document).ready(function () {
    @if (TempData["ErrorMessage"] != null) {
        var errorMessage = "@Html.Raw(TempData["ErrorMessage"])";
        $('#errorMessage').text(errorMessage);
        $('#errorModal').modal('show');
    }
});
<script src="https://code.jquery.com/jquery-3.7.1.js" integrity="sha256-eKhayi8LEQwp4NKxN+CfCh+3qOVUtJn3QNZ0TciWLP4="
    crossorigin="anonymous"></script>
<script>