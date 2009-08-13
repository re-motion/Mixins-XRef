<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">



<xsl:template name="interface">
<p>testsss</p>
	<xsl:call-template name="interfaceList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="interfaces" select="/MixinXRefReport/Interfaces/Interface" />  
	</xsl:call-template>

</xsl:template>

</xsl:stylesheet>
