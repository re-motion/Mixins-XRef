using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

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

    public Assembly[] GetAssemblies (Func<Assembly, bool> filter = null)
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

      return (filter != null ? assemblies.Where (filter) : assemblies).ToArray ();
    }

    private Assembly CurrentDomainAssemblyResolve (object sender, ResolveEventArgs args)
    {
      // All assemblies in the target directory have already been loaded.
      // Therefore, we can be sure that the referenced assembly has already been loaded if it is in the right directory.
      var assemblyName = new AssemblyName (args.Name);
      var assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => AssemblyName.ReferenceMatchesDefinition(assemblyName, a.GetName()));
      return assembly;
    }

    private Assembly LoadAssembly (string assemblyFile)
    {
      Assembly loadedAssembly = null;
      try
      {
        loadedAssembly = Assembly.LoadFile(assemblyFile);
      }
      catch (FileNotFoundException fileNotFoundException)
      {
        XRef.Log.SendInfo (fileNotFoundException.Message);
      }
      catch (FileLoadException fileLoadException)
      {
        XRef.Log.SendInfo (fileLoadException.Message);
      }
      catch (BadImageFormatException badImageFormatException)
      {
        XRef.Log.SendInfo (badImageFormatException.Message);
      }

      if (loadedAssembly != null)
      {
        var mscorlibAssembly = typeof (object).Assembly;
        var mscorlibReference = loadedAssembly.GetReferencedAssemblies().FirstOrDefault (a => a.Name == mscorlibAssembly.GetName().Name);
        if (mscorlibReference == null)
        {
          XRef.Log.SendWarning (
            "Assembly '{0}' does not reference the same core library as this tool ('{1}'), it is skipped.",
            loadedAssembly.CodeBase, 
            mscorlibAssembly.FullName);
          return null;
        }
        else if (mscorlibReference.Version != mscorlibAssembly.GetName ().Version)
        {
          XRef.Log.SendWarning (
            "Assembly '{0}' references a core library '{1}', but this tool only works with references to core library '{2}'.",
            loadedAssembly.CodeBase,
            mscorlibReference,
            mscorlibAssembly.FullName);
          return null;
        }
      }

      return loadedAssembly;
    }
  }
}