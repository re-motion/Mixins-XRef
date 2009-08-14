<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">

<xsl:template name="htmlSite">
	<xsl:param name="siteTitle" />
	<xsl:param name="siteFileName" />
	<xsl:param name="bodyContentTemplate" />
	
	<xsl:result-document format="standardHtmlOutputFormat" href="{$siteFileName}">
		<html xmlns="http://www.w3.org/1999/xhtml">
			<head>
				<title><xsl:value-of select="$siteTitle" /></title>
				<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
				<!-- include resources -->
				<xsl:call-template name="includeResources"/>
			</head>
			<body>
			
				<!-- list of all callable templates -->
				<xsl:choose>
					<!-- index -->
					<xsl:when test="$bodyContentTemplate = 'index'"><xsl:call-template name="index"/></xsl:when>
					<!-- assembly index + assembly sites -->
					<xsl:when test="$bodyContentTemplate = 'assembly'"><xsl:call-template name="assembly"/></xsl:when>
					<!-- assembly detail site -->
					<xsl:when test="$bodyContentTemplate = 'assemblyDetail'"><xsl:call-template name="assemblyDetail"/></xsl:when>
					<!-- involved type index + involved type sites -->
					<xsl:when test="$bodyContentTemplate = 'involvedType'"><xsl:call-template name="involvedType"/></xsl:when>
					<!-- involved type detail site -->
					<xsl:when test="$bodyContentTemplate = 'involvedTypeDetail'"><xsl:call-template name="involvedTypeDetail"/></xsl:when>
					<!-- interface index + interface sites -->
					<xsl:when test="$bodyContentTemplate = 'interface'"><xsl:call-template name="interface"/></xsl:when>
					<!-- interface detail site -->
					<xsl:when test="$bodyContentTemplate = 'interfaceDetail'"><xsl:call-template name="interfaceDetail"/></xsl:when>
					<!-- attribute index + attribute sites -->
					<xsl:when test="$bodyContentTemplate = 'attribute'"><xsl:call-template name="attribute"/></xsl:when>
					<!-- attribute detail site -->
					<xsl:when test="$bodyContentTemplate = 'attributeDetail'"><xsl:call-template name="attributeDetail"/></xsl:when>
					
					<!-- fail fast -->
					<xsl:otherwise>
						<xsl:message terminate="yes" >template rule '<xsl:value-of select="$bodyContentTemplate" />' could not be found</xsl:message>
					</xsl:otherwise>
				</xsl:choose>
				
			<p>
				<img src="http://www.w3.org/Icons/valid-xhtml10-blue" alt="Valid XHTML 1.0 Strict" height="31" width="88" />
			 </p>
			 
			</body>
		</html>
	</xsl:result-document>
</xsl:template>

<xsl:template name="includeResources">
	
	<link rel="stylesheet" type="text/css" href="resources/style.css" />
	<!--
	<script type="text/javascript" src="{$dir}/jquery.tablesorter.cookie.js"></script>
	-->
	
</xsl:template>

</xsl:stylesheet>
