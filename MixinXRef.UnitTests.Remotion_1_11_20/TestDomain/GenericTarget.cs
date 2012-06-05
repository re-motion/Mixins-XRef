using Remotion.Mixins;

namespace MixinXRef.UnitTests.Remotion_1_11_20.TestDomain
{
  // Mixin3 implements IDisposeable
  [Uses (typeof(Mixin3))]
  [Uses (typeof(ClassWithBookAttribute))]
  public class GenericTarget<TParameter1, TParameter2>
  {
  }
}