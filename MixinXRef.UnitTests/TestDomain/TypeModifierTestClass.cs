using System;

namespace MixinXRef.UnitTests.TestDomain
{
  public class TypeModifierTestClass
  {
    protected class ProtectedClass {}
    protected internal class ProtectedInternalClass { }
    internal class InternalClass { }
    private struct PrivateStruct {}
  }

  internal class TopLevelInternalClass { }
}