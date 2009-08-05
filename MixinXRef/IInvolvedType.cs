using System;

namespace MixinXRef
{
  public interface IInvolvedType
  {
    Type Type { get; }
    bool IsTarget { get; }
    bool IsMixin { get; }
  }
}