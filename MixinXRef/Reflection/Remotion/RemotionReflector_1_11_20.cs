using System;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public class RemotionReflector_1_11_20 : RemotionReflectorBase
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
        : base (assemblyDirectory, remotionAssemblyFileNames)
    {
      _remotionAssembly = GetRemotionAssembly (remotionAssemblyFileNames, 0, RemotionAssemblyFileNames);
      _remotionInterfaceAssembly = GetRemotionAssembly (remotionAssemblyFileNames, 1, RemotionAssemblyFileNames);
    }

    protected Assembly GetRemotionAssembly (string[] actualAssemblyFileNames, int index, string[] constantAssemblyFileNames)
    {
      return actualAssemblyFileNames.Length > index
             && actualAssemblyFileNames[index] == constantAssemblyFileNames[index]
                 ? RemotionAssemblies[index]
                 : null;
    }

    public override bool IsNonApplicationAssembly(Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      return assembly.GetCustomAttributes (false).Any (attribute => attribute.GetType().Name == "NonApplicationAssemblyAttribute");
    }

    public override bool IsConfigurationException(Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.ConfigurationException";
    }

    public override bool IsValidationException (Exception exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      return exception.GetType().FullName == "Remotion.Mixins.Validation.ValidationException";
    }

    public override bool IsInfrastructureType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.Assembly.GetName().Name == "Remotion.Interfaces";
    }

    public override bool IsInheritedFromMixin (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var mixinBaseType = _remotionInterfaceAssembly.GetType ("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom (type);
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionUtilityType = _remotionAssembly.GetType ("Remotion.Mixins.TargetClassDefinitionUtility", true);
      return ReflectedObject.CallMethod (targetClassDefinitionUtilityType, "GetConfiguration", targetType, mixinConfiguration);
    }

    public override ReflectedObject BuildConfigurationFromAssemblies(Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var declarativeConfigurationBuilderType = _remotionAssembly.GetType("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod (declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", new object[] { assemblies });
    }

    public override Assembly FindRemotionAssembly (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return assemblies.SingleOrDefault (a => a.GetName().Name == "Remotion");
    }
  }
}