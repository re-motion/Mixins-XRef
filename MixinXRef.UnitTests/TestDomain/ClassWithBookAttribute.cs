using System;

namespace MixinXRef.UnitTests.TestDomain
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