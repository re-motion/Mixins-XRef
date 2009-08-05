using System;
using System.Collections.Generic;
using Remotion.Mixins;
using Remotion.Utilities;
using System.Linq;

namespace MixinXRef
{
  public class InvolvedTypeFinder : ITargetClassFinder
  {
    private readonly MixinConfiguration _mixinConfiguration;

    public InvolvedTypeFinder (MixinConfiguration mixinConfiguration)
    {
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      _mixinConfiguration = mixinConfiguration;
    }

    public Type[] FindTargetClasses ()
    {
      //return _mixinConfiguration.ClassContexts.Select (classContext => classContext.Type).ToArray ();
      List<Type> involvedTypes = new List<Type>();
      foreach (var context in _mixinConfiguration.ClassContexts)
      {
        involvedTypes.Add (context.Type);
        involvedTypes.AddRange (context.Mixins.Select(mixin => mixin.MixinType));
      }
      return involvedTypes.Distinct().ToArray();
    }
  }
}