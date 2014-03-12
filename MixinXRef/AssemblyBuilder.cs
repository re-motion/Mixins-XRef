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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Xml.Linq;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class AssemblyBuilder
  {
    private readonly IEnumerable<string> _ignore;
    private readonly string _assemblyDirectory;
    private Dictionary<string, AssemblyName> _assembliesInPrivateBinPath;

    public AssemblyBuilder (string assemblyDirectory, IEnumerable<string> ignore = null)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      _ignore = ignore ?? Enumerable.Empty<string> ();
      _assemblyDirectory = Path.GetFullPath (assemblyDirectory);

      _assembliesInPrivateBinPath = GetAssembliesInPrivateBinPath();

      // register assembly reference resolver
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
    }

    public Assembly[] GetAssemblies (Func<Assembly, bool> filter = null)
    {
      var assemblies = new List<Assembly> ();

      foreach (var assemblyFile in Directory.GetFiles (_assemblyDirectory, "*.dll", SearchOption.AllDirectories))
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

      foreach (var assemblyFile in Directory.GetFiles (_assemblyDirectory, "*.exe", SearchOption.AllDirectories))
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
      AssemblyName privateAssemblyName;
      if (_assembliesInPrivateBinPath.TryGetValue (args.Name, out privateAssemblyName))
        return Assembly.Load (privateAssemblyName);

      // All assemblies in the target directory have already been loaded.
      // Therefore, we can be sure that the referenced assembly has already been loaded if it is in the right directory.
      var assemblyName = new AssemblyName (args.Name);
      var matchingAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where (a => AssemblyName.ReferenceMatchesDefinition (assemblyName, a.GetName())).ToList();
      if (matchingAssemblies.Count > 1)
      {
        var specificVersion = assemblyName.Version;

        var requestedAssembly = matchingAssemblies.FirstOrDefault (a => a.GetName().Version == specificVersion);
        if (requestedAssembly == null)
          throw new InvalidOperationException (
              string.Format (
                  "Could not resolve assemlby '{0}'. Multiple loaded assemblies with the same name were found. The requested version is '{1}', the loaded versions are '{2}'.",
                  args.Name,
                  specificVersion,
                  string.Join (", ", matchingAssemblies.Select (a => a.GetName().Version))));

        return requestedAssembly;
      }

      return matchingAssemblies.SingleOrDefault();
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
        else if (IsSilverlightAssembly(loadedAssembly, mscorlibReference))
        {
          XRef.Log.SendWarning (
            "Assembly '{0}' in '{1}' references a core library '{2}', and is most likely a silverlight assembly. This tool does not support silverlight.",
            loadedAssembly.FullName,
            loadedAssembly.Location,
            mscorlibReference);
          return null;
        }
      }

      return loadedAssembly;
    }

    private bool IsSilverlightAssembly (Assembly loadedAssembly, AssemblyName mscorlibReference)
    {
      var silverlightMscorlibVersion = new Version (2, 0, 5, 0);
      if (mscorlibReference.Version == silverlightMscorlibVersion)
      {
        var targetFrameworkAttribute = loadedAssembly.CustomAttributes.OfType<TargetFrameworkAttribute>().SingleOrDefault();
        var isPortableAssembly = targetFrameworkAttribute != null && targetFrameworkAttribute.FrameworkName.Contains (".NETPortable");

        return !isPortableAssembly;
      }
      return false;
    }

    private Dictionary<string,AssemblyName> GetAssembliesInPrivateBinPath ()
    {
      var privateBinPaths = (AppDomain.CurrentDomain.RelativeSearchPath ?? "")
          .Split (new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
          .Select (p => Path.Combine (AppDomain.CurrentDomain.BaseDirectory, p))
          .Where (Directory.Exists)
          .Select (p => new DirectoryInfo (p))
          .ToArray();

      return privateBinPaths
          .SelectMany (d => d.EnumerateFileSystemInfos ("*.dll").Concat (d.EnumerateFileSystemInfos ("*.exe")))
          .Select (GetAssemblyNameOrNull)
          .Where (a => a != null)
          .ToDictionary (a => a.FullName, a => a);
    }

    private AssemblyName GetAssemblyNameOrNull (FileSystemInfo file)
    {
      try
      {
        return AssemblyName.GetAssemblyName (file.FullName);
      }
      catch (FileNotFoundException fileNotFoundException)
      {
        XRef.Log.SendInfo (fileNotFoundException.Message);
        return null;
      }
      catch (FileLoadException fileLoadException)
      {
        XRef.Log.SendInfo (fileLoadException.Message);
        return null;
      }
      catch (BadImageFormatException badImageFormatException)
      {
        XRef.Log.SendInfo (badImageFormatException.Message);
        return null;
      }
    }
  }
}