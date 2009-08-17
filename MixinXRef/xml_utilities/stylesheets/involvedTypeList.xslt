<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">

<xsl:template name="involvedTypeList">
	<xsl:param name="rootMCR" />	
	<xsl:param name="involvedTypes" />
	<xsl:param name="dir" />
	<xsl:param name="caption" />
	
	<xsl:call-template name="tableTemplate">
		<xsl:with-param name="rootMCR" select="$rootMCR"/>
		<xsl:with-param name="items" select="$involvedTypes"/>
		<xsl:with-param name="dir" select="$dir"/>
		<xsl:with-param name="tableName">involvedTypeListTable</xsl:with-param>
		<xsl:with-param name="emptyText">No involved types found</xsl:with-param>
		<xsl:with-param name="caption" select="$caption"/>
	</xsl:call-template>		
	
</xsl:template>

<xsl:template name="involvedTypeListTable">
	<xsl:param name="rootMCR" />	
	<xsl:param name="involvedTypes" />
	<xsl:param name="dir" />
	<xsl:param name="caption" />

	<table>
		<caption><xsl:value-of select="$caption" /> (<xsl:value-of select="count( $involvedTypes )" />)</caption>
		<thead>
			<tr>
				<th>Namespace</th>
				<th>Name</th>
				<th># of Mixins applied</th>
				<th># of TargetClasses</th>
				<th>Assembly</th>
			</tr>
		</thead>
		<tfoot>
			<tr>
				<td><xsl:value-of select="count( distinct-values( $involvedTypes/@namespace ) )" /></td>
				<td><xsl:value-of select="count( $involvedTypes )" /></td>
				<td>-</td>
				<td>-</td>
				<td><xsl:value-of select="count( distinct-values( $involvedTypes/@assembly-ref ) )" /></td>
			</tr>
		</tfoot>
		<tbody>
			<xsl:for-each select="$involvedTypes">
				<tr>
					<td><xsl:value-of select="@namespace"/></td>
					<td>
						<xsl:call-template name="GenerateInvolvedTypeLink">
							<xsl:with-param name="rootMCR" select="$rootMCR" />
							<xsl:with-param name="involvedTypeId" select="@id" />
							<xsl:with-param name="dir" select="$dir" />
						</xsl:call-template>					
					</td>
					<td><xsl:value-of select="count( Mixins/Mixin )"/></td>
					<td><xsl:value-of select="count( Targets/Target )"/></td>
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