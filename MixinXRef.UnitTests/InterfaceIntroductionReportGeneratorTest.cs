using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InterfaceIntroductionReportGeneratorTest
  {
    [Test]
    public void GenerateXm_NoIntroducedInterfaces ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();
      var reportGenerator = new InterfaceIntroductionReportGenerator (typeof (TargetClass2), typeof(Mixin2), mixinConfiguration, new IdentifierGenerator<Type>());
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("InterfaceIntroductions");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithIntroducedInterfaces ()
    {
      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass2>().AddMixin<Mixin3>()
          .BuildConfiguration();

      // TargetClass2 does not implement any interface
      // Mixin3 introduces interface IDisposable
      var reportGenerator = new InterfaceIntroductionReportGenerator (typeof (TargetClass2), typeof(Mixin3), mixinConfiguration, interfaceIdentifierGenerator);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "InterfaceIntroductions",
          new XElement (
              "Interface",
              new XAttribute ("ref", "0")
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ForGenericTargetClass ()
    {
      var interfaceIdentifierGenerator = new IdentifierGenerator<Type> ();

      var mixinConfiguration = MixinConfiguration.ActiveConfiguration;

      var reportGenerator = new InterfaceIntroductionReportGenerator (typeof (GenericTarget<>), typeof (Mixin3), mixinConfiguration, interfaceIdentifierGenerator);
      
      var output = reportGenerator.GenerateXml ();

      Assert.That (output, Is.Null);
    } 
  }
}