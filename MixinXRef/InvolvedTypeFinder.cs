using System;
using Remotion.Mixins;
using Remotion.Utilities;


namespace MixinXRef
{
  public class InvolvedTypeFinder : IInvolvedTypeFinder
  {
    private readonly MixinConfiguration _mixinConfiguration;

    public InvolvedTypeFinder (MixinConfiguration mixinConfiguration)
    {
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      _mixinConfiguration = mixinConfiguration;
    }

    public InvolvedType[] FindInvolvedTypes ()
    {
      //return _mixinConfiguration.ClassContexts.Select (classContext => classContext.Type).ToArray ();
      InvolvedTypeStore involvedTypes = new InvolvedTypeStore();

      foreach (var targetContext in _mixinConfiguration.ClassContexts)
      {
        involvedTypes.GetOrCreateValue (targetContext.Type).ClassContext = targetContext;

        foreach (var mixinContext in targetContext.Mixins)
          involvedTypes.GetOrCreateValue (mixinContext.MixinType).TargetTypes.Add (targetContext.Type);
      }

      return involvedTypes.ToArray();
    }
  }
}