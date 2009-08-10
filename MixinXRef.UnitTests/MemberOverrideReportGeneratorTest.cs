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
  public class MemberOverrideReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoOverriddenMembers ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (TargetClass1));
      type1.ClassContext = mixinConfiguration.ClassContexts.First();

      var reportGenerator = new MemberOverrideReportGenerator (type1, typeof (Mixin1), mixinConfiguration);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("MemberOverrides");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithOverriddenMembers ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetDoSomething>().AddMixin<MixinDoSomething>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (TargetDoSomething));
      type1.ClassContext = mixinConfiguration.ClassContexts.First();


      var reportGenerator = new MemberOverrideReportGenerator (type1, typeof (MixinDoSomething), mixinConfiguration);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "MemberOverrides",
          new XElement (
              "Member",
              new XAttribute ("type", "Method"),
              new XAttribute ("name", "DoSomething")
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ForGenericTargetClass ()
    {
      
      var mixinConfiguration = MixinConfiguration.ActiveConfiguration;
      var type1 = new InvolvedType (typeof (GenericTarget<>));

      var reportGenerator = new MemberOverrideReportGenerator (type1, typeof (Mixin3), mixinConfiguration);

      var output = reportGenerator.GenerateXml ();

      Assert.That (output, Is.Null);
    }
  }
}