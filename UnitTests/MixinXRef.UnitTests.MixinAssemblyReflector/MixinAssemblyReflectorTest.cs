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
using MixinXRef.Utility;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using ComposedInterfacesTestClass = MixinXRef.UnitTests.MixinAssemblyReflector.TestDomain.ComposedInterfacesTestClass;
using Mixin1 = MixinXRef.UnitTests.MixinAssemblyReflector.TestDomain.Mixin1;
using TargetClass1 = MixinXRef.UnitTests.MixinAssemblyReflector.TestDomain.TargetClass1;

namespace MixinXRef.UnitTests.MixinAssemblyReflector
{
  [TestFixture]
  public class MixinAssemblyReflectorTest
  {
    private IRemotionReflector _remotionReflector;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new Reflectors.MixinAssemblyReflector().Initialize (".");
    }

    [Test]
    public void IsRelevantAssemblyForConfiguration ()
    {
      var assembly1 = typeof (IDisposable).Assembly;
      var assembly2 = typeof (MixinAssemblyReflectorTest).Assembly;

      Assert.IsFalse (_remotionReflector.IsRelevantAssemblyForConfiguration (assembly1));
      Assert.IsTrue (_remotionReflector.IsRelevantAssemblyForConfiguration (assembly2));
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
    public void BuildConfigurationFromAssemblies ()
    {
      var assemblies = new[] { typeof (TargetClass1).Assembly, typeof (object).Assembly };

      var reflectedOuput = _remotionReflector.BuildConfigurationFromAssemblies (assemblies);
      var expectedOutput = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);

      Assert.That (reflectedOuput.To<MixinConfiguration> ().ClassContexts, Is.EqualTo (expectedOutput.ClassContexts));
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ().AddMixin<Mixin1> ()
          .BuildConfiguration ();

      var reflectedOutput = _remotionReflector.GetTargetClassDefinition (typeof (TargetClass1), new ReflectedObject (mixinConfiguration), new ReflectedObject (mixinConfiguration.ClassContexts.GetWithInheritance (typeof (TargetClass1))));
      var expectedOutput = TargetClassDefinitionFactory.CreateTargetClassDefinition (mixinConfiguration.ClassContexts.First ());

      Assert.That (reflectedOutput.To<TargetClassDefinition> (), Is.InstanceOf (typeof (TargetClassDefinition)));
      Assert.That (reflectedOutput.To<TargetClassDefinition> ().FullName, Is.EqualTo (expectedOutput.FullName));
    }
  }
}
