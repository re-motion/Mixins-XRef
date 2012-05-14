using System;
using System.IO;
using System.Reflection;
using MixinXRef.Reflection.Remotion;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Context;
using Rhino.Mocks;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AssemblyBuilderTest
  {
    [Test]
    public void GetAssemblies_WithoutNonApplicationAssemblyAttribute ()
    {
      const string assemblyDirectory = "assemblyTestDirectory";
      string[] assemblyFiles = { "nunit.framework.dll", "Remotion.dll", "MixinXRef.exe" };

      // delete whole test directory if present
      if (Directory.Exists (assemblyDirectory))
        Directory.Delete (assemblyDirectory, true);

      // create sub directory in 'debug' directory of unit tests
      Directory.CreateDirectory (assemblyDirectory);

      // copy assembly files
      foreach (var file in assemblyFiles)
        File.Copy (file, Path.Combine (assemblyDirectory, file));
      
      // add a NonApplicationAssembly manually from bin\Debug\TestDomain
      File.Copy (@"TestDomain\MixinXRef.UnitTests.NonApplicationAssembly.dll", Path.Combine (assemblyDirectory, "MixinXRef.UnitTests.NonApplicationAssembly.dll"));

      // load assemblies from directory
      var assemblyBuilder = new AssemblyBuilder (assemblyDirectory);

      var a1 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll")));
      var a2 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, "nunit.framework.dll")));
      var a3 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, "MixinXRef.exe")));
      var a4 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, "MixinXRef.UnitTests.NonApplicationAssembly.dll")));

      var remotionReflectorStub = MockRepository.GenerateStub<IRemotionReflector> ();
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a1)).Return (false);
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a2)).Return (false);
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a3)).Return (false);
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a4)).Return (true);
      var output = assemblyBuilder.GetAssemblies (a => !remotionReflectorStub.IsNonApplicationAssembly (a));

      // *.ddl in alphabetic order, then *.exe in alphabetic order
      //  { "nunit.framework.dll", "Remotion.dll", "MixinXRef.exe" }
      Assert.That (output, Is.EqualTo (new[] { a2, a1, a3 }));
    }
  }
}