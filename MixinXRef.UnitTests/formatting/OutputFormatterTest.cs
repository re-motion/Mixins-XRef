using System;
using System.Collections.Generic;
using System.Reflection;
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
    private OutputFormatter _outputFormatter;

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

      Assert.That (output, Is.EqualTo ("GenericTarget<string, int>"));
    }

    [Test]
    public void GetFormattedTypeName_ContainsGenericArguments ()
    {
      var output = _outputFormatter.GetFormattedTypeName (typeof (ContainsGenericArguments<>).BaseType);

      Assert.That (output, Is.EqualTo ("Dictionary<TKey, int>"));
    }

    public class ContainsGenericArguments<TKey> : Dictionary<TKey, int>
    {
    }

    [Test]
    public void CreateModifierMarkup ()
    {
      var output = _outputFormatter.CreateModifierMarkup ("keyword1 keyword2");
      var expectedOutput = new XElement ("Modifiers", new XElement ("Keyword", "keyword1"), new XElement ("Keyword", "keyword2"));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateSignatureMarkup ()
    {
      var constructorOutput = _outputFormatter.CreateSignatureMarkup ("ClassName ()", MemberTypes.Constructor);
      var expectedConstructorOutput = _outputFormatter.CreateConstructorMarkup ("ClassName ()");
      
      Assert.That (constructorOutput.ToString (), Is.EqualTo (expectedConstructorOutput.ToString ()));
    }

    [Test]
    public void CreateConstructorMarkup ()
    {
      var output = _outputFormatter.CreateConstructorMarkup ("ClassName (string Parameter1)");
      var expectedOutput = new XElement ("Signature", new XElement ("Type", "ClassName"));
      _outputFormatter.CreateParameterMarkup ("(string Parameter1)", expectedOutput);

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateParameterMarkup ()
    {
      var output = new XElement ("TestElement");
      const string parameters = "(string Parameter1, type2 Parameter2)";
      _outputFormatter.CreateParameterMarkup (parameters, output);
      var expectedOutput = new XElement (
          "TestElement", 
          new XElement("Text", "("),
          new XElement("Keyword", "string"),
          new XElement("Text", "Parameter1,"),
          new XElement("Type", "type2"),
          new XElement("Text", "Parameter2"),
          new XElement("Text", ")")
          );

      Assert.That (output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }
  }
}