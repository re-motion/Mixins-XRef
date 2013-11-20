using System;
using Remotion.Mixins;

namespace MixinSampleAssembly
{
  [Extends (typeof (ConflictingClass))]
  public class ConflictingMixinB
  {
    public virtual void Foo ()
    {
    }
  }
}