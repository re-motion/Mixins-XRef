using System;

namespace MixinXRef.UnitTests.Remotion_1_13_23.TestDomain
{
  [Book (1337, Title = "C# in depth")]
  public class ClassWithBookAttribute
  {
  }

  public class BookAttribute : Attribute
  {
    private readonly int _id;


    public BookAttribute (int id)
    {
      _id = id;
    }

    public string Title { get; set; }
  }
}