using System;

namespace MixinXRef.UnitTests.TestDomain
{
  
  public abstract class ModifierTestClass
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
  }
  public delegate void ChangedEventHandler(object sender, EventArgs e);


  public abstract class SubModifierTestClass : ModifierTestClass
  {
    public abstract override void PublicAbstractMethod();
  }
  
}