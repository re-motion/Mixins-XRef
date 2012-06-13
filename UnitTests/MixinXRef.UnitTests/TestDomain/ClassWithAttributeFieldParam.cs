using System;

namespace MixinXRef.UnitTests.TestDomain
{
  [FieldParam(new[] { "AttributeParam1", "AttributeParam2"})]
  public class ClassWithAttributeFieldParam 
  {
  }

  public class FieldParamAttribute : Attribute
  {
    public FieldParamAttribute (string[] stringArray)
    {
    }
  }
}