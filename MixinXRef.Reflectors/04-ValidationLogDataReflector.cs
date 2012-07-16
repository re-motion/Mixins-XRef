using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility;

namespace MixinXRef.Reflectors
{
  [ReflectorSupport ("Remotion", "1.13.133")]
  public class ValidationLogDataReflector : RemotionReflectorBase
  {
    public override ReflectedObject GetValidationLogFromValidationException (System.Exception validationException)
    {
      ArgumentUtility.CheckNotNull ("validationException", validationException);

      return new ReflectedObject (validationException).GetProperty ("ValidationLogData");
    }
  }
}
