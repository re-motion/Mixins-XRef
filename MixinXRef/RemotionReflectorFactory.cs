using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef
{
  public static class RemotionReflectorFactory
  {
    private static readonly IDictionary<string, IRemotionReflector> s_reflectors = new Dictionary<string, IRemotionReflector> ();

    public static IRemotionReflector Create (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      var fullPath = Path.GetFullPath (assemblyDirectory);
      IRemotionReflector reflector;
      if (!s_reflectors.TryGetValue (fullPath, out reflector))
        s_reflectors.Add (fullPath,
                          reflector = RemotionReflectorProviderFactory.GetReflector ("Remotion",
                                                                                     DetectVersion (assemblyDirectory),
                                                                                     assemblyDirectory));
      return reflector;
    }

    private static Version DetectVersion (string assemblyDirectory)
    {
      return new Version("1.13.141");
      // Assumption: There is always a 'Remotion.dll'
      return AssemblyName.GetAssemblyName (Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll"))).Version;
    }
  }
}