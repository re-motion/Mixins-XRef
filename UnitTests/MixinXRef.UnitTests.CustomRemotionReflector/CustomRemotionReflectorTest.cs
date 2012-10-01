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
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using TalkBack;
using TalkBack.Brokers;

namespace MixinXRef.UnitTests.CustomRemotionReflector
{
  [TestFixture]
  public class CustomRemotionReflectorTest
  {
    [Test]
    public void UseCustomRemotionReflector_NonExistingType ()
    {
      var assemblyDir = Path.GetFullPath (@".");
      var outputDir = Path.GetFullPath (@"CustomReflectorOutput");

      var sender = MockRepository.GenerateStub<IMessageSender> ();
      sender.Expect (s => s.SendError ("Custom reflector can not be found"));

      var result = XRef.Run (new XRefArguments
      {
        AssemblyDirectory = assemblyDir,
        OutputDirectory = outputDir,
        ReflectorSource = ReflectorSource.CustomReflector,
        CustomReflectorAssemblyQualifiedTypeName =
          "Namespace.NonExistingType, MixinXRef.UnitTests.CustomRemotionReflector",
        OverwriteExistingFiles = true
      }, sender);

      Assert.That (result, Is.False);
    }

    [Test]
    public void UseCustomRemotionReflector_NonExistingAssembly ()
    {
      var assemblyDir = Path.GetFullPath (@".");
      var outputDir = Path.GetFullPath (@"CustomReflectorOutput");

      var sender = MockRepository.GenerateStub<IMessageSender> ();
      sender.Expect (s => s.SendError ("Custom reflector can not be found"));
     
      var result = XRef.Run(new XRefArguments
                 {
                   AssemblyDirectory = assemblyDir,
                   OutputDirectory = outputDir,
                   ReflectorSource = ReflectorSource.CustomReflector,
                   CustomReflectorAssemblyQualifiedTypeName =
                     "MixinXRef.UnitTests.CustomRemotionReflector.CustomRemotionReflector, NonExistingAssembly",
                   OverwriteExistingFiles = true
                 }, sender);

      Assert.That(result, Is.False);
    }
  }
}