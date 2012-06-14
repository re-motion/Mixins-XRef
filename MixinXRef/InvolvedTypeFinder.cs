using System;
using System.Linq;
using System.Reflection;
using System.Text;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;
using TalkBack;

namespace MixinXRef
{
  public class InvolvedTypeFinder : IInvolvedTypeFinder
  {
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
      var involvedTypes = new InvolvedTypeStore ();
      var classContexts = _mixinConfiguration.GetProperty ("ClassContexts");

      foreach (var assembly in _assemblies)
      {
        try
        {
          foreach (var type in assembly.GetTypes ())
          {
            var classContext = classContexts.CallMethod ("GetWithInheritance", type);
            if (classContext != null)
            {
              var involvedType = involvedTypes.GetOrCreateValue (type);
              var targetClassDefinition = GetTargetClassDefinition (type, classContext);
              involvedType.ClassContext = classContext;
              involvedType.TargetClassDefinition = targetClassDefinition;

              foreach (var mixinContext in classContext.GetProperty ("Mixins"))
              {
                var mixinType = mixinContext.GetProperty ("MixinType").To<Type> ();
                var mixin = involvedTypes.GetOrCreateValue (mixinType);
                mixin.TargetTypes.Add (involvedType, GetMixinDefiniton (mixinType, targetClassDefinition));
              }
            }

            // also add classes which inherit from Mixin<> or Mixin<,>, but are actually not used as Mixins (not in ClassContexts)
            if (_remotionReflector.IsInheritedFromMixin (type) && !_remotionReflector.IsInfrastructureType (type))
              involvedTypes.GetOrCreateValue (type);
          }
        }
        catch (ReflectionTypeLoadException ex)
        {
          var loaderExceptionLog = new StringBuilder ();
          foreach (var loaderException in ex.LoaderExceptions)
            loaderExceptionLog.AppendFormat ("   {1}{0}", Environment.NewLine, loaderException.Message);

          XRef.Log.SendWarning ("Unable to analyze '{1}' in '{2}' because some referenced assemblies could not be loaded: {0}{3}",
            Environment.NewLine, assembly.FullName, assembly.Location, loaderExceptionLog);
        }
      }

      var additionalTypesCollector = new AdditionalTypesCollector ();

      foreach (IVisitableInvolved involvedType in involvedTypes)
        involvedType.Accept (additionalTypesCollector);

      foreach (var additionalType in additionalTypesCollector.AdditionalTypes)
        involvedTypes.GetOrCreateValue (additionalType);

      return involvedTypes.ToArray ();
    }

    private ReflectedObject GetMixinDefiniton (Type mixinType, ReflectedObject targetClassDefinition)
    {
      return targetClassDefinition == null ? null : targetClassDefinition.CallMethod ("GetMixinByConfiguredType", mixinType);
    }

    public ReflectedObject GetTargetClassDefinition (Type type, ReflectedObject classContext)
    {
      if (type.IsGenericTypeDefinition || type.IsInterface)
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