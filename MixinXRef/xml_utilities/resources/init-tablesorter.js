$(document).ready( function() {
	initTableSorter();
	setSelectedIndexClass();
});

function initTableSorter() {
	/* get all tables */
	var ts = $("table");
	
	/* set unique id for each table */
	ts.each( function(n) {
		this.id = location.href + "_table_" + n;
	});
				
	/* tablesorter magic */
	ts.tablesorter({ widgets: ['cookie', 'zebra'], sortList: [[0,0],[1,0]] });
}

function setSelectedIndexClass(currentFileName) {
	$("#navigation a").filter(function () {
		/* does the link in the navigation bar point to the current document? */
		return this.href == location.href;
	}).addClass("currentIndex");
}
