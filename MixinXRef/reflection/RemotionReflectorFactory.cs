using System;
using System.IO;
using System.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef.Reflection
{
  public class RemotionReflectorFactory
  {
    private readonly IRemotionReflector _remotionReflection;

    public RemotionReflectorFactory (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      var fullAssemblyPath = Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll"));
      var remotionAssembly = Assembly.LoadFile (fullAssemblyPath);
      _remotionReflection = DetectVersion (remotionAssembly);
    }

    public IRemotionReflector DetectVersion(Assembly remotionAssembly)
    {
      ArgumentUtility.CheckNotNull ("remotionAssembly", remotionAssembly);

      // TODO: more generic version detection
      var versionString = remotionAssembly.GetName().Version.ToString();

      switch (versionString)
      {
        case "1.11.20.13":
          return new RemotionReflector_1_11_20 (remotionAssembly);
        case "1.13.23.2":
          return null;

        default:
          throw new NotSupportedException (String.Format ("The remotion assembly version '{0}' is not supported.", versionString));
      }
    }

    public IRemotionReflector RemotionReflection
    {
      get { return _remotionReflection; }
    }
  }
}