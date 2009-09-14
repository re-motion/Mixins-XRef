using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AdditionalDependencyReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoDependencies ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<AdditionalDependenciesTest.TargetClass>()
          .AddMixin<AdditionalDependenciesTest.Mixin1>()
          .AddMixin<AdditionalDependenciesTest.Mixin2>()
          .AddMixin<AdditionalDependenciesTest.Mixin3>()
          .WithDependencies<AdditionalDependenciesTest.Mixin1, AdditionalDependenciesTest.Mixin2>()
          .BuildConfiguration();

      // Mixin1 has no depencies
      var explicitDependencies = mixinConfiguration.ClassContexts.Single().Mixins.First().ExplicitDependencies;

      var dependencies = new ReflectedObject (explicitDependencies);
      var output = new AdditionalDependencyReportGenerator (dependencies).GenerateXml();
      var expectedOutput = new XElement ("AdditionalDependencies");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}