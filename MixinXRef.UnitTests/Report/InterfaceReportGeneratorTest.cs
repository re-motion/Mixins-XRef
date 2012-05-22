using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Reflection.Utility;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using MixinXRef.UnitTests.Helpers;
using Rhino.Mocks;


namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class InterfaceReportGeneratorTest
  {
    private IOutputFormatter _outputFormatter;
    private IRemotionReflector _remotionReflector;

    [SetUp]
    public void SetUp ()
    {
      _outputFormatter = new OutputFormatter ();
      _remotionReflector = MockRepository.GenerateStub<IRemotionReflector> ();
    }

    [Test]
    public void GenerateXml_ZeroInterfaces ()
    {
      var reportGenerator = ReportFactory.CreateInterfaceReportGenerator (_remotionReflector, _outputFormatter);
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("Interfaces");
      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_WithInterfaces ()
    {
      // TargetClass1 implements IDisposable
      var involvedType = new InvolvedType (typeof (TargetClass1));

      var reportGenerator = ReportFactory.CreateInterfaceReportGenerator (_remotionReflector, _outputFormatter, involvedType);
      var output = reportGenerator.GenerateXml ();

      var memberReportGenerator = ReportFactory.CreateMemberReportGenerator (typeof (IDisposable), _outputFormatter);
      var expectedOutput = new XElement (
          "Interfaces",
          new XElement (
              "Interface",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "IDisposable"),
              new XAttribute ("is-complete-interface", false),
              memberReportGenerator.GenerateXml (),
              new XElement (
                  "ImplementedBy",
                  new XElement (
                      "InvolvedType-Reference",
                      new XAttribute ("ref", "0"))
                  )
              ));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_WithCompleteInterface ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<CompleteInterfacesTestClass.MyMixinTarget> ()
          .AddCompleteInterface<CompleteInterfacesTestClass.ICMyMixinTargetMyMixin> ()
          .AddMixin<CompleteInterfacesTestClass.MyMixin> ()
          .BuildConfiguration ();

      var involvedType = new InvolvedType (typeof (CompleteInterfacesTestClass.MyMixinTarget));
      var classContext = mixinConfiguration.ClassContexts.GetWithInheritance (typeof (CompleteInterfacesTestClass.MyMixinTarget));
      involvedType.ClassContext = new ReflectedObject (classContext);

      var reportGenerator = ReportFactory.CreateInterfaceReportGenerator (_remotionReflector, _outputFormatter, involvedType);
      var output = reportGenerator.GenerateXml ();

      var memberReportGenerator = ReportFactory.CreateMemberReportGenerator (typeof (CompleteInterfacesTestClass.ICMyMixinTargetMyMixin), _outputFormatter);
      var expectedOutput = new XElement (
          "Interfaces",
          new XElement (
              "Interface",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "CompleteInterfacesTestClass.ICMyMixinTargetMyMixin"),
              new XAttribute ("is-complete-interface", true),
              memberReportGenerator.GenerateXml (),
              new XElement (
                  "ImplementedBy",
                  new XElement (
                      "InvolvedType-Reference",
                      new XAttribute ("ref", "0"))
                  )
              ));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GetCompleteInterfaces ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<CompleteInterfacesTestClass.MyMixinTarget> ()
          .AddCompleteInterface<CompleteInterfacesTestClass.ICMyMixinTargetMyMixin> ()
          .AddMixin<CompleteInterfacesTestClass.MyMixin> ()
          .BuildConfiguration ();

      var involvedType = new InvolvedType (typeof (CompleteInterfacesTestClass.MyMixinTarget));
      var classContext = mixinConfiguration.ClassContexts.GetWithInheritance (typeof (CompleteInterfacesTestClass.MyMixinTarget));
      involvedType.ClassContext = new ReflectedObject (classContext);

      var reportGenerator = ReportFactory.CreateInterfaceReportGenerator (_remotionReflector, _outputFormatter, involvedType);
      var output = reportGenerator.GetCompleteInterfaces ();

      Assert.That (output, Is.EquivalentTo (classContext.CompleteInterfaces));
    }
  }
}