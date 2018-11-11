var onBeginAddFeed = function () {
    $("#AddButton").prop('disabled', true);
};

var onCompleteAddFeed = function () {
    $("#AddButton").prop('disabled', false);
};

var onSuccessAddFeed = function (context) {
    showNotification("Feed saved successfully.")
    $("#Url").val("").focus();
};

var onFailedAddFeed = function (context) {
    if (context.status === 409) {
        showNotification("Feed URL already exists.")
    } else {
        showNotification("Failed to save the feed. Please retry after sometime.")
    }
    $("#Url").val("").focus();
};

var showNotification = function (message) {
    var snackbar = document.getElementById("snackbar");
    snackbar.innerHTML = "";
    snackbar.className = "show";
    snackbar.innerHTML = message;
    setTimeout(function () { snackbar.className = snackbar.className.replace("show", ""); }, 3000);
}