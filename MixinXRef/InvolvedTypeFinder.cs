using System;
using System.Collections.Generic;
using Remotion.Mixins;
using Remotion.Utilities;
using System.Linq;

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

    public IInvolvedType[] FindInvolvedTypes ()
    {
      //return _mixinConfiguration.ClassContexts.Select (classContext => classContext.Type).ToArray ();
      List<InvolvedType> involvedTypes = new List<InvolvedType>();
      foreach (var context in _mixinConfiguration.ClassContexts)
      {
        involvedTypes.Add (new InvolvedType(context.Type, true, false));
        involvedTypes.AddRange (context.Mixins.Select(mixin => new InvolvedType(mixin.MixinType, false, true)));
      }
      return involvedTypes.Distinct().ToArray();
    }
  }
}