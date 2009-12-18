using System;
using System.Collections.Generic;
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
    private readonly IRemotionReflector _remotionReflector;

    public AssemblyBuilder (string assemblyDirectory, IRemotionReflector remotionReflector)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);

      _assemblyDirectory = Path.GetFullPath (assemblyDirectory);
      _remotionReflector = remotionReflector;

      // register assembly reference resolver
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
    }

    public Assembly[] GetAssemblies ()
    {
      var assemblies = new List<Assembly>();

      foreach (var assemblyFile in Directory.GetFiles (_assemblyDirectory, "*.dll"))
      {
        var loadedAssembly = LoadAssembly (assemblyFile);
        if (loadedAssembly != null)
          assemblies.Add (loadedAssembly);
      }
      
      foreach (var assemblyFile in Directory.GetFiles (_assemblyDirectory, "*.exe"))
      {
        var loadedAssembly = LoadAssembly (assemblyFile);
        if (loadedAssembly != null)
          assemblies.Add (loadedAssembly);
      }

      return assemblies.Where (a => !_remotionReflector.IsNonApplicationAssembly (a)).ToArray();
    }

    private Assembly CurrentDomainAssemblyResolve (object sender, ResolveEventArgs args)
    {
      // All assemblies in the target directory have already been loaded.
      // Therefore, we can be sure that the referenced assembly has already been loaded if it is in the right directory.
      AssemblyName assemblyName = new AssemblyName (args.Name);
      return
          AppDomain.CurrentDomain.GetAssemblies().Where (a => AssemblyName.ReferenceMatchesDefinition (assemblyName, a.GetName())).SingleOrDefault();
    }

    private Assembly LoadAssembly (string assemblyFile)
    {
        try
        {
          return Assembly.LoadFile (assemblyFile);
          
        }
        catch (FileNotFoundException fileNotFoundException)
        {
          Console.Out.WriteLine (fileNotFoundException.Message);
        }
        catch (FileLoadException fileLoadException)
        {
          Console.Out.WriteLine (fileLoadException.Message);
        }
        catch (BadImageFormatException badImageFormatException)
        {
          Console.Out.WriteLine (badImageFormatException.Message);
        }

      return null;
    }
  }
}