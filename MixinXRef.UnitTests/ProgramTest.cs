using System;
using System.IO;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class ProgramTest
  {
    public static RemotionReflection GetRemotionReflection()
    {
      var remotionReflection = new RemotionReflection();
      remotionReflection.SetRemotionAssembly(typeof(TargetClassDefinitionUtility).Assembly);
      return remotionReflection;
    }

    public const string _userPromptOnExistingOutputDirectory =
        "Output directory 'existingOutputDirectory' does already exist\r\nDo you want override the directory and including files? [y/N] ";

    private Program _program;
    private TextWriter _standardOutput;
    private IRemotionReflection _remotionReflection;
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _standardOutput = new StringWriter();
      _remotionReflection = ProgramTest.GetRemotionReflection();
      _outputFormatter = new OutputFormatter();

      _program = new Program (new StringReader (""), _standardOutput, _remotionReflection, _outputFormatter);
    }


    [Test]
    public void CheckArguments_InvalidArgumentCount ()
    {
      var arguments = new[] { "twoParametersRequired" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (-1));
      Assert.That (_standardOutput.ToString(), Is.EqualTo ("usage: mixinxref <assemblyDirectory> <outputDirectory>\r\n"));
    }

    [Test]
    public void CheckArguments_InvalidAssemblyDirectory ()
    {
      var arguments = new[] { "invalidAssemblyDirectory", "doesNotMatter" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (-2));
      Assert.That (_standardOutput.ToString(), Is.EqualTo ("Input directory 'invalidAssemblyDirectory' does not exist\r\n"));
    }

    [Test]
    public void CheckArguments_InvalidOutputDirectory ()
    {
      var arguments = new[] { ".", "invalidOutputDirectory" };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (-3));
      Assert.That (_standardOutput.ToString(), Is.EqualTo ("Output directory 'invalidOutputDirectory' does not exist\r\n"));
    }

    [Test]
    public void CheckArguments_ValidDirectories ()
    {
      var arguments = new[] { ".", "." };
      var output = _program.CheckArguments (arguments);
      Assert.That (output, Is.EqualTo (0));
      Assert.That (_standardOutput.ToString(), Is.EqualTo (""));
    }

    [Test]
    public void CreateOrOverrideOutputDirectory_NonExistingDirectory ()
    {
      const string outputDirectory = "nonExistingOutputDirectory";

      Assert.That (Directory.Exists (outputDirectory), Is.False);
      var output = _program.CreateOrOverrideOutputDirectory (outputDirectory);

      Assert.That (Directory.Exists (outputDirectory), Is.True);
      Assert.That (output, Is.EqualTo (0));
      Assert.That (_standardOutput.ToString(), Is.EqualTo (""));

      Directory.Delete (outputDirectory);
    }

    [Test]
    public void CreateOrOverrideOutputDirectory_ExistingDirectory_EndOfFileOnStandardInput ()
    {
      const string outputDirectory = "existingOutputDirectory";

      Directory.CreateDirectory (outputDirectory);
      Assert.That (Directory.Exists (outputDirectory), Is.True);

      // _standardInput is empty -> readLine retrieves null because EOF is reached
      var output = _program.CreateOrOverrideOutputDirectory (outputDirectory);

      Assert.That (Directory.Exists (outputDirectory), Is.True);
      Assert.That (output, Is.EqualTo (1));
      Assert.That (_standardOutput.ToString(), Is.EqualTo (_userPromptOnExistingOutputDirectory));

      Directory.Delete (outputDirectory);
    }

    [Test]
    public void CreateOrOverrideOutputDirectory_ExistingDirectory_UserDeniesOverride ()
    {
      const string outputDirectory = "existingOutputDirectory";

      Directory.CreateDirectory (outputDirectory);
      Assert.That (Directory.Exists (outputDirectory), Is.True);

      // setup input "n" for No
      _program = new Program (new StringReader ("n"), _standardOutput, _remotionReflection, _outputFormatter);

      var output = _program.CreateOrOverrideOutputDirectory (outputDirectory);

      Assert.That (Directory.Exists (outputDirectory), Is.True);
      Assert.That (output, Is.EqualTo (1));
      Assert.That (_standardOutput.ToString(), Is.EqualTo (_userPromptOnExistingOutputDirectory));

      Directory.Delete (outputDirectory);
    }

    [Test]
    public void CreateOrOverrideOutputDirectory_ExistingDirectory_UserAllowsOverride ()
    {
      const string outputDirectory = "existingOutputDirectory";

      Directory.CreateDirectory (outputDirectory);
      Assert.That (Directory.Exists (outputDirectory), Is.True);

      // setup input "YES" for Yes
      _program = new Program (new StringReader ("YES"), _standardOutput, _remotionReflection, _outputFormatter);

      var output = _program.CreateOrOverrideOutputDirectory (outputDirectory);

      Assert.That (Directory.Exists (outputDirectory), Is.True);
      Assert.That (output, Is.EqualTo (0));
      Assert.That (_standardOutput.ToString(), Is.EqualTo (_userPromptOnExistingOutputDirectory));

      Directory.Delete (outputDirectory);
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
      var expectedOutput = new AssemblyBuilder (assemblyDirectory, _remotionReflection).GetAssemblies();

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

      _program.SaveXmlDocument (assemblies, xmlFile);

      Assert.That (File.Exists (xmlFile), Is.True);

      File.Delete (xmlFile);
    }
  }
}