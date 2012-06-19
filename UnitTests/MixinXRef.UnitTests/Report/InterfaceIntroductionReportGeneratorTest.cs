using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Report
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
      type1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

      var interfaceIntroductions = GetInterfaceIntroductions (type1, typeof (Mixin2), mixinConfiguration);
      var reportGenerator = new InterfaceIntroductionReportGenerator (interfaceIntroductions, new IdentifierGenerator<Type>());

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
      type1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

      // TargetClass2 does not implement any interface
      // Mixin3 introduces interface IDisposable
      var interfaceIntroductions = GetInterfaceIntroductions (type1, typeof (Mixin3), mixinConfiguration);
      var reportGenerator = new InterfaceIntroductionReportGenerator (interfaceIntroductions, interfaceIdentifierGenerator);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "InterfaceIntroductions",
          new XElement (
              "IntroducedInterface",
              new XAttribute ("ref", "0")
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private ReflectedObject GetInterfaceIntroductions (InvolvedType targetType, Type mixinType, MixinConfiguration mixinConfiguration)
    {
      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration);
      return new ReflectedObject (targetClassDefinition.GetMixinByConfiguredType (mixinType).InterfaceIntroductions);
    }
  }
}