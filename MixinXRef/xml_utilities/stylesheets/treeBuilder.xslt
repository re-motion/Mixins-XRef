<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
<xsl:template name="treeBuilder">
	<xsl:param name="involvedType" />
	
	<xsl:if test="$involvedType/@is-mixin = true()">
		<div><span class="treeHeader">Targets (<xsl:value-of select="count( $involvedType/Targets/Target )" />)</span></div>
		
		<xsl:call-template name="inOrderTreeWalk">
			<!-- starting point for recursion: get all targets for this mixin and get rid of targets which base-ref points to another target of the same ==> only get root targets -->
			<xsl:with-param name="targets" select="/MixinXRefReport/InvolvedTypes/InvolvedType[ (ru:contains($involvedType/Targets/Target/@ref, @id)) and not(ru:contains($involvedType/Targets/Target/@ref, @base-ref)) ]" /> 
		</xsl:call-template>
	</xsl:if>
</xsl:template>	

<xsl:template name="inOrderTreeWalk">
	<xsl:param name="targets" />
	
	<ul>	
		<xsl:for-each select="$targets">
			<xsl:variable name="subTargets" select="/MixinXRefReport/InvolvedTypes/InvolvedType[@base-ref = current()/@id]" />
			<li>
				<xsl:if test="exists($subTargets)">
					<span><xsl:value-of select="@name"/></span><a href="{@id}.html" class="tree-link"> [link]</a>
					
					<!-- recursive call -->
					<xsl:call-template name="inOrderTreeWalk">
						<xsl:with-param name="targets" select="$subTargets" />
					</xsl:call-template>
				</xsl:if>
				<xsl:if test="empty( $subTargets )">
					<xsl:value-of select="@name"/><a href="{@id}.html" class="tree-link"> [link]</a>
				</xsl:if>
			</li>		
		</xsl:for-each>	
	</ul>
</xsl:template>
	
</xsl:stylesheet>
