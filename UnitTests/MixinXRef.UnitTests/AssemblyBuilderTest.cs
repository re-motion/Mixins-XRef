// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
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