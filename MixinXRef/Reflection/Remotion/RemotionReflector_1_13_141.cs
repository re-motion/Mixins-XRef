using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public class RemotionReflector_1_13_141 : RemotionReflector_1_13_23
  {
    public new static readonly string[] RemotionAssemblyFileNames = new[] { "Remotion.dll", "Remotion.Mixins.dll" };

    private readonly Assembly _mixinsAssembly;

    // Constructor for factory
    public RemotionReflector_1_13_141 (string assemblyDirectory)
        : this (ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory), RemotionAssemblyFileNames)
    {
    }

    // Constructor for derived classes
    protected RemotionReflector_1_13_141 (string assemblyDirectory, string[] remotionAssemblyFileNames)
        : base (assemblyDirectory, remotionAssemblyFileNames)
    {
      _mixinsAssembly = GetRemotionAssembly (remotionAssemblyFileNames, 1, RemotionAssemblyFileNames);
    }

    public override bool IsInfrastructureType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.Assembly.GetName ().Name == "Remotion.Mixins";
    }

    public override bool IsInheritedFromMixin (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var mixinBaseType = _mixinsAssembly.GetType ("Remotion.Mixins.IInitializableMixin", true);
      return mixinBaseType.IsAssignableFrom (type);
    }

    public override ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      var declarativeConfigurationBuilderType = _mixinsAssembly.GetType ("Remotion.Mixins.Context.DeclarativeConfigurationBuilder", true);
      return ReflectedObject.CallMethod (declarativeConfigurationBuilderType, "BuildConfigurationFromAssemblies", new object[] { assemblies });
    }

    public override ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      var targetClassDefinitionFactoryType = _mixinsAssembly.GetType ("Remotion.Mixins.Definitions.TargetClassDefinitionFactory", true);
      return ReflectedObject.CallMethod (targetClassDefinitionFactoryType, "CreateTargetClassDefinition", classContext);
    }

    public override Assembly FindRemotionAssembly (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return assemblies.SingleOrDefault (a => a.GetName ().Name == "Remotion.Mixins");
    }
  }
}