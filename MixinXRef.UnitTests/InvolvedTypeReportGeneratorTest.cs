using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InvolvedTypeReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoInvolvedTypes ()
    {
      var reportGenerator = CreateReportGenerator (
          new Assembly[0], new MixinConfiguration(), new IdentifierGenerator<Type>(), new IdentifierGenerator<Type>(), new IdentifierGenerator<Type>());
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("InvolvedTypes");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ForGenericTypeDefinition ()
    {
      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();
      var involvedTypeIdentifierGenerator = new IdentifierGenerator<Type>();

      var mixinConfiguration = new MixinConfiguration();

      var involvedType1 = new InvolvedType (typeof (GenericTarget<>), false);

      var reportGenerator = CreateReportGenerator (
          new Assembly[0], mixinConfiguration, 
          involvedTypeIdentifierGenerator,
          interfaceIdentifierGenerator, 
          attributeIdentifierGenerator,
          involvedType1);

      XElement output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "GenericTarget`1"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", true),
              new MemberReportGenerator (involvedType1.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType1.Type, interfaceIdentifierGenerator).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType1.Type, attributeIdentifierGenerator).GenerateXml(),
              new MixinReferenceReportGenerator (involvedType1, mixinConfiguration, involvedTypeIdentifierGenerator, interfaceIdentifierGenerator).
                  GenerateXml()
          ));
      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_InvolvedTypes ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var involvedType1 = new InvolvedType (typeof (TargetClass1), false);
      involvedType1.ClassContext = mixinConfiguration.ClassContexts.First();
      var involvedType2 = new InvolvedType (typeof (TargetClass2), false);
      involvedType2.ClassContext = mixinConfiguration.ClassContexts.Last();
      var involvedType3 = new InvolvedType (typeof (Mixin1), true);
      var involvedType4 = new InvolvedType (typeof (Mixin2), true);

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();
      var involvedTypeIdentifierGenerator = new IdentifierGenerator<Type>();

      InvolvedTypeReportGenerator reportGenerator = CreateReportGenerator (
          new Assembly[0],
          mixinConfiguration,
          involvedTypeIdentifierGenerator,
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          involvedType1,
          involvedType2,
          involvedType3,
          involvedType4);

      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass1"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", false),
              new MemberReportGenerator (involvedType1.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType1.Type, interfaceIdentifierGenerator).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType1.Type, attributeIdentifierGenerator).GenerateXml(),
              new MixinReferenceReportGenerator (involvedType1, mixinConfiguration, involvedTypeIdentifierGenerator, interfaceIdentifierGenerator).
                  GenerateXml()),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "2"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass2"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", false),
              new MemberReportGenerator (involvedType2.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType2.Type, interfaceIdentifierGenerator).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType2.Type, attributeIdentifierGenerator).GenerateXml(),
              new MixinReferenceReportGenerator (involvedType2, mixinConfiguration, involvedTypeIdentifierGenerator, interfaceIdentifierGenerator).
                  GenerateXml()),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "Mixin1"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", true),
              new XAttribute ("is-generic-definition", false),
              new MemberReportGenerator (involvedType3.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType3.Type, interfaceIdentifierGenerator).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType3.Type, attributeIdentifierGenerator).GenerateXml(),
              new MixinReferenceReportGenerator (involvedType3, mixinConfiguration, involvedTypeIdentifierGenerator, interfaceIdentifierGenerator).
                  GenerateXml()),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "3"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "Mixin2"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", true),
              new XAttribute ("is-generic-definition", false),
              new MemberReportGenerator (involvedType4.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType4.Type, interfaceIdentifierGenerator).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType4.Type, attributeIdentifierGenerator).GenerateXml(),
              new MixinReferenceReportGenerator (involvedType4, mixinConfiguration, involvedTypeIdentifierGenerator, interfaceIdentifierGenerator).
                  GenerateXml())
          );


      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_DifferentAssemblies ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1), false);
      involvedType1.ClassContext = new ClassContext (typeof (TargetClass1));
      var involvedType2 = new InvolvedType (typeof (object), false);

      var involvedTypeIdentifierGenerator = new IdentifierGenerator<Type>();
      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();

      var mixinConfiguration = new MixinConfiguration();

      InvolvedTypeReportGenerator reportGenerator =
          CreateReportGenerator (
              new[] { typeof (InvolvedType).Assembly, typeof (InvolvedTypeReportGeneratorTest).Assembly, typeof (object).Assembly },
              mixinConfiguration,
              involvedTypeIdentifierGenerator,
              interfaceIdentifierGenerator,
              attributeIdentifierGenerator,
              involvedType1,
              involvedType2);

      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "1"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass1"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", false),
              new MemberReportGenerator (involvedType1.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType1.Type, interfaceIdentifierGenerator).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType1.Type, attributeIdentifierGenerator).GenerateXml(),
              new MixinReferenceReportGenerator (involvedType1, mixinConfiguration, involvedTypeIdentifierGenerator, interfaceIdentifierGenerator).
                  GenerateXml()
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "2"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "Object"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-generic-definition", false),
              new MemberReportGenerator (involvedType2.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType2.Type, interfaceIdentifierGenerator).GenerateXml(),
              new AttributeReferenceReportGenerator (involvedType2.Type, attributeIdentifierGenerator).GenerateXml(),
              new MixinReferenceReportGenerator (involvedType2, mixinConfiguration, involvedTypeIdentifierGenerator, interfaceIdentifierGenerator).
                  GenerateXml()
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private InvolvedTypeReportGenerator CreateReportGenerator (
        Assembly[] referencedAssemblies,
        MixinConfiguration mixinConfiguration,
        IIdentifierGenerator<Type> involvedTypeIdentifier,
        IIdentifierGenerator<Type> interfaceIdentifierGenerator,
        IIdentifierGenerator<Type> attributeIdentifierGenerator,
        params InvolvedType[] involvedTypes)
    {
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();

      foreach (var referencedAssembly in referencedAssemblies)
        assemblyIdentifierGenerator.GetIdentifier (referencedAssembly);

      return new InvolvedTypeReportGenerator (
          involvedTypes,
          mixinConfiguration,
          assemblyIdentifierGenerator,
          involvedTypeIdentifier,
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator
          );
    }
  }
}