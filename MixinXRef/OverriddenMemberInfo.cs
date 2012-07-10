using System.Reflection;

namespace MixinXRef
{
  public class OverriddenMemberInfo
  {
    public MemberInfo BaseMemberInfo { get; private set; }
    public MemberInfo OverrideMemberInfo { get; private set; }

    public OverriddenMemberInfo(MemberInfo overriddenMember, MemberInfo overridingMember)
    {
      BaseMemberInfo = overriddenMember;
      OverrideMemberInfo = overridingMember;
    }
  }
}