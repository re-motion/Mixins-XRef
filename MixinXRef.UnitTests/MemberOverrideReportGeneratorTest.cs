using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

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

      var memberOverrides = GetMemberOverrides (type1, typeof (Mixin1), mixinConfiguration);

      var reportGenerator = new MemberOverrideReportGenerator (memberOverrides);

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

      var memberOverrides = GetMemberOverrides (type1, typeof (MixinDoSomething), mixinConfiguration);

      var reportGenerator = new MemberOverrideReportGenerator (memberOverrides);

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

    private IEnumerable<MemberDefinitionBase> GetMemberOverrides (InvolvedType targetType, Type mixinType, MixinConfiguration mixinConfiguration)
    {
      var targetClassDefinition = targetType.GetTargetClassDefinition (mixinConfiguration);
      return targetClassDefinition.GetMixinByConfiguredType (mixinType).GetAllOverrides ();

    }
  }
}