using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Remotion_1_13_141.TestDomain
{
  public class CompleteInterfacesTestClass
  {
    public class MyMixinTarget
    {
      public void A ()
      {
        Console.WriteLine ("A");
      }
    }

    public class MyMixin : Mixin<MyMixinTarget>
    {
      public void B ()
      {
        Console.WriteLine ("B");
      }
    }

    public interface ICMyMixinTargetMyMixin
    {
      void A ();
      void B ();
    }
  }
}