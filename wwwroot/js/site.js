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
                RenderFeedContent(response);
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
    var template = document.getElementById("TabHeading");
    var templateHtml = template.innerHTML;
    var listHtml = "";
    for (var i = 0; i < source.length; i++) {
        var key = i;
        if (i === 0) {
            listHtml += templateHtml.replace(/{{title}}/g, source[key]["title"])
                .replace(/{{id}}/g, i)
                .replace(/{{cssclass}}/g, " show active")
                .replace(/{{key}}/g, source[key]["id"])
                .replace(/{{feedCount}}/g, source[key]["feedCount"]);
        } else {
            listHtml += templateHtml.replace(/{{title}}/g, source[key]["title"])
                .replace(/{{id}}/g, i)
                .replace(/{{cssclass}}/g, "")
                .replace(/{{key}}/g, source[key]["id"])
                .replace(/{{feedCount}}/g, source[key]["feedCount"]);
        }
    }

    document.getElementById("FeedDisplay").innerHTML = listHtml;
}

var RenderFeedContent = function (source) {

    var tabContentTemplate = document.getElementById("TabContent");
    var tabContentTemplateHtml = tabContentTemplate.innerHTML;
    var tabContentListHtml = "";
    var counter = 0;
    for (var i = 0; i < source.length; i++) {
        var key = i;
        if (i === 0) {
            tabContentListHtml += tabContentTemplateHtml.replace(/{{title}}/g, source[key]["title"])
                .replace(/{{id}}/g, i)
                .replace(/{{key}}/g, source[key]["id"])
                .replace(/{{cssclass}}/g, " show active");
        } else {
            tabContentListHtml += tabContentTemplateHtml.replace(/{{title}}/g, source[key]["title"])
                .replace(/{{id}}/g, i)
                .replace(/{{key}}/g, source[key]["id"])
                .replace(/{{cssclass}}/g, "");
        }
    }

    document.getElementById("FeedContent").innerHTML = tabContentListHtml;
}


$(function () {
    fetchFeeds();
});