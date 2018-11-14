var onBeginAddFeed = function () {
    $("#AddButton").prop('disabled', true);
};

var onCompleteAddFeed = function () {
    $("#AddButton").prop('disabled', false);
};

var onSuccessAddFeed = function (context) {
    showNotification("Feed saved successfully.")
    $("#Url").val("").focus();
    fetchFeeds();
};

var onFailedAddFeed = function (context) {
    if (context.status === 409) {
        showNotification("Feed URL already exists.")
    } else {
        showNotification("Failed to save the feed. Please retry after sometime.")
    }
    $("#Url").val("").focus();
};

var fetchFeeds = function () {
    if (isAuthenticated) {
        var url = "/Feed/GetFeedsAsync";
        $.getJSON(url, function (response) {
            console.log(response);
            if (response.length !== 0) {
                RenderFeedItems(response);
            }
        });
    }
}

var showNotification = function (message) {
    var snackbar = document.getElementById("snackbar");
    snackbar.innerHTML = "";
    snackbar.className = "show";
    snackbar.innerHTML = message;
    setTimeout(function () { snackbar.className = snackbar.className.replace("show", ""); }, 3000);
}

var RenderFeedItems = function (source) {
    var template = document.getElementById("FeedItem");
    var templateHtml = template.innerHTML;
    var listHtml = "";
    for (var key in source) {
        listHtml += templateHtml.replace(/{{title}}/g, source[key]["title"])
            .replace(/{{feedCount}}/g, source[key]["feedCount"]);
    }

    document.getElementById("FeedDisplay").innerHTML = listHtml;
}


$(function () {
    fetchFeeds();
});