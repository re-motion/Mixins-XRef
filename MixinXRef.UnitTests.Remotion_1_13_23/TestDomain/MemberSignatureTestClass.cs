using System;
using MixinXRef.UnitTests.Remotion_1_13_23.TestDomain;
using Remotion.Collections;

namespace MixinXRef.UnitTests.Remotion_1_13_23.TestDomain
{

  public class MemberSignatureTestClass : IExplicitInterface
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
    {
      float Calculate ();  
    }

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

    public interface INestedExplicitInterface : IExplicitInterface
    {
      
    }

    public class SubClass :IExplicitInterface
    {
      string IExplicitInterface.Version ()
      {
        throw new NotImplementedException();
      }

      int IExplicitInterface.Count
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      event EventHandler IExplicitInterface.eventHandler
      {
        add { throw new NotImplementedException(); }
        remove { throw new NotImplementedException(); }
      }
    }

    // explicit interface
    string IExplicitInterface.Version ()
    {
      throw new NotImplementedException();
    }

    int IExplicitInterface.Count
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    event EventHandler IExplicitInterface.eventHandler
    {
      add { throw new NotImplementedException(); }
      remove { throw new NotImplementedException(); }
    }
  }


  public interface IExplicitInterface
  {
    string Version ();
    int Count { get; set; }
    event EventHandler eventHandler;
  }
}