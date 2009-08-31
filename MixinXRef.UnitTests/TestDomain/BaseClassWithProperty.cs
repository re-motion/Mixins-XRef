using System;

namespace MixinXRef.UnitTests.TestDomain
{
  public class BaseClassWithProperty
  {
    public virtual string PropertyName { get; set; }
    public virtual void DoSomething() { }
  }
}