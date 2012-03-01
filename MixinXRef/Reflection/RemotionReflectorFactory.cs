using System;
using System.IO;
using System.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef.Reflection
{
  public class RemotionReflectorFactory
  {
    public IRemotionReflector Create (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      var remotionReflectorType = DetectVersion (assemblyDirectory);

      return (IRemotionReflector) Activator.CreateInstance (remotionReflectorType, assemblyDirectory);
    }

    public IRemotionReflector Create (string assemblyDirectory, string customRemotionReflectorAssemblyQualifiedName)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);
      ArgumentUtility.CheckNotNull ("customRemotionReflectorAssemblyQualifiedName", customRemotionReflectorAssemblyQualifiedName);

      var remotionReflectorType = Type.GetType (customRemotionReflectorAssemblyQualifiedName, true, false);

      return (IRemotionReflector) Activator.CreateInstance (remotionReflectorType, assemblyDirectory);
    }

    private Type DetectVersion (string assemblyDirectory)
    {
      // Assumption: There is always a 'Remotion.dll'
      var remotionAssembly = Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll")));
      var version = remotionAssembly.GetName().Version;

      if (version.CompareTo (new Version (1, 13, 141)) >= 0)
        return typeof (RemotionReflector_1_13_141);
      if (version.CompareTo (new Version (1, 13, 140)) == 0)
        throw NewNotSupportedException (new Version (1, 13, 140));
      if (version.CompareTo (new Version(1, 13, 23)) >= 0)
        return typeof (RemotionReflector_1_13_23);
      if (version.CompareTo (new Version (1, 11, 20)) >= 0)
        return typeof (RemotionReflector_1_11_20);

      throw NewNotSupportedException (version);
    }

    private NotSupportedException NewNotSupportedException (Version remotionVersion)
    {
      return new NotSupportedException (string.Format ("The remotion assembly version '{0}' is not supported.", remotionVersion));
    }
  }
}