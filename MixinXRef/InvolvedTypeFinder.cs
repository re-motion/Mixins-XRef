using System;
using System.Reflection;
using MixinXRef.Reflection;

namespace MixinXRef
{
  public class InvolvedTypeFinder : IInvolvedTypeFinder
  {
    // MixinConfiguration _mixinConfiguration;
    private readonly ReflectedObject _mixinConfiguration;
    private readonly Assembly[] _assemblies;
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;

    public InvolvedTypeFinder (
        ReflectedObject mixinConfiguration,
        Assembly[] assemblies,
        ErrorAggregator<Exception> configurationErrors,
        ErrorAggregator<Exception> validationErrors)
    {
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);

      _mixinConfiguration = mixinConfiguration;
      _assemblies = assemblies;
      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
    }

    public InvolvedType[] FindInvolvedTypes ()
    {
      var involvedTypes = new InvolvedTypeStore();

      foreach (var assembly in _assemblies)
      {
        foreach (var type in assembly.GetTypes())
        {
          ReflectedObject classContext = _mixinConfiguration.GetProperty ("ClassContexts").CallMethod ("GetWithInheritance", type);
          if (classContext != null)
          {
            involvedTypes.GetOrCreateValue (type).ClassContext = classContext;

            foreach (var mixinContext in classContext.GetProperty ("Mixins"))
            {
              involvedTypes.GetOrCreateValue (mixinContext.GetProperty ("MixinType").To<Type>()).TargetTypes.Add (
                  classContext.GetProperty ("Type").To<Type>(), null);
            }
          }
        }
      }

      return involvedTypes.ToArray();
    }
  }
}