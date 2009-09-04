using System;
using System.Collections.Generic;
using Remotion.Collections;

namespace MixinXRef.UnitTests.TestDomain
{
  
  public abstract class MemberModifierTestClass : IDisposable
  {
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

    public abstract void PublicAbstractMethod();


    public class NestedClass
    {
    }

    public void Dispose ()
    {
      throw new NotImplementedException();
    }

    public long MethodWithParams (int intParam, string stringParam, AssemblyBuilder assemblyBuilderParam) { return 0; }
    protected MultiDictionary<string, int> _dictionary;
  }
  public delegate void ChangedEventHandler(object sender, EventArgs e);


  public abstract class SubModifierTestClass : MemberModifierTestClass
  {
    public abstract override void PublicAbstractMethod();

    public override sealed void PublicVirtualMethod()
    {
    }
  }
  
}