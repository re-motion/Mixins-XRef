<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">

<xsl:template name="attributeList">
	<xsl:param name="rootMCR" />	
	<xsl:param name="attributes" />
	<xsl:param name="dir" />
	
	<xsl:call-template name="tableTemplate">
		<xsl:with-param name="rootMCR" select="$rootMCR"/>
		<xsl:with-param name="items" select="$attributes"/>
		<xsl:with-param name="dir" select="$dir"/>
		<xsl:with-param name="tableName">attributeListTable</xsl:with-param>
		<xsl:with-param name="emptyText">No attributes found</xsl:with-param>
	</xsl:call-template>
	
</xsl:template>

<xsl:template name="attributeListTable">
	<xsl:param name="rootMCR" />	
	<xsl:param name="attributes" />
	<xsl:param name="dir" />
	
		<table>
			<caption>Attributes (<xsl:value-of select="count( $attributes )" />)</caption>
			<thead>
				<tr>
					<th>Namespace</th>
					<th>Name</th>
					<th># of Uses</th>
					<th>Assembly</th>	
				</tr>
			</thead>
			<tfoot>
				<tr>
					<td><xsl:value-of select="count( distinct-values( $attributes/@namespace ) )" /></td>
					<td><xsl:value-of select="count( $attributes )" /></td>
					<td>-</td>
					<td><xsl:value-of select="count( distinct-values( $attributes/@assembly-ref ) )" /></td>
				</tr>
			</tfoot>
			<tbody>
				<xsl:for-each select="$attributes">
					<tr>
						<td><xsl:value-of select="@namespace"/></td>
						<td>
							<xsl:call-template name="GenerateAttributeLink">
								<xsl:with-param name="rootMCR" select="$rootMCR" />
								<xsl:with-param name="attributeId" select="@id" />
								<xsl:with-param name="dir" select="$dir" />
							</xsl:call-template>
						</td>
						<td><xsl:value-of select="count( AppliedTo/InvolvedType )" /></td>
						<td>
							<xsl:call-template name="GenerateAssemblyLink">
								<xsl:with-param name="rootMCR" select="$rootMCR" />
								<xsl:with-param name="assemblyId" select="@assembly-ref" />
								<xsl:with-param name="dir" select="$dir" />
							</xsl:call-template>
						</td>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
</xsl:template>
	
</xsl:stylesheet>