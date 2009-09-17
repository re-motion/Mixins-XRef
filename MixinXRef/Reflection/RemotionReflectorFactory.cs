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

      var remotionAssembly = GetRemotionAssembly (assemblyDirectory);
      var remotionReflectorType = DetectVersion (remotionAssembly);

      return (IRemotionReflector) Activator.CreateInstance (remotionReflectorType, remotionAssembly);
    }

    public IRemotionReflector Create (string assemblyDirectory, string customRemotionReflectorAssemblyQualifiedName)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);
      ArgumentUtility.CheckNotNull ("customRemotionReflectorAssemblyQualifiedName", customRemotionReflectorAssemblyQualifiedName);

      var remotionAssembly = GetRemotionAssembly(assemblyDirectory);
      var remotionReflectorType = Type.GetType (customRemotionReflectorAssemblyQualifiedName, true, false);

      return (IRemotionReflector) Activator.CreateInstance (remotionReflectorType, remotionAssembly);
    }

    private Assembly GetRemotionAssembly (string assemblyDirectory)
    {
      var fullAssemblyPath = Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll"));
      return Assembly.LoadFile (fullAssemblyPath);
    }

    private Type DetectVersion (Assembly remotionAssembly)
    {
      // TODO: more generic version detection
      var version = remotionAssembly.GetName().Version;
      
      if (version.CompareTo(new Version(1,13,23)) >= 0)
        return typeof (RemotionReflector_1_13_23);
      if (version.CompareTo (new Version (1, 11, 20)) >= 0)
        return typeof (RemotionReflector_1_11_20);

      throw new NotSupportedException (string.Format ("The remotion assembly version '{0}' is not supported.", version));
    }
  }
}