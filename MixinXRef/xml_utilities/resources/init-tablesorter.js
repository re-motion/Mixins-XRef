$(document).ready( function() {
	/* get all tables */
	var ts = $("table");
	
	ts.each( function(n) {
		this.id = location.href + "_table_" + n;
		alert(this.id);
	});
				
	/* tablesorter magic */
	ts.tablesorter({ widgets: ['cookie', 'zebra'] });
});