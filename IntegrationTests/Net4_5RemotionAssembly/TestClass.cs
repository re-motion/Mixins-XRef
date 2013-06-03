using System;
using Remotion.Mixins;

namespace Net4_5RemotionAssembly
{
  public class TestClass
  {

    public TestClass ()
    {
      
    }

  }

  [Extends(typeof(TestClass))]
  public class TestClassMixin : Mixin<TestClass>
  {
    
  }
}
