using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;

namespace MixinXRef.UnitTests.Stub
{
  public class RemotionReflectorStub : ReflectorProvider
  {
    public RemotionReflectorStub (string component, Version version, IEnumerable<_Assembly> assemblies, string assemblyDirectory)
      : base (component, version, assemblies, assemblyDirectory)
    { }

    public void CallMethod (MethodBase method)
    {
      ReceivedReflector = GetCompatibleReflector (method);
    }

    public IRemotionReflector ReceivedReflector { get; private set; }
  }
}
