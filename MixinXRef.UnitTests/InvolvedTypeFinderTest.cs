using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace MixinXRef.UnitTests
{
  [TestFixture]
  public class InvolvedTypeFinderTest
  {
    private ErrorAggregator<Exception> _configurationErrors;
    private ErrorAggregator<Exception> _validationErrors;

    [SetUp]
    public void SetUp ()
    {
      _configurationErrors = new ErrorAggregator<Exception>();
      _validationErrors = new ErrorAggregator<Exception>();
    }

    [Test]
    public void FindInvolvedTypes_EmptyConfiguration ()
    {
      var mixinConfiguration = new MixinConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder (mixinConfiguration, new Assembly[0]);

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      Assert.That (involvedTypes, Is.Empty);
    }

    [Test]
    public void FindInvolvedTypes_WithOneTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass1>().AddMixin<Mixin1>().BuildConfiguration();
      var assembly = typeof (Mixin1).Assembly;
      var involvedTypeFinder = CreateInvolvedTypeFinder (mixinConfiguration, new[] { assembly });

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType (typeof (TargetClass1));
      expectedType1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());
      expectedType1.TargetClassDefintion = CreateTargetClassDefintion<TargetClass1> (mixinConfiguration);

      var expectedType2 = new InvolvedType (typeof (Mixin1));
      expectedType2.TargetTypes.Add (
          typeof (TargetClass1), expectedType1.TargetClassDefintion.CallMethod ("GetMixinByConfiguredType", typeof (Mixin1)));

      Assert.That (involvedTypes, Is.EquivalentTo (GetAdditonalAssemblyInvolvedTypes (expectedType1, expectedType2)));
    }

    [Test]
    public void FindInvolvedTypes_WithMoreTargets ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder (mixinConfiguration, new[] { typeof (Mixin1).Assembly });

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType (typeof (TargetClass1));
      expectedType1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());
      expectedType1.TargetClassDefintion = CreateTargetClassDefintion<TargetClass1> (mixinConfiguration);

      var expectedType2 = new InvolvedType (typeof (Mixin1));
      expectedType2.TargetTypes.Add (
          typeof (TargetClass1), expectedType1.TargetClassDefintion.CallMethod ("GetMixinByConfiguredType", typeof (Mixin1)));

      var expectedType3 = new InvolvedType (typeof (TargetClass2));
      expectedType3.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.Last());
      expectedType3.TargetClassDefintion = CreateTargetClassDefintion<TargetClass2> (mixinConfiguration);

      var expectedType4 = new InvolvedType (typeof (Mixin2));
      expectedType4.TargetTypes.Add (
          typeof (TargetClass2), expectedType3.TargetClassDefintion.CallMethod ("GetMixinByConfiguredType", typeof (Mixin2)));

      Assert.That (
          involvedTypes, Is.EquivalentTo (GetAdditonalAssemblyInvolvedTypes (expectedType1, expectedType2, expectedType3, expectedType4)));
    }

    [Test]
    public void FindInvolvedTypes_WithMixedMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<Mixin1>().AddMixin<Mixin2>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder (mixinConfiguration, new[] { typeof (Mixin1).Assembly });

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType (typeof (TargetClass1));
      expectedType1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());
      expectedType1.TargetClassDefintion = CreateTargetClassDefintion<TargetClass1> (mixinConfiguration);

      var expectedType2 = new InvolvedType (typeof (Mixin1));
      expectedType2.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.Last());
      expectedType2.TargetClassDefintion = CreateTargetClassDefintion<Mixin1> (mixinConfiguration);
      expectedType2.TargetTypes.Add (
          typeof (TargetClass1), expectedType1.TargetClassDefintion.CallMethod ("GetMixinByConfiguredType", typeof (Mixin1)));

      var expectedType3 = new InvolvedType (typeof (Mixin2));
      expectedType3.TargetTypes.Add (typeof (Mixin1), expectedType2.TargetClassDefintion.CallMethod ("GetMixinByConfiguredType", typeof (Mixin2)));

      Assert.That (involvedTypes, Is.EquivalentTo (GetAdditonalAssemblyInvolvedTypes (expectedType1, expectedType2, expectedType3)));
    }

    [Test]
    public void FindInvolvedTypes_WithTargetClassInheritance ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<Mixin1>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder (mixinConfiguration, new[] { typeof (Mixin1).Assembly });

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType (typeof (UselessObject));
      expectedType1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());
      expectedType1.TargetClassDefintion = CreateTargetClassDefintion<UselessObject> (mixinConfiguration);

      var expectedType2 = new InvolvedType (typeof (ClassInheritsFromUselessObject));
      expectedType2.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.GetWithInheritance (typeof (ClassInheritsFromUselessObject)));
      expectedType2.TargetClassDefintion = CreateTargetClassDefintion<ClassInheritsFromUselessObject> (mixinConfiguration);

      var expectedType3 = new InvolvedType (typeof (Mixin1));
      expectedType3.TargetTypes.Add (
          typeof (UselessObject), expectedType1.TargetClassDefintion.CallMethod ("GetMixinByConfiguredType", typeof (Mixin1)));
      expectedType3.TargetTypes.Add (
          typeof (ClassInheritsFromUselessObject), expectedType2.TargetClassDefintion.CallMethod ("GetMixinByConfiguredType", typeof (Mixin1)));


      Assert.That (involvedTypes, Is.EquivalentTo (GetAdditonalAssemblyInvolvedTypes (expectedType1, expectedType2, expectedType3)));
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder (mixinConfiguration, typeof (UselessObject).Assembly);
      var targetType = typeof (TargetClass1);
      var classContextForTargetType = new ReflectedObject (mixinConfiguration.ClassContexts.GetWithInheritance (targetType));

      var output = involvedTypeFinder.GetTargetClassDefinition (targetType, classContextForTargetType).To<TargetClassDefinition> ();
      var expectedOutput = TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration);

      Assert.That (output, Is.EqualTo (expectedOutput));
    }

    [Test]
    public void GetTargetClassDefinition_GenericTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass (typeof (GenericTarget<,>)).AddMixin<Mixin1>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder (mixinConfiguration, typeof (UselessObject).Assembly);
      var targetType = typeof (GenericTarget<,>);
      var classContextForTargetType = new ReflectedObject (mixinConfiguration.ClassContexts.GetWithInheritance (targetType));
      var output = involvedTypeFinder.GetTargetClassDefinition (targetType, classContextForTargetType);

      Assert.That (_configurationErrors.Exceptions.Count(), Is.EqualTo (0));
      Assert.That (_validationErrors.Exceptions.Count(), Is.EqualTo (0));
      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_MixinConfigurationError ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<MixinWithConfigurationError>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder (mixinConfiguration, typeof (UselessObject).Assembly);
      var targetType = typeof (UselessObject);
      var classContextForTargetType = new ReflectedObject (mixinConfiguration.ClassContexts.GetWithInheritance (targetType));

      var output = involvedTypeFinder.GetTargetClassDefinition (targetType, classContextForTargetType);

      Assert.That (_configurationErrors.Exceptions.Count(), Is.EqualTo (1));
      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_MixinValidationError ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<UselessObject>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder (mixinConfiguration, typeof (UselessObject).Assembly);
      var targetType = typeof (UselessObject);
      var classContextForTargetType = new ReflectedObject (mixinConfiguration.ClassContexts.GetWithInheritance (targetType));

      var output = involvedTypeFinder.GetTargetClassDefinition (targetType, classContextForTargetType);

      Assert.That (_validationErrors.Exceptions.Count(), Is.EqualTo (1));
      Assert.That (output, Is.Null);
    }

    private InvolvedTypeFinder CreateInvolvedTypeFinder (MixinConfiguration mixinConfiguration, params Assembly[] assemblies)
    {
      return new InvolvedTypeFinder (
          new ReflectedObject (mixinConfiguration),
          assemblies,
          _configurationErrors,
          _validationErrors,
          ProgramTest.GetRemotionReflection());
    }

    private ReflectedObject CreateTargetClassDefintion<ForType> (MixinConfiguration mixinConfiguration)
    {
      return new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (typeof (ForType), mixinConfiguration));
    }

    private InvolvedType[] GetAdditonalAssemblyInvolvedTypes (params InvolvedType[] explicitInvolvedTypes)
    {
      var implicitInvolvedTypes = new List<InvolvedType>();
      var remotionReflector = ProgramTest.GetRemotionReflection();
      var assembly = typeof (Mixin1).Assembly;

      foreach (var type in assembly.GetTypes())
      {
        // also add classes which inherit from Mixin<> or Mixin<,>, but are actually not used as Mixins (not in ClassContexts)
        if (remotionReflector.IsInheritedFromMixin (type) && !remotionReflector.IsInfrastructureType (type))
          implicitInvolvedTypes.Add (new InvolvedType (type));
      }

      var allInvolvedTypes = new InvolvedType[explicitInvolvedTypes.Length + implicitInvolvedTypes.Count];
      explicitInvolvedTypes.CopyTo (allInvolvedTypes, 0);
      implicitInvolvedTypes.CopyTo (allInvolvedTypes, explicitInvolvedTypes.Length);

      return allInvolvedTypes;
    }
  }
}