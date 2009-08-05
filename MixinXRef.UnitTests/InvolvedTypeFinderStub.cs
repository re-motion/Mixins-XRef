using System;

namespace MixinXRef.UnitTests
{
  public class InvolvedTypeFinderStub : IInvolvedTypeFinder
  {
    private readonly Type[] _types;

    public InvolvedTypeFinderStub (params Type[] types)
    {
      _types = types;
    }

    public Type[] FindInvolvedTypes ()
    {
      return _types;
    }
  }
}