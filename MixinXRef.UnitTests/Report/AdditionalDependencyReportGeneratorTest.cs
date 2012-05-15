using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class AdditionalDependencyReportGeneratorTest
  {
    private MixinConfiguration _mixinConfiguration;
    private IOutputFormatter _outputFormatter;
    private IIdentifierGenerator<Type> _identifierGenerator;

    [SetUp]
    public void SetUp ()
    {
      _mixinConfiguration = MixinConfiguration.BuildNew().ForClass<AdditionalDependenciesTest.TargetClass>()
          .AddMixin<AdditionalDependenciesTest.Mixin1>()
          .AddMixin<AdditionalDependenciesTest.Mixin2>()
          .AddMixin<AdditionalDependenciesTest.Mixin3>()
          .WithDependencies<AdditionalDependenciesTest.Mixin1, AdditionalDependenciesTest.Mixin2>()
          .BuildConfiguration();
      _outputFormatter = new OutputFormatter();
      _identifierGenerator = new IdentifierGenerator<Type>();
    }

    [Test]
    public void GenerateXml_NoDependencies ()
    {
      // Mixin1 has no depencies
      var explicitDependencies = _mixinConfiguration.ClassContexts.Single().Mixins.First().ExplicitDependencies;

      var dependencies = new ReflectedObject (explicitDependencies);
      var output = new AdditionalDependencyReportGenerator (dependencies, _identifierGenerator, _outputFormatter).GenerateXml();
      var expectedOutput = new XElement ("AdditionalDependencies");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithDependencies ()
    {
      var explicitDependencies = _mixinConfiguration.ClassContexts.Single().Mixins.Last().ExplicitDependencies;

      var dependencies = new ReflectedObject (explicitDependencies);
      var output = new AdditionalDependencyReportGenerator (dependencies, _identifierGenerator, _outputFormatter).GenerateXml ();
      var expectedOutput = new XElement (
          "AdditionalDependencies",
          new XElement (
              "AdditionalDependency",
              new XAttribute ("ref", "0"),
              new XAttribute ("instance-name", "AdditionalDependenciesTest.Mixin1")),
          new XElement (
              "AdditionalDependency",
              new XAttribute ("ref", "1"),
              new XAttribute ("instance-name", "AdditionalDependenciesTest.Mixin2"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}