using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InvolvedTypeReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoInvolvedTypes ()
    {
      var reportGenerator = CreateReportGenerator (new Assembly[0], new IdentifierGenerator<Type>(), new IdentifierGenerator<Type> (), new IdentifierGenerator<Type> ());
      XElement output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("InvolvedTypes");
      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_InvolvedTypes ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var involvedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      involvedType1.ClassContext = mixinConfiguration.ClassContexts.First();
      var involvedType2 = new InvolvedType (typeof (TargetClass2), true, false);
      involvedType2.ClassContext = mixinConfiguration.ClassContexts.Last();
      var involvedType3 = new InvolvedType (typeof (Mixin1), false, true);

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();
      var involvedTypeIdentifierGenerator = new IdentifierGenerator<Type>();

      InvolvedTypeReportGenerator reportGenerator = CreateReportGenerator (
          new Assembly[0], involvedTypeIdentifierGenerator, interfaceIdentifierGenerator, attributeIdentifierGenerator, involvedType1, involvedType2, involvedType3);

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
              new MemberReportGenerator (involvedType1.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType1.Type, interfaceIdentifierGenerator),
              new AttributeReferenceReportGenerator (involvedType1.Type, attributeIdentifierGenerator),
              new MixinReferenceReportGenerator (involvedType1, involvedTypeIdentifierGenerator)
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass2"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new MemberReportGenerator (involvedType2.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType2.Type, interfaceIdentifierGenerator),
              new AttributeReferenceReportGenerator (involvedType2.Type, attributeIdentifierGenerator),
              new MixinReferenceReportGenerator (involvedType2, involvedTypeIdentifierGenerator)
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "2"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "Mixin1"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", true),
              new MemberReportGenerator (involvedType3.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType3.Type, interfaceIdentifierGenerator),
              new AttributeReferenceReportGenerator (involvedType3.Type, attributeIdentifierGenerator),
              new MixinReferenceReportGenerator (involvedType3, involvedTypeIdentifierGenerator)
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_DifferentAssemblies ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1), true, false);
      var involvedType2 = new InvolvedType (typeof (object), false, false);

      var involvedTypeIdentifierGenerator = new IdentifierGenerator<Type>();
      var interfaceIdentifierGenerator = new IdentifierGenerator<Type> ();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type> ();

      InvolvedTypeReportGenerator reportGenerator =
          CreateReportGenerator (
              new[] { typeof (IdentifierGenerator<>).Assembly, typeof (InvolvedTypeReportGeneratorTest).Assembly, typeof (object).Assembly },
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
              new MemberReportGenerator (involvedType1.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType1.Type, interfaceIdentifierGenerator),
              new AttributeReferenceReportGenerator (involvedType1.Type, attributeIdentifierGenerator),
              new MixinReferenceReportGenerator (involvedType1, involvedTypeIdentifierGenerator)
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "2"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "Object"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", false),
              new MemberReportGenerator (involvedType2.Type).GenerateXml(),
              new InterfaceReferenceReportGenerator (involvedType2.Type, interfaceIdentifierGenerator),
              new AttributeReferenceReportGenerator (involvedType2.Type, attributeIdentifierGenerator),
              new MixinReferenceReportGenerator (involvedType2, involvedTypeIdentifierGenerator)
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private InvolvedTypeReportGenerator CreateReportGenerator (
        Assembly[] referencedAssemblies,
        IdentifierGenerator<Type> involvedTypeIdentifier,
        IdentifierGenerator<Type> interfaceIdentifierGenerator,
        IdentifierGenerator<Type> attributeIdentifierGenerator,
        params InvolvedType[] involvedTypes)
    {
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();

      foreach (var referencedAssembly in referencedAssemblies)
        assemblyIdentifierGenerator.GetIdentifier (referencedAssembly);

      return new InvolvedTypeReportGenerator (
          involvedTypes,
          assemblyIdentifierGenerator,
          involvedTypeIdentifier,
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator
          );
    }
  }
}