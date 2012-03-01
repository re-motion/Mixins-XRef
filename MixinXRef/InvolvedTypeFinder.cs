using System;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class InvolvedTypeFinder : IInvolvedTypeFinder
  {
    // MixinConfiguration _mixinConfiguration;
    private readonly ReflectedObject _mixinConfiguration;
    private readonly Assembly[] _assemblies;
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;
    private readonly IRemotionReflector _remotionReflector;

    public InvolvedTypeFinder (
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
      var classContexts = _mixinConfiguration.GetProperty ("ClassContexts");

      foreach (var assembly in _assemblies)
      {
        foreach (var type in assembly.GetTypes())
        {
          var classContext = classContexts.CallMethod ("GetWithInheritance", type);
          if (classContext != null)
          {
            var targetClassDefinition = GetTargetClassDefinition (type, classContext);
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

      return involvedTypes.ToSortedArray();
    }

    public ReflectedObject GetMixinDefiniton (Type mixinType, ReflectedObject targetClassDefinition)
    {
      return targetClassDefinition == null ? null : targetClassDefinition.CallMethod ("GetMixinByConfiguredType", mixinType);
    }

    public ReflectedObject GetTargetClassDefinition (Type type, ReflectedObject classContext)
    {
      if (type.IsGenericTypeDefinition)
        return null;

      try
      {
        // may throw ConfigurationException or ValidationException
        return _remotionReflector.GetTargetClassDefinition (type, _mixinConfiguration, classContext);
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