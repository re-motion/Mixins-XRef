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
		<caption>Members&#160;(<xsl:value-of select="count( $members )"/>)</caption>
		<thead>
			<tr>
				<th>Name</th>
				<th>Type</th>
        <th>Modifiers</th>
        <th>Signature</th>
			</tr>
		</thead>
		<tfoot>
			<tr>
				<td><xsl:value-of select="count( $members )"/></td>
				<td>-</td>
			</tr>
		</tfoot>
		<tbody>
			<xsl:for-each select="$members">
				<tr>
					<td><xsl:value-of select="@name"/></td>
					<td><xsl:value-of select="@type"/></td>
					<td>
						<xsl:apply-templates select="Modifiers" />
					</td>
				  <td><xsl:value-of select="@signature"/></td>
				</tr>
			</xsl:for-each>
		</tbody>
	</table>
</xsl:template>
	
<xsl:template match="Modifiers/Keyword">
	<span class="keyword"><xsl:value-of select="."/></span>
</xsl:template>

</xsl:stylesheet>
