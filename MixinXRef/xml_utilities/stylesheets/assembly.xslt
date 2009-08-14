<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
	
<xsl:function name="ru:GetInvolvedTypesForAssembly">
	<xsl:param name="rootMCR" />
	<xsl:param name="assemblyId" />
	<xsl:copy-of select="$rootMCR//InvolvedTypes/InvolvedType[@assembly-ref = $assemblyId]" />
</xsl:function>

<xsl:function name="ru:GetInterfacesForAssembly">
	<xsl:param name="rootMCR" />
	<xsl:param name="assemblyId" />
	<xsl:copy-of select="$rootMCR//Interfaces/Interface[@assembly-ref = $assemblyId]" />
</xsl:function>

<xsl:function name="ru:GetAttributesForAssembly">
	<xsl:param name="rootMCR" />
	<xsl:param name="assemblyId" />
	<xsl:copy-of select="$rootMCR//Attributes/Attribute[@assembly-ref = $assemblyId]" />
</xsl:function>


<xsl:template name="assembly">
	<table>
		<caption>Analyzed assemblies (<xsl:value-of select="ru:GetOverallAssemblyCountExclED(/)" />)</caption>
		<thead>
			<tr>
				<th>Name</th>
				<th>Version</th>
				<th># of Targets</th>
				<th># of Mixins</th>
				<th># of InvolvedTypes</th>
				<th>Location</th>
			</tr>
		</thead>
		<tfoot>
			<tr>
				<td><xsl:value-of select="ru:GetOverallAssemblyCountExclED(/)" /></td>
				<td>-</td>
				<td><xsl:value-of select="ru:GetOverallTargetClassCount(/)" /></td>
				<td><xsl:value-of select="ru:GetOverallMixinCount(/)" /></td>
				<td><xsl:value-of select="count( //InvolvedTypes/InvolvedType )" /></td>
				<td>-</td>
			</tr>
		</tfoot>
		<tbody>
			<xsl:for-each select="//Assemblies/Assembly">
				<tr>
					<td>
						<xsl:call-template name="GenerateAssemblyLink">
							<xsl:with-param name="rootMCR" select="/" />
							<xsl:with-param name="assemblyId" select="@id" />
							<xsl:with-param name="dir" >.</xsl:with-param>
						</xsl:call-template>
					</td>
					<td><xsl:value-of select="@version"/></td>
					<td><xsl:value-of select="count( ru:GetInvolvedTypesForAssembly(/, @id)[@is-target = true()] )"/></td>
					<td><xsl:value-of select="count( ru:GetInvolvedTypesForAssembly(/, @id)[@is-mixin = true()] )"/></td>
					<td><xsl:value-of select="count( ru:GetInvolvedTypesForAssembly(/, @id) )"/></td>
					<td><xsl:value-of select="@location"/></td>
				</tr>
				
				<!-- generate assembly detail site for current assembly -->
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
	<xsl:call-template name="involvedTypeList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="involvedTypes" select="ru:GetInvolvedTypesForAssembly(/, @id)" />  
		<xsl:with-param name="dir">..</xsl:with-param>
		<xsl:with-param name="caption">Involved Types</xsl:with-param>
	</xsl:call-template>
	
	<xsl:call-template name="interfaceList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="interfaces" select="ru:GetInterfacesForAssembly(/, @id)" /> 
		<xsl:with-param name="dir">..</xsl:with-param>
	</xsl:call-template>
	
		<xsl:call-template name="attributeList">
		<xsl:with-param name="rootMCR" select="/" />		
		<xsl:with-param name="attributes" select="ru:GetAttributesForAssembly(/, @id)" /> 
		<xsl:with-param name="dir">..</xsl:with-param>
	</xsl:call-template>
</xsl:template>

</xsl:stylesheet>