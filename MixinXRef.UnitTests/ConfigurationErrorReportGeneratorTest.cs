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
      var configurationException1 = new ConfigurationException ("test configuration exception", new Exception ("inner exception"));

      errorAggregator.AddException (configurationException1);
      var reportGenerator = new ConfigurationErrorReportGenerator (errorAggregator);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement (
          "ConfigurationErrors",
          new XElement (
              "Exception",
              new XAttribute ("type", configurationException1.GetType()),
              new XElement ("Message", configurationException1.Message),
              new XElement ("StackTrace", configurationException1.StackTrace),
              new XElement (
                  "Exception",
                  new XAttribute ("type", configurationException1.InnerException.GetType()),
                  new XElement ("Message", configurationException1.InnerException.Message),
                  new XElement ("StackTrace", configurationException1.InnerException.StackTrace)
                  )
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}