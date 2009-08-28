using System;
using System.Reflection;

namespace MixinXRef
{
  public interface IRemotionReflection
  {
    bool IsNonApplicationAssembly (Assembly assembly);
  }
}