using System;
using Remotion.Mixins;

namespace MixinSampleAssembly
{
  [Extends (typeof (ConflictingClass))]
  public class ConflictingMixinA
  {
    public virtual void Foo ()
    {
    }
  }
}