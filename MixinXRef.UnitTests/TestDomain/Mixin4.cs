using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  [Extends (typeof (TargetClass1))]
  public class Mixin4 : Mixin<IDisposable>
  {
  }
}