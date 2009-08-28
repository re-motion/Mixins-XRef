using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AttributeIntroductionReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoIntroducedAttribute ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass2>().AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (TargetClass2));
      type1.ClassContext = mixinConfiguration.ClassContexts.First();

      var attributeIntroductions = GetAttributeIntroductions (type1, typeof (Mixin1), mixinConfiguration);
      var reportGenerator = new AttributeIntroductionReportGenerator (attributeIntroductions, new IdentifierGenerator<Type>(), new RemotionReflection());
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("AttributeIntroductions");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithIntroducedAttributes ()
    {
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<ObjectWithInheritableAttribute>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (UselessObject));
      type1.ClassContext = mixinConfiguration.ClassContexts.First();

      var attributeIntroductions = GetAttributeIntroductions (type1, typeof (ObjectWithInheritableAttribute), mixinConfiguration);
      var reportGenerator = new AttributeIntroductionReportGenerator (attributeIntroductions, attributeIdentifierGenerator, new RemotionReflection());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "AttributeIntroductions",
          new XElement (
              "Attribute",
              new XAttribute ("ref", "0")
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private ReflectedObject GetAttributeIntroductions (
        InvolvedType targetType, Type mixinType, MixinConfiguration mixinConfiguration)
    {
      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration);
      return new ReflectedObject(targetClassDefinition.GetMixinByConfiguredType (mixinType).AttributeIntroductions);
    }
  }
}