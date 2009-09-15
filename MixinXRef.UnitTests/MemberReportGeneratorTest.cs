using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class MemberReportGeneratorTest
  {
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _outputFormatter = new OutputFormatter();
    }

    [Test]
    public void GenerateXml_InterfaceWithZeroMembers ()
    {
      var reportGenerator = CreateMemberReportGenerator (typeof (IUseless), null);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("Members");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_InterfaceWithMembers ()
    {
      var reportGenerator = CreateMemberReportGenerator (typeof (IDisposable), null);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "Members",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "Dispose"),
              _outputFormatter.CreateModifierMarkup ("", "public abstract"),
              _outputFormatter.CreateMethodMarkup ("Dispose", typeof (void), new ParameterInfo[0])
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ObjectWithoutOwnMembers ()
    {
      var reportGenerator = CreateMemberReportGenerator (typeof (UselessObject), new InvolvedType (typeof (UselessObject)));

      var output = reportGenerator.GenerateXml();

      // enhancement: surpress output of default constructor if generated by compiler
      var expectedOutput = new XElement (
          "Members",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              _outputFormatter.CreateModifierMarkup ("", "public"),
              _outputFormatter.CreateConstructorMarkup ("UselessObject", new ParameterInfo[0])
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_PropertyWithoutGetAndSet_Overriden ()
    {
      var reportGenerator = CreateMemberReportGenerator (typeof (ClassWithProperty), new InvolvedType (typeof (ClassWithProperty)));

      var output = reportGenerator.GenerateXml();

      // MemberReportGenerator removes get_* and set_* functions of properties
      var expectedOutput = new XElement (
          "Members",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "DoSomething"),
              _outputFormatter.CreateModifierMarkup ("", "public override"),
              _outputFormatter.CreateMethodMarkup ("DoSomething", typeof (void), new ParameterInfo[0])
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              _outputFormatter.CreateModifierMarkup ("", "public"),
              _outputFormatter.CreateConstructorMarkup ("ClassWithProperty", new ParameterInfo[0])
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Property),
              new XAttribute ("name", "PropertyName"),
              _outputFormatter.CreateModifierMarkup ("", "public override"),
              _outputFormatter.CreatePropertyMarkup ("PropertyName", typeof (string))
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml ()
    {
      var type = typeof (MemberOverrideTestClass.Target);
      var mixinConfiguration =
          MixinConfiguration.BuildNew().ForClass<MemberOverrideTestClass.Target>().AddMixin<MemberOverrideTestClass.Mixin1>().BuildConfiguration();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (type, mixinConfiguration));
      var involvedType = new InvolvedType (type);
      involvedType.TargetClassDefintion = targetClassDefinition;
      var memberInfo = type.GetMember ("OverriddenMethod")[0];

      var reportGenerator = CreateMemberReportGenerator (type, involvedType);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "Members",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "TemplateMethod"),
              _outputFormatter.CreateModifierMarkup ("OverrideMixin ", "public"),
              _outputFormatter.CreateMethodMarkup ("TemplateMethod", typeof (void), new ParameterInfo[0]),
              new XElement("Overrides")
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "OverriddenMethod"),
              _outputFormatter.CreateModifierMarkup ("", "public virtual"),
              _outputFormatter.CreateMethodMarkup ("OverriddenMethod", typeof (void), new ParameterInfo[0]),
              reportGenerator.GetOverrides(memberInfo)
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              _outputFormatter.CreateModifierMarkup ("", "public"),
              _outputFormatter.CreateConstructorMarkup ("Target", new ParameterInfo[0]),
              new XElement ("Overrides")
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void HasOverrideMixinAttribute_False ()
    {
      var reportGenerator = CreateMemberReportGenerator (typeof (object), new InvolvedType (typeof (object)));
      var memberInfo = typeof (object).GetMember ("ToString")[0];
      var output = reportGenerator.HasOverrideMixinAttribute (memberInfo);

      Assert.That (output, Is.False);
    }

    [Test]
    public void HasOverrideMixinAttribute_True ()
    {
      var type = typeof (MemberOverrideTestClass.Target);
      var mixinConfiguration =
          MixinConfiguration.BuildNew().ForClass<MemberOverrideTestClass.Target>().AddMixin<MemberOverrideTestClass.Mixin1>().BuildConfiguration();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (type, mixinConfiguration));
      var involvedType = new InvolvedType (type) { TargetClassDefintion = targetClassDefinition };

      var reportGenerator = CreateMemberReportGenerator (type, involvedType);

      var memberInfo = type.GetMember ("TemplateMethod")[0];
      var output = reportGenerator.HasOverrideMixinAttribute (memberInfo);

      Assert.That (output, Is.True);
    }

    [Test]
    public void HasOverrideTargetAttribute_False ()
    {
      var reportGenerator = CreateMemberReportGenerator (typeof (object), new InvolvedType (typeof (object)));
      var memberInfo = typeof (object).GetMember ("ToString")[0];
      var output = reportGenerator.HasOverrideTargetAttribute (memberInfo);

      Assert.That (output, Is.False);
    }

    [Test]
    public void HasOverrideTargetAttribute_True ()
    {
      var mixinType = typeof (MemberOverrideTestClass.Mixin1);
      var targetType = typeof (MemberOverrideTestClass.Target);
      var mixinConfiguration =
          MixinConfiguration.BuildNew()
              .ForClass<MemberOverrideTestClass.Target>().AddMixin<MemberOverrideTestClass.Mixin1>()
              .BuildConfiguration();
      var involvedType = new InvolvedType (mixinType);
      involvedType.TargetTypes.Add (
          targetType, new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration).Mixins[mixinType]));
      var reportGenerator = CreateMemberReportGenerator (mixinType, involvedType);

      var memberInfo = mixinType.GetMember ("OverriddenMethod")[0];
      var output = reportGenerator.HasOverrideTargetAttribute (memberInfo);

      Assert.That (output, Is.True);
    }

    [Test]
    public void GetOverrides_False ()
    {
      var reportGenerator = CreateMemberReportGenerator (typeof (object), null);
      var memberInfo = typeof (object).GetMember ("ToString")[0];

      var output = reportGenerator.GetOverrides (memberInfo);

      Assert.That (output, Is.Null);
    }

    [Test]
    public void GetOverrides_ForTargetTrue ()
    {
      var mixinType = typeof (MemberOverrideTestClass.Mixin1);
      var targetType = typeof (MemberOverrideTestClass.Target);
      var mixinConfiguration =
          MixinConfiguration.BuildNew().ForClass<MemberOverrideTestClass.Target>().AddMixin<MemberOverrideTestClass.Mixin1>().BuildConfiguration();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
                         {
                             TargetClassDefintion = targetClassDefinition,
                             ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First())
                         };

      var reportGenerator = CreateMemberReportGenerator (mixinType, involvedType);

      var memberInfo = mixinType.GetMember ("OverriddenMethod")[0];
      var output = reportGenerator.GetOverrides (memberInfo);
      var expectedOutput =
          new XElement (
              "Overrides",
              new XElement (
                  "Mixin",
                  new XAttribute ("ref", 0),
                  new XAttribute ("instance-name", "MemberOverrideTestClass.Mixin1")
                  ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private MemberReportGenerator CreateMemberReportGenerator (Type mixinType, InvolvedType involvedType)
    {
      return new MemberReportGenerator (mixinType, involvedType, new IdentifierGenerator<Type>(), _outputFormatter);
    }
  }
}