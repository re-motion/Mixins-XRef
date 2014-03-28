// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 

using System;
using System.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.UnitTests.Net4_5SupportReflector.TestDomain;
using MixinXRef.UnitTests.NonApplicationAssembly;
using MixinXRef.Utility;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.Reflection.TypeDiscovery;

namespace MixinXRef.UnitTests.Net4_5SupportReflector
{
  [TestFixture]
  public class Net4_5SupportReflectorTest
  {
    private IRemotionReflector _remotionReflector;
    private TargetClassDefinition _targetClassDefinition;
    private MixinDefinition _mixinDefinition;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new Reflectors.Net4_5SupportReflector ().Initialize(".");

      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ()
          .AddMixin<Mixin1> ()
          .BuildConfiguration ();

      _targetClassDefinition = TargetClassDefinitionFactory.CreateAndValidate (mixinConfiguration.ClassContexts.First ());
      _mixinDefinition = _targetClassDefinition.GetMixinByConfiguredType (typeof (Mixin1));
    }

    [Test]
    public void IsRelevantAssemblyForConfiguration ()
    {
      var assembly1 = typeof (IDisposable).Assembly;
      var assembly2 = typeof (Net4_5SupportReflectorTest).Assembly;

      Assert.IsFalse (_remotionReflector.IsRelevantAssemblyForConfiguration (assembly1));
      Assert.IsTrue (_remotionReflector.IsRelevantAssemblyForConfiguration (assembly2));
    }

    [Test]
    public void IsNonApplicationAssembly_False ()
    {
      var assembly = typeof (IDisposable).Assembly;
      var output = _remotionReflector.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.False);
    }

    [Test]
    public void IsNonApplicationAssembly_True ()
    {
      var assembly = typeof (ClassForNonApplicationAssembly).Assembly;
      var output = _remotionReflector.IsNonApplicationAssembly (assembly);

      Assert.That (output, Is.True);
    }

    [Test]
    public void IsConfigurationException ()
    {
      var configurationException = new ConfigurationException ("configurationException");

      var outputTrue = _remotionReflector.IsConfigurationException (configurationException);
      var outputFalse = _remotionReflector.IsConfigurationException (new Exception ());

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsValidationException ()
    {
      var validationLogData = new ValidationLogData ();
      var validationException = new ValidationException (validationLogData);

      var outputTrue = _remotionReflector.IsValidationException (validationException);
      var outputFalse = _remotionReflector.IsValidationException (new Exception ());

      Assert.That (outputTrue, Is.True);
      Assert.That (outputFalse, Is.False);
    }

    [Test]
    public void IsInfrastructureType ()
    {
      var initializableMixinType = typeof (IInitializableMixin);

      var outputTrue = _remotionReflector.IsInfrastructureType (initializableMixinType);
      var outputFalse = _remotionReflector.IsInfrastructureType (typeof (IDisposable));

      Assert.IsTrue (outputTrue);
      Assert.IsFalse (outputFalse);
    }

    [Test]
    public void IsInheritedFromMixin ()
    {
      var outputTrue1 = _remotionReflector.IsInheritedFromMixin (typeof (Mixin<>));
      // Mixin<,> inherits from Mixin<>
      var outputTrue2 = _remotionReflector.IsInheritedFromMixin (typeof (Mixin<,>));
      // MemberOverrideWithInheritanceTest.CustomMixin inherits from Mixin<>
      var outputTrue3 = _remotionReflector.IsInheritedFromMixin (typeof (ComposedInterfacesTestClass.MyMixin));
      var outputFalse = _remotionReflector.IsInheritedFromMixin (typeof (object));

      Assert.True (outputTrue1);
      Assert.True (outputTrue2);
      Assert.True (outputTrue3);
      Assert.False (outputFalse);
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ()
          .AddMixin<Mixin1> ()
          .BuildConfiguration ();

      var reflectedOutput = _remotionReflector.GetTargetClassDefinition (typeof (TargetClass1), new ReflectedObject (mixinConfiguration), new ReflectedObject (mixinConfiguration.ClassContexts.GetWithInheritance (typeof (TargetClass1))));
      var expectedOutput = TargetClassDefinitionFactory.CreateAndValidate (mixinConfiguration.ClassContexts.First ());

      Assert.That (reflectedOutput.To<TargetClassDefinition> (), Is.InstanceOf (typeof (TargetClassDefinition)));
      Assert.That (reflectedOutput.To<TargetClassDefinition> ().FullName, Is.EqualTo (expectedOutput.FullName));
    }

    [Test]
    public void IsNonApplicationAssembly ()
    {
      var assembly1 = typeof (IDisposable).Assembly;
      var assembly2 = typeof (ClassForNonApplicationAssembly).Assembly;

      Assert.IsFalse (_remotionReflector.IsNonApplicationAssembly (assembly1));
      Assert.IsTrue (_remotionReflector.IsNonApplicationAssembly (assembly2));
    }

    [Test]
    public void InitializeLogging ()
    {
      Assert.That (() => _remotionReflector.InitializeLogging(), Throws.Nothing);
    }

    [Test]
    public void GetTypeDiscoveryService ()
    {
      var expected = ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService();
      var actual = _remotionReflector.GetTypeDiscoveryService();

      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void BuildConfigurationFromAssemblies ()
    {
      var assemblies = new[] { typeof (TargetClass1).Assembly, typeof (object).Assembly };

      var reflectedOuput = _remotionReflector.BuildConfigurationFromAssemblies (assemblies);
      var expectedOutput = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);

      Assert.That (reflectedOuput.To<MixinConfiguration> ().ClassContexts, Is.EqualTo (expectedOutput.ClassContexts));
    }

    [Test]
    public void GetValidationLogFromValidationException ()
    {
      var validationLogData = new ValidationLogData ();
      var validationException = new ValidationException (validationLogData);

      var reflectedValidationLog = _remotionReflector.GetValidationLogFromValidationException (validationException);
      var result = reflectedValidationLog.To<ValidationLogNullObject> ();
      Assert.That (result, Is.Not.Null);
    }

    [Test]
    public void GetComposedInterfaces ()
    {
      var classContext = new ReflectedObject (new ClassContext (typeof (object), new MixinContext[0], new[] { typeof (int), typeof (double) }));

      var result = _remotionReflector.GetComposedInterfaces (classContext);

      Assert.That (result, Is.EqualTo (new[] { typeof (int), typeof (double) }));
    }

    [Test]
    public void GetNextCallDependencies ()
    {
      var nextCallDependencies = _mixinDefinition.NextCallDependencies;
      var output = _remotionReflector.GetNextCallDependencies (new ReflectedObject (_mixinDefinition));

      Assert.That (output, Is.EquivalentTo (nextCallDependencies));
    }

    [Test]
    public void GetTargetCallDependencies ()
    {
      var targetCallDependencies = _mixinDefinition.TargetCallDependencies;
      var output = _remotionReflector.GetTargetCallDependencies (new ReflectedObject (_mixinDefinition));

      Assert.That (output, Is.EquivalentTo (targetCallDependencies));
    }

  }
}
