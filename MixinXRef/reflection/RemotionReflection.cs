using System;
using System.Linq;
using System.Reflection;

namespace MixinXRef.Reflection
{
  public class RemotionReflection : IRemotionReflection
  {
    public bool IsNonApplicationAssembly (Assembly assembly)
    {
      return
          assembly.GetCustomAttributes (false).Any (
              attribute => attribute.GetType().FullName == "Remotion.Reflection.NonApplicationAssemblyAttribute");
    }

    public bool IsConfigurationException (Exception exception)
    {
      return exception.GetType().FullName == "Remotion.Mixins.ConfigurationException";
    }

    public bool IsValidationException (Exception exception)
    {
      return exception.GetType().FullName == "Remotion.Mixins.Validation.ValidationException";
    }
  }
}