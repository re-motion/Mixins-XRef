using System;
using System.Reflection;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Utilities;


namespace MixinXRef
{
  public class InvolvedTypeFinder : IInvolvedTypeFinder
  {
    private readonly MixinConfiguration _mixinConfiguration;
    private readonly Assembly[] _assemblies;

    public InvolvedTypeFinder (MixinConfiguration mixinConfiguration, Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      _mixinConfiguration = mixinConfiguration;
      _assemblies = assemblies;
    }

    public InvolvedType[] FindInvolvedTypes ()
    {
      var involvedTypes = new InvolvedTypeStore();

      foreach (var assembly in _assemblies)
      {
        foreach (var type in assembly.GetTypes())
        {
          ClassContext classContext = _mixinConfiguration.ClassContexts.GetWithInheritance (type);
          if (classContext != null)
          {
            involvedTypes.GetOrCreateValue (type).ClassContext = classContext;

            foreach (var mixinContext in classContext.Mixins)
              involvedTypes.GetOrCreateValue (mixinContext.MixinType).TargetTypes.Add (classContext.Type);
          }
        }
      }

      return involvedTypes.ToArray();
    }
  }
}