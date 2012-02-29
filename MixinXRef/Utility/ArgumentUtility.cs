using System;

namespace MixinXRef.Utility
{
  public class ArgumentUtility
  {
    public static T CheckNotNull<T> (string argumentName, T argumentValue)
    {
// ReSharper disable CompareNonConstrainedGenericWithNull
      if (argumentValue == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
        throw new ArgumentNullException (argumentName);

      return argumentValue;
    }
  }
}