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

    //[Test]
    //public void CreateSignatureMarkup_Method ()
    //{
    //  var parameterTypes = new Type[] {typeof(string), typeof(int), typeof(UselessObject)};
    //  var parameterNames = new string[] { "StringParamter", "IntParamter", "UselessObjectParamter" };
    //  //, parameterTypes, parameterNames
    //  var output = _outputFormatter.CreateSignatureMarkup (typeof (string).Name, "ToString", MemberTypes.Method);
      
    //  var expectedOutput = new XElement (
    //      "Signature",
    //      new XElement ("Keyword", "string"),
    //      new XElement ("Name", "ToString"),
    //      new XElement ("BeginList", "("), 
    //      new XElement ("EndList", ")")
    //      );

    //  Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    //}

    //[Test]
    //public void CreateSignatureMarkup_Property ()
    //{
    //  var output = _outputFormatter.CreateSignatureMarkup ("string", "Name", MemberTypes.Property);

    //  var expectedOutput = new XElement (
    //      "Signature",
    //      new XElement ("Keyword", "string"),
    //      new XElement ("Name", "Name")
    //      );

    //  Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    //}

    //[Test]
    //public void CreateSignatureMarkup_Constructor()
    //{
    //  var output = _outputFormatter.CreateSignatureMarkup ("ClassType", "ClassName", MemberTypes.Constructor);

    //  var expectedOutput = new XElement (
    //      "Signature",
    //      new XElement ("Keyword", "ClassType"),
    //      new XElement ("Name", "ClassName"),
    //      new XElement ("BeginList", "("),
    //      new XElement ("EndList", ")")
    //      );

    //  Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    //}
  }
}