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
    public void CreateConstructorMarkup ()
    {
      var output = _outputFormatter.CreateConstructorMarkup ("ClassName", new ParameterInfo[0]);
      var expectedOutput = new XElement ("Signature", new XElement ("Name", "ClassName"), new XElement ("Text", "("), new XElement ("Text", ")"));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void AddParameterMarkup_WithTypeAndKeyword ()
    {
      var output = new XElement ("Signature");
      var parameterInfos = typeof (MemberSignatureTestClass).GetMethod ("MethodWithParams").GetParameters();

      // int intParam, string stringParam, AssemblyBuilder assemblyBuilderParam
      _outputFormatter.AddParameterMarkup (parameterInfos, output);
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Text", "("),
          new XElement ("Keyword", "int"),
          new XElement ("Text", "intParam,"),
          new XElement ("Keyword", "string"),
          new XElement ("Text", "stringParam,"),
          new XElement ("Type", "AssemblyBuilder"),
          new XElement ("Text", "assemblyBuilderParam"),
          new XElement ("Text", ")")
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateMethodMarkup ()
    {
      var output = _outputFormatter.CreateMethodMarkup ("MethodName", typeof (string), new ParameterInfo[0]);
      var expectedOutput = new XElement (
          "Signature",
          new XElement ("Keyword", "string"),
          new XElement ("Name", "MethodName")
          );
      _outputFormatter.AddParameterMarkup (new ParameterInfo[0], expectedOutput);

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void CreateEventMarkup()
    {
      var output = _outputFormatter.CreateEventMarkup("EventName", typeof(ChangedEventHandler));
      var expectedOutput = new XElement(
          "Signature",
          new XElement("Keyword", "event"),
          new XElement("Type", "ChangedEventHandler"),
          new XElement("Name", "EventName")
          );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void CreateFieldMarkup()
    {
      var output = _outputFormatter.CreateFieldMarkup("FieldName", typeof(int));
      var expectedOutput = new XElement(
          "Signature",
          new XElement("Keyword", "int"),
          new XElement("Name", "FieldName")
          );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void CreatePropertyMarkup()
    {
      var output = _outputFormatter.CreatePropertyMarkup("PropertyName", typeof(int));
      var expectedOutput = new XElement(
          "Signature",
          new XElement("Keyword", "int"),
          new XElement("Name", "PropertyName")
          );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void CreateNestedTypeMarkup_NestedEnumeration()
    {

      var output = _outputFormatter.CreateNestedTypeMarkup(typeof(MemberSignatureTestClass.NestedEnumeration));
      var expectedOutput = new XElement(
          "Signature",
          new XElement("Keyword", "enum"),
          new XElement("Name", "NestedEnumeration")
          );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void CreateNestedTypeMarkup_NestedStruct()
    {

      var output = _outputFormatter.CreateNestedTypeMarkup(typeof(MemberSignatureTestClass.NestedStruct));
      var expectedOutput = new XElement(
          "Signature",
          new XElement("Keyword", "struct"),
          new XElement("Name", "NestedStruct"),
          new XElement("Text", ":"),
          new XElement("Type", "IDisposable")
          );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void CreateNestedTypeMarkup_NestedInterface()
    {

      var output = _outputFormatter.CreateNestedTypeMarkup(typeof(MemberSignatureTestClass.INestedInterface));
      var expectedOutput = new XElement(
          "Signature",
          new XElement("Keyword", "interface"),
          new XElement("Name", "INestedInterface"),
          new XElement("Text", ":"),
          new XElement("Type", "IDisposable"),
          new XElement("Text", ","),
          new XElement("Type", "ICloneable")
          );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void CreateNestedTypeMarkup_NestedClass()
    {

      var output = _outputFormatter.CreateNestedTypeMarkup(typeof(MemberSignatureTestClass.NestedClassWithInterfaceAndInheritance));
      var expectedOutput = new XElement(
          "Signature",
          new XElement("Keyword", "class"),
          new XElement("Name", "NestedClassWithInterfaceAndInheritance"),
          new XElement("Text", ":"),
          new XElement("Type", "GenericTarget<string, int>"),
          new XElement("Text", ","),
          new XElement("Type", "IDisposable")
          );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    
  }
}