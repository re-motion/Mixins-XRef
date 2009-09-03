using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.formatting
{
  [TestFixture]
  public class OutputFormatterTest
  {
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _outputFormatter = new OutputFormatter();
    }

    [Test]
    public void GetFormattedTypeName_NormalType ()
    {
      var output = _outputFormatter.GetFormattedTypeName (typeof (UselessObject));

      Assert.That (output, Is.EqualTo ("UselessObject"));
    }

    [Test]
    public void GetFormattedTypeName_GenericDefinition ()
    {
      var output = _outputFormatter.GetFormattedTypeName (typeof (GenericTarget<,>));
      
      Assert.That (output, Is.EqualTo ("GenericTarget<TParameter1, TParameter2>"));
    }

    [Test]
    public void GetFormattedTypeName_GenericType ()
    {
      var output = _outputFormatter.GetFormattedTypeName (typeof (GenericTarget<string, int>));
      
      Assert.That (output, Is.EqualTo ("GenericTarget<String, Int32>"));
    }

    [Test]
    public void GetFormattedTypeName_ContainsGenericArguments ()
    {
      var output = _outputFormatter.GetFormattedTypeName (typeof (ContainsGenericArguments<>).BaseType);

      Assert.That (output, Is.EqualTo ("Dictionary<TKey, Int32>"));
    }

    public class ContainsGenericArguments<TKey> : Dictionary<TKey, int>{}

    [Test]
    public void CreateModifierMarkup ()
    {
      var output = _outputFormatter.CreateModifierMarkup("keyword1 keyword2");
      var expectedOutput = new XElement("Modifiers", new XElement("Keyword", "keyword1"), new XElement("Keyword", "keyword2"));

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

  }
}