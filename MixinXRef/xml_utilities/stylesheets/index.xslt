<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
	
<xsl:template name="index">
	<h1>Mixin Documentation Summary @<xsl:value-of select="/MixinXRefReport/@creation-time" /></h1>
	
	<p><xsl:value-of select="ru:GetOverallAssemblyCount(/)" /> assemblies with <xsl:value-of  select="ru:GetOverallTargetClassCount(/)"/> TargetClasses and <xsl:value-of select="ru:GetOverallMixinCount(/)" /> Mixins have been examined.</p>
	
	<p>Use one of the index sites to start browsing. Hold down [Shift] to sort on multiple columns.</p>
	
	<xsl:call-template name="errorList"/>

</xsl:template>

</xsl:stylesheet>
