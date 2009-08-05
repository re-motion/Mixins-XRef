using System;

namespace MixinXRef.UnitTests
{
  public class InvolvedTypeFinderStub : IInvolvedTypeFinder
  {
    private readonly IInvolvedType[] _types;

    public InvolvedTypeFinderStub (params IInvolvedType[] types)
    {
      _types = types;
    }

    public IInvolvedType[] FindInvolvedTypes ()
    {
      return _types;
    }
  }
}