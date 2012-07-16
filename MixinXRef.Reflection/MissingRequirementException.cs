using System;

namespace MixinXRef.Reflection
{
  internal class MissingRequirementException : Exception
  {
    public MissingRequirementException(string requiredAssembly) : base(string.Format("The required assembly '{0}' could not be found", requiredAssembly))
    {
      
    }
  }
}