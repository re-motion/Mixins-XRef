using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using MixinXRef.Reflection.Utility;
using System.Configuration;
using System.IO;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.RemotionReflector
{
  public class RemotionReflectorProvider : ReflectorProviderBase, IRemotionReflector
  {
    private static readonly string s_reflectorAssembly = ConfigurationManager.AppSettings["remotionReflectorProviderAssemblies"];
    private static IEnumerable<Assembly> s_assemblies;

    private readonly string _assemblyDirectory;

    public RemotionReflectorProvider (string component, Version version, string assemblyDirectory)
    {
      _assemblyDirectory = assemblyDirectory;
      Component = component;
      Version = version;
    }

    public override object[] GetParameters ()
    {
      return new object[] { _assemblyDirectory };
    }

    protected override IEnumerable<Assembly> GetAssemblies ()
    {
      if (s_assemblies == null)
      {
        var path = s_reflectorAssembly == null ? "." : PathUtility.GetDirectoryName (s_reflectorAssembly);
        path = string.IsNullOrEmpty (path) ? "." : path;
        var file = Path.GetFileName (s_reflectorAssembly) ?? "*.dll";

        s_assemblies = Directory.GetFiles (Path.GetFullPath (path), file).Select (Assembly.LoadFile);
      }
      return s_assemblies;
    }

    public bool IsNonApplicationAssembly (Assembly assembly)
    {
      return CallCompatibleMethod<bool> (MethodBase.GetCurrentMethod (), assembly);
    }

    public bool IsConfigurationException (Exception exception)
    {
      return CallCompatibleMethod<bool> (MethodBase.GetCurrentMethod (), exception);
    }

    public bool IsValidationException (Exception exception)
    {
      return CallCompatibleMethod<bool> (MethodBase.GetCurrentMethod (), exception);
    }

    public bool IsInfrastructureType (Type type)
    {
      return CallCompatibleMethod<bool> (MethodBase.GetCurrentMethod (), type);
    }

    public bool IsInheritedFromMixin (Type type)
    {
      return CallCompatibleMethod<bool> (MethodBase.GetCurrentMethod (), type);
    }

    public ReflectedObject GetTargetClassDefinition (Type targetType, ReflectedObject mixinConfiguration, ReflectedObject classContext)
    {
      return CallCompatibleMethod<ReflectedObject> (MethodBase.GetCurrentMethod (), targetType, mixinConfiguration, classContext);
    }

    public ReflectedObject BuildConfigurationFromAssemblies (Assembly[] assemblies)
    {
      return CallCompatibleMethod<ReflectedObject> (MethodBase.GetCurrentMethod (), new object[] { assemblies });
    }

    public ReflectedObject GetValidationLogFromValidationException (Exception validationException)
    {
      return CallCompatibleMethod<ReflectedObject> (MethodBase.GetCurrentMethod (), validationException);
    }

    public ReflectedObject GetTargetCallDependencies (ReflectedObject mixinDefinition)
    {
      return CallCompatibleMethod<ReflectedObject> (MethodBase.GetCurrentMethod (), mixinDefinition);
    }

    public ReflectedObject GetNextCallDependencies (ReflectedObject mixinDefinition)
    {
      return CallCompatibleMethod<ReflectedObject> (MethodBase.GetCurrentMethod (), mixinDefinition);
    }

    public string TargetComponent
    {
      get { return Component; }
    }

    public Version TargetVersion
    {
      get { return Version; }
    }
  }
}