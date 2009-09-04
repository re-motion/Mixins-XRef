using System;

namespace MixinXRef.UnitTests.TestDomain
{
  public class TypeModifierTestClass
  {
    protected TypeModifierTestClass (int Param1, string Param2, Remotion.Reflection.ApplicationAssemblyFinderFilter Param3)
    {}

    protected class ProtectedClass {}
    protected internal class ProtectedInternalClass { }
    internal class InternalClass { }
    private class PrivateClass{} 
    public struct PublicStruct {}
  }

  internal class TopLevelInternalClass {}

  public sealed class PublicSealedClass {}

  public abstract class PublicAbstractClass {}
}