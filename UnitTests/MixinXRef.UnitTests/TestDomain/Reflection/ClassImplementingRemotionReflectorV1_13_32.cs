using System;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;

namespace MixinXRef.UnitTests.TestDomain.Reflection
{
  [ReflectorSupport("Remotion", "1.13.32")]
  public class ClassImplementingRemotionReflectorV1_13_32 : RemotionReflectorBase
  {
    public override bool IsInfrastructureType (Type type)
    {
      return true;
    }
  }
}
