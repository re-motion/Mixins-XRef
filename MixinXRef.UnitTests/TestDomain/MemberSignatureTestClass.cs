using System;
using Remotion.Collections;

namespace MixinXRef.UnitTests.TestDomain
{
  
  public class MemberSignatureTestClass : IDisposable
  {
    protected MemberSignatureTestClass (int Param1, string Param2, Remotion.Reflection.ApplicationAssemblyFinderFilter Param3)
    {}

    public void PublicMethod ()
    {
    }

    protected string ProtectedProperty { get; set; }

    protected internal event ChangedEventHandler ProtectedInternalEvent;

    internal string InternalField;

    private string _privateField;

    public virtual void PublicVirtualMethod()
    {
    }

    public class NestedClass
    { }

    public class NestedClassWithInterfaceAndInheritance : GenericTarget<string, int>, IDisposable
    {
      public void Dispose ()
      {
        throw new NotImplementedException();
      }
    }

    public interface INestedInterface : IDisposable, ICloneable
    {}

    public enum NestedEnumeration {}

    public struct NestedStruct : IDisposable {
      public void Dispose ()
      {
        throw new NotImplementedException();
      }
    }

    public void Dispose ()
    {
      throw new NotImplementedException();
    }

    public long MethodWithParams (int intParam, string stringParam, AssemblyBuilder assemblyBuilderParam) { return 0; }
    protected MultiDictionary<string, int> _dictionary;
  }
  
}