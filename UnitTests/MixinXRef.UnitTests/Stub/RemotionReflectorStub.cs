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
    public RemotionReflectorStub (string component, Version version, IEnumerable<_Assembly> assemblies, IEnumerable<object> constructParameters)
      : base (component, version, assemblies, constructParameters)
    { }

    public void CallMethod (MethodBase method)
    {
      ReceivedReflector = GetCompatibleReflector (method);
    }

    public IRemotionReflector ReceivedReflector { get; private set; }
  }
}
