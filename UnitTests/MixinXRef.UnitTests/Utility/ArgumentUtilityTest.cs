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
using System.Diagnostics;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class ArgumentUtilityTest
  {
    [Test]
    public void CheckNotNull_NotNull ()
    {
      ArgumentUtility.CheckNotNull ("name", "value");
    }

    [Test]
    public void CheckNotNull_IsNull ()
    {
      try
      {
        object obj = null;
        ArgumentUtility.CheckNotNull ("name", obj);
        Assert.Fail ("expected exception not thrown");
      }
      catch (ArgumentNullException argumentNullException)
      {
        Assert.That (argumentNullException.Message, Is.EqualTo ("Value cannot be null.\r\nParameter name: name"));
      }
    }

    [Test]
    public void CheckNotNull_ReturnsSame ()
    {
      var original = new object();
      var returned = ArgumentUtility.CheckNotNull ("name", original);

      Assert.That (returned, Is.SameAs (original));
    }
  }
}