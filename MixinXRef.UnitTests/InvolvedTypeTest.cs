using System;
using System.Linq;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InvolvedTypeTest
  {
    [Test]
    public void Equals_True ()
    {
      var type1 = new InvolvedType (typeof (TargetClass1), false);
      var type2 = new InvolvedType (typeof (TargetClass1), false);

      Assert.That (type1, Is.EqualTo (type2));
    }

    [Test]
    public void Equals_False_TypeDoesntMatch ()
    {
      var type1 = new InvolvedType (typeof (TargetClass1), false);
      var type2 = new InvolvedType (typeof (TargetClass2), false);

      Assert.That (type1, Is.Not.EqualTo (type2));
    }

    [Test]
    public void Equals_False_IsTargetDoesntMatch ()
    {
      var type1 = new InvolvedType (typeof (TargetClass1), false);
      type1.ClassContext = new ClassContext(typeof(TargetClass1));
      var type2 = new InvolvedType (typeof (TargetClass1), false);

      Assert.That (type1, Is.Not.EqualTo (type2));
    }

    [Test]
    public void Equals_False_IsMixinDoesntMatch ()
    {
      var type1 = new InvolvedType (typeof (TargetClass1), false);
      var type2 = new InvolvedType (typeof (TargetClass1), true);

      Assert.That (type1, Is.Not.EqualTo (type2));
    }

    [Test]
    public void Equals_False_ClassContextDoesntMatch ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (TargetClass1));
      var type2 = new InvolvedType (typeof (TargetClass1));

      type1.ClassContext = mixinConfiguration.ClassContexts.First();
      type2.ClassContext = mixinConfiguration.ClassContexts.Last();

      Assert.That (type1, Is.Not.EqualTo (type2));
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var type1 = new InvolvedType (typeof (TargetClass1), false);
      var type2 = new InvolvedType (typeof (TargetClass1), false);

      Assert.That (type1.GetHashCode(), Is.EqualTo (type2.GetHashCode()));
    }

    [Test]
    public void ClassContextProperty_ForTargetClass ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (TargetClass1));
      type1.ClassContext = mixinConfiguration.ClassContexts.First();

      Assert.That (type1.IsTarget, Is.True);
      Assert.That (type1.ClassContext, Is.Not.Null);
    }

    [Test]
    public void ClassContextProperty_ForNonTargetClass ()
    {
      var type1 = new InvolvedType (typeof (object));

      Assert.That (type1.IsTarget, Is.False);
      try
      {
        var result = type1.ClassContext;
        Assert.Fail ("Expected exception was not thrown");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Involved type is not a target class"));
      }
    }

  }
}