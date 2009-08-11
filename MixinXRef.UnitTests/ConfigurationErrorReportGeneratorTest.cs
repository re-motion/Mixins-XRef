using System;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class ConfigurationErrorReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoErrors ()
    {
      var errorAggregator = new ErrorAggregator<ConfigurationException>();
      var reportGenerator = new ConfigurationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement ("ConfigurationErrors");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithErrors ()
    {
      var errorAggregator = new ErrorAggregator<ConfigurationException>();
      var configurationException1 = new ConfigurationException ("test configuration exception 1", new Exception ("inner exception"));
      var configurationException2 = new ConfigurationException ("test configuration excpetion 2");

      errorAggregator.AddException (configurationException1);
      errorAggregator.AddException (configurationException2);
      var reportGenerator = new ConfigurationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "ConfigurationErrors",
          new RecursiveExceptionReportGenerator (configurationException1).GenerateXml(),
          new RecursiveExceptionReportGenerator (configurationException2).GenerateXml()
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}