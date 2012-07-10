using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class InvolvedTypeMember : IVisitableInvolved
  {
    private static readonly IEqualityComparer<MemberInfo> s_equalityComparer = new MemberDefinitionEqualityComparer();

    public InvolvedTypeMember(MemberInfo memberInfo, ReflectedObject targetMemberDefinition,
                              IEnumerable<ReflectedObject> mixinMemberDefinitions)
    {
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      MemberInfo = memberInfo;
      SubMemberInfos = GetSubMemberInfos(memberInfo);
      MixinMemberDefinitions = mixinMemberDefinitions ?? Enumerable.Empty<ReflectedObject>();
      TargetMemberDefinition = targetMemberDefinition;

      MixinOverrideInfos = GetMixinOverrideInfos();
      TargetOverrideInfos = GetTargetOverrideInfos();
      OverridingMixinTypes = GetOverridingMixinTypes();
      OverridingTargetTypes = GetOverridingTargetTypes();
    }

    private IEnumerable<MemberInfo> GetSubMemberInfos(MemberInfo memberInfo)
    {
      var subMembers = new List<MemberInfo>();

      if (memberInfo.MemberType == MemberTypes.Property)
      {
        var propInfo = ((PropertyInfo) memberInfo);

        var getMethod = propInfo.GetGetMethod ();
        if (getMethod != null)
          subMembers.Add (getMethod);

        var setMethod = propInfo.GetSetMethod ();
        if (setMethod != null)
          subMembers.Add (setMethod);
      }

      if (memberInfo.MemberType == MemberTypes.Event)
      {
        var eventInfo = ((EventInfo) memberInfo);

        subMembers.Add(eventInfo.GetAddMethod());
        subMembers.Add(eventInfo.GetRemoveMethod());
      }

      return subMembers;
    }

    private IEnumerable<OverrideInfo> GetTargetOverrideInfos ()
    {
      if (!MixinMemberDefinitions.Any())
        return Enumerable.Empty<OverrideInfo> ();

      return MixinMemberDefinitions.Select(m => new {Override = m, Base = m.GetProperty("BaseAsMember")}).Where(
        m => m.Base != null).Select(
          m => new {Override = m.Override, BaseMember = m.Base.GetProperty("MemberInfo").To<MemberInfo>()}).DistinctBy(
            m => m.BaseMember).Select(m => new OverrideInfo(m.Override, m.BaseMember));
    }

    private IEnumerable<OverrideInfo> GetMixinOverrideInfos ()
    {
      if (TargetMemberDefinition == null)
        return Enumerable.Empty<OverrideInfo> ();

      var baseMember = TargetMemberDefinition.GetProperty("BaseAsMember");

      if (baseMember == null)
        return Enumerable.Empty<OverrideInfo> ();

      return new[] { new OverrideInfo (TargetMemberDefinition, baseMember.GetProperty ("MemberInfo").To<MemberInfo> ()) };
    }

    private IEnumerable<Type> GetOverridingMixinTypes()
    {
      if (TargetMemberDefinition == null)
        return Enumerable.Empty<Type>();

      return
        TargetMemberDefinition.GetProperty("Overrides").Select(
          o => o.GetProperty("DeclaringClass").GetProperty("Type").To<Type>());
    }

    private IEnumerable<Type> GetOverridingTargetTypes()
    {
      if (!MixinMemberDefinitions.Any())
        return Enumerable.Empty<Type>();

      return
        MixinMemberDefinitions.SelectMany(m => m.GetProperty("Overrides")).Select(
          o => o.GetProperty("DeclaringClass").GetProperty("Type").To<Type>());
    }

    public MemberInfo MemberInfo { get; private set; }
    public IEnumerable<MemberInfo> SubMemberInfos { get; private set; } 

    public ReflectedObject TargetMemberDefinition { get; private set; }
    public IEnumerable<ReflectedObject> MixinMemberDefinitions { get; private set; }

    public IEnumerable<OverrideInfo> MixinOverrideInfos { get; private set; }
    public IEnumerable<OverrideInfo> TargetOverrideInfos { get; private set; }

    public IEnumerable<Type> OverridingMixinTypes { get; private set; }
    public IEnumerable<Type> OverridingTargetTypes { get; private set; }

    public void Accept(IInvolvedVisitor involvedVisitor)
    {
      involvedVisitor.Visit(this);
    }

    public override bool Equals(object obj)
    {
      var other = obj as InvolvedTypeMember;
      return other != null && s_equalityComparer.Equals(MemberInfo, other.MemberInfo);
    }

    public override int GetHashCode()
    {
      return MemberInfo.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format("{0}: {1}", typeof (InvolvedTypeMember).FullName, MemberInfo);
    }
  }
}