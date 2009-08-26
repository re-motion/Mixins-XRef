$(document).ready(function() {

    initTableSorter();
    setSelectedIndexClass();
    initTreeView();

    prepareCollapsing();
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

        if ($(this).hasClass("noSorting"))
            return;

        var widgetSetting = ['cookie'];
        /* css3 selector 'nth-child()' with IE? - no way */
        if (jQuery.browser.msie)
            widgetSetting = ['cookie', 'zebra'];

        /* init tablesorter */
        $(this).tablesorter({
            widthFixed: true,
            widgets: widgetSetting
        });

        var rowCount = $(this).find("tbody tr").length;
        /* pager: only on index sites with a table with more than 30 rows */
        if (rowCount > 30 && onIndexSite()) {

            $("div.pager select.pagesize").bind("change", function() {
                tablesorterCookieJar.set(getCookieName() + '_pagesize', $(this).attr("value"));
            });

            var pageSize = tablesorterCookieJar.get(getCookieName() + '_pagesize');

            if (pageSize == undefined) pageSize = 30;
            $("div.pager select.pagesize").attr("value", pageSize);

            $(this).tablesorterPager({
                container: $(".pager"),
                size: pageSize,
                positionFixed: false
            });


        } else {
            $(".pager").hide();
        }
    });
}

function setSelectedIndexClass() {
    $("#navigation a").filter(function() {
        /* does the link in the navigation bar point to the current document? */
        return this.href == location.href;
    }).addClass("currentIndex");
}

function onIndexSite() {
    return location.href.indexOf("index.html") != -1;
}

function prepareCollapsing() {
    /* make only non index site collapse-able */
    if (onIndexSite())
        return;

    var cookieName = getCookieName();
    var cookie = $.cookie(cookieName);

    if (cookie == undefined) {
        saveCookie();
    }

    var cookieValue = $.cookie(cookieName);
    var classArray = cookieValue.split(",");

    $("caption, .treeHeader").each(function(n) {

        if (this.tagName.toUpperCase() == "CAPTION") {
            $(this).addClass(classArray[n]);

            if (classArray[n] == "hidden") {
                $(this).nextAll("thead, tfoot, tbody").hide();
            }

            $(this).click(function() {
                $(this).toggleClass("visible").toggleClass("hidden");

                if ($(this).hasClass("hidden"))
                    $(this).nextAll("thead, tfoot, tbody").hide();
                else
                    $(this).nextAll("thead, tfoot, tbody").show();
                saveCookie();
            });
        } else { // tagname = DIV           

            // internet explorer doesn't like collapsing of trees
            if (!jQuery.browser.msie) {

                $(this).addClass(classArray[n]);

                if (classArray[n] == "hidden") {
                    $(".treeview").hide();
                }

                $(".treeHeader").click(function() {
                    $(".treeHeader").toggleClass("visible").toggleClass("hidden");
                    $(".treeview").toggle();
                    saveCookie();
                });
            }
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
