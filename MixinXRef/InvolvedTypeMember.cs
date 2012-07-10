using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class AdvancedMemberInfo
  {
    public MemberInfo MainMember { get; private set; }
    public IEnumerable<MemberInfo> SubMembers { get { return _subMembers; }}

    private readonly List<MemberInfo> _subMembers = new List<MemberInfo>(); 

    public AdvancedMemberInfo(MemberInfo memberInfo)
    {
      Initialize(memberInfo);
    }

    private void Initialize(MemberInfo memberInfo)
    {
      MainMember = memberInfo;

      if (memberInfo.MemberType == MemberTypes.Property)
      {
        var propInfo = ((PropertyInfo) memberInfo);

        var getMethod = propInfo.GetGetMethod();
        if (getMethod != null)
          _subMembers.Add(getMethod);

        var setMethod = propInfo.GetSetMethod();
        if (setMethod != null)
          _subMembers.Add(setMethod);
      }

      if (memberInfo.MemberType == MemberTypes.Event)
      {
        var eventInfo = ((EventInfo) memberInfo);

        _subMembers.Add(eventInfo.GetAddMethod());
        _subMembers.Add(eventInfo.GetRemoveMethod());
      }
    }

    public AdvancedMemberInfo(ReflectedObject overrideMember, MemberInfo baseMember)
    {
      if (baseMember.MemberType == MemberTypes.Property)
      {
        var propInfo = ((PropertyInfo) baseMember);

        var getMethod = propInfo.GetGetMethod ();
        if (getMethod != null && overrideMember.GetProperty("GetMethod") != null)
          _subMembers.Add (getMethod);

        var setMethod = propInfo.GetSetMethod ();
        if (setMethod != null && overrideMember.GetProperty ("SetMethod") != null)
          _subMembers.Add(setMethod);
      }
      else
      {
        Initialize(baseMember);
      }
    }
  }

  public class InvolvedTypeMember : IVisitableInvolved
  {
    private static readonly IEqualityComparer<MemberInfo> s_equalityComparer = new MemberDefinitionEqualityComparer();

    public InvolvedTypeMember(MemberInfo memberInfo, ReflectedObject targetMemberDefinition,
                              IEnumerable<ReflectedObject> mixinMemberDefinitions)
    {
      ArgumentUtility.CheckNotNull("memberInfo", memberInfo);

      MemberInfo = new AdvancedMemberInfo(memberInfo);
      MixinMemberDefinitions = mixinMemberDefinitions ?? Enumerable.Empty<ReflectedObject>();
      TargetMemberDefinition = targetMemberDefinition;

      OverriddenMixinMembers = GetOverriddenMixinMembers();
      OverriddenTargetMembers = GetOverriddenTargetMembers();
      OverridingMixinTypes = GetOverridingMixinTypes();
      OverridingTargetTypes = GetOverridingTargetTypes();
    }

    private IEnumerable<AdvancedMemberInfo> GetOverriddenTargetMembers()
    {
      if (!MixinMemberDefinitions.Any())
        return Enumerable.Empty<AdvancedMemberInfo> ();

      return MixinMemberDefinitions.Select(m => new {Override = m, Base = m.GetProperty("BaseAsMember")}).Where(
        m => m.Base != null).Select(
          m => new {Override = m.Override, BaseMember = m.Base.GetProperty("MemberInfo").To<MemberInfo>()}).DistinctBy(
            m => m.BaseMember).Select(m => new AdvancedMemberInfo(m.Override, m.BaseMember));
    }

    private IEnumerable<AdvancedMemberInfo> GetOverriddenMixinMembers ()
    {
      if (TargetMemberDefinition == null)
        return Enumerable.Empty<AdvancedMemberInfo> ();

      var baseMember = TargetMemberDefinition.GetProperty("BaseAsMember");

      if (baseMember == null)
        return Enumerable.Empty<AdvancedMemberInfo>();

      return new[] {new AdvancedMemberInfo(TargetMemberDefinition, baseMember.GetProperty("MemberInfo").To<MemberInfo>())};
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

    public AdvancedMemberInfo MemberInfo { get; private set; }

    public ReflectedObject TargetMemberDefinition { get; private set; }
    public IEnumerable<ReflectedObject> MixinMemberDefinitions { get; private set; }

    public IEnumerable<AdvancedMemberInfo> OverriddenMixinMembers { get; private set; }
    public IEnumerable<AdvancedMemberInfo> OverriddenTargetMembers { get; private set; }
    public IEnumerable<Type> OverridingMixinTypes { get; private set; }
    public IEnumerable<Type> OverridingTargetTypes { get; private set; }

    public void Accept(IInvolvedVisitor involvedVisitor)
    {
      involvedVisitor.Visit(this);
    }

    public override bool Equals(object obj)
    {
      var other = obj as InvolvedTypeMember;
      return other != null && s_equalityComparer.Equals(MemberInfo.MainMember, other.MemberInfo.MainMember);
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