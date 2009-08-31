using System;

namespace MixinXRef.UnitTests.TestDomain
{
  public class ClassWithProperty : BaseClassWithProperty
  {
    public override string PropertyName { get; set; }
    public override void DoSomething() { }
  }
}