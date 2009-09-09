using System;

namespace MixinXRef.UnitTests.TestDomain
{
  public class FullReportGeneratorTestClass
  {
    protected void ProtectedMethod () {}

    public string _stringField;

    protected Type Property { get; set; }

    public class FullReportGeneratorNestedClass : IDisposable
    {
      public void Dispose ()
      {
        throw new NotImplementedException();
      }
    }
  }
}