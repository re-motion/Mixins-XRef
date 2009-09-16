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

      return CreateInstance (assemblyDirectory, null);
    }

    public IRemotionReflector Create (string assemblyDirectory, string customRemotionReflectorAssemblyQualifiedName)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);
      ArgumentUtility.CheckNotNull ("customRemotionReflectorAssemblyQualifiedName", customRemotionReflectorAssemblyQualifiedName);

      var remotionReflectionType = Type.GetType (customRemotionReflectorAssemblyQualifiedName);

      return CreateInstance (assemblyDirectory, remotionReflectionType);
    }

    private IRemotionReflector CreateInstance (string assemblyDirectory, Type remotionReflectionType)
    {
      var fullAssemblyPath = Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll"));
      var remotionAssembly = Assembly.LoadFile (fullAssemblyPath);

      remotionReflectionType = remotionReflectionType ?? DetectVersion (remotionAssembly);

      return (IRemotionReflector) Activator.CreateInstance (remotionReflectionType, remotionAssembly);
    }

    private Type DetectVersion (Assembly remotionAssembly)
    {
      // TODO: more generic version detection
      var version = remotionAssembly.GetName().Version;

      if (version.Major == 1 && version.Minor == 13 && version.Build == 23)
        return typeof (RemotionReflector_1_13_23);
      if (version.Major == 1 && version.Minor == 11 && version.Build == 20)
        return typeof (RemotionReflector_1_11_20);

      throw new NotSupportedException (string.Format ("The remotion assembly version '{0}' is not supported.", version));
    }
  }
}