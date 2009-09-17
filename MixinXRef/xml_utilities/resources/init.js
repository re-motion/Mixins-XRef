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

    // add parser through the tablesorter addParser method 
    $.tablesorter.addParser({
        // set a unique id 
        id: 'modifiers',
        is: function(s) {
            // return false so this parser is not auto detected 
            return false;
        },
        format: function(s) {

            var index = 0;
            // format your data for normalization
            if (s.indexOf("public") != -1)
                index = 0;
            if (s.indexOf("protected internal") != -1)
                index = 10;
            if (s.indexOf("protected") != -1)
                index = 20;
            if (s.indexOf("internal") != -1)
                index = 30;
            if (s.indexOf("private") != -1)
                index = 40;

            if (s.indexOf("abstract") != -1)
                index += 1;
            if (s.indexOf("virtual") != -1)
                index += 2;
            if (s.indexOf("virtual") != -1)
                index += 3;

            return index;
        },
        // set type, either numeric or text 
        type: 'numeric'
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

        var caption = $(this).find("caption").html().substring(0, 7);
        if (caption == "Members") {
            /* init tablesorter */
            $(this).tablesorter({
                widthFixed: true,
                widgets: widgetSetting,
                textExtraction: function(node) {
                    if ($(node).find("span.Name").text() != "")
                        return ($(node).find("span.Name").text());
                    if ($(node).find("span.Keyword").text() != "")
                        return ($(node).find("span.Keyword").text()); 
                    return $(node).text();
                },
                headers:
                {
                    2: { sorter: 'modifiers' }
                }
            });
        }
        else {
            /* init tablesorter */
            $(this).tablesorter({
                widthFixed: true,
                widgets: widgetSetting
            });
        }
        var rowCount = $(this).find("tbody tr").length;
        /* pager: only on index sites with a table with more than 30 rows */
        if (rowCount >= 20 && onIndexSite()) {
            /* register event handler on dropdown-box */
            $("div.pager select.pagesize").bind("change", function() {
                tablesorterCookieJar.set(getCookieName() + '_pagesize', $(this).attr("value"));
            });
            /* page size */
            var pageSize = tablesorterCookieJar.get(getCookieName() + '_pagesize');
            if (pageSize == undefined) pageSize = 20;
            $("div.pager select.pagesize").attr("value", pageSize);
            /* allow the user to enter the page side by hand */
            $("form").bind("submit", function() {
                var newPageNr = parseInt($("div.pager input.pagedisplay").attr("value")) - 1;
                var tableId = ($("table").attr("id"));
                tablesorterCookieJar.set(tableId + "_currentPage", newPageNr);
            });

            /* current page */
            var currentPage = tablesorterCookieJar.get(this.id + "_currentPage");
            if (currentPage == undefined) currentPage = 0;

            $(this).tablesorterPager({
                container: $(".pager"),
                positionFixed: false,
                size: pageSize,
                page: currentPage
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
    return location.href.indexOf("_index.html") != -1;
}

function prepareCollapsing() {
    /* make only non index site collapse-able */
    if (onIndexSite())
        return;

    var cookieName = getCookieName();
    var cookie = $.cookie(cookieName);

    if (cookie == undefined) {
        $("caption:contains('Mixins')").addClass("visible");
        $("caption:contains('Attributes')").addClass("visible");
        $(".treeHeader").addClass("visible");
        if ($("h2 a").text() == "[Involved Interface]")
            $("caption:contains('Members')").addClass("visible");
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
                    $(this).parent().next(".treeview").hide();
                }

                $(this).click(function() {
                    $(this).toggleClass("visible").toggleClass("hidden");
                    $(this).parent().next(".treeview").toggle();
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