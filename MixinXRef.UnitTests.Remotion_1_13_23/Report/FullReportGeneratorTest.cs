using System;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Report;
using MixinXRef.UnitTests.Remotion_1_13_23.TestDomain;
using MixinXRef.UnitTests.Stub;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace MixinXRef.UnitTests.Remotion_1_13_23.Report
{
  [TestFixture]
  public class FullReportGeneratorTest
  {
    private ErrorAggregator<Exception> _configurationErros;
    private ErrorAggregator<Exception> _validatonErrors;
    private IRemotionReflector _remotionReflector;

    [SetUp]
    public void SetUp ()
    {
      _configurationErros = new ErrorAggregator<Exception> ();
      _validatonErrors = new ErrorAggregator<Exception> ();
      _remotionReflector = new RemotionReflector_1_13_23(typeof(TargetClassDefinitionFactory).Assembly, typeof(Mixin<>).Assembly);
    }

    [Test]
    public void FullReportGenerator_Empty ()
    {
      var reportGenerator = new FullReportGenerator (
          new Assembly[0],
          new InvolvedType[0],
          _configurationErros,
          _validatonErrors,
          _remotionReflector,
          new OutputFormatter ());

      var output = reportGenerator.GenerateXmlDocument ();

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

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void FullReportGenerator_NonEmpty ()
    {
      var assemblies = new AssemblyBuilder (".", _remotionReflector).GetAssemblies ();

      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ().AddMixin<Mixin1> ()
          .ForClass<TargetClass2> ().AddMixin<Mixin2> ()
        // creates validation error
          .ForClass<Mixin3> ().AddMixin<Mixin3> ()
          .ForClass<MemberModifierTestClass> ().AddMixin<MemberModifierTestClass> ()
          .ForClass<UselessObject> ().AddMixin<FullReportGeneratorTestClass> ().AddMixin<ClassWithAlphabeticOrderingAttribute> ()
          .ForClass (typeof (GenericTarget<,>)).AddMixin<ClassWithBookAttribute> ()
        // creates configuration error
          .ForClass<CompleteInterfacesTestClass.MyMixinTarget> ().AddCompleteInterface<CompleteInterfacesTestClass.ICMyMixinTargetMyMixin> ().AddMixin<CompleteInterfacesTestClass.MyMixin> ()
          .BuildConfiguration ();

      var involvedTypes = new InvolvedTypeFinderStub (
          new ReflectedObject (mixinConfiguration),
          new[] { typeof (Mixin1).Assembly },
          _configurationErros,
          _validatonErrors,
          _remotionReflector).FindInvolvedTypes ();

      var reportGenerator = new FullReportGenerator (
          assemblies,
          involvedTypes,
          _configurationErros,
          _validatonErrors,
          _remotionReflector,
          new OutputFormatter ());
      var output = reportGenerator.GenerateXmlDocument ();
      
      var expectedOutput = XDocument.Load (@"..\..\TestDomain\fullReportGeneratorExpectedOutput.xml");
      
      // the creation time of the validiation file is different from the creation time of the generated report
      expectedOutput.Root.FirstAttribute.Value = reportGenerator.CreationTime;

      Assert.That (output.ToString ().ToLower (), Is.EqualTo (expectedOutput.ToString ().ToLower ()));
    }
  }
}