<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
<xsl:template name="treeBuilder">
	<xsl:param name="caption" />
	<xsl:param name="treeNodes" />
  <xsl:param name="allReferences" />

  <div><span class="treeHeader"><xsl:value-of select="$caption" /></span></div>

  <div id="treeViewID">
    <xsl:call-template name="buildTree">
      <!-- treeNodes: get all involved classes which implements this attribute and get rid of involved classes which base-ref points to a class which also implements that interface 
			==> only get root implementing classes  -->
			<xsl:with-param name="rootNodes" select="$treeNodes[not(ru:contains($allReferences, @base-ref))]" /> <!--  and  ru:contains($allReferences, @id) -->
      <!-- nonRootNodes = allTypes - treeNodes -->
			<xsl:with-param name="nonRootNodes" select="$treeNodes[(ru:contains($allReferences, @base-ref))]" />
		</xsl:call-template>
  </div>
  
</xsl:template>


<xsl:template name="buildTree">
	<xsl:param name="rootNodes" />
	<xsl:param name="nonRootNodes" />
  	
	<ul>	
		<xsl:for-each select="$rootNodes">
		<xsl:sort select="@name"/>
			<xsl:variable name="subTypes" select="$nonRootNodes[@base-ref = current()/@id]" />
			<li>
			
				<xsl:if test="exists($subTypes)">							
					<xsl:variable name="subTree">
						<!-- recursive call -->
						<xsl:call-template name="buildTree">
							<xsl:with-param name="rootNodes" select="$subTypes" />
							<xsl:with-param name="nonRootNodes" select="$nonRootNodes" />
						</xsl:call-template>
					</xsl:variable>				
					
					<span title="{ ru:GetToolTip(/, .) }">					
						<xsl:value-of select="@name"/> (<xsl:value-of select="count($subTree/descendant::*[not(child::*)]) div 2"/>)
					</span>
					<a href="../involvedTypes/{@id}.html" class="tree-link"> [link]</a>
					<xsl:copy-of select="$subTree"/>					
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
