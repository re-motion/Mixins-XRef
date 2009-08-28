using System;
using System.Reflection;

namespace MixinXRef.Reflection
{
  public interface IRemotionReflection
  {
    bool IsNonApplicationAssembly (Assembly assembly);
    bool IsConfigurationException (Exception exception);
    bool IsValidationException (Exception exception);
  }
}