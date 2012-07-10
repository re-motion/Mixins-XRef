using System.Collections.Generic;
using System.Reflection;
using MixinXRef.Reflection;

namespace MixinXRef
{
  public class OverrideInfo
  {
    public MemberInfo MainMemberInfo { get; private set; }
    public IEnumerable<OverriddenMemberInfo> SubMemberInfos { get { return _subMemberInfos; } }

    private readonly List<OverriddenMemberInfo> _subMemberInfos = new List<OverriddenMemberInfo>();

    public OverrideInfo (ReflectedObject overrideMember, MemberInfo baseMember)
    {
      MainMemberInfo = baseMember;

      if (baseMember.MemberType == MemberTypes.Property)
      {
        var propInfo = ((PropertyInfo) baseMember);

        var getMethod = propInfo.GetGetMethod ();
        if (getMethod != null && overrideMember.GetProperty("GetMethod") != null)
          _subMemberInfos.Add (new OverriddenMemberInfo(getMethod, overrideMember.GetProperty("GetMethod").GetProperty("MethodInfo").To<MemberInfo>()));

        var setMethod = propInfo.GetSetMethod ();
        if (setMethod != null && overrideMember.GetProperty ("SetMethod") != null)
          _subMemberInfos.Add (new OverriddenMemberInfo (setMethod, overrideMember.GetProperty ("SetMethod").GetProperty ("MethodInfo").To<MemberInfo> ()));
      }

      if (baseMember.MemberType == MemberTypes.Event)
      {
        var eventInfo = ((EventInfo) baseMember);

        _subMemberInfos.Add (new OverriddenMemberInfo (eventInfo.GetAddMethod (), overrideMember.GetProperty ("AddMethod").GetProperty ("MethodInfo").To<MemberInfo> ()));
        _subMemberInfos.Add (new OverriddenMemberInfo (eventInfo.GetRemoveMethod (), overrideMember.GetProperty ("RemoveMethod").GetProperty ("MethodInfo").To<MemberInfo> ()));
      }
    }
  }
}