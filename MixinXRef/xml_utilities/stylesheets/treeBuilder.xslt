<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
<xsl:template name="treeBuilder">
	<xsl:param name="caption" />
	<xsl:param name="rootTypes" />
  <xsl:param name="allReferences" />

  <div><span class="treeHeader"><xsl:value-of select="$caption" /></span></div>

  <div id="treeViewID">
    <xsl:call-template name="buildTree">
      <!-- rootTypes: get all involved classes which implements this attribute and get rid of involved classes which base-ref points to a class which also implements that interface 
			==> only get root implementing classes  -->
			<xsl:with-param name="rootTypes" select="$rootTypes[not(ru:contains($allReferences, @base-ref))]" />
      <!-- nonRootTypes = allTypes - rootTypes -->
			<xsl:with-param name="nonRootTypes" select="$rootTypes[(ru:contains($allReferences, @base-ref))]" />
      <xsl:with-param name="allReferences" select="$allReferences" />
		</xsl:call-template>
  </div>
  
</xsl:template>

<xsl:function name="ru:GetRecursiveTreeCount">
  <xsl:param name="rootMCR" />
  <xsl:param name="rootType" />
  <xsl:param name="allReferences" />
  
  <xsl:variable name="subTypes" select="$rootMCR[@base-ref = $rootType/@id  and  ru:contains($allReferences, @id)]" />
  <xsl:copy-of select="sum( for $subType in $subTypes return ru:GetRecursiveTreeCount($rootMCR, $subType, $allReferences) ) + 1" />
</xsl:function>

<xsl:template name="buildTree">
	<xsl:param name="rootTypes" />
	<xsl:param name="nonRootTypes" />
  <xsl:param name="allReferences" />
	
	<ul>	
		<xsl:for-each select="$rootTypes">
      <xsl:sort select="@name"/>
			<xsl:variable name="subTypes" select="$nonRootTypes[@base-ref = current()/@id]" />
			<li>
				<xsl:if test="exists($subTypes)">
					<span title="{ ru:GetToolTip(/, .) }">
            <xsl:value-of select="@name"/> (<xsl:value-of select="ru:GetRecursiveTreeCount($nonRootTypes, ., $allReferences) - 1"/>)
          </span>
          <a href="../involvedTypes/{@id}.html" class="tree-link"> [link]</a>
					
					<!-- recursive call -->
					<xsl:call-template name="buildTree">
						<xsl:with-param name="rootTypes" select="$subTypes" />
						<xsl:with-param name="nonRootTypes" select="$nonRootTypes" />
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
