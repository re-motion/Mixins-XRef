<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
<xsl:template name="involvedType">

	<xsl:call-template name="involvedTypeList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="involvedTypes" select="/MixinXRefReport/InvolvedTypes/InvolvedType" />  
		<xsl:with-param name="dir">.</xsl:with-param>
		<xsl:with-param name="caption">Involved Types</xsl:with-param>
	</xsl:call-template>

	<xsl:for-each select="/MixinXRefReport/InvolvedTypes/InvolvedType" >
		<!-- generate interface detail site for each interface -->
		<xsl:call-template name="involvedTypeDetailSite" />	
	</xsl:for-each>
</xsl:template>

<xsl:template name="involvedTypeDetailSite">
	<xsl:call-template name="htmlSite">
			<xsl:with-param name="siteTitle">Involved Type Detail</xsl:with-param>
			<xsl:with-param name="siteFileName">involvedTypes/<xsl:value-of select="@id"/>.html</xsl:with-param>
			<xsl:with-param name="bodyContentTemplate">involvedTypeDetail</xsl:with-param>
	</xsl:call-template>
</xsl:template>

<xsl:template name="involvedTypeDetail">
	<h1><xsl:value-of select="@name" /></h1><h2>[<a href="../involvedType_index.html">Involved Type</a>]</h2>

	<div class="involvedType-summary">
		<xsl:value-of select="summary" />
	</div>
	
	<xsl:call-template name="publicMemberList">
			<xsl:with-param name="members" select="PublicMembers/Member"/>
	</xsl:call-template>

	<xsl:call-template name="interfaceList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="interfaces" select="/MixinXRefReport/Interfaces/Interface[ImplementedBy/InvolvedType/@ref = current()/@id]" />
		<xsl:with-param name="dir">..</xsl:with-param>
	</xsl:call-template>
	
	<xsl:call-template name="attributeList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="attributes" select="/MixinXRefReport/Attributes/Attribute[ImplementedBy/InvolvedType/@ref = current()/@id]" />
		<xsl:with-param name="dir">..</xsl:with-param>
	</xsl:call-template>

</xsl:template>	
	
</xsl:stylesheet>
