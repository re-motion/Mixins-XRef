using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MixinXRef.UnitTests.CustomRemotionReflector
{
  [TestFixture]
  public class CustomRemotionReflectorTest
  {
    [Test]
    public void UseCustomRemotionReflector_True ()
    {
      // using 'MixinXRef.UnitTests.CustomRemotionReflector.CustomRemotionReflector, MixinXRef.UnitTests.CustomRemotionReflector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'

      Directory.CreateDirectory (Path.GetFullPath (@".\MixinDoc"));

      var assemblyDir = Path.GetFullPath (@".");
      var outputDir = Path.GetFullPath (@".");

      var mixinXRef = new Process ();
      mixinXRef.StartInfo.FileName = @".\MixinXRef.exe";
      mixinXRef.StartInfo.Arguments = assemblyDir + " " + outputDir + " "
                                      + "\"MixinXRef.UnitTests.CustomRemotionReflector.CustomRemotionReflector, MixinXRef.UnitTests.CustomRemotionReflector\"";
      mixinXRef.StartInfo.RedirectStandardError = true;
      mixinXRef.StartInfo.RedirectStandardOutput = true;
      mixinXRef.StartInfo.RedirectStandardInput = true;

      mixinXRef.StartInfo.UseShellExecute = false;

      mixinXRef.Start ();
      mixinXRef.StandardInput.WriteLine ("N");
      Console.Error.Write (mixinXRef.StandardError.ReadToEnd ());
      var output = mixinXRef.StandardOutput.ReadToEnd ();
      mixinXRef.WaitForExit ();

      var exitCode = mixinXRef.ExitCode;
      
      // Directory.Delete (Path.GetFullPath (@".\MixinDoc"));

      Assert.That (exitCode, Is.EqualTo (0));
      Assert.That (output.StartsWith ("RemotionReflector 'MixinXRef.UnitTests.CustomRemotionReflector.CustomRemotionReflector' is used."), Is.True);
    }

    [Test]
    public void UseCustomRemotionReflector_NonExistingType ()
    {
      var assemblyDir = Path.GetFullPath (@".");
      var outputDir = Path.GetFullPath (@".");

      var mixinXRef = new Process ();
      mixinXRef.StartInfo.FileName = @".\MixinXRef.exe";

      mixinXRef.StartInfo.Arguments = assemblyDir + " " + outputDir + " "
                                      + "\"Namespace.NonExistingType, MixinXRef.UnitTests.CustomRemotionReflector\"";
      mixinXRef.StartInfo.RedirectStandardError = true;
      mixinXRef.StartInfo.RedirectStandardOutput = true;
      mixinXRef.StartInfo.RedirectStandardInput = true;

      mixinXRef.StartInfo.UseShellExecute = false;

      mixinXRef.Start ();
      Console.Error.Write (mixinXRef.StandardError.ReadToEnd ());
      var output = mixinXRef.StandardOutput.ReadToEnd ();
      mixinXRef.WaitForExit ();

      var exitCode = mixinXRef.ExitCode;

      const string expectedOutput = "Could not load type 'Namespace.NonExistingType' from assembly 'MixinXRef.UnitTests.CustomRemotionReflector'.\r\n";

      Assert.That (exitCode, Is.EqualTo (-5));
      Assert.That (output, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void UseCustomRemotionReflector_NonExistingAssembly ()
    {
      var assemblyDir = Path.GetFullPath (@".");
      var outputDir = Path.GetFullPath (@".");

      var mixinXRef = new Process ();
      mixinXRef.StartInfo.FileName = @".\MixinXRef.exe";

      mixinXRef.StartInfo.Arguments = assemblyDir + " " + outputDir + " "
                                      + "\"MixinXRef.UnitTests.CustomRemotionReflector.CustomRemotionReflector, NonExistingAssembly\"";
      mixinXRef.StartInfo.RedirectStandardError = true;
      mixinXRef.StartInfo.RedirectStandardOutput = true;
      mixinXRef.StartInfo.RedirectStandardInput = true;

      mixinXRef.StartInfo.UseShellExecute = false;

      mixinXRef.Start ();
      Console.Error.Write (mixinXRef.StandardError.ReadToEnd ());
      var output = mixinXRef.StandardOutput.ReadToEnd ();
      mixinXRef.WaitForExit ();

      var exitCode = mixinXRef.ExitCode;

      const string expectedOutput = "Could not load file or assembly 'NonExistingAssembly' or one of its dependencies. The system cannot find the file specified.\r\n";

      Assert.That (exitCode, Is.EqualTo (-5));
      Assert.That (output, Is.EqualTo (expectedOutput));
    }
  }
}