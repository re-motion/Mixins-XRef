using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using System.Xml.XPath;

namespace MixinXRef.UnitTests.Report
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
              new XAttribute ("is-declared-by-this-class", true),
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
              new XAttribute ("is-declared-by-this-class", true),
              _outputFormatter.CreateModifierMarkup ("", "public"),
              _outputFormatter.CreateConstructorMarkup ("UselessObject", new ParameterInfo[0])
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_PropertyWithoutGetAndSet_Overridden ()
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
              new XAttribute ("is-declared-by-this-class", true),
              _outputFormatter.CreateModifierMarkup ("", "public override"),
              _outputFormatter.CreateMethodMarkup ("DoSomething", typeof (void), new ParameterInfo[0])
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              new XAttribute ("is-declared-by-this-class", true),
              _outputFormatter.CreateModifierMarkup ("", "public"),
              _outputFormatter.CreateConstructorMarkup ("ClassWithProperty", new ParameterInfo[0])
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Property),
              new XAttribute ("name", "PropertyName"),
              new XAttribute ("is-declared-by-this-class", true),
              _outputFormatter.CreateModifierMarkup ("", "public override"),
              _outputFormatter.CreatePropertyMarkup ("PropertyName", typeof (string))
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_TargetClassWithOverriddenBaseClassMember ()
    {
      var type = typeof (InheritatedTargetClass);
      var mixinConfiguration =
          MixinConfiguration.BuildNew ().ForClass<InheritatedTargetClass> ().AddMixin<MixinOverridesTargetClassMember> ().BuildConfiguration ();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (type, mixinConfiguration));
      var involvedType = new InvolvedType (type);
      involvedType.TargetClassDefintion = targetClassDefinition;
      var memberInfo = type.GetMember ("MyBaseClassMethod")[0];

      var reportGenerator = CreateMemberReportGenerator (type, involvedType);

      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "Members",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "MyNewMethod"),
              new XAttribute ("is-declared-by-this-class", true),
              _outputFormatter.CreateModifierMarkup ("", "public virtual"),
              _outputFormatter.CreateMethodMarkup ("MyNewMethod", typeof (void), new ParameterInfo[0]),
              new XElement ("Overrides")
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "MyBaseClassMethod"),
              new XAttribute ("is-declared-by-this-class", false),
              _outputFormatter.CreateModifierMarkup ("", "public override"),
              _outputFormatter.CreateMethodMarkup ("MyBaseClassMethod", typeof (void), new ParameterInfo[0]),
              GenerateOverrides("0", "MixinOverridesTargetClassMember")
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              new XAttribute ("is-declared-by-this-class", true),
              _outputFormatter.CreateModifierMarkup ("", "public"),
              _outputFormatter.CreateConstructorMarkup ("InheritatedTargetClass", new ParameterInfo[0]),
              new XElement ("Overrides")
              )
          );

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    private XElement GenerateOverrides (string referenceID, string instanceName)
    {
      return new XElement ("Overrides", new XElement ("Mixin-Reference", new XAttribute("ref", referenceID), new XAttribute("instance-name", instanceName)));
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

      var reportGenerator = CreateMemberReportGenerator (type, involvedType);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "Members",
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "TemplateMethod"),
              new XAttribute ("is-declared-by-this-class", true),
              _outputFormatter.CreateModifierMarkup ("OverrideMixin ", "public"),
              _outputFormatter.CreateMethodMarkup ("TemplateMethod", typeof (void), new ParameterInfo[0]),
              new XElement ("Overrides")
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Method),
              new XAttribute ("name", "OverriddenMethod"),
              new XAttribute ("is-declared-by-this-class", true),
              _outputFormatter.CreateModifierMarkup ("", "public virtual"),
              _outputFormatter.CreateMethodMarkup ("OverriddenMethod", typeof (void), new ParameterInfo[0]),
              GenerateOverrides ("0", "MemberOverrideTestClass.Mixin1")
              ),
          new XElement (
              "Member",
              new XAttribute ("type", MemberTypes.Constructor),
              new XAttribute ("name", ".ctor"),
              new XAttribute ("is-declared-by-this-class", true),
              _outputFormatter.CreateModifierMarkup ("", "public"),
              _outputFormatter.CreateConstructorMarkup ("MemberOverrideTestClass.Target", new ParameterInfo[0]),
              new XElement ("Overrides")
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void HasOverrideMixinAttribute_False ()
    {
      var reportGenerator = CreateMemberReportGenerator (typeof (object), new InvolvedType (typeof (object)));
      var output = reportGenerator.GenerateXml ().XPathSelectElement ("Member[@name='ToString']").Element ("Modifiers").Element ("Type");

      Assert.That (output, Is.Null);
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

      var output = reportGenerator.GenerateXml ().XPathSelectElement ("Member[@name='TemplateMethod']").Element ("Modifiers").Element ("Type");

      Assert.That (output.Value, Is.EqualTo ("OverrideMixin"));
    }

    [Test]
    public void HasOverrideTargetAttribute_False ()
    {
      var reportGenerator = CreateMemberReportGenerator (typeof (object), new InvolvedType (typeof (object)));
      var output = reportGenerator.GenerateXml ().XPathSelectElement ("Member[@name='ToString']").Element ("Modifiers").Element ("Type");

      Assert.That (output, Is.Null);
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
      
      var output = reportGenerator.GenerateXml ().XPathSelectElement ("Member[@name='OverriddenMethod']").Element ("Modifiers").Element ("Type");

      Assert.That (output.Value, Is.EqualTo ("OverrideTarget"));
    }

    [Test]
    public void name ()
    {
      var x = new XElement ("a");
      var y = new XElement ("a");

      Assert.That (x.Value, Is.EqualTo (y.Value));
    }

    [Test]
    public void GetOverrides_NoOverrides ()
    {
      var targetType = typeof (TargetClass1);
      var mixinConfiguration =
          MixinConfiguration.BuildNew()
              .ForClass<TargetClass1>().AddMixin<Mixin1>()
              .BuildConfiguration();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
                         {
                             TargetClassDefintion = targetClassDefinition,
                             ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First())
                         };

      var reportGenerator = CreateMemberReportGenerator (targetType, involvedType);

      //var memberInfo = targetType.GetMember ("Dispose")[0];
      var output = reportGenerator.GenerateXml();
      
      Assert.That (output.XPathSelectElement ("Member[@name='Dispose']").Element("Overrides").HasElements, Is.False);
    }

    [Test]
    public void GetOverrides_WithOverrides ()
    {
      var targetType = typeof (MemberOverrideTestClass.Target);
      var mixinConfiguration =
          MixinConfiguration.BuildNew()
              .ForClass<MemberOverrideTestClass.Target>().AddMixin<MemberOverrideTestClass.Mixin1>()
              .BuildConfiguration();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
                         {
                             TargetClassDefintion = targetClassDefinition,
                             ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First())
                         };

      var reportGenerator = CreateMemberReportGenerator (targetType, involvedType);

      //var memberInfo = targetType.GetMember ("OverriddenMethod")[0];
      //var output = reportGenerator.GetOverrides (memberInfo);

      var output = reportGenerator.GenerateXml();
      
      var expectedOutput =
          new XElement (
              "Overrides",
              new XElement (
                  "Mixin-Reference",
                  new XAttribute ("ref", 0),
                  new XAttribute ("instance-name", "MemberOverrideTestClass.Mixin1")
                  ));

      Assert.That (output.XPathSelectElement ("Member[@name='OverriddenMethod']").Element ("Overrides").ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GetOverrides_WithOverrides_ForMemberInBaseClass ()
    {
      var targetType = typeof (BaseMemberOverrideTestClass.Target);
      var mixinConfiguration =
          MixinConfiguration.BuildNew ()
              .ForClass<BaseMemberOverrideTestClass.Target> ().AddMixin<BaseMemberOverrideTestClass.Mixin1> ()
              .BuildConfiguration ();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
      {
        TargetClassDefintion = targetClassDefinition,
        ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ())
      };

      var reportGenerator = CreateMemberReportGenerator (targetType, involvedType);

      //var memberInfo = targetType.GetMember ("OverriddenMethod")[0];
      var output = reportGenerator.GenerateXml();
      var expectedOutput =
          new XElement (
              "Overrides",
              new XElement (
                  "Mixin-Reference",
                  new XAttribute ("ref", 0),
                  new XAttribute ("instance-name", "BaseMemberOverrideTestClass.Mixin1")
                  ));

      Assert.That (output.XPathSelectElement ("Member[@name='OverriddenMethod']").Element ("Overrides").ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

     [Test]
    public void GetOverrides_WithoutOverrides_ForMemberHiddenByDerivedClass ()
    {
      var targetType = typeof (HiddenMemberTestClass.Target);
      var mixinConfiguration =
          MixinConfiguration.BuildNew ()
              .ForClass<HiddenMemberTestClass.Target> ().AddMixin<HiddenMemberTestClass.Mixin1> ()
              .BuildConfiguration ();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
      {
        TargetClassDefintion = targetClassDefinition,
        ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ())
      };

      var reportGenerator = CreateMemberReportGenerator (targetType, involvedType);
      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("Overrides");

      Assert.That (output.XPathSelectElement ("Member[@name='HiddenMethod']").Element ("Overrides").HasElements, Is.False);
    }

    private MemberReportGenerator CreateMemberReportGenerator (Type mixinType, InvolvedType involvedType)
    {
      return new MemberReportGenerator (mixinType, involvedType, new IdentifierGenerator<Type>(), _outputFormatter);
    }
  }
}