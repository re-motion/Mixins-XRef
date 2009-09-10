using System;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
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
      public void C ()
      {
        Console.WriteLine ("D");
      }
    }

    public interface ICMyMixinTargetMyMixin
    {
      void A ();
      void B ();
    }
  }
}