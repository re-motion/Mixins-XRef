using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
      _remotionReflector = new RemotionReflector_1_13_23(".");
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
      
      // The creation time of the validiation file is different from the creation time of the generated report
      expectedOutput.Root.FirstAttribute.Value = reportGenerator.CreationTime;
      // Absolute paths may differ depending on the environment
      RemoveAbsolutePathsFromStackTraces (expectedOutput);
      RemoveAbsolutePathsFromStackTraces (output);

      Assert.That (output.ToString ().ToLower (), Is.EqualTo (expectedOutput.ToString ().ToLower ()));
    }

    private void RemoveAbsolutePathsFromStackTraces (XDocument document)
    {
      var configurationExceptions = document.Root.Descendants ("ConfigurationErrors").Descendants ("Exception");
      var validationExceptions = document.Root.Descendants ("ValidationErrors").Descendants ("Exception");
      var allExceptions = configurationExceptions.Concat (validationExceptions);

      foreach (var exception in allExceptions)
      {
        var stackTraceElement = exception.Descendants ("StackTrace").Single();
        var comparableStackTrace = _absolutePathRegex.Replace (stackTraceElement.Value, @"<path-removed>\");
        stackTraceElement.SetValue (comparableStackTrace);
      }
    }

    // at MixinXRef.Reflection.ReflectedObject.CallMethod(Type type, String methodName, Object[] parameters) in C:\Development\MixinXRef\trunk\MixinXRef\Reflection\ReflectedObject.cs:line 45
    private const string c_regexPattern = @"C:\\.*\\(?=.*\.cs)";
    private readonly Regex _absolutePathRegex = new Regex (c_regexPattern, RegexOptions.IgnoreCase);
  }
}