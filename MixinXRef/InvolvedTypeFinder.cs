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

      foreach (var context in _mixinConfiguration.ClassContexts)
      {
        var targetType = involvedTypes.GetOrCreateValue (context.Type);
        targetType.ClassContext = context;

        foreach (var mixin in context.Mixins)
          involvedTypes.GetOrCreateValue (mixin.MixinType).IsMixin = true;
      }

      return involvedTypes.ToArray();
    }
  }
}