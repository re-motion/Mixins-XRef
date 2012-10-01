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
    private readonly IEnumerable<string> _ignore;
    private readonly string _assemblyDirectory;

    public AssemblyBuilder (string assemblyDirectory, IEnumerable<string> ignore = null)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      _ignore = ignore ?? Enumerable.Empty<string> ();
      _assemblyDirectory = Path.GetFullPath (assemblyDirectory);

      // register assembly reference resolver
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
    }

    public Assembly[] GetAssemblies (Func<Assembly, bool> filter = null)
    {
      var assemblies = new List<Assembly> ();

      foreach (var assemblyFile in Directory.GetFiles (_assemblyDirectory, "*.dll"))
      {
        if (!IsIgnoredAssembly (assemblyFile))
        {
          var loadedAssembly = LoadAssembly (assemblyFile);
          if (loadedAssembly != null)
            assemblies.Add (loadedAssembly);
        }
        else
        {
          XRef.Log.SendInfo ("Ignoring {0}", assemblyFile);
        }
      }

      foreach (var assemblyFile in Directory.GetFiles (_assemblyDirectory, "*.exe"))
      {
        if (!IsIgnoredAssembly (assemblyFile))
        {
          var loadedAssembly = LoadAssembly (assemblyFile);
          if (loadedAssembly != null)
            assemblies.Add (loadedAssembly);
        }
        else
        {
          XRef.Log.SendInfo ("Ignoring {0}", assemblyFile);
        }
      }

      return (filter != null ? assemblies.Where (filter) : assemblies).ToArray ();
    }

    private bool IsIgnoredAssembly (string assemblyFile)
    {
      try
      {
        return _ignore.Contains (AssemblyName.GetAssemblyName (assemblyFile).Name);
      }
      catch (BadImageFormatException badImageFormatException)
      {
        XRef.Log.SendInfo (badImageFormatException.Message);
      }
      return true;
    }

    private Assembly CurrentDomainAssemblyResolve (object sender, ResolveEventArgs args)
    {
      // All assemblies in the target directory have already been loaded.
      // Therefore, we can be sure that the referenced assembly has already been loaded if it is in the right directory.
      var assemblyName = new AssemblyName (args.Name);
      var assembly = AppDomain.CurrentDomain.GetAssemblies ().SingleOrDefault (a => AssemblyName.ReferenceMatchesDefinition (assemblyName, a.GetName ()));
      return assembly;
    }

    private Assembly LoadAssembly (string assemblyFile)
    {
      Assembly loadedAssembly = null;
      try
      {
        loadedAssembly = Assembly.LoadFile (assemblyFile);
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
        var mscorlibReference = loadedAssembly.GetReferencedAssemblies ().FirstOrDefault (a => a.Name == mscorlibAssembly.GetName ().Name);
        if (mscorlibReference == null)
        {
          XRef.Log.SendWarning (
            "Assembly '{0}' in '{1}' does not reference the same core library as this tool ('{2}'), it is skipped.",
            loadedAssembly.FullName,
            loadedAssembly.Location,
            mscorlibAssembly.FullName);
          return null;
        }
        else if (mscorlibReference.Version != mscorlibAssembly.GetName ().Version)
        {
          XRef.Log.SendWarning (
            "Assembly '{0}' in '{1}' references a core library '{2}', but this tool only works with references to core library '{3}'.",
            loadedAssembly.FullName,
            loadedAssembly.Location,
            mscorlibReference,
            mscorlibAssembly.FullName);
          return null;
        }
      }

      return loadedAssembly;
    }
  }
}