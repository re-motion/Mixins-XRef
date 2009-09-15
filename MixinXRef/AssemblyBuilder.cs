using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class AssemblyBuilder
  {
    private readonly string _assemblyDirectory;
    private readonly IRemotionReflection _remotionReflection;

    public AssemblyBuilder (string assemblyDirectory, IRemotionReflection remotionReflection)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);

      _assemblyDirectory = Path.GetFullPath (assemblyDirectory);
      _remotionReflection = remotionReflection;

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

      return assemblies.Where (a => !_remotionReflection.IsNonApplicationAssembly(a)).ToArray();
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