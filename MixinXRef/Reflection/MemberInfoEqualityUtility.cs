using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection
{
  public class MemberInfoEqualityUtility
  {
    public static bool MemberEquals (object obj1, object obj2)
    {
      ArgumentUtility.CheckNotNull ("obj1", obj1);
      ArgumentUtility.CheckNotNull ("obj2", obj2);
      
      var memberInfo1 = obj1 as MemberInfo;
      var memberInfo2 = obj2 as MemberInfo;

      return memberInfo1 != null 
        && memberInfo2 != null
        && memberInfo1.DeclaringType == memberInfo2.DeclaringType
        && memberInfo1.MetadataToken == memberInfo2.MetadataToken
        ;
    }
  }
}