<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
	
<xsl:template name="index">
	<h1>Mixin Documentation Summary </h1> <h2> @<xsl:value-of select="/MixinXRefReport/@creation-time" /></h2>
	
	<p>
    <div>
      <label>Assemblies:</label>
      <xsl:value-of select="ru:GetOverallAssemblyCount(/)"/>
    </div>
    <div>
      <label>Target classes:</label>
      <xsl:value-of select="ru:GetOverallTargetClassCount(/)"/>
    </div>
    <div>
      <label>Mixins:</label>
      <xsl:value-of select="ru:GetOverallMixinCount(/)"/>
    </div>
  </p>
	
	<p>Use one of the index pages to start browsing. Hold down [Shift] to sort multiple columns.</p>
	
	<xsl:call-template name="errorList"/>

</xsl:template>

</xsl:stylesheet>
