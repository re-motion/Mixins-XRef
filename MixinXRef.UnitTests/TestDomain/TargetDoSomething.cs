using System;

namespace MixinXRef.UnitTests.TestDomain
{
  public class TargetDoSomething : IDoSomething
  {
    public virtual void DoSomething ()
    {
      throw new NotImplementedException();
    }
  }
}