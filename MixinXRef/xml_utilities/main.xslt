<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
	version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru"
	>
	
<xsl:output
	name="standardHtmlOutputFormat"
	method="html"
	indent="yes"
	omit-xml-declaration="yes"
	doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"
    doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"
    />
    
    
 <!-- keys -->
<xsl:key name="assembly" match="//Assemblies/Assembly" use="@id" />
<xsl:key name="type" match="//InvolvedTypes/InvolvedType" use="@id" />
<xsl:key name="interface" match="//Interfaces/Interface" use="@id" />
<xsl:key name="attribute" match="//Attributes/Attribute" use="@id" />

<!-- include sub stylesheets for sites -->
<xsl:include href="stylesheets/template.xslt" />
<xsl:include href="stylesheets/index.xslt" />
<xsl:include href="stylesheets/assembly.xslt" />

<!-- component stylesheets -->
<xsl:include href="stylesheets/involvedTypeList.xslt" />


<!-- 'main' template -->
<xsl:template match="/">
	<!-- main index (summary) -->
	<xsl:call-template name="htmlSite">
		<xsl:with-param name="siteTitle">Mixin Documentation Index</xsl:with-param>
		<xsl:with-param name="siteFileName">index.html</xsl:with-param>
		<xsl:with-param name="bodyContentTemplate">index</xsl:with-param>
	</xsl:call-template>
	<!-- assembly index + assembly sites -->
	<xsl:call-template name="htmlSite">
		<xsl:with-param name="siteTitle">Assembly Index</xsl:with-param>
		<xsl:with-param name="siteFileName">assembly_index.html</xsl:with-param>
		<xsl:with-param name="bodyContentTemplate">assembly</xsl:with-param>
	</xsl:call-template>
	
	
</xsl:template>


<!-- overall count functions -->
<xsl:function name="ru:GetOverallTargetClassCount">
	<xsl:param name="rootMCR" />
	<xsl:copy-of select="count( $rootMCR//InvolvedTypes/InvolvedType[@is-target = true()] )" />
</xsl:function>

<xsl:function name="ru:GetOverallMixinCount">
	<xsl:param name="rootMCR" />
	<xsl:copy-of select="count( $rootMCR//InvolvedTypes/InvolvedType[@is-mixin = true()] )" />
</xsl:function>

<xsl:function name="ru:GetOverallAssemblyCountExclED">
	<xsl:param name="rootMCR" />
	<xsl:copy-of select="count( $rootMCR//Assemblies/Assembly )" />
</xsl:function>


<!-- link generation templates -->
<xsl:template name="GenerateAssemblyLink">
	<xsl:param name="rootMCR" />
	<xsl:param name="assemblyId"/>
	<xsl:param name="fromIndexSite" select="false()"/>
	
	<xsl:variable name="dir" select="if($fromIndexSite) then 'assemblies/' else '' "/>
	
	<a href="{$dir}{$assemblyId}.html"><xsl:value-of select="$rootMCR/key('assembly', $assemblyId)/@name" /></a>
</xsl:template>

</xsl:stylesheet>
