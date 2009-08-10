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
      var type1 = new InvolvedType (typeof (TargetClass1));
      var type2 = new InvolvedType (typeof (TargetClass1));

      Assert.That (type1, Is.EqualTo (type2));
    }

    [Test]
    public void Equals_False_TypeDoesntMatch ()
    {
      var type1 = new InvolvedType (typeof (TargetClass1));
      var type2 = new InvolvedType (typeof (TargetClass2));

      Assert.That (type1, Is.Not.EqualTo (type2));
    }

    [Test]
    public void Equals_False_IsTargetDoesntMatch ()
    {
      var type1 = new InvolvedType (typeof (TargetClass1));
      type1.ClassContext = new ClassContext (typeof (TargetClass1));
      var type2 = new InvolvedType (typeof (TargetClass1));

      Assert.That (type1, Is.Not.EqualTo (type2));
    }

    [Test]
    public void Equals_False_IsMixinDoesntMatch ()
    {
      var mixinContext = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .BuildConfiguration().ClassContexts.First().Mixins.First();

      var type1 = new InvolvedType (typeof (TargetClass1));
      var type2 = new InvolvedType (typeof (Mixin1));
      type2.TargetTypes.Add (typeof (TargetClass1));

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
      var type1 = new InvolvedType (typeof (TargetClass1));
      var type2 = new InvolvedType (typeof (TargetClass1));

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
        var output = type1.ClassContext;
        Assert.Fail ("Expected exception was not thrown");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Involved type is not a target class"));
      }
    }

    [Test]
    public void MixinContextsProperty_ForNonMixin ()
    {
      var type1 = new InvolvedType (typeof (object));

      Assert.That (type1.IsMixin, Is.False);
      Assert.That (type1.TargetTypes.Count, Is.EqualTo (0));
    }

    [Test]
    public void MixinContextsProperty_ForMixin ()
    {
      var type1 = new InvolvedType (typeof (object));
      type1.TargetTypes.Add (typeof (TargetClass1));

      Assert.That (type1.IsMixin, Is.True);
      Assert.That (type1.TargetTypes.Count, Is.GreaterThan (0));
    }
  }
}