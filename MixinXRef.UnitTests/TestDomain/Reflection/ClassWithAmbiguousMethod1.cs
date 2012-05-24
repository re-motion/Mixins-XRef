using System;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Reflection;

namespace MixinXRef.UnitTests.TestDomain.Reflection
{
  [ReflectorSupport("Remotion", "1.11.20")]
  public class ClassWithAmbiguousMethod1 : RemotionReflectorBase
  {
    public override bool IsInfrastructureType (Type type)
    {
      return true;
    }
  }
}
