using System;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Utility;
using Remotion.Mixins.Definitions;

namespace MixinXRef
{
  public class InvolvedTypeFinder : IInvolvedTypeFinder
  {
    // MixinConfiguration _mixinConfiguration;
    private readonly ReflectedObject _mixinConfiguration;
    private readonly Assembly[] _assemblies;
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;
    private readonly IRemotionReflection _remotionReflection;

    public InvolvedTypeFinder (
        ReflectedObject mixinConfiguration,
        Assembly[] assemblies,
        ErrorAggregator<Exception> configurationErrors,
        ErrorAggregator<Exception> validationErrors,
        IRemotionReflection remotionReflection
        )
    {
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);

      _mixinConfiguration = mixinConfiguration;
      _assemblies = assemblies;
      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
      _remotionReflection = remotionReflection;
    }

    public InvolvedType[] FindInvolvedTypes ()
    {
      var involvedTypes = new InvolvedTypeStore();

      foreach (var assembly in _assemblies)
      {
        foreach (var type in assembly.GetTypes())
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
              involvedTypes.GetOrCreateValue(mixinType).TargetTypes.Add(
                  classContext.GetProperty("Type").To<Type>(), GetMixinDefiniton(mixinType, targetClassDefinition));
            }
          }
        }
      }

      return involvedTypes.ToArray();
    }

    public ReflectedObject GetMixinDefiniton(Type mixinType, ReflectedObject targetClassDefinition)
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
        return _remotionReflection.GetTargetClassDefinition (type, _mixinConfiguration);
      }
      catch (Exception configurationOrValidationException)
      {
        if (_remotionReflection.IsConfigurationException (configurationOrValidationException))
          _configurationErrors.AddException (configurationOrValidationException);
        else if (_remotionReflection.IsValidationException (configurationOrValidationException))
          _validationErrors.AddException (configurationOrValidationException);
        else
          throw;
      }
      // MixinConfiguration is not valid
      return null;
    }
  }
}