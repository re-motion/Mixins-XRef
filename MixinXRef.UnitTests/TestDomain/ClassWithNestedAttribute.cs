using System;

namespace MixinXRef.UnitTests.TestDomain
{
  [Nested]
  public class ClassWithNestedAttribute
  {
    public class NestedAttribute : Attribute
    {
    }
  }
}