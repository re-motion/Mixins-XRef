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
using System.Linq;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.UnitTests.NewMixinDependenciesReflector.TestDomain;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;

namespace MixinXRef.UnitTests.NewMixinDependenciesReflector
{
  [TestFixture]
  public class NewMixinDependenciesReflectorTest
  {
    private IRemotionReflector _remotionReflector;
    private TargetClassDefinition _targetClassDefinition;
    private MixinDefinition _mixinDefinition;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new Reflectors.NewMixinDependenciesReflector ();

      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ()
          .AddMixin<Mixin1> ()
          .BuildConfiguration ();

      _targetClassDefinition = TargetClassDefinitionFactory.CreateTargetClassDefinition (mixinConfiguration.ClassContexts.First ());
      _mixinDefinition = _targetClassDefinition.GetMixinByConfiguredType (typeof (Mixin1));
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
