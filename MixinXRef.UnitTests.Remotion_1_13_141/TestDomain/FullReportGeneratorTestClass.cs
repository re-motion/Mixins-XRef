using System;

namespace MixinXRef.UnitTests.Remotion_1_13_141.TestDomain
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