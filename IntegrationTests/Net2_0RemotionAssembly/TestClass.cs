using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Mixins;

namespace Net2_0RemotionAssembly
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
