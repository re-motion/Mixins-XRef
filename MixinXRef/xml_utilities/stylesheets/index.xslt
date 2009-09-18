<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns="http://www.w3.org/1999/xhtml" xmlns:ru="http://www.rubicon-it.com"
	exclude-result-prefixes="xs fn ru">
	
	
<xsl:template name="index">
	<h1>Mixin Documentation Summary </h1> <h2> @<xsl:value-of select="/MixinXRefReport/@creation-time" /></h2>
	
	<div class="message">
    <div>
      <label>Assemblies:</label>
      <xsl:value-of select="ru:GetOverallAssemblyCount(/)"/>
    </div>
    <div>
      <label>Mixins:</label>
      <xsl:value-of select="ru:GetOverallMixinCount(/)"/>
    </div>
    <div>
      <label class="dubiosInvolvedType" >Mixed Mixins:</label>
      <xsl:value-of select="count( /MixinXRefReport/InvolvedTypes/InvolvedType[@is-target = true() and @is-mixin = true()] )"/>
    </div>
    <div>
      <label class="unusedMixinClass" >Non Applied Mixins:</label>
      <xsl:value-of select="count( /MixinXRefReport/InvolvedTypes/InvolvedType[@is-target = false() and @is-mixin = false()] )"/>
    </div>
    <div>
      <label>Target Classes:</label>
      <xsl:value-of select="ru:GetOverallTargetClassCount(/)"/>
    </div>
    <div>
      <label>Involved Interfaces:</label>
      <xsl:value-of select="count( /MixinXRefReport/Interfaces/Interface )"/>
    </div>
    <div>
      <label>Involved Attributes:</label>
      <xsl:value-of select="count( /MixinXRefReport/Attributes/Attribute )"/>
    </div>
  </div>
	
	<p>Use one of the index pages to start browsing. Hold down [Shift] to sort multiple columns.</p>
	
	<xsl:call-template name="errorList"/>
	
	<p>
		<img src="http://www.w3.org/Icons/valid-xhtml10-blue" alt="Valid XHTML 1.0 Strict" height="31" width="88" />
	 </p>

</xsl:template>

</xsl:stylesheet>
