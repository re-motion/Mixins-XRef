$(document).ready(function() {

    /* changing default settings for tablesorterPager */
    $.tablesorterPager.defaults.size = 30;
    // fixed position created issues in different browsers
    $.tablesorterPager.defaults.positionFixed = false;

    initTableSorter();
    setSelectedIndexClass();
    initTreeView();
    
    // internet explorer doesn't like collapsing
    if (!jQuery.browser.msie) prepareCollapsing();
});

function getCookieName() {
    var file_name = document.location.href;
    var firstQuestionMark = (file_name.indexOf("?") == -1) ? file_name.length : file_name.indexOf("?");
    var mixinDoc = file_name.lastIndexOf("MixinDoc/");
    return file_name.substring(mixinDoc + 9, firstQuestionMark).replace("/", "_");
}

function initTableSorter() {
    //assign the sortStart event 
    $("table").bind("sortStart", function() {
        $("#overlay").show();
    }).bind("sortEnd", function() {
        $("#overlay").hide();
    });

    /* get all tables */
    var ts = $("table");
    var tablesorterCookieJar = $.cookieJar('tablesorter', {
        cookie: { path: '/' }
    });

    /* set unique id for each table */
    ts.each(function(n) {
        this.id = getCookieName() + "_table_" + n;

        var sortList = tablesorterCookieJar.get(this.id);

        if (sortList == undefined) {
            var sortList = [[0, 0], [1, 0]];
            tablesorterCookieJar.set(this.id, sortList);
        }
    });

    ts.each(function() {
        var rowCount = $(this).find("tbody tr").length;

        /* use tablesorter widghet for tables with more than 100 rows */
        if (rowCount > 100) {
            $(this).tablesorter({
			    widthFixed: true,
                widgets: ['cookie']
            })
			.tablesorterPager({container: $("#pager")});
        } else {
            $(this).tablesorter({
                widgets: ['cookie']
            });
        }
    });
}

function setSelectedIndexClass() {
    $("#navigation a").filter(function() {
        /* does the link in the navigation bar point to the current document? */
        return this.href == location.href;
    }).addClass("currentIndex");
}

function prepareCollapsing() {
    /* make only non index site collapse-able */
    if (location.href.indexOf("index.html") != -1)
        return;

    var cookieName = getCookieName();
    var cookie = $.cookie(cookieName);

    if (cookie == undefined) {
        saveCookie();
    }

    var cookieValue = $.cookie(cookieName);
    var classArray = cookieValue.split(",");

    $("caption, .treeHeader").each(function(n) {
        $(this).addClass(classArray[n]);
        if (this.tagName.toUpperCase() == "CAPTION") {
            if (classArray[n] == "hidden") {
                $(this).nextAll("thead, tfoot, tbody").hide();
            }
            $(this).click(function() {
                $(this).toggleClass("visible").toggleClass("hidden");
                $(this).nextAll("thead, tfoot, tbody").toggle();
                saveCookie();
            });
        } else { // tagname = DIV
            if (classArray[n] == "hidden") {
                $(".treeview").hide();
            }
            $(".treeHeader").click(function() {
                $(".treeview").toggle();
                $(".treeHeader").toggleClass("visible").toggleClass("hidden");
                saveCookie();
            });
        }
    });
}

function saveCookie() {
    var collapseElements = $("caption, .treeHeader");
    var cookieValue = "";

    collapseElements.each(function(n) {
        if (n != 0)
            cookieValue += ",";
        cookieValue += ($(this).hasClass("visible") ? "visible" : "hidden");
    });

    $.cookie(getCookieName(), cookieValue);
}

function initTreeView() {
    $("ul:nth-of-type(2)").treeview({
        collapsed: true,
        persist: "cookie",
        cookieId: getCookieName() + "_treeview"
    });
}
