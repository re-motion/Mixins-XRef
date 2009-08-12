using System;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;

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
      var assemblies = new AssemblyBuilder (".").GetAssemblies();
      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);
      var involvedTypes = new InvolvedTypeFinder (mixinConfiguration).FindInvolvedTypes();

      var reportGenerator = new FullReportGenerator (assemblies, involvedTypes, mixinConfiguration);
      var output = reportGenerator.GenerateXmlDocument();

      var expectedOutput = XDocument.Load (@"..\..\TestDomain\fullReportGeneratorExpectedOutput.xml");

      // the creation time of the validiation file is different from the creation time of the generated report
      expectedOutput.Root.FirstAttribute.Value = reportGenerator.CreationTime;

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}