using System;
using System.Diagnostics;
using System.Xml.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class SummaryPickerTest
  {
    private readonly XElement noSummary = new XElement ("summary", "No summary found.");
    private SummaryPicker _summaryPicker;

    [SetUp]
    public void SetUp ()
    {
      _summaryPicker = new SummaryPicker();
    }

    [Test]
    public void GetSummary_ForNonExistingXmlFile ()
    {
      var output = _summaryPicker.GetSummary(typeof(object));
      var expectedOutput = noSummary;

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GetSummary_ForValidTypeWithXmlFile_SummaryPresent()
    {
      var output = _summaryPicker.GetSummary(typeof(MixinConfiguration));

      Console.WriteLine (typeof (MixinConfiguration).Assembly);

      var expectedOutput = new XElement("summary", "Constitutes a mixin configuration (ie. a set of classes associated with mixins) and manages the mixin configuration for the current thread.");

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void NormalizeAndTrim_PlainElement ()
    {
      XElement output = new XElement ("Test-element", "   test1   \r\n test2 is     Test    ");

      _summaryPicker.NormalizeAndTrim (output);

      var expectedOutput = new XElement ("Test-element", "test1 test2 is Test");

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void NormalizeAndTrim_WithNestedElements ()
    {
      XElement output = new XElement ("Test-element", 
        "   test   ", 
        new XElement("innerElement", " test     of   \t inner   \r\nelement   "),
        " end  "
        );

      _summaryPicker.NormalizeAndTrim (output);

      var expectedOutput = new XElement ("Test-element", "test", new XElement("innerElement", " test of inner element "), " end");

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}