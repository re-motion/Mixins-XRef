using System;
using System.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
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
      type1.ClassContext = new ReflectedObject (new ClassContext (typeof (TargetClass1)));
      var type2 = new InvolvedType (typeof (TargetClass1));
      type2.ClassContext = new ReflectedObject (new ClassContext (typeof (TargetClass1)));

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
      type1.ClassContext = new ReflectedObject (new ClassContext (typeof (TargetClass1)));
      var type2 = new InvolvedType (typeof (TargetClass1));

      Assert.That (type1, Is.Not.EqualTo (type2));
    }

    [Test]
    public void Equals_False_IsMixinDoesntMatch ()
    {
      var type1 = new InvolvedType (typeof (TargetClass1));
      var type2 = new InvolvedType (typeof (Mixin1));
      type2.TargetTypes.Add (new InvolvedType(typeof (TargetClass1)), null);

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

      type1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());
      type2.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.Last());

      Assert.That (type1, Is.Not.EqualTo (type2));
    }

    [Test]
    public void Equals_False_TargetClassDefintionDoesntMatch()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var type1 = new InvolvedType(typeof(TargetClass1));
      var type2 = new InvolvedType(typeof(TargetClass1));

      type1.TargetClassDefinition = new ReflectedObject(TargetClassDefinitionUtility.GetConfiguration(typeof(TargetClass1), mixinConfiguration));
      type2.TargetClassDefinition = new ReflectedObject(TargetClassDefinitionUtility.GetConfiguration(typeof(TargetClass2), mixinConfiguration));

      Assert.That(type1, Is.Not.EqualTo(type2));
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
      type1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

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
        Assert.That (ex.Message, Is.EqualTo ("Involved type is not a target class."));
      }
    }

    [Test]
    public void TargetClassDefinitionProperty_ForNonGenericTargetClass()
    {
      var type = typeof(TargetClass1);
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass(type).AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType(type);
      type1.ClassContext = new ReflectedObject(mixinConfiguration.ClassContexts.First());
      type1.TargetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (type, mixinConfiguration));

      Assert.That(type1.IsTarget, Is.True);
      Assert.That(type1.ClassContext, Is.Not.Null);
    }

    [Test]
    public void TargetClassDefinitionProperty_ForNonTargetClass ()
    {
      var type1 = new InvolvedType (typeof (object));

      Assert.That (type1.IsTarget, Is.False);
      try
      {
        var output = type1.TargetClassDefinition;
        Assert.Fail ("Expected exception was not thrown");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Involved type is either not a target class or a generic target class."));
      }
    }

    [Test]
    public void TargetClassDefinitionProperty_ForGenericTargetClass ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass (typeof (GenericTarget<,>)).AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (GenericTarget<,>));
      type1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

      Assert.That (type1.IsTarget, Is.True);
      try
      {
        var output = type1.TargetClassDefinition;
        Assert.Fail ("Expected exception was not thrown");
      }
      catch (InvalidOperationException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("Involved type is either not a target class or a generic target class."));
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
      type1.TargetTypes.Add (new InvolvedType(typeof (TargetClass1)), null);

      Assert.That (type1.IsMixin, Is.True);
      Assert.That (type1.TargetTypes.Count, Is.GreaterThan (0));
    }
  }
}