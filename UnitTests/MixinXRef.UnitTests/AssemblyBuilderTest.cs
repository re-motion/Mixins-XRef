using System.IO;
using System.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using NUnit.Framework;
using Rhino.Mocks;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class AssemblyBuilderTest
  {
    private const string AssemblyDirectory = "assemblyTestDirectory";

    [TestFixtureSetUp]
    public void SetUp ()
    {
      string[] assemblyFiles = { "nunit.framework.dll", "Remotion.dll", "MixinXRef.dll" };

      // delete whole test directory if present
      if (Directory.Exists (AssemblyDirectory))
        Directory.Delete (AssemblyDirectory, true);

      // create sub directory in 'debug' directory of unit tests
      Directory.CreateDirectory (AssemblyDirectory);

      // copy assembly files
      foreach (var file in assemblyFiles)
        File.Copy (file, Path.Combine (AssemblyDirectory, file));

      // add a NonApplicationAssembly manually from bin\Debug\TestDomain
      File.Copy (@"TestDomain\MixinXRef.UnitTests.NonApplicationAssembly.dll", Path.Combine (AssemblyDirectory, "MixinXRef.UnitTests.NonApplicationAssembly.dll"));
    }

    [Test]
    public void GetAssemblies_ZeroAssemblies ()
    {
      const string assemblyDirectory = "directoryWithZeroAssemblies";

      Directory.CreateDirectory (assemblyDirectory);

      var assemblyBuilder = new AssemblyBuilder (assemblyDirectory);
      var output = assemblyBuilder.GetAssemblies ();

      Assert.That (output, Is.Empty);
      Directory.Delete (assemblyDirectory);
    }

    [Test]
    public void GetAssemblies_WithAssemblies ()
    {
      var a1 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (AssemblyDirectory, "Remotion.dll")));
      var a2 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (AssemblyDirectory, "nunit.framework.dll")));
      var a3 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (AssemblyDirectory, "MixinXRef.dll")));
      var a4 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (AssemblyDirectory, "MixinXRef.UnitTests.NonApplicationAssembly.dll")));

      var remotionReflectorStub = MockRepository.GenerateStub<IRemotionReflector> ();
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a1)).Return (false);
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a2)).Return (false);
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a3)).Return (false);
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a4)).Return (true);

      var assemblyBuilder = new AssemblyBuilder (AssemblyDirectory);
      var output = assemblyBuilder.GetAssemblies (assembly => !remotionReflectorStub.IsNonApplicationAssembly (assembly));

      CollectionAssert.AreEquivalent (new[] { a1, a2, a3 }, output);
    }

    [Test]
    public void GetAssemblies_WithoutNonApplicationAssemblyAttribute ()
    {
      // load assemblies from directory
      var assemblyBuilder = new AssemblyBuilder (AssemblyDirectory);

      var a1 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (AssemblyDirectory, "Remotion.dll")));
      var a2 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (AssemblyDirectory, "nunit.framework.dll")));
      var a3 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (AssemblyDirectory, "MixinXRef.dll")));
      var a4 = Assembly.LoadFile (Path.GetFullPath (Path.Combine (AssemblyDirectory, "MixinXRef.UnitTests.NonApplicationAssembly.dll")));

      var remotionReflectorStub = MockRepository.GenerateStub<IRemotionReflector> ();
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a1)).Return (false);
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a2)).Return (false);
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a3)).Return (false);
      remotionReflectorStub.Stub (r => r.IsNonApplicationAssembly (a4)).Return (true);
      var output = assemblyBuilder.GetAssemblies (a => !remotionReflectorStub.IsNonApplicationAssembly (a));

      // *.ddl in alphabetic order, then *.exe in alphabetic order
      //  { "nunit.framework.dll", "Remotion.dll", "MixinXRef.exe" }
      Assert.That (output, Is.EquivalentTo (new[] { a1, a2, a3 }));
    }
  }
}