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