using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class TargetCallDependenciesReportGeneratorTest
  {
    private IRemotionReflector _remotionReflector;
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = Helpers.RemotionReflectorFactory.GetRemotionReflection ();
      _outputFormatter = new OutputFormatter ();
    }

    [Test]
    public void GenerateXml_InterfaceImplementedOnTargetClass ()
    {
      var targetType = new InvolvedType (typeof (TargetClass1));

      var mixinConfiguration = MixinConfiguration.BuildNew ().ForClass<TargetClass1> ().AddMixin<Mixin4> ().BuildConfiguration ();
      targetType.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ());
      targetType.TargetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration));
      var mixinContext = targetType.ClassContext.GetProperty ("Mixins").First ();
      var mixinDefinition = targetType.TargetClassDefinition.CallMethod ("GetMixinByConfiguredType", mixinContext.GetProperty ("MixinType").To<Type> ());

      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly> ();

      var output = new TargetCallDependenciesReportGenerator (mixinDefinition, assemblyIdentifierGenerator,
                                                             _remotionReflector, _outputFormatter).GenerateXml ();

      var expectedOutput = new XElement ("TargetCallDependencies",
                                        new XElement ("Dependency",
                                                     new XAttribute ("assembly-ref", "0"),
                                                     new XAttribute ("namespace", "System"),
                                                     new XAttribute ("name", "IDisposable"),
                                                     new XAttribute ("is-interface", true),
                                                     new XAttribute ("is-implemented-by-target", true),
                                                     new XAttribute ("is-added-by-mixin", false),
                                                     new XAttribute ("is-implemented-dynamically", false)));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_InterfaceDynamicallyImplemented ()
    {
      var targetType = new InvolvedType (typeof (TargetClass3));

      var mixinConfiguration = MixinConfiguration.BuildNew ().ForClass<TargetClass3> ().AddMixin<Mixin4> ().BuildConfiguration ();
      targetType.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ());
      targetType.TargetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration));
      var mixinContext = targetType.ClassContext.GetProperty ("Mixins").First ();
      var mixinDefinition = targetType.TargetClassDefinition.CallMethod ("GetMixinByConfiguredType", mixinContext.GetProperty ("MixinType").To<Type> ());

      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly> ();

      var output = new TargetCallDependenciesReportGenerator (mixinDefinition, assemblyIdentifierGenerator,
                                                             _remotionReflector, _outputFormatter).GenerateXml ();

      var expectedOutput = new XElement ("TargetCallDependencies",
                                        new XElement ("Dependency",
                                                     new XAttribute ("assembly-ref", "0"),
                                                     new XAttribute ("namespace", "System"),
                                                     new XAttribute ("name", "IDisposable"),
                                                     new XAttribute ("is-interface", true),
                                                     new XAttribute ("is-implemented-by-target", false),
                                                     new XAttribute ("is-added-by-mixin", false),
                                                     new XAttribute ("is-implemented-dynamically", true)));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}
