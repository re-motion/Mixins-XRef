using System;
using System.IO;
using System.Reflection;
using MixinXRef.Reflection.RemotionReflector;

namespace MixinXRef.UnitTests.Helpers
{
  public static class RemotionReflectorFactory
  {
    public static IRemotionReflector GetRemotionReflection ()
    {
      // TODO Replace with mock if possible
      return new RemotionReflector ("Remotion", new Version ("1.11.20"), new[] { Assembly.LoadFile (Path.GetFullPath ("MixinXRef.Reflectors.dll")) }, ".");
    }
  }
}
