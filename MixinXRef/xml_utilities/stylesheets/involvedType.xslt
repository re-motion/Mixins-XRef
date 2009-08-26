<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
<xsl:template name="involvedType">

	<xsl:call-template name="involvedTypeList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="involvedTypes" select="/MixinXRefReport/InvolvedTypes/InvolvedType" />  
		<xsl:with-param name="dir">.</xsl:with-param>
		<xsl:with-param name="caption">Involved&#160;Types</xsl:with-param>
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
	<h1><xsl:value-of select="@name" /></h1><h2><a href="../involvedType_index.html">[Involved Type]</a></h2>
	
	<div>
		from assembly 
		<xsl:call-template name="GenerateAssemblyLink">
			<xsl:with-param name="rootMCR" select="/" />
			<xsl:with-param name="assemblyId" select="@assembly-ref"/>
			<xsl:with-param name="dir">..</xsl:with-param>
		</xsl:call-template>
	</div>

	<fieldset>
		<legend>Summary</legend>
		
		<div>Base: <span><xsl:call-template name="involvedTypeBaseLink"/></span></div>
		
		<xsl:if test="@is-target = true()">
			<xsl:variable name="mixinCount" select="count( Mixins/Mixin )"/>
			<div>This type is a Target with <xsl:value-of select="$mixinCount" /> Mixin<xsl:if test="$mixinCount > 1">s</xsl:if> applied.</div>
		</xsl:if>
		
		<xsl:if test="@is-mixin = true()">
			<xsl:variable name="targetCount" select="count( Targets/Target )"/>
			<div>This type is a Mixin and is applied to <xsl:value-of select="$targetCount" /> Target<xsl:if test="$targetCount > 1">s</xsl:if>.</div>
		</xsl:if>
		
		<xsl:if test="@is-generic-definition = true()">
			<div><span class="dubiosInvolvedType">This type is a generic definition. Therefore detailed Mixin information is not available.</span></div>
		</xsl:if>
		
		<div class="involvedType-summary">
			<xsl:apply-templates select="summary" />
		</div>
	</fieldset>
	
	<xsl:call-template name="publicMemberList">
			<!-- summaries may contain other tags, eg. 'cref' and content -->
			<xsl:with-param name="members" select="PublicMembers/Member"/>
	</xsl:call-template>

	<xsl:call-template name="interfaceList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="interfaces" select="/MixinXRefReport/Interfaces/Interface[ImplementedBy/InvolvedType/@ref = current()/@id]" />
		<xsl:with-param name="dir">..</xsl:with-param>
	</xsl:call-template>
	
	<xsl:call-template name="attributeList">
		<xsl:with-param name="rootMCR" select="/" />
		<xsl:with-param name="attributes" select="/MixinXRefReport/Attributes/Attribute[AppliedTo/InvolvedType/@ref = current()/@id]" />
    <xsl:with-param name="attributeRefs" select="Attributes/Attribute"/>
		<xsl:with-param name="dir">..</xsl:with-param>
	</xsl:call-template>
	
	<xsl:call-template name="mixinList">
		<xsl:with-param name="involvedType" select="." />
	</xsl:call-template>
	
	<xsl:call-template name="treeBuilder">
		<xsl:with-param name="involvedType" select="." />
	</xsl:call-template>

</xsl:template>


<xsl:template match="InvolvedType/summary/*">
	<b>
		<xsl:value-of select="."/>
		<xsl:value-of select="substring(@*, 3)"/>
	</b>
</xsl:template>

<xsl:template name="involvedTypeBaseLink">
	<xsl:if test="@base-ref != 'none' ">
		<a href="{@base-ref}.html"><xsl:value-of select="@base"/></a>
	</xsl:if>
	<xsl:if test="@base-ref = 'none' ">
		<xsl:value-of select="@base"/>
	</xsl:if>
</xsl:template>

</xsl:stylesheet>
