<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">

<xsl:template name="errorList">

	<xsl:if test="count(//Exception) > 0">
		<p class="errorsFound"><xsl:value-of select="count(//Error)" /> errors detected!</p>
		
		
		
	</xsl:if>
	<xsl:if test="count(//Exception) = 0">
		<p class="noErrors">No Errors were found. (Mixin Configuration Errors, Validation Errors)</p>
	</xsl:if>
	
	
<!-- TODO
	<xsl:choose>
		<xsl:when test="count(//Exception) > 0">
			<div class="TODO">

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
	</xsl:choose>
-->
	
</xsl:template>
	
</xsl:stylesheet>