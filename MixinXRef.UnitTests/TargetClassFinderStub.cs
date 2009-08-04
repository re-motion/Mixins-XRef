using System;

namespace MixinXRef.UnitTests
{
  public class TargetClassFinderStub : ITargetClassFinder
  {
    private readonly Type[] _types;

    public TargetClassFinderStub (params Type[] types)
    {
      _types = types;
    }

    public Type[] FindTargetClasses ()
    {
      return _types;
    }
  }
}