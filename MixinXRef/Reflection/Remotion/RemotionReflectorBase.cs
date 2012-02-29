using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public abstract class RemotionReflectorBase : IRemotionReflector
  {
    protected RemotionReflectorBase (string assemblyDirectory, string[] remotionAssemblyFileNames)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);
      ArgumentUtility.CheckNotNull ("remotionAssemblyFileNames", remotionAssemblyFileNames);

      RemotionAssemblies = remotionAssemblyFileNames
          .Select (fileName => LoadFile (assemblyDirectory, fileName))
          .ToArray();
    }

    protected Assembly[] RemotionAssemblies { get; private set; }

    private Assembly LoadFile (string assemblyDirectory, string assemblyFileName)
    {
      return Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, assemblyFileName)));
    }

    public abstract bool IsNonApplicationAssembly (Assembly assembly);
    public abstract bool IsConfigurationException (Exception exception);
    public abstract bool IsValidationException (Exception exception);
    public abstract bool IsInfrastructureType (Type type);
    public abstract bool IsInheritedFromMixin (Type type);
    public abstract ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext);
    public abstract ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies);
    public abstract Assembly FindRemotionAssembly (Assembly[] assemblies);
  }
}