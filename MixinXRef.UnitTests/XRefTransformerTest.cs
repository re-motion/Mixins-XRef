using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class XRefTransformerTest
  {

    [Test]
    public void GeneateHtmlFromXml_NonExistingXmlInputFile ()
    {
      // save and redirect standard error
      var standardError = Console.Error;
      var textWriter = new StringWriter();
      Console.SetError (textWriter);

       var transfomer = new XRefTransformer ("invalidFile.xml", "C:/");

      // error code 2 means - source file does not exist
      Assert.That(transfomer.GenerateHtmlFromXml(), Is.EqualTo(2));
      Assert.That (textWriter.ToString(), Is.EqualTo ("Source file invalidFile.xml does not exist\r\n"));

      // restore standard error
      Console.SetError (standardError);
    }

    [Test]
    public void GeneateHtmlFromXml_ValidXmlInputFile()
    {
      const string documentationDirectory = "MixinDoc";
      var fileName = Path.Combine (documentationDirectory, "index.html");

      Directory.CreateDirectory(documentationDirectory);
      var transfomer = new XRefTransformer(@"..\..\TestDomain\fullReportGeneratorExpectedOutput.xml", "MixinDoc");

      Assert.That(File.Exists(fileName), Is.False);
      Assert.That(transfomer.GenerateHtmlFromXml(), Is.EqualTo(0));
      Assert.That(File.Exists(fileName), Is.True);

      Directory.Delete (documentationDirectory, true);
    }
  }
}