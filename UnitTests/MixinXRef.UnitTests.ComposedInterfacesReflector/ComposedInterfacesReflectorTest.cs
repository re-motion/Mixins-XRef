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
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using NUnit.Framework;
using Remotion.Mixins.Context;

namespace MixinXRef.UnitTests.ComposedInterfacesReflector
{
  [TestFixture]
  public class ComposedInterfacesReflectorTest
  {
    private IRemotionReflector _remotionReflector;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new Reflectors.ComposedInterfacesReflector ();
    }

    [Test]
    public void GetComposedInterfaces ()
    {
      var classContext = new ReflectedObject (new ClassContext (typeof (object), new MixinContext[0], new[] { typeof (int), typeof (double) }));

      var result = _remotionReflector.GetComposedInterfaces (classContext);

      Assert.That (result, Is.EqualTo (new[] { typeof (int), typeof (double) }));
    }
  }
}
