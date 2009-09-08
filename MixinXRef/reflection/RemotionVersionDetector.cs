using System;
using System.IO;
using System.Reflection;

namespace MixinXRef.Reflection
{
  public class RemotionVersionDetector
  {
    private readonly IRemotionReflection _remotionReflection;

    public RemotionVersionDetector (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      var fullAssemblyPath = Path.GetFullPath (Path.Combine (assemblyDirectory, "Remotion.dll"));
      var remotionAssembly = Assembly.LoadFile (fullAssemblyPath);
      _remotionReflection = DetectVersion (remotionAssembly);
    }

    public IRemotionReflection DetectVersion (Assembly remotionAssembly)
    {
      ArgumentUtility.CheckNotNull ("remotionAssembly", remotionAssembly);

      var versionString = remotionAssembly.GetName().Version.ToString();

      switch (versionString)
      {
        case "1.11.20.13":
          return new RemotionReflection08 (remotionAssembly);
        case "1.13.23.2":
          return null;

        default:
          throw new NotSupportedException (String.Format ("The remotion assembly version '{0}' is not supported.", versionString));
      }
    }

    public IRemotionReflection RemotionReflection
    {
      get { return _remotionReflection; }
    }
  }
}