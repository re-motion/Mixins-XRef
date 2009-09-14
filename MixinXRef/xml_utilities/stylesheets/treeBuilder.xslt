<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
<xsl:template name="treeBuilder">
	<xsl:param name="caption" />
	<xsl:param name="rootTypes" />
  <xsl:param name="allReferences" />

		<div><span class="treeHeader"><xsl:value-of select="$caption" /></span></div>
		
		<xsl:call-template name="inOrderTreeWalk">
			<xsl:with-param name="rootTypes" select="$rootTypes" />
      <xsl:with-param name="allReferences" select="$allReferences" />
		</xsl:call-template>
	
</xsl:template>

<xsl:function name="ru:GetRecursiveTreeCount">
  <xsl:param name="rootMCR" />
  <xsl:param name="rootType" />
  <xsl:param name="allReferences" />
  
  <xsl:variable name="subTypes" select="$rootMCR/MixinXRefReport/InvolvedTypes/InvolvedType[@base-ref = $rootType/@id  and  ru:contains($allReferences, @id)]" />
  <xsl:copy-of select="sum( for $subType in $subTypes return ru:GetRecursiveTreeCount($rootMCR, $subType, $allReferences) ) + 1" />
</xsl:function>
  
<xsl:template name="inOrderTreeWalk">
	<xsl:param name="rootTypes" />
  <xsl:param name="allReferences" />
	
	<ul>	
		<xsl:for-each select="$rootTypes">
      <xsl:sort select="@name"/>
			<xsl:variable name="subTypes" select="/MixinXRefReport/InvolvedTypes/InvolvedType[@base-ref = current()/@id  and  ru:contains($allReferences, @id)]" />
			<li>
				<xsl:if test="exists($subTypes)">
					<span title="{ ru:GetToolTip(/, .) }">
            <xsl:value-of select="@name"/> (<xsl:value-of select="ru:GetRecursiveTreeCount(/, ., $allReferences) - 1"/>)
          </span>
          <a href="../involvedTypes/{@id}.html" class="tree-link"> [link]</a>
					
					<!-- recursive call -->
					<xsl:call-template name="inOrderTreeWalk">
						<xsl:with-param name="rootTypes" select="$subTypes" />
            <xsl:with-param name="allReferences" select="$allReferences" />
					</xsl:call-template>
				</xsl:if>
				<xsl:if test="empty( $subTypes )">
          <span title="{ ru:GetToolTip(/, .) }">
            <xsl:value-of select="@name"/>
          </span>
          <a href="../involvedTypes/{@id}.html" class="tree-link"> [link]</a>
          
				</xsl:if>
			</li>		
		</xsl:for-each>	
	</ul>
</xsl:template>
	
</xsl:stylesheet>
