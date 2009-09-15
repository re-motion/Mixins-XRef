using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class FullReportGeneratorTest
  {
    private ErrorAggregator<Exception> _configurationErros;
    private ErrorAggregator<Exception> _validatonErrors;

    [SetUp]
    public void SetUp ()
    {
      _configurationErros = new ErrorAggregator<Exception>();
      _validatonErrors = new ErrorAggregator<Exception>();
    }

    [Test]
    public void FullReportGenerator_Empty ()
    {
      var reportGenerator = new FullReportGenerator (
          new Assembly[0],
          new InvolvedType[0],
          new ReflectedObject (new MixinConfiguration()),
          _configurationErros,
          _validatonErrors,
          ProgramTest.GetRemotionReflection(),
          new OutputFormatter());

      var output = reportGenerator.GenerateXmlDocument();

      var expectedOutput =
          new XDocument (
              new XElement (
                  "MixinXRefReport",
                  new XAttribute ("creation-time", reportGenerator.CreationTime),
                  new XElement ("Assemblies"),
                  new XElement ("InvolvedTypes"),
                  new XElement ("Interfaces"),
                  new XElement ("Attributes"),
                  new XElement ("ConfigurationErrors"),
                  new XElement ("ValidationErrors")
                  ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void FullReportGenerator_NonEmpty ()
    {
      var assemblies = new AssemblyBuilder (".", ProgramTest.GetRemotionReflection()).GetAssemblies();

      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .ForClass<UselessObject> ().AddMixin<FullReportGeneratorTestClass> ().AddMixin<ClassWithAlphabeticOrderingAttribute>()
          .ForClass (typeof (GenericTarget<,>)).AddMixin<ClassWithBookAttribute>()
          .ForClass<CompleteInterfacesTestClass.MyMixinTarget> ()
          .AddCompleteInterface<CompleteInterfacesTestClass.ICMyMixinTargetMyMixin> ()
          .AddMixin<CompleteInterfacesTestClass.MyMixin> ()
          .BuildConfiguration();

      var involvedTypes = new InvolvedTypeFinder (
          new ReflectedObject (mixinConfiguration),
          new[] { typeof (Mixin1).Assembly },
          _configurationErros,
          _validatonErrors,
          ProgramTest.GetRemotionReflection()).FindInvolvedTypes();

      var reportGenerator = new FullReportGenerator (
          assemblies,
          involvedTypes,
          new ReflectedObject (mixinConfiguration),
          _configurationErros,
          _validatonErrors,
          ProgramTest.GetRemotionReflection(),
          new OutputFormatter());
      var output = reportGenerator.GenerateXmlDocument();
      
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\fullReportGeneratorExpectedOutput.xml");

      // the creation time of the validiation file is different from the creation time of the generated report
      expectedOutput.Root.FirstAttribute.Value = reportGenerator.CreationTime;

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}