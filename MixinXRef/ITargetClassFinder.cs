using System;

namespace MixinXRef
{
  public interface ITargetClassFinder
  {
    Type[] FindTargetClasses ();
  }
}