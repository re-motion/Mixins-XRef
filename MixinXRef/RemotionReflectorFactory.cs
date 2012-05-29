using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef
{
  public static class RemotionReflectorFactory
  {
    public static IRemotionReflector Create (string assemblyDirectory, string reflectorSource)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);
      ArgumentUtility.CheckNotNull ("reflectorSource", reflectorSource);

      var assemblies = GetReflectorAssemblies (reflectorSource);

      if (!assemblies.Any ())
        throw new ArgumentException ("There are no assemblies matching the given reflector source", "reflectorSource");

      return new RemotionReflector ("Remotion", DetectVersion (assemblyDirectory), assemblies, new[] { assemblyDirectory });
    }

    public static IRemotionReflector Create (string assemblyDirectory, Type customReflector)
    {
      return (IRemotionReflector) Activator.CreateInstance (customReflector, assemblyDirectory);
    }

    private static Version DetectVersion (string assemblyDirectory)
    {
      // Assumption: There is always a 'Remotion.dll'
      return AssemblyName.GetAssemblyName (Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll"))).Version;
    }

    private static IEnumerable<_Assembly> GetReflectorAssemblies (string reflectorPath)
    {
      var path = PathUtility.GetDirectoryName (reflectorPath);
      path = string.IsNullOrEmpty (path) ? "." : path;
      var file = Path.GetFileName (reflectorPath) ?? "*.dll";

      return Directory.GetFiles (Path.GetFullPath (path), file).Select (f => (_Assembly) Assembly.LoadFile (f));
    }
  }
}