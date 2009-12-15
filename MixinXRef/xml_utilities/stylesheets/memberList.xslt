<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
 
<xsl:template name="memberList">
	<xsl:param name="members"/>
	
	<xsl:call-template name="tableTemplate">
		<xsl:with-param name="rootMCR"></xsl:with-param>
		<xsl:with-param name="items" select="$members"/>
		<xsl:with-param name="dir"></xsl:with-param>
		<xsl:with-param name="tableName">memberListTable</xsl:with-param>
		<xsl:with-param name="emptyText">No Public Members</xsl:with-param>
	</xsl:call-template>
	
</xsl:template>

<xsl:template name="memberListTable">
<xsl:param name="members"/>

	<table>
		<caption>Declared&#160;Members&#160;(<xsl:value-of select="count( $members[@is-declared-by-this-class = true()] )"/>)</caption>
		<thead>
			<tr>
				<th>Name</th>
				<th>Type</th>
        <th>Modifiers</th>
        <th>Signature</th>
        <xsl:if test="exists($members[@is-declared-by-this-class = true()]/Overrides/Mixin)">
          <th>Overridden By</th>  
        </xsl:if>
			</tr>
		</thead>
		<tfoot>
			<tr>
				<td><xsl:value-of select="count( $members[@is-declared-by-this-class = true()] )"/></td>
				<td>-</td>
        <td>-</td>
        <td>-</td>
        <xsl:if test="exists($members[@is-declared-by-this-class = true()]/Overrides/Mixin)">
          <td>-</td>
        </xsl:if>
			</tr>
		</tfoot>
		<tbody>
			<xsl:for-each select="$members[@is-declared-by-this-class = true()] ">
				<tr>
          <td id="{@name}">
            <xsl:value-of select="@name"/>
          </td>
					<td><xsl:value-of select="@type"/></td>
					<td>
						<xsl:apply-templates select="Modifiers" />
					</td>
				  <td><xsl:apply-templates select="Signature" /></td>
          <xsl:if test="exists($members[@is-declared-by-this-class = true()]/Overrides/Mixin)">
            <td>
              <xsl:for-each select="Overrides/Mixin">
                <xsl:if test="position() != 1">, </xsl:if>
                <xsl:call-template name="GenerateMixinReferenceLink">
                  <xsl:with-param name="mixin" select="." />
                </xsl:call-template>
              </xsl:for-each>
            </td>
          </xsl:if>
				</tr>
			</xsl:for-each>
		</tbody>
	</table>
  
  <xsl:if test="count( $members[@is-declared-by-this-class = false()] ) > 0">
  <table>
    <caption>
      Base&#160;Members&#160;overridden&#160;by&#160;mixins&#160;(<xsl:value-of select="count( $members[@is-declared-by-this-class = false()] )"/>)
    </caption>
    <thead>
      <tr>
        <th>Name</th>
        <th>Type</th>
        <th>Modifiers</th>
        <th>Signature</th>
        <xsl:if test="exists($members[@is-declared-by-this-class = false()]/Overrides)">
          <th>Overridden By</th>
        </xsl:if>
      </tr>
    </thead>
    <tfoot>
      <tr>
        <td>
          <xsl:value-of select="count( $members[@is-declared-by-this-class = false()] )"/>
        </td>
        <td>-</td>
        <td>-</td>
        <td>-</td>
        <xsl:if test="exists($members[@is-declared-by-this-class = false()]/Overrides)">
          <td>-</td>
        </xsl:if>
      </tr>
    </tfoot>
    <tbody>
      <xsl:for-each select="$members[@is-declared-by-this-class = false()] ">
        <tr>
          <td id="{@name}">
            <xsl:value-of select="@name"/>
          </td>
          <td>
            <xsl:value-of select="@type"/>
          </td>
          <td>
            <xsl:apply-templates select="Modifiers" />
          </td>
          <td>
            <xsl:apply-templates select="Signature" />
          </td>
          <xsl:if test="exists($members/Overrides)">
            <td>
              <xsl:for-each select="Overrides/Mixin">
                <xsl:if test="position() != 1"> <br/> </xsl:if>
                <xsl:call-template name="GenerateMixinReferenceLink">
                  <xsl:with-param name="mixin" select="." />
                </xsl:call-template>
              </xsl:for-each>
            </td>
          </xsl:if>
        </tr>
      </xsl:for-each>
    </tbody>
  </table>
  </xsl:if>
  
</xsl:template>


<!-- without strip-space, each span would be created in an own line. -->
<xsl:strip-space elements="Modifiers Signature" />
  
<xsl:template match="Keyword | Type | Text | Name | ParameterName | ExplicitInterfaceName" >
	<span class="{name(.)}"><xsl:value-of select="."/>
    <xsl:if test=". != '(' and . != '[' and name(.) != 'ParameterName' and name(.) != 'ExplicitInterfaceName' and following-sibling::*[1] !=  ',' and following-sibling::*[1] !=  ']'">
      <xsl:text> </xsl:text>
    </xsl:if>
  </span>
</xsl:template>
  
</xsl:stylesheet>
