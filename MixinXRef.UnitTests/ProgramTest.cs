using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class ProgramTest
  {
    public const string _userPromptOnExistingOutputDirectory =
        "Output directory 'existingOutputDirectory' does already exist\r\nDo you want override the directory and including files? [y/N] ";

    private TextWriter _standardOutput;
    private TextWriter _redirectedOutput;

    private TextReader _standardInput;

    [SetUp]
    public void SetUp ()
    {
      _standardOutput = Console.Out;
      _redirectedOutput = new StringWriter();
      Console.SetOut (_redirectedOutput);

      _standardInput = Console.In;
    }

    [TearDown]
    public void TearDown ()
    {
      Console.SetOut (_standardOutput);
      Console.SetIn (_standardInput);
    }


    [Test]
    public void CheckArguments_InvalidArgumentCount ()
    {
      var arguments = new[] { "twoParametersRequired" };
      var output = Program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (-1));
      Assert.That (_redirectedOutput.ToString(), Is.EqualTo ("usage: mixinxref <assemblyDirectory> <outputDirectory>\r\n"));
    }

    [Test]
    public void CheckArguments_InvalidAssemblyDirectory ()
    {
      var arguments = new[] { "invalidAssemblyDirectory", "doesNotMatter" };
      var output = Program.CheckArguments(arguments);
      Assert.That(output, Is.EqualTo(-2));
      Assert.That(_redirectedOutput.ToString(), Is.EqualTo("Input directory 'invalidAssemblyDirectory' does not exist\r\n"));
    }

    [Test]
    public void CheckArguments_InvalidOutputDirectory()
    {
      var arguments = new[] { ".", "invalidOutputDirectory" };
      var output = Program.CheckArguments(arguments);
      Assert.That(output, Is.EqualTo(-3));
      Assert.That(_redirectedOutput.ToString(), Is.EqualTo("Output directory 'invalidOutputDirectory' does not exist\r\n"));
    }

    [Test]
    public void CheckArguments_ValidDirectories()
    {
      var arguments = new[] { ".", "." };
      var output = Program.CheckArguments(arguments);
      Assert.That(output, Is.EqualTo(0));
      Assert.That(_redirectedOutput.ToString(), Is.EqualTo(""));
    }

    [Test]
    public void CreateOrOverrideOutputDirectory_NonExistingDirectory()
    {
      const string outputDirectory = "nonExistingOutputDirectory";

      Assert.That (Directory.Exists (outputDirectory), Is.False);
      var output = Program.CreateOrOverrideOutputDirectory (outputDirectory);

      Assert.That(Directory.Exists(outputDirectory), Is.True);
      Assert.That (output, Is.EqualTo (0));
      Assert.That (_redirectedOutput.ToString(), Is.EqualTo (""));

      Directory.Delete (outputDirectory);
    }

    [Test]
    public void CreateOrOverrideOutputDirectory_ExistingDirectory_EndOfFileOnStandardInput()
    {
      const string outputDirectory = "existingOutputDirectory";

      Directory.CreateDirectory(outputDirectory);
      Assert.That(Directory.Exists(outputDirectory), Is.True);

      //redirect input, no input -> readLine retrieves null because EOF is reached
      var redirectedInputNo = new StringReader("");
      Console.SetIn(redirectedInputNo);

      var output = Program.CreateOrOverrideOutputDirectory(outputDirectory);

      Assert.That(Directory.Exists(outputDirectory), Is.True);
      Assert.That(output, Is.EqualTo(1));
      Assert.That(_redirectedOutput.ToString(), Is.EqualTo(_userPromptOnExistingOutputDirectory));

      Directory.Delete(outputDirectory);
    }

    [Test]
    public void CreateOrOverrideOutputDirectory_ExistingDirectory_UserDeniesOverride()
    {
      const string outputDirectory = "existingOutputDirectory";

      Directory.CreateDirectory (outputDirectory);
      Assert.That(Directory.Exists(outputDirectory), Is.True);

      //redirect input
      var redirectedInputNo = new StringReader("n");
      Console.SetIn (redirectedInputNo);

      var output = Program.CreateOrOverrideOutputDirectory(outputDirectory);

      Assert.That(Directory.Exists(outputDirectory), Is.True);
      Assert.That(output, Is.EqualTo(1));
      Assert.That(_redirectedOutput.ToString(), Is.EqualTo(_userPromptOnExistingOutputDirectory));

      Directory.Delete(outputDirectory);
    }

    [Test]
    public void CreateOrOverrideOutputDirectory_ExistingDirectory_UserAllowsOverride()
    {
      const string outputDirectory = "existingOutputDirectory";

      Directory.CreateDirectory(outputDirectory);
      Assert.That(Directory.Exists(outputDirectory), Is.True);

      //redirect input
      var redirectedInputYes = new StringReader("YES");
      Console.SetIn(redirectedInputYes);

      var output = Program.CreateOrOverrideOutputDirectory(outputDirectory);

      Assert.That(Directory.Exists(outputDirectory), Is.True);
      Assert.That(output, Is.EqualTo(0));
      Assert.That(_redirectedOutput.ToString(), Is.EqualTo(_userPromptOnExistingOutputDirectory));

      Directory.Delete(outputDirectory);
    }

    [Test]
    public void GetAssemblies_ZeroAssemblies ()
    {
      const string assemblyDirectory = "directoryWithZeroAssemblies";

      Directory.CreateDirectory (assemblyDirectory);
      Assert.That(Directory.Exists(assemblyDirectory), Is.True);

      var output = Program.GetAssemblies (assemblyDirectory);
      Assert.That (output, Is.Null);
      Assert.That(_redirectedOutput.ToString(), Is.EqualTo("'directoryWithZeroAssemblies' contains no assemblies\r\n"));

      Directory.Delete (assemblyDirectory);
    }

    [Test]
    public void GetAssemblies_WithAssemblies()
    {
      const string assemblyDirectory = ".";

      Assert.That(Directory.Exists(assemblyDirectory), Is.True);

      var output = Program.GetAssemblies(assemblyDirectory);
      var expectedOutput = new AssemblyBuilder (assemblyDirectory).GetAssemblies();

      Assert.That(output, Is.EqualTo(expectedOutput));
      Assert.That(_redirectedOutput.ToString(), Is.EqualTo(""));
    }
  }
}