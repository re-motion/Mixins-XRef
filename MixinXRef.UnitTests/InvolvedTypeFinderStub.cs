using System;

namespace MixinXRef.UnitTests
{
  public class InvolvedTypeFinderStub : ITargetClassFinder
  {
    private readonly Type[] _types;

    public InvolvedTypeFinderStub (params Type[] types)
    {
      _types = types;
    }

    public Type[] FindTargetClasses ()
    {
      return _types;
    }
  }
}