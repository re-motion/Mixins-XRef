<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
<xsl:template name="assembly">
	<table>
		<thead>
			<tr>
				<th>Name</th>
				<th>Version</th>
				<th># of Targets</th>
				<th># of Mixins</th>
				<th># of InvolvedTypes</th>
			</tr>
		</thead>
		<tfoot>
			<tr>
				<td><xsl:value-of select="count( //Assemblies/Assembly ) - 1" /> (excl. External Dependencies)</td>
				<td>-</td>
				<td><xsl:value-of select="ru:GetOverallTargetClassCount(/)" /></td>
				<td><xsl:value-of select="ru:GetOverallMixinCount(/)" /></td>
				<td><xsl:value-of select="count( //InvolvedTypes/InvolvedType )" /></td>
			</tr>
		</tfoot>
		<tbody>
			<xsl:for-each select="//Assemblies/Assembly">
				<tr>
					<td><a href="assemblies/{@id}.html"><xsl:value-of select="@name"/></a></td>
					<td><xsl:value-of select="@version"/></td>
					<td><xsl:value-of select="count( //InvolvedTypes/InvolvedType[@assembly-ref = current()/@id and @is-target = true()] )"/></td>
					<td><xsl:value-of select="count( //InvolvedTypes/InvolvedType[@assembly-ref = current()/@id and @is-mixin = true()] )"/></td>
					<td><xsl:value-of select="count( //InvolvedTypes/InvolvedType[@assembly-ref = current()/@id] )"/></td>
				</tr>
				
				<xsl:call-template name="assemblyDetailSite" />
			</xsl:for-each>
		</tbody>
	</table>
</xsl:template>

<xsl:template name="assemblyDetailSite">
	<xsl:call-template name="htmlSite">
			<xsl:with-param name="siteTitle">Assembly Detail</xsl:with-param>
			<xsl:with-param name="siteFileName">assemblies/<xsl:value-of select="@id"/>.html</xsl:with-param>
			<xsl:with-param name="bodyContentTemplate">assemblyDetail</xsl:with-param>
	</xsl:call-template>
</xsl:template>

<xsl:template name="assemblyDetail">
</xsl:template>

</xsl:stylesheet>