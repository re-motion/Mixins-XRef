using System;

namespace MixinXRef.UnitTests.TestDomain
{
  
  public class ModifierTestClass
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
  }

  public delegate void ChangedEventHandler(object sender, EventArgs e);
}