<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" exclude-result-prefixes="xs fn">

<xsl:template name="htmlSite">
	<xsl:param name="siteTitle" />
	<xsl:param name="siteFileName" />
	<xsl:param name="bodyContentTemplate" />
	
	<xsl:result-document format="standardHtmlOutputFormat" href="{$siteFileName}">
	<html xmlns="http://www.w3.org/1999/xhtml">
		<head>
			<title><xsl:value-of select="$siteTitle" /></title>
			<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
		</head>
		<body>
		
			<!-- list of all callable templates -->
			<xsl:choose>
				<!-- index -->
				<xsl:when test="$bodyContentTemplate eq 'index'"><xsl:call-template name="index"/></xsl:when>
				<!-- assembly index + assembly sites -->
				<xsl:when test="$bodyContentTemplate eq 'assembly'"><xsl:call-template name="assembly"/></xsl:when>
			</xsl:choose>
			
			
		<p>
			<img src="http://www.w3.org/Icons/valid-xhtml10" alt="Valid XHTML 1.0 Strict" height="31" width="88" />
		 </p>
		 
		</body>
	</html>
	</xsl:result-document>
	
</xsl:template>
</xsl:stylesheet>
