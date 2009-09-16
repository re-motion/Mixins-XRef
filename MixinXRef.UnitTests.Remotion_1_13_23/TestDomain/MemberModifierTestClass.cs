using System;

namespace MixinXRef.UnitTests.Remotion_1_13_23.TestDomain
{
  
  public abstract class MemberModifierTestClass : IDisposable
  {
    static MemberModifierTestClass ()
    {
    }

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

    public static int _staticField;

    public static void StaticMethod ()
    {}

    public static class NestedStaticClass
    {}

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