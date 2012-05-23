using System;
using System.IO;
using System.Reflection;
using MixinXRef.Formatting;
using MixinXRef.Reflection.RemotionReflector;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class ProgramTest
  {

    public const string _userPromptOnExistingOutputDirectory =
        "Output directory 'existingOutputDirectory' does already exist.\r\nDo you want to override the directory and including files? [y/N] ";

    private Program _program;
    private TextWriter _standardOutput;
    private IOutputFormatter _outputFormatter;

    public static IRemotionReflector GetRemotionReflection ()
    {
      // TODO Replace with mock if possible
      return new RemotionReflectorProvider("Remotion", new Version("1.11.20"), ".");
    }

    [SetUp]
    public void SetUp ()
    {
      _standardOutput = new StringWriter();
      _outputFormatter = new OutputFormatter();

      _program = new Program (new StringReader (""), _standardOutput, _outputFormatter);
    }


    [Test]
    public void CheckArguments_InvalidArgumentCount ()
    {
      var arguments = new[] { "twoParametersRequired" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (-1));
      Assert.That (_standardOutput.ToString (), Is.EqualTo ("usage: mixinxref assemblyDirectory outputDirectory [customRemotionReflectorAssemblyQualifiedName] [-force]\r\nQuitting MixinXRef\r\n"));
    }

    [Test]
    public void CheckArguments_InvalidAssemblyDirectory ()
    {
      var arguments = new[] { "invalidAssemblyDirectory", "doesNotMatter" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (-2));
      Assert.That (_standardOutput.ToString (), Is.EqualTo ("Input directory 'invalidAssemblyDirectory' does not exist\r\nQuitting MixinXRef\r\n"));
    }

    [Test]
    public void CheckArguments_OutputDoesNotExist ()
    {
      var arguments = new[] { ".", "newOutputDirectory" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (0));
    }

    [Test]
    public void CheckArguments_OutputDoesExistAndIsEmpty ()
    {
      Directory.CreateDirectory ("emptyDir");

      var arguments = new[] { ".", "emptyDir" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (0));
    }

    [Test]
    public void CheckArguments_OutputDirectoryNotEmpty ()
    {
      Directory.CreateDirectory ("invalidOutputDirectory");
      Directory.CreateDirectory ("invalidOutputDirectory\\dummyFolder");

      var arguments = new[] { ".", "invalidOutputDirectory" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (-3));
      Assert.That (_standardOutput.ToString (), Is.EqualTo ("Output directory 'invalidOutputDirectory' is not empty\r\nQuitting MixinXRef\r\n"));
    }

    [Test]
    public void CheckArguments_OutputDirectoryNotEmptyWithForce_1 ()
    {
      Directory.CreateDirectory ("invalidOutputDirectory");
      Directory.CreateDirectory ("invalidOutputDirectory\\dummyFolder");

      var arguments = new[] { ".", "invalidOutputDirectory", "-force" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (0));
    }

    [Test]
    public void CheckArguments_OutputDirectoryNotEmptyWithForce_2 ()
    {
      Directory.CreateDirectory ("invalidOutputDirectory");
      Directory.CreateDirectory ("invalidOutputDirectory\\dummyFolder");

      var arguments = new[] { ".", "invalidOutputDirectory", "customReflector", "-force" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (0));
    }

    [Test]
    public void CheckArguments_OutputDirectoryContainsInvalidCharacter ()
    {
      var arguments = new[] { ".", "does<NotMatter" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (-4));
      Assert.That (_standardOutput.ToString (), Is.EqualTo ("Output directory 'does<NotMatter' contains invalid characters\r\nQuitting MixinXRef\r\n"));
    }

    [Test]
    public void CheckArguments_ValidDirectories ()
    {
      var arguments = new[] { ".", "MixinDoc" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (0));
      Assert.That (_standardOutput.ToString(), Is.EqualTo (""));
    }

    [Test]
    public void GetAssemblies_ZeroAssemblies ()
    {
      const string assemblyDirectory = "directoryWithZeroAssemblies";

      Directory.CreateDirectory (assemblyDirectory);
      Assert.That (Directory.Exists (assemblyDirectory), Is.True);

      var output = _program.GetAssemblies (assemblyDirectory);
      Assert.That (output, Is.Null);
      Assert.That (_standardOutput.ToString(), Is.EqualTo ("'directoryWithZeroAssemblies' contains no assemblies\r\n"));

      Directory.Delete (assemblyDirectory);
    }

    [Test]
    public void GetAssemblies_WithAssemblies ()
    {
      const string assemblyDirectory = "assemblyTestDirectory";

      Assert.That (Directory.Exists (assemblyDirectory), Is.True);

      var a1 = Assembly.LoadFile(Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll")));
      var a2 = Assembly.LoadFile(Path.GetFullPath (Path.Combine (assemblyDirectory, "nunit.framework.dll")));
      var a3 = Assembly.LoadFile(Path.GetFullPath (Path.Combine (assemblyDirectory, "MixinXRef.exe")));
      var a4 = Assembly.LoadFile(Path.GetFullPath (Path.Combine (assemblyDirectory, "MixinXRef.UnitTests.NonApplicationAssembly.dll")));

      var remotionReflectorStub = MockRepository.GenerateStub<IRemotionReflector>();
      remotionReflectorStub.Stub(r => r.IsNonApplicationAssembly(a1)).Return(false);
      remotionReflectorStub.Stub(r => r.IsNonApplicationAssembly(a2)).Return(false);
      remotionReflectorStub.Stub(r => r.IsNonApplicationAssembly(a3)).Return(false);
      remotionReflectorStub.Stub(r => r.IsNonApplicationAssembly(a4)).Return(true);
      _program.SetRemotionReflector (remotionReflectorStub);

      var output = _program.GetAssemblies (assemblyDirectory);

      CollectionAssert.AreEquivalent(new[] { a1, a2, a3 }, output);
      Assert.That (_standardOutput.ToString(), Is.EqualTo (""));
    }

    [Test]
    public void SaveXmlDocument_ForNonEmptyAssemblyArray ()
    {
      const string assemblyDirectory = "assemblyTestDirectory";
      const string xmlFile = "testOutputFile.xml";

      Assert.That (File.Exists (xmlFile), Is.False);

      var a1 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll")));
      var a2 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, "nunit.framework.dll")));
      var a3 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, "MixinXRef.exe")));
      
      _program.SetRemotionReflector(GetRemotionReflection());

      _program.GenerateAndSaveXmlDocument (new[] { a1, a2, a3 }, xmlFile);

      Assert.That (File.Exists (xmlFile), Is.True);

      File.Delete (xmlFile);
    }
  }
}