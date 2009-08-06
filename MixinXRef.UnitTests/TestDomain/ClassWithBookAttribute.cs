using System;

namespace MixinXRef.UnitTests.TestDomain
{
  [Book (1, Title = "C# in depth")]
  public class ClassWithBookAttribute
  {
  }

  internal class BookAttribute : Attribute
  {
    private readonly int _id;


    public BookAttribute (int id)
    {
      _id = id;
    }

    public string Title { get; set; }
  }
}