using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

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

      var type1 = new InvolvedType (typeof (TargetClass2));
      type1.ClassContext = mixinConfiguration.ClassContexts.First ();

      var interfaceIntroductions = GetInterfaceIntroductions (type1, typeof (Mixin2), mixinConfiguration);
      var reportGenerator = new InterfaceIntroductionReportGenerator (interfaceIntroductions, new IdentifierGenerator<Type> ());
      
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

      var type1 = new InvolvedType (typeof (TargetClass2));
      type1.ClassContext = mixinConfiguration.ClassContexts.First ();

      // TargetClass2 does not implement any interface
      // Mixin3 introduces interface IDisposable
      var interfaceIntroductions = GetInterfaceIntroductions (type1, typeof (Mixin3), mixinConfiguration);
      var reportGenerator = new InterfaceIntroductionReportGenerator (interfaceIntroductions, interfaceIdentifierGenerator);
      
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "InterfaceIntroductions",
          new XElement (
              "Interface",
              new XAttribute ("ref", "0")
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> GetInterfaceIntroductions (InvolvedType targetType, Type mixinType, MixinConfiguration mixinConfiguration)
    {
      var targetClassDefinition = targetType.GetTargetClassDefinition (mixinConfiguration);
      return targetClassDefinition.GetMixinByConfiguredType (mixinType).InterfaceIntroductions;

    }
  }
}