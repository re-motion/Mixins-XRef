using System;
using System.Xml.Linq;
using MixinXRef.Reflection;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class InterfaceReferenceReportGeneratorTest
  {
    [Test]
    public void GenerateXml_ZeroInterfaces ()
    {
      var involvedType = new InvolvedType (typeof (object));
      var reportGenerator = new InterfaceReferenceReportGenerator (involvedType, new IdentifierGenerator<Type>(), ProgramTest.GetRemotionReflection());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("Interfaces");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithInterfaces ()
    {
      // TargetClass1 implements IDisposealbe
      var involvedType = new InvolvedType (typeof (TargetClass1));
      var reportGenerator = new InterfaceReferenceReportGenerator (involvedType, new IdentifierGenerator<Type>(), ProgramTest.GetRemotionReflection());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Interfaces",
          new XElement ("Interface", new XAttribute ("ref", "0"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithCompleteInterface ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<CompleteInterfacesTestClass.MyMixinTarget>()
          .AddCompleteInterface<CompleteInterfacesTestClass.ICMyMixinTargetMyMixin>()
          .AddMixin<CompleteInterfacesTestClass.MyMixin>()
          .BuildConfiguration();

      // MyMixinTarget does not implement any interfaces! (but ICMyMixinTargetMyMixin is added to class context as a complete interface)
      var involvedType = new InvolvedType (typeof (CompleteInterfacesTestClass.MyMixinTarget));
      var classContext = mixinConfiguration.ClassContexts.GetWithInheritance (typeof (CompleteInterfacesTestClass.MyMixinTarget));
      involvedType.ClassContext = new ReflectedObject (classContext);

      var reportGenerator = new InterfaceReferenceReportGenerator (involvedType, new IdentifierGenerator<Type>(), ProgramTest.GetRemotionReflection());
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Interfaces",
          new XElement ("Interface", new XAttribute ("ref", "0"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}