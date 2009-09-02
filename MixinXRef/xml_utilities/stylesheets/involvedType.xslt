<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
<xsl:template name="involvedType">

	<xsl:call-template name="involvedTypeList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="involvedTypes" select="/MixinXRefReport/InvolvedTypes/InvolvedType" />  
		<xsl:with-param name="dir">.</xsl:with-param>
		<xsl:with-param name="caption">Involved&#160;Classes</xsl:with-param>
	</xsl:call-template>

  <xsl:for-each select="/MixinXRefReport/InvolvedTypes/InvolvedType" >
		<!-- generate interface detail site for each interface -->
		<xsl:call-template name="involvedTypeDetailSite" />	
	</xsl:for-each>
</xsl:template>

<xsl:template name="involvedTypeDetailSite">
	<xsl:call-template name="htmlSite">
			<xsl:with-param name="siteTitle">Involved Class Detail</xsl:with-param>
			<xsl:with-param name="siteFileName">involvedTypes/<xsl:value-of select="@id"/>.html</xsl:with-param>
			<xsl:with-param name="bodyContentTemplate">involvedTypeDetail</xsl:with-param>
	</xsl:call-template>
</xsl:template>

<xsl:template name="involvedTypeDetail">
	<h1><xsl:value-of select="@name" /></h1><h2><a href="../involvedType_index.html">[Involved Class]</a></h2>

	<fieldset>
		<legend>Summary</legend>

    <div>
      <label>Namespace:</label>
      <xsl:value-of select="@namespace"/>
    </div>
    <div>
      <label>Assembly:</label>
      <xsl:call-template name="GenerateAssemblyLink">
        <xsl:with-param name="rootMCR" select="/" />
        <xsl:with-param name="assemblyId" select="@assembly-ref"/>
        <xsl:with-param name="dir">..</xsl:with-param>
      </xsl:call-template>
    </div>
    <div>
      <label>Base:</label>
      <xsl:call-template name="involvedTypeBaseLink"/>
    </div>
	<div>
      <label>Interfaces:</label>
		<xsl:for-each select="Interfaces/Interface/@ref">
			<xsl:if test="position() != 1">, </xsl:if>
			<xsl:call-template name="GenerateInterfaceLink">
				<xsl:with-param name="rootMCR" select="/" />
				<xsl:with-param name="interfaceId" select="." />
				<xsl:with-param name="dir">..</xsl:with-param>
			</xsl:call-template>			
		</xsl:for-each>
    </div>
    <div>
      <label>Mixins applied:</label>
      <xsl:value-of select="count( Mixins/Mixin )"/>
    </div>

    <div>
      <label>Applied to:</label>
      <xsl:value-of select="count( Targets/Target )"/>
    </div>
		
		<xsl:if test="@is-generic-definition = true()">
			<div><span class="dubiosInvolvedType">This type is a generic definition. Therefore detailed Mixin information is not available.</span></div>
		</xsl:if>
		
		<div class="involvedType-summary">
			<xsl:apply-templates select="summary" />
		</div>
	</fieldset>

	<xsl:call-template name="mixinList">
		<xsl:with-param name="involvedType" select="." />
	</xsl:call-template>

	<xsl:if test="@is-mixin = true()"> 
		<xsl:call-template name="treeBuilder">
		  <xsl:with-param name="caption">Targets&#160;(<xsl:value-of select="count( Targets/Target )" />)</xsl:with-param>
			<!-- starting point for recursion: get all targets for this mixin and get rid of targets which base-ref points to another target of the same ==> only get root targets -->
			<xsl:with-param name="rootTypes" select="/MixinXRefReport/InvolvedTypes/InvolvedType[ (ru:contains(current()/Targets/Target/@ref, @id)) and not(ru:contains(current()/Targets/Target/@ref, @base-ref)) ]" />
      <xsl:with-param name="allReferences" select="Targets/Target/@ref" />
		</xsl:call-template>
		
	</xsl:if>
	<xsl:if test="@is-mixin = false()"> 
		<div class="emptyText">No Targets</div>
	</xsl:if>
	
	<xsl:call-template name="attributeRefList">
		<xsl:with-param name="rootMCR" select="/" />
    <xsl:with-param name="attributeRefs" select="Attributes/Attribute"/>
		<xsl:with-param name="dir">..</xsl:with-param>
	</xsl:call-template>
			
	<xsl:call-template name="publicMemberList">
			<!-- summaries may contain other tags, eg. 'cref' and content -->
			<xsl:with-param name="members" select="PublicMembers/Member"/>
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
