using System;
using System.IO;
using System.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class RemotionReflectorFactory
  {
    public IRemotionReflector Create (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      return RemotionReflectorProviderFactory.GetReflector ("Remotion", DetectVersion (assemblyDirectory), assemblyDirectory);
    }

    private static Version DetectVersion (string assemblyDirectory)
    {
      // Assumption: There is always a 'Remotion.dll'
      return AssemblyName.GetAssemblyName (Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll"))).Version;
    }
  }
}