using System;

namespace MixinXRef.Utility
{
  public class ArgumentUtility
  {
    public static void CheckNotNull (string argumentName, object argumentValue)
    {
      if (argumentValue == null)
        throw new ArgumentNullException (argumentName);
    }
  }
}