using System;
using System.IO;
using MixinXRef.Formatting;
using MixinXRef.Reflection.Remotion;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class ProgramTest
  {
    public static IRemotionReflector GetRemotionReflection()
    {
      return new RemotionReflector_1_11_20(typeof(TargetClassDefinitionUtility).Assembly, typeof(Mixin<>).Assembly);
    }

    public const string _userPromptOnExistingOutputDirectory =
        "Output directory 'existingOutputDirectory' does already exist.\r\nDo you want to override the directory and including files? [y/N] ";

    private Program _program;
    private TextWriter _standardOutput;
    private IRemotionReflector _remotionReflector;
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _standardOutput = new StringWriter();
      _remotionReflector = ProgramTest.GetRemotionReflection();
      _outputFormatter = new OutputFormatter();

      _program = new Program (new StringReader (""), _standardOutput, _outputFormatter);
      _program.SetRemotionReflector (_remotionReflector);
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
      const string assemblyDirectory = ".";

      Assert.That (Directory.Exists (assemblyDirectory), Is.True);

      var output = _program.GetAssemblies (assemblyDirectory);
      var expectedOutput = new AssemblyBuilder (assemblyDirectory, _remotionReflector).GetAssemblies();

      Assert.That (output, Is.EqualTo (expectedOutput));
      Assert.That (_standardOutput.ToString(), Is.EqualTo (""));
    }

    [Test]
    public void SaveXmlDocument_ForNonEmptyAssemblyArray ()
    {
      const string assemblyDirectory = ".";
      const string xmlFile = "testOutputFile.xml";

      Assert.That (File.Exists (xmlFile), Is.False);

      // returns null or an array of assemblies with length > 0, never an empty array
      var assemblies = _program.GetAssemblies (assemblyDirectory);

      _program.GenerateAndSaveXmlDocument (assemblies, xmlFile);

      Assert.That (File.Exists (xmlFile), Is.True);

      File.Delete (xmlFile);
    }
  }
}