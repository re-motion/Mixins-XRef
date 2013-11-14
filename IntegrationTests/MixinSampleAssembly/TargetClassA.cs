using System;
using Remotion.Mixins;

namespace MixinSampleAssembly
{
  [Uses (typeof (UsedMixinB))]
  public class TargetClassA
  {
    public virtual void Foo ()
    {
    }
  
    [OverrideMixin]
    public virtual void Foo2 ()
    {
    }

  }
}