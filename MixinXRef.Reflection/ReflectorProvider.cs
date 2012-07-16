using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.Reflection
{
  public abstract class ReflectorProvider
  {
    private readonly string _component;
    private readonly Version _version;
    private readonly string _assemblyDirectory;
    private readonly IEnumerable<Type> _reflectorTypes;
    private readonly IDictionary<MethodBase, IRemotionReflector> _reflectorInstances = new Dictionary<MethodBase, IRemotionReflector> ();

    protected ReflectorProvider (string component, Version version, IEnumerable<_Assembly> assemblies, string assemblyDirectory)
    {
      _component = component;
      _version = version;
      _assemblyDirectory = assemblyDirectory;

      _reflectorTypes =
        assemblies.SelectMany(a => a.GetExportedTypes()).Where(IsValidReflector);

      if (!_reflectorTypes.Any ())
        throw new ArgumentException ("There are no valid reflectors in the given assemblies", "assemblies");

      CheckAssemblyRequirements(_reflectorTypes.OrderByDescending(
        t => t.GetAttribute<ReflectorSupportAttribute>().MinVersion).First(), assemblyDirectory);
    }

    private void CheckAssemblyRequirements(Type reflectorType, string assemblyDirectory)
    {
      foreach (var requiredAssembly in reflectorType.GetAttribute<ReflectorSupportAttribute> ().RequiredAssemblies)
        if (!File.Exists (Path.Combine (assemblyDirectory, requiredAssembly)))
          throw new MissingRequirementException(requiredAssembly);
    }

    protected IRemotionReflector GetCompatibleReflector (MethodBase methodBase)
    {
      IRemotionReflector reflector;
      if (!_reflectorInstances.TryGetValue (methodBase, out reflector))
        _reflectorInstances.Add (methodBase, reflector = FindCompatibleReflector (methodBase));

      return reflector;
    }

    private bool IsValidReflector (Type type)
    {
      var attribute = type.GetAttribute<ReflectorSupportAttribute> ();
      return attribute != null &&
             typeof (IRemotionReflector).IsAssignableFrom (type) &&
             attribute.Component == _component &&
             _version >= attribute.MinVersion;
    }

    private IRemotionReflector FindCompatibleReflector (MethodBase methodBase)
    {
      var parameterTypes = methodBase.GetParameters ().Select (p => p.ParameterType).ToArray ();
      var methods = _reflectorTypes
        .Select (
          t =>
          t.GetMethod (methodBase.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly, null, parameterTypes, null))
        .Where (m => m != null)
        .OrderByDescending (m => m.DeclaringType.GetAttribute<ReflectorSupportAttribute> ().MinVersion).ToArray ();

      if (!methods.Any ())
        throw new NotSupportedException (string.Format ("There is no reflector that supports {0} in version {1} for {2}",
                                                        methodBase, _version, _component));

      if (methods.Length > 1 &&
          methods[0].DeclaringType.GetAttribute<ReflectorSupportAttribute> ().MinVersion ==
          methods[1].DeclaringType.GetAttribute<ReflectorSupportAttribute> ().MinVersion)
        throw new AmbiguousMatchException (
          string.Format ("There are two or more implementations of {0} with MinVersion {1}", methods[0],
                        methods[0].DeclaringType.GetAttribute<ReflectorSupportAttribute> ().MinVersion));

      var reflector = methods[0].DeclaringType;
      return CreateInstanceOf (reflector, _assemblyDirectory);
    }

    private static IRemotionReflector CreateInstanceOf (Type type, string assemblyDirectory)
    {
      return ((IRemotionReflector) Activator.CreateInstance(type)).Initialize(assemblyDirectory);
    }
  }
}