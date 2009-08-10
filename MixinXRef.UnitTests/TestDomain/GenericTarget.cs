using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  // Mixin3 implements IDisposeable
  [Uses (typeof(Mixin3))]
  [Uses (typeof(ClassWithBookAttribute))]
  public class GenericTarget<T>
  {
  }
}