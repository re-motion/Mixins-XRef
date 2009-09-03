using System;

namespace MixinXRef.UnitTests.TestDomain
{
  
  public class VisibilityTestClass
  {
    public void PublicMethod ()
    {
    }

    protected virtual string ProtectedProperty { get; set; }

    protected internal event ChangedEventHandler ProtectedInternalEvent;

    internal string InternalField;

    private string _privateField;
  }

  public delegate void ChangedEventHandler(object sender, EventArgs e);
}