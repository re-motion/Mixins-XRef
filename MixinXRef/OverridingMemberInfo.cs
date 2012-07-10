using System;
using System.Collections.Generic;
using System.Reflection;

namespace MixinXRef
{
  public class OverridingMemberInfo
  {
    private readonly MemberInfo _memberInfo;

    public enum OverrideType
    {
      Target,
      Mixin
    }

    private readonly IList<MemberInfo> _overriddenTargetMembers = new List<MemberInfo>();
    public IEnumerable<MemberInfo> OverriddenTargetMembers { get { return _overriddenTargetMembers; } }
    private readonly IList<MemberInfo> _overriddenMixinMembers = new List<MemberInfo> ();
    public IEnumerable<MemberInfo> OverriddenMixinMembers { get { return _overriddenMixinMembers; } }
    public OverridingMemberInfo(MemberInfo memberInfo)
    {
      _memberInfo = memberInfo;
    }

    public void AddOverriddenMember(MemberInfo memberInfo, OverrideType type)
    {
      switch(type)
      {
        case OverrideType.Target:
          _overriddenTargetMembers.Add(memberInfo);
          break;
        case OverrideType.Mixin:
          _overriddenMixinMembers.Add(memberInfo);
          break;
        default:
          throw new ArgumentOutOfRangeException("type");
      }
    }

    public static implicit operator MemberInfo(OverridingMemberInfo o)
    {
      return o._memberInfo;
    }

    public static implicit operator OverridingMemberInfo(MemberInfo m)
    {
      return new OverridingMemberInfo(m);
    }
  }
}