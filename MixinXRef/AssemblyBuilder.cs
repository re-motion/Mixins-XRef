using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MixinXRef
{
  public class AssemblyBuilder
  {
    private readonly string _assemblyDirectory;

    public AssemblyBuilder (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      _assemblyDirectory = Path.GetFullPath (assemblyDirectory);

      // register assembly reference resolver
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
    }

    public Assembly[] GetAssemblies ()
    {
      // get all assemblies
      string[] dlls = Directory.GetFiles (_assemblyDirectory, "*.dll");
      string[] exes = Directory.GetFiles (_assemblyDirectory, "*.exe");

      string[] assemblyFiles = new string[dlls.Length + exes.Length];
      dlls.CopyTo (assemblyFiles, 0);
      exes.CopyTo (assemblyFiles, dlls.Length);

      var assemblies = new Assembly[assemblyFiles.Length];
      for (int i = 0; i < assemblyFiles.Length; i++)
        assemblies[i] = Assembly.LoadFile (assemblyFiles[i]);

      return assemblies
          .Where (
          a => !a.GetCustomAttributes (false).Any (attribute => attribute.GetType().FullName == "Remotion.Reflection.NonApplicationAssemblyAttribute"))
          .ToArray();
      //return assemblies.Where (a => !a.IsDefined (typeof (NonApplicationAssemblyAttribute), false)).ToArray();
    }

    private Assembly CurrentDomainAssemblyResolve (object sender, ResolveEventArgs args)
    {
      // All assemblies in the target directory have already been loaded.
      // Therefore, we can be sure that the referenced assembly has already been loaded if it is in the right directory.
      AssemblyName assemblyName = new AssemblyName (args.Name);
      return
          AppDomain.CurrentDomain.GetAssemblies().Where (a => AssemblyName.ReferenceMatchesDefinition (assemblyName, a.GetName())).SingleOrDefault();
    }
  }
}