using System.Collections.Generic;
using System.Reflection;

namespace MixinXRef.Utility
{
  public class MemberDefinitionEqualityComparer : IEqualityComparer<MemberInfo>
  {
    public bool Equals (MemberInfo x, MemberInfo y)
    {
      return x != null && y != null
             && x.DeclaringType == y.DeclaringType
             && x.MetadataToken == y.MetadataToken;
    }

    public int GetHashCode (MemberInfo obj)
    {
      return obj.DeclaringType.GetHashCode () ^ obj.MetadataToken.GetHashCode ();
    }
  }
}
