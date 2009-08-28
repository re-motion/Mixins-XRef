using System;
using System.Linq;
using System.Reflection;

namespace MixinXRef
{
  public class RemotionReflection : IRemotionReflection
  {
    public bool IsNonApplicationAssembly (Assembly assembly)
    {
      return assembly.GetCustomAttributes (false).Any (attribute => attribute.GetType().FullName == "Remotion.Reflection.NonApplicationAssemblyAttribute");
    }
  }
}