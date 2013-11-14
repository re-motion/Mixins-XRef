using System;
using Remotion.Mixins;

namespace MixinSampleAssembly
{
  [Extends (typeof (TargetClassA))]
  public class MixinClassA
  {
    [OverrideTarget]
    public virtual void Foo ()
    {
    }
  }
}