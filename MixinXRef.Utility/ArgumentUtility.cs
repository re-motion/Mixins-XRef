using System;
using System.Diagnostics;

namespace MixinXRef.Utility
{
  [DebuggerStepThrough]
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