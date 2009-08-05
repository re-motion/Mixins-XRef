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
      Dictionary<Type, InvolvedType> involvedTypes = new Dictionary<Type, InvolvedType>();

      foreach (var context in _mixinConfiguration.ClassContexts)
      {
        Type targetType = context.Type;
        if (!involvedTypes.ContainsKey (targetType))
        {
          involvedTypes.Add (targetType, new InvolvedType (targetType, true, false));
        }
        else
        {
          involvedTypes[targetType].IsTarget = true;
        }

        foreach (var mixin in context.Mixins)
        {
          Type mixinType = mixin.MixinType;
          if (!involvedTypes.ContainsKey (mixinType))
          {
            involvedTypes.Add (mixinType, new InvolvedType (mixinType, false, true));
          }
          else
          {
            involvedTypes[mixinType].IsMixin = true;
          }
        }
      }

      return involvedTypes.Values.ToArray();
    }
  }
}