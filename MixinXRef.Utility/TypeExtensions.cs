using System;
using System.Linq;

namespace MixinXRef.Utility
{
  public static class TypeExtensions
  {
    public static T GetAttribute<T> (this Type type, bool inherit = false) where T : Attribute
    {
      return (T) type.GetCustomAttributes (typeof (T), inherit).FirstOrDefault ();
    }
  }
}
