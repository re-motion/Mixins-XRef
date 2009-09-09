using System;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Explore
{
  [TestFixture]
  public class ComplexMemberOverrideTest
  {
    [Test]
    public void MemberOverrideTargetOverrideMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass (typeof (Target)).AddMixin (typeof (MixinOverrideTarget))
          .ForClass (typeof (MixinOverrideTarget)).AddMixin (typeof (MixinOverrideMixin))
          .BuildConfiguration();

      // Target (1) => MixinOverrideTarget (2) => MixinOverrideMixin (3)
      MixinConfiguration.SetActiveConfiguration (mixinConfiguration);

      var target = ObjectFactory.Create<Target>().With();

      target.WriteType();
    }

    #region TestDomain for ComplexMemberOverrideTest

    public interface ITarget
    {
      void WriteType ();
    }

    public class Target
    {
      public virtual void WriteType ()
      {
        Console.WriteLine (1);
      }
    }

    public class MixinOverrideTarget : Mixin<Target, ITarget>
    {
      [OverrideTarget]
      public virtual void WriteType ()
      {
        Console.WriteLine (2);
        Base.WriteType();
      }
    }

    public class MixinOverrideMixin : Mixin<MixinOverrideTarget, ITarget>
    {
      [OverrideTarget]
      public void WriteType ()
      {
        Console.WriteLine (3);
        Base.WriteType();
      }
    }

    #endregion
  }
}