using System;

namespace MixinXRef.UnitTests
{
  public class InvolvedTypeFinderStub : IInvolvedTypeFinder
  {
    private readonly InvolvedType[] _types;

    public InvolvedTypeFinderStub (params InvolvedType[] types)
    {
      _types = types;
    }

    public InvolvedType[] FindInvolvedTypes ()
    {
      return _types;
    }
  }
}