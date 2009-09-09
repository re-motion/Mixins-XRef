using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Explore
{
  [TestFixture]
  public class SimpleMemberOverridesTest
  {
    [Test]
    public void SimpleOverrideByMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass (typeof (Target)).AddMixin (typeof (MixinOverrideTarget))
          .BuildConfiguration();
      var targetClassDefiniton = TargetClassDefinitionUtility.GetConfiguration (typeof (Target), mixinConfiguration);
      var targetClassOverrides = targetClassDefiniton.GetAllMembers().Where (mdb => mdb.Name == "WriteType").Single().Overrides;
      var mixinOverrides = targetClassDefiniton.Mixins[0].GetAllMembers().Where (mdb => mdb.Name == "WriteType").Single().Overrides;

      Assert.That (targetClassOverrides.Count, Is.EqualTo (1));
      Assert.That (mixinOverrides.Count, Is.EqualTo (0));
      Assert.That(targetClassOverrides[0].DeclaringClass.Type, Is.EqualTo(typeof(MixinOverrideTarget)));
    }

    #region TestDomain for SimpleOverrideByMixin

    public class Target
    {
      public virtual void WriteType ()
      {
        Console.WriteLine (GetType());
      }
    }

    public class MixinOverrideTarget
    {
      [OverrideTarget]
      public void WriteType ()
      {
        Console.WriteLine (GetType());
      }
    }

    #endregion


    [Test]
    public void SimpleOverrideByTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass (typeof (TargetOverrideMixin)).AddMixin (typeof (TemplateMixin))
          .BuildConfiguration();
      var targetClassDefiniton = TargetClassDefinitionUtility.GetConfiguration (typeof (TargetOverrideMixin), mixinConfiguration);
      var targetClassOverrides = targetClassDefiniton.GetAllMembers().Where (mdb => mdb.Name == "WriteType").Single().Overrides;
      var mixinOverrides = targetClassDefiniton.Mixins[0].GetAllMembers().Where (mdb => mdb.Name == "WriteType").Single().Overrides;

      Assert.That(targetClassOverrides.Count, Is.EqualTo(0));
      Assert.That(mixinOverrides.Count, Is.EqualTo(1));
      Assert.That(mixinOverrides[0].DeclaringClass.Type, Is.EqualTo(typeof(TargetOverrideMixin)));
    }

    #region TestDomain for SimpleOverrideByTarget

    public class TargetOverrideMixin
    {
      [OverrideMixin]
      public void WriteType ()
      {
        Console.WriteLine (GetType());
      }
    }

    public class TemplateMixin : Mixin<TargetOverrideMixin>
    {
      public virtual void WriteType ()
      {
        Console.WriteLine (GetType());
      }
    }

    #endregion
  }
}