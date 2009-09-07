using System;

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

    public virtual void PublicVirtualMethod() {}

    public abstract void PublicAbstractMethod();

    public void Dispose ()
    {
      throw new NotImplementedException ();
    }


    public readonly string _readonlyField;

    public class NestedClass {}

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