using System;
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
      var output = _outputFormatter.GetCSharpLikeName (typeof (UselessObject));

      Assert.That (output, Is.EqualTo ("UselessObject"));
    }

    [Test]
    public void GetCSharpLikeName_GenericDefinition ()
    {
      var output = _outputFormatter.GetCSharpLikeName (typeof (GenericTarget<,>));
      
      Assert.That (output, Is.EqualTo ("GenericTarget<TParameter1, TParameter2>"));
    }

    [Test]
    public void GetCSharpLikeName_GenericType ()
    {
      var output = _outputFormatter.GetCSharpLikeName (typeof (GenericTarget<string, int>));
      
      Assert.That (output, Is.EqualTo ("GenericTarget<String, Int32>"));
    }

    [Test]
    public void GetCSharpLikeName_ContainsGenericArguments ()
    {
      var output = _outputFormatter.GetCSharpLikeName (typeof (ContainsGenericArguments<>).BaseType);

      Assert.That (output, Is.EqualTo ("GenericTarget<TParameter1, Int32>"));
    }

    public class ContainsGenericArguments<TParameter1> : GenericTarget<TParameter1, int>
    {
      
    }
  }
}