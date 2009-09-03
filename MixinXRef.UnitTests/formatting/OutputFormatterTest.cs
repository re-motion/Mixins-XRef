using System;
using System.Collections.Generic;
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
    public void GetCSharpLikeName_NormalType ()
    {
      var output = _outputFormatter.GetFormattedTypeName (typeof (UselessObject));

      Assert.That (output, Is.EqualTo ("UselessObject"));
    }

    [Test]
    public void GetCSharpLikeName_GenericDefinition ()
    {
      var output = _outputFormatter.GetFormattedTypeName (typeof (GenericTarget<,>));
      
      Assert.That (output, Is.EqualTo ("GenericTarget<TParameter1, TParameter2>"));
    }

    [Test]
    public void GetCSharpLikeName_GenericType ()
    {
      var output = _outputFormatter.GetFormattedTypeName (typeof (GenericTarget<string, int>));
      
      Assert.That (output, Is.EqualTo ("GenericTarget<String, Int32>"));
    }

    [Test]
    public void GetCSharpLikeName_ContainsGenericArguments ()
    {
      var output = _outputFormatter.GetFormattedTypeName (typeof (ContainsGenericArguments<>).BaseType);

      Assert.That (output, Is.EqualTo ("Dictionary<TKey, Int32>"));
    }

    public class ContainsGenericArguments<TKey> : Dictionary<TKey, int>{}

    [Test]
    public void CreateModifierMarkup_FewestPossibleModifiers ()
    {
      var output = _outputFormatter.CreateModifierMarkup("public", false);
      const string expectedOutput = "<span class=\"keyword\">public</span>";

      Assert.That(output, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void CreateModifierMarkup_Overriden()
    {
      var output = _outputFormatter.CreateModifierMarkup("public", true);
      const string expectedOutput = "<span class=\"keyword\">public</span><span class=\"keyword\">overridden</span>";

      Assert.That(output, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void CreateModifierMarkup_MemberVisibility()
    {
      var output = _outputFormatter.CreateModifierMarkup("public", false);
      const string expectedOutput = "<span class=\"keyword\">public</span>";

      Assert.That(output, Is.EqualTo(expectedOutput));
    }
  }
}