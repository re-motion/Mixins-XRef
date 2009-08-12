<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
	
<xsl:template name="index">
	<h1>Mixin Documentation Summary @<xsl:value-of select="/MixinXRefReport/@creationTime" /></h1>
	
	<p><xsl:value-of select="count( //Assemblies/Assembly )" /> assemblies have been examined.</p>
	
	<p><xsl:value-of  select="ru:GetOverallTargetClassCount(/)"/> TargetClasses and <xsl:value-of select="ru:GetOverallMixinCount(/)" /> Mixins have been reviewed. </p>
	
	<p>Use one of the index sites to start browsing. Hold down [Shift] to sort on multiple columns.</p>
	
	<!--
	<xsl:choose>
					<xsl:when test="count(//Error) > 0">
						<div class="warning">
							<p>
								<xsl:value-of select="count(//Error)" /> errors detected!
							</p>
							<h3>Configuration Errors</h3>
							<xsl:for-each select="//ConfigurationErrors/Error" >
								<p><xsl:value-of select="position()" />: <xsl:value-of select="." /></p>
							</xsl:for-each>
							<h3>Validation Errors</h3>
							<xsl:for-each select="//ValidationErrors/Error" >
								<p><xsl:value-of select="position()" />: <xsl:value-of select="." /></p>
							</xsl:for-each>
						</div>
					</xsl:when>
					<xsl:otherwise>
						<p>No Errors were found. (Mixin Configuration Error, Validation Error)</p>
					</xsl:otherwise>
				</xsl:choose>
-->


</xsl:template>

</xsl:stylesheet>
