<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">

<!-- general utility functions -->
<xsl:function name="ru:contains">
	<xsl:param name="sequence" />
	<xsl:param name="searchItem" />
	
	<xsl:copy-of select=" exists( index-of( $sequence, $searchItem ) )" />
</xsl:function>

<xsl:function name="ru:getDubiosInvolvedTypeClass">
	<xsl:param name="it"/>
	
	<xsl:copy-of select="if ( $it/@is-target = true() and $it/@is-mixin = true() ) then 'dubiosInvolvedType' else '' "/>
</xsl:function>

<!-- overall count functions -->
<xsl:function name="ru:GetOverallTargetClassCount">
	<xsl:param name="rootMCR" />
	<xsl:copy-of select="count( $rootMCR//InvolvedTypes/InvolvedType[@is-target = true()] )" />
</xsl:function>

<xsl:function name="ru:GetOverallMixinCount">
	<xsl:param name="rootMCR" />
	<xsl:copy-of select="count( $rootMCR//InvolvedTypes/InvolvedType[@is-mixin = true()] )" />
</xsl:function>

<xsl:function name="ru:GetOverallAssemblyCount">
	<xsl:param name="rootMCR" />
	<xsl:copy-of select="count( $rootMCR//Assemblies/Assembly )" />
</xsl:function>


<!-- link generation templates -->
<xsl:template name="GenerateGenericLink">
	<xsl:param name="rootMCR" />
	<xsl:param name="id"/>
	<xsl:param name="keyName"/>
	<xsl:param name="dir"/>
	
	<a href="{$dir}{$id}.html"><xsl:value-of select="$rootMCR/key($keyName, $id)/@name" /></a>
</xsl:template>

<xsl:template name="GenerateAssemblyLink">
	<xsl:param name="rootMCR" />
	<xsl:param name="assemblyId"/>
	<xsl:param name="dir"/>
		
	<xsl:if test="$assemblyId = 'none'">
		<xsl:text>external</xsl:text>
	</xsl:if>
	<xsl:if test="$assemblyId != 'none'">
		<xsl:call-template name="GenerateGenericLink">
			<xsl:with-param name="rootMCR" select="$rootMCR" />
			<xsl:with-param name="id" select="$assemblyId"/>
			<xsl:with-param name="keyName">assembly</xsl:with-param>		
			<xsl:with-param name="dir" select="concat($dir, '/assemblies/')"/>
		</xsl:call-template>
	</xsl:if>
</xsl:template>

<xsl:template name="GenerateInvolvedTypeLink">
	<xsl:param name="rootMCR" />
	<xsl:param name="involvedTypeId"/>
	<xsl:param name="dir"/>
		
	<xsl:call-template name="GenerateGenericLink">
		<xsl:with-param name="rootMCR" select="$rootMCR" />
		<xsl:with-param name="id" select="$involvedTypeId"/>
		<xsl:with-param name="keyName">involvedType</xsl:with-param>		
		<xsl:with-param name="dir" select="concat($dir, '/involvedTypes/')"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="GenerateMixinReferenceLink">
	<xsl:param name="mixin"/>
	
	<a href="{$mixin/@ref}.html"><xsl:value-of select="$mixin/@instance-name" /></a>
</xsl:template>

<xsl:template name="GenerateInterfaceLink">
	<xsl:param name="rootMCR" />
	<xsl:param name="interfaceId"/>
	<xsl:param name="dir"/>
	
	<xsl:call-template name="GenerateGenericLink">
		<xsl:with-param name="rootMCR" select="$rootMCR" />
		<xsl:with-param name="id" select="$interfaceId"/>
		<xsl:with-param name="keyName">interface</xsl:with-param>		
		<xsl:with-param name="dir" select="concat($dir, '/interfaces/')"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="GenerateAttributeLink">
	<xsl:param name="rootMCR" />
	<xsl:param name="attributeId"/>
	<xsl:param name="dir"/>
	
	<xsl:call-template name="GenerateGenericLink">
		<xsl:with-param name="rootMCR" select="$rootMCR" />
		<xsl:with-param name="id" select="$attributeId"/>
		<xsl:with-param name="keyName">attribute</xsl:with-param>		
		<xsl:with-param name="dir" select="concat($dir, '/attributes/')"/>
	</xsl:call-template>
</xsl:template>


</xsl:stylesheet>
