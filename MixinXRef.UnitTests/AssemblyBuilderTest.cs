using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Context;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AssemblyBuilderTest
  {
    [Test]
    public void GetAssemblies ()
    {
      const string assemblyDirectory = "assemblyTestDirectory";
      string[] assemblyFiles = { "nunit.framework.dll", "Remotion.dll", "MixinXRef.exe" };

      // delete whole test directory if present
      if (Directory.Exists(assemblyDirectory))
        Directory.Delete(assemblyDirectory, true);

      // create sub directory in 'debug' directory of unit tests
      Directory.CreateDirectory (assemblyDirectory);
      
      // copy assembly files
      foreach (var file in assemblyFiles)
        File.Copy (file, Path.Combine (assemblyDirectory, file));

      // load assemblies from directory
      var assemblyBuilder = new AssemblyBuilder (assemblyDirectory);
      var output = assemblyBuilder.GetAssemblies();

      // *.ddl in alphabetic order, then *.exe in alphabetic order
      //  { "nunit.framework.dll", "Remotion.dll", "MixinXRef.exe" }
      var expectedOutput = new[] { typeof(Assert).Assembly,  typeof(ClassContext).Assembly, typeof(InvolvedType).Assembly };

      for (int i = 0; i < expectedOutput.Length; i++)
      {
        Assert.That (AssemblyName.ReferenceMatchesDefinition (output[i].GetName(), expectedOutput[i].GetName()), Is.True);
      }

    }
  }
}