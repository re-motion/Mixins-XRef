using System;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Validation;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class FullReportGeneratorTest
  {
    [Test]
    public void FullReportGenerator_Empty ()
    {
      var reportGenerator = new FullReportGenerator (new Assembly[0], new InvolvedType[0], new MixinConfiguration());

      var output = reportGenerator.GenerateXmlDocument();

      var expectedOutput = 
        new XDocument (
          new XElement (
              "MixinXRefReport",
              new XAttribute("creation-time", reportGenerator.CreationTime),
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
    public void FullReportGenerator_NonEmpty()
    {
      var assemblies = new AssemblyBuilder (".").GetAssemblies();
      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies(assemblies);
      var involvedTypes = new InvolvedTypeFinder(mixinConfiguration).FindInvolvedTypes();

      var reportGenerator = new FullReportGenerator(assemblies, involvedTypes, mixinConfiguration);
      var output = reportGenerator.GenerateXmlDocument();

      // following code is required to generate expected output
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();
      var readonlyassemblyIdentifierGenerator = assemblyIdentifierGenerator.GetReadonlyIdentiferGenerator("none");
      var involvedTypeIdentiferGenerator = new IdentifierGenerator<Type>();
      var interfaceIdentiferGenerator = new IdentifierGenerator<Type>();
      var attributeIdentiferGenerator = new IdentifierGenerator<Type>();
      var configurationErrors = new ErrorAggregator<ConfigurationException>();
      var validationErrors = new ErrorAggregator<ValidationException>();

      var assemblyReport = new AssemblyReportGenerator(
          assemblies, involvedTypes, assemblyIdentifierGenerator, involvedTypeIdentiferGenerator);
      var involvedReport = new InvolvedTypeReportGenerator(
          involvedTypes,
          mixinConfiguration,
          readonlyassemblyIdentifierGenerator,
          involvedTypeIdentiferGenerator,
          interfaceIdentiferGenerator,
          attributeIdentiferGenerator,
          configurationErrors,
          validationErrors);
      var interfaceReport = new InterfaceReportGenerator(
          involvedTypes, readonlyassemblyIdentifierGenerator, involvedTypeIdentiferGenerator, interfaceIdentiferGenerator);
      var attributeReport = new AttributeReportGenerator(
          involvedTypes, readonlyassemblyIdentifierGenerator, involvedTypeIdentiferGenerator, attributeIdentiferGenerator);
      var configurationErrorReport = new ConfigurationErrorReportGenerator(configurationErrors);
      var validationErrorReport = new ValidationErrorReportGenerator(validationErrors);
      
      var compositeReportGenerator = new CompositeReportGenerator(
          assemblyReport,
          involvedReport,
          interfaceReport,
          attributeReport,
          configurationErrorReport,
          validationErrorReport);
      
      var expectedOutput = compositeReportGenerator.GenerateXml();
      expectedOutput.Add(new XAttribute("creation-time", reportGenerator.CreationTime));

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }
  }
}