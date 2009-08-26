<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">

<xsl:template name="attributeList">
	<xsl:param name="rootMCR" />	
	<xsl:param name="attributes" />
  <xsl:param name="attributeRefs" />
	<xsl:param name="dir" />
	
	<xsl:call-template name="tableTemplate">
		<xsl:with-param name="rootMCR" select="$rootMCR"/>
		<xsl:with-param name="items" select="$attributes"/>
		<xsl:with-param name="additionalItems" select="$attributeRefs" />
		<xsl:with-param name="dir" select="$dir"/>
		<xsl:with-param name="tableName">attributeListTable</xsl:with-param>
		<xsl:with-param name="emptyText">No Attributes</xsl:with-param>
	</xsl:call-template>
	
</xsl:template>

<xsl:template name="attributeListTable">
	<xsl:param name="rootMCR" />	
	<xsl:param name="attributes" />
	<xsl:param name="attributeRefs" />
	<xsl:param name="dir" />
	
		<table>
			<caption>Attributes&#160;(<xsl:value-of select="count( $attributes )" />)</caption>
			<thead>
				<tr>
					<th>Namespace</th>
					<th>Name</th>
					<th># of Uses</th>
					<th>Assembly</th>	
					<xsl:if test="$attributeRefs">
						<th>Arguments</th>
					</xsl:if>
				</tr>
			</thead>
			<tfoot>
				<tr>
					<td><xsl:value-of select="count( distinct-values( $attributes/@namespace ) )" /></td>
					<td><xsl:value-of select="count( $attributes )" /></td>
					<td>-</td>
					<td><xsl:value-of select="count( distinct-values( $attributes/@assembly-ref ) )" /></td>
					<xsl:if test="$attributeRefs">
						<td>-</td>
					</xsl:if>
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
						<xsl:if test="$attributeRefs">
							<td>
								<xsl:call-template name="tableTemplate">
									<xsl:with-param name="rootMCR" select="$rootMCR"/>
									<xsl:with-param name="items" select="$attributeRefs[ @ref = current()/@id ]/Argument"/>
									<xsl:with-param name="dir" select="$dir"/>
									<xsl:with-param name="tableName">attributeArgumentListTable</xsl:with-param>
									<xsl:with-param name="emptyText">No Arguments</xsl:with-param>
								</xsl:call-template>
							</td>
					</xsl:if>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
</xsl:template>
	
<xsl:template name="attributeArgumentListTable">
	<xsl:param name="rootMCR" />	
	<xsl:param name="arguments" />

	<table>
		<caption>Arguments&#160;(<xsl:value-of select="count( $arguments )" />)</caption>
		<thead>
			<tr>
				<th>Kind</th>
				<th>Name</th>
				<th>Type</th>
				<th>Value</th>	
			</tr>
		</thead>
		<tfoot>
			<tr>
				<td>-</td>
				<td><xsl:value-of select="count( $arguments )" /></td>
				<td>-</td>
				<td>-</td>
			</tr>
		</tfoot>
		<tbody>
			<xsl:for-each select="$arguments">
				<tr>
					<td><xsl:value-of select="@kind"/></td>
					<td><xsl:value-of select="@name"/></td>
					<td><xsl:value-of select="@type"/></td>
					<td><xsl:value-of select="@value"/></td>
				</tr>
			</xsl:for-each>
		</tbody>
	</table>
	
</xsl:template>


</xsl:stylesheet>