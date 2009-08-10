using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AttributeIntroductionReportGeneratorTest
  {

    [Test]
    public void GenerateXml_NoIntroducedAttribute ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass2> ().AddMixin<Mixin1> ()
          .BuildConfiguration ();

      var type1 = new InvolvedType (typeof (TargetClass2));
      type1.ClassContext = mixinConfiguration.ClassContexts.First ();

      var reportGenerator = new AttributeIntroductionReportGenerator (type1, typeof (Mixin1), mixinConfiguration, new IdentifierGenerator<Type> ());
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("AttributeIntroductions");

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_WithIntroducedAttributes ()
    {
      var attributeIdentifierGenerator = new IdentifierGenerator<Type> ();
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<UselessObject> ().AddMixin<ObjectWithInheritableAttribute> ()
          .BuildConfiguration ();

      var type1 = new InvolvedType (typeof (UselessObject));
      type1.ClassContext = mixinConfiguration.ClassContexts.First ();

      var reportGenerator = new AttributeIntroductionReportGenerator (type1, typeof (ObjectWithInheritableAttribute), mixinConfiguration, attributeIdentifierGenerator);

      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "AttributeIntroductions",
          new XElement (
              "Attribute",
              new XAttribute ("ref", "0")
              ));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_ForGenericTargetClass ()
    {
      var attributeIdentifierGenerator = new IdentifierGenerator<Type> ();
      var mixinConfiguration = MixinConfiguration.ActiveConfiguration;
      var type1 = new InvolvedType (typeof (GenericTarget<>));

      var reportGenerator = new AttributeIntroductionReportGenerator (type1, typeof (ClassWithBookAttribute), mixinConfiguration, attributeIdentifierGenerator);

      var output = reportGenerator.GenerateXml ();

      Assert.That (output, Is.Null);
    } 
  }
}