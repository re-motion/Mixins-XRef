using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public class RemotionReflector_1_11_20 : IRemotionReflector
  {
    public static readonly string[] RemotionAssemblyFileNames = new[] { "Remotion.dll", "Remotion.Interfaces.dll" };

    private readonly Assembly _remotionAssembly;
    private readonly Assembly _remotionInterfaceAssembly;

    // Constructor for factory
    public RemotionReflector_1_11_20 (string assemblyDirectory)
        : this (ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory), RemotionAssemblyFileNames)
    {
    }

    // Constructor for derived classes
    protected RemotionReflector_1_11_20 (string assemblyDirectory, string[] remotionAssemblyFileNames)
    {
      _remotionAssembly = LoadIfAvailable(RemotionAssemblyFileNames[0], assemblyDirectory, remotionAssemblyFileNames);
      _remotionInterfaceAssembly = LoadIfAvailable (RemotionAssemblyFileNames[1], assemblyDirectory, remotionAssemblyFileNames);
    }

    protected Assembly LoadIfAvailable (string assemblyFileName, string assemblyDirectory, string[] remotionAssemblyFileNames)
    {
      int index = Array.IndexOf (remotionAssemblyFileNames, assemblyFileName);
      return index != -1 ? Assembly.LoadFile (Path.GetFullPath (Path.Combine (assemblyDirectory, assemblyFileName))) : null;
    }

    public virtual bool IsNonApplicationAssembly(Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return assembly.GetCustomAttributes (false).Any (attribute => attribute.GetType().Name == "NonApplicationAssemblyAttribute");
    }

    public virtual bool IsConfigurationException (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.ConfigurationException";
    }

    public virtual bool IsValidationException (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.Validation.ValidationException";
    }

    public virtual bool IsInfrastructureType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.Assembly.GetName().Name == "Remotion.Interfaces";
    }

    public virtual bool IsInheritedFromMixin (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var mixinBaseType = _remotionInterfaceAssembly.GetType ("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom (type);
    }

    public virtual ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionUtilityType = _remotionAssembly.GetType ("Remotion.Mixins.TargetClassDefinitionUtility", true);
      return ReflectedObject.CallMethod (targetClassDefinitionUtilityType, "GetConfiguration", targetType, mixinConfiguration);
    }

    public virtual ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var declarativeConfigurationBuilderType = _remotionAssembly.GetType("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod (declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", new object[] { assemblies });
    }

    public virtual Assembly FindRemotionAssembly (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return assemblies.SingleOrDefault (a => a.GetName().Name == "Remotion");
    }
  }
}