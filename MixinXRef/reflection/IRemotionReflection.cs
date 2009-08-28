using System;
using System.Reflection;

namespace MixinXRef.Reflection
{
  public interface IRemotionReflection
  {
    bool IsNonApplicationAssembly (Assembly assembly);
  }
}