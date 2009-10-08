using System;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;


namespace MixinXRef.UnitTests.Stub
{
  public class InvolvedTypeFinderStub : IInvolvedTypeFinder
  {
 // MixinConfiguration _mixinConfiguration;
    private readonly ReflectedObject _mixinConfiguration;
    private readonly Assembly[] _assemblies;
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;
    private readonly IRemotionReflector _remotionReflector;

    public InvolvedTypeFinderStub (
        ReflectedObject mixinConfiguration,
        Assembly[] assemblies,
        ErrorAggregator<Exception> configurationErrors,
        ErrorAggregator<Exception> validationErrors,
        IRemotionReflector remotionReflector
        )
    {
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);

      _mixinConfiguration = mixinConfiguration;
      _assemblies = assemblies;
      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
      _remotionReflector = remotionReflector;
    }

    public InvolvedType[] FindInvolvedTypes ()
    {
      var involvedTypes = new InvolvedTypeStore();

      Array.Sort (_assemblies, (assembly1, assembly2) => assembly1.ToString().CompareTo (assembly2.ToString()));

      foreach (var assembly in _assemblies)
      {
        var types = assembly.GetTypes();
        Array.Sort (types, (type1, type2) => type1.Name.CompareTo (type2.Name));
        
        foreach (var type in types)
        {
          var classContext = _mixinConfiguration.GetProperty ("ClassContexts").CallMethod ("GetWithInheritance", type);
          if (classContext != null)
          {
            var targetClassDefinition = GetTargetClassDefinition (type);
            involvedTypes.GetOrCreateValue (type).ClassContext = classContext;
            involvedTypes.GetOrCreateValue (type).TargetClassDefintion = targetClassDefinition;

            foreach (var mixinContext in classContext.GetProperty ("Mixins"))
            {
              var mixinType = mixinContext.GetProperty ("MixinType").To<Type>();
              involvedTypes.GetOrCreateValue (mixinType).TargetTypes.Add (
                  classContext.GetProperty ("Type").To<Type>(), GetMixinDefiniton (mixinType, targetClassDefinition));
            }
          }

          // also add classes which inherit from Mixin<> or Mixin<,>, but are actually not used as Mixins (not in ClassContexts)
          if (_remotionReflector.IsInheritedFromMixin (type) && !_remotionReflector.IsInfrastructureType (type))
            involvedTypes.GetOrCreateValue (type);
        }
      }

      return involvedTypes.ToArray();
    }

    public ReflectedObject GetMixinDefiniton (Type mixinType, ReflectedObject targetClassDefinition)
    {
      return targetClassDefinition == null ? null : targetClassDefinition.CallMethod ("GetMixinByConfiguredType", mixinType);
    }

    public ReflectedObject GetTargetClassDefinition (Type type)
    {
      if (type.IsGenericTypeDefinition)
        return null;

      try
      {
        // may throw ConfigurationException or ValidationException
        return _remotionReflector.GetTargetClassDefinition (type, _mixinConfiguration);
      }
      catch (Exception configurationOrValidationException)
      {
        if (_remotionReflector.IsConfigurationException (configurationOrValidationException))
          _configurationErrors.AddException (configurationOrValidationException);
        else if (_remotionReflector.IsValidationException (configurationOrValidationException))
          _validationErrors.AddException (configurationOrValidationException);
        else
          throw;
      }
      // MixinConfiguration is not valid
      return null;
    }
  }
}