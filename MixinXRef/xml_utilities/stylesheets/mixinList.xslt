<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
<xsl:template name="mixinList">
	<xsl:param name="involvedType"/>
	
	<xsl:call-template name="tableTemplate">
		<xsl:with-param name="rootMCR" select="/"/>
		<xsl:with-param name="items" select="$involvedType/Mixins/Mixin"/>
		<xsl:with-param name="dir">..</xsl:with-param>
		<xsl:with-param name="tableName">mixinListTable</xsl:with-param>
		<xsl:with-param name="emptyText">No Mixins</xsl:with-param>
    <xsl:with-param name="target" select="$involvedType"/>
	</xsl:call-template>
</xsl:template>

<xsl:template name="mixinListTable">	
	<xsl:param name="rootMCR" />
	<xsl:param name="mixinRefs" />
	<xsl:param name="dir" />
  <xsl:param name="target" />
	
	<xsl:variable name="mixins" select="/MixinXRefReport/InvolvedTypes/InvolvedType[ ru:contains($mixinRefs/@ref, @id) ]" />
	
		<table>
			<caption>Mixins&#160;(<xsl:value-of select="count( $mixins )" />)</caption>
			<thead>
				<tr>
					<th>Index</th>
					<th>Name</th>
					<th>applied to # Targets</th>
					<th>Relation</th>
					<th>Interface Introductions</th>
					<th>Attribute Introductions</th>
					<th>Overriden Members</th>
          <th>Additional Dependencies</th>
          <th>Introduced Member Visibility</th>
				</tr>
			</thead>
			<tfoot>
				<tr>
					<td>-</td>
					<td><xsl:value-of select="count( $mixins )" /></td>
					<td>-</td>
					<td>-</td>

          <xsl:if test="$target/@is-generic-definition = false()">
					  <td>
              <xsl:value-of select="count( distinct-values( $mixinRefs/InterfaceIntroductions/Interface/@ref) )"/>
              (non-distinct: <xsl:value-of select="count($mixinRefs/InterfaceIntroductions/Interface/@ref)"/>)
            </td>
					  <td>
              <xsl:value-of select="count( distinct-values( $mixinRefs/AttributeIntroductions/Attribute/@ref) )"/>
              (non-distinct: <xsl:value-of select="count($mixinRefs/AttributeIntroductions/Attribute/@ref)"/>)
            </td>
					  <td>
              <xsl:value-of select="count( distinct-values( $mixinRefs/MemberOverrides/Member/@name) )"/>
              (non-distinct: <xsl:value-of select="count($mixinRefs/MemberOverrides/Member/@name)"/>)
            </td>
          </xsl:if>
          <xsl:if test="$target/@is-generic-definition = true()">
            <td>n/a</td>
            <td>n/a</td>
            <td>n/a</td>
          </xsl:if>
          <td>-</td>
          <td>-</td>
				</tr>
			</tfoot>
			<tbody>
				<xsl:for-each select="$mixinRefs">				
					<xsl:variable name="mixin" select=" key('involvedType', @ref) "/>
					
					<tr class="{ ru:getDubiosInvolvedTypeClass(.) }">
						<td><xsl:value-of select="@index"/></td>
						<td>
							<xsl:call-template name="GenerateMixinReferenceLink">
								<xsl:with-param name="mixin" select="." />
							</xsl:call-template>
						</td>
						<td><xsl:value-of select="count( $mixin/Targets/Target )" /></td>
						<td><xsl:value-of select="@relation" /></td>

            <xsl:call-template name="mixinSpecificData">
              <xsl:with-param name="target" select="$target" />
            </xsl:call-template>

            <!-- Additional Dependencies -->
            <td>
              <xsl:for-each select="AdditionalDependencies/Mixin">
                <xsl:if test="position() != 1">, </xsl:if>
                <xsl:call-template name="GenerateMixinReferenceLink">
                  <xsl:with-param name="mixin" select="."/>
                </xsl:call-template>
              </xsl:for-each>
            </td>
            
            <td><xsl:value-of select="@introduced-member-visibility"/></td>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
		
</xsl:template>

<xsl:template name="mixinSpecificData">
  <xsl:param name="target" />

  <!-- output mixin specific data if target is not a generic type definition -->
  <xsl:if test="$target/@is-generic-definition = false()">
    <!-- Interface Introductions -->
    <td>
      <xsl:for-each select="InterfaceIntroductions/Interface">
        <xsl:sort select="/MixinXRefReport/Interfaces/Interface[@id = current()/@ref]/@name"/>
        <xsl:if test="position() != 1">, </xsl:if>
        <xsl:call-template name="GenerateInterfaceLink">
          <xsl:with-param name="rootMCR" select="/" />
          <xsl:with-param name="interfaceId" select="@ref" />
          <xsl:with-param name="dir">..</xsl:with-param>
        </xsl:call-template>
      </xsl:for-each>
    </td>
    <!-- Attribute Introductions -->
    <td>
      <xsl:for-each select="AttributeIntroductions/Attribute">
        <xsl:sort select="/MixinXRefReport/Attributes/Attribute[@id = current()/@ref]/@name"/>
        <xsl:if test="position() != 1">, </xsl:if>
        <xsl:call-template name="GenerateAttributeLink">
          <xsl:with-param name="rootMCR" select="/" />
          <xsl:with-param name="attributeId" select="@ref" />
          <xsl:with-param name="dir">..</xsl:with-param>
        </xsl:call-template>
      </xsl:for-each>
    </td>
    <!-- Member Overrides -->
    <td>
      <xsl:for-each select="MemberOverrides/Member">
        <xsl:sort select="@name" />
        <xsl:if test="position() != 1">, </xsl:if>
        <xsl:value-of select="@name"/><span class="small-method-type">[<xsl:value-of select="@type"/>]</span>
      </xsl:for-each>
    </td>
  </xsl:if>

  <xsl:if test="$target/@is-generic-definition = true()">
    <td class="dubiosInvolvedType">n/a</td>
    <td class="dubiosInvolvedType">n/a</td>
    <td class="dubiosInvolvedType">n/a</td>
  </xsl:if>
</xsl:template>

</xsl:stylesheet>