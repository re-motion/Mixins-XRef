using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Explore
{
  [TestFixture]
  public class MemberOverrideWithInheritanceTest
  {
    [TearDown]
    public void TearDown ()
    {
      MixinConfiguration.ResetMasterConfiguration();
    }

    [Test]
    public void InheritanceBehavior_OverrideMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<BaseClass>().AddMixin<CustomMixin>().BuildConfiguration();
      MixinConfiguration.SetActiveConfiguration (mixinConfiguration);

      var subClass = (IAlgorithm) ObjectFactory.Create<SubClass>().With();
      var output = subClass.AlgorithmMethod();

      Assert.That (output, Is.EqualTo ("2, complex algorithm"));
    }

    #region TestDomain for InheritanceBehavior_OverrideMixin

    public class BaseClass
    {
      public virtual string DoSomething ()
      {
        return "1";
      }
    }

    public class SubClass : BaseClass
    {
      [OverrideMixin]
      public override string DoSomething ()
      {
        return "2";
      }
    }

    public class CustomMixin : Mixin<BaseClass>, IAlgorithm
    {
      public virtual string DoSomething ()
      {
        return "standard impl";
      }

      public string AlgorithmMethod ()
      {
        return DoSomething() + ", complex algorithm";
      }
    }

    public interface IAlgorithm
    {
      string AlgorithmMethod ();
    }

    #endregion

    [Test]
    public void InheritanceBehavior_OverrideTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass>().AddMixin<SubMixin>().BuildConfiguration();
      MixinConfiguration.SetActiveConfiguration (mixinConfiguration);

      var targetClass = ObjectFactory.Create<TargetClass>().With();
      var output = targetClass.DoSomething();

      Assert.That (output, Is.EqualTo("2"));
    }

    #region TestDomain for InheritanceBehavior_OverrideTarget

    public class TargetClass
    {
      public virtual string DoSomething ()
      {
        return "target class";
      }
    }

    public class BaseMixin
    {
      [OverrideTarget]
      public virtual string DoSomething ()
      {
        return "1";
      }
    }

    public class SubMixin : BaseMixin
    {
      public override string DoSomething ()
      {
        return "2";
      }
    }

    #endregion
  }
}