using System;
using System.Xml.Linq;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class TargetReferenceReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NonMixin ()
    {
      var type1 = new InvolvedType (typeof (object));

      var reportGenerator = new TargetReferenceReportGenerator (type1, new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_ForMixin ()
    {
      var type1 = new InvolvedType (typeof (Mixin1));
      type1.TargetTypes.Add (new InvolvedType(typeof (TargetClass1)), null);

      var reportGenerator = new TargetReferenceReportGenerator (type1, new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "Targets",
          new XElement (
              "Target",
              new XAttribute ("ref", "0")
              )
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}