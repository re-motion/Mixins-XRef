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
    private static readonly IEqualityComparer<MemberInfo> s_equalityComparer = new MemberDefinitionEqualityComparer ();

    public InvolvedTypeMember (MemberInfo memberInfo, ReflectedObject targetMemberDefinition, IEnumerable<ReflectedObject> mixinMemberDefinitions)
    {
      ArgumentUtility.CheckNotNull ("memberInfo", memberInfo);

      MemberInfo = memberInfo;
      MixinMemberDefinitions = mixinMemberDefinitions ?? Enumerable.Empty<ReflectedObject> ();
      TargetMemberDefinition = targetMemberDefinition;

      OverriddenMixinMembers = GetOverriddenMixinMembers ();
      OverriddenTargetMembers = GetOverriddenTargetMembers ();
      OverridingMixinTypes = GetOverridingMixinTypes ();
      OverridingTargetTypes = GetOverridingTargetTypes ();
    }

    private IEnumerable<MemberInfo> GetOverriddenTargetMembers ()
    {
      if (!MixinMemberDefinitions.Any ())
        return Enumerable.Empty<MemberInfo> ();

      var members = MixinMemberDefinitions.Select (m => m.GetProperty ("BaseAsMember")).Where (m => m != null).Select (
          m => m.GetProperty ("MemberInfo").To<MemberInfo> ()).Distinct ();

      return members;
    }

    private IEnumerable<MemberInfo> GetOverriddenMixinMembers ()
    {
      if (TargetMemberDefinition == null)
        return Enumerable.Empty<MemberInfo> ();

      var baseMember = TargetMemberDefinition.GetProperty ("BaseAsMember");

      if (baseMember == null)
        return Enumerable.Empty<MemberInfo> ();
      
      var memberInfo = baseMember.GetProperty ("MemberInfo").To<MemberInfo> ();

      return new[] { memberInfo };
    }

    private IEnumerable<Type> GetOverridingMixinTypes ()
    {
      if (TargetMemberDefinition == null)
        return Enumerable.Empty<Type> ();

      return TargetMemberDefinition.GetProperty ("Overrides").Select (o => o.GetProperty ("DeclaringClass").GetProperty ("Type").To<Type> ());
    }

    private IEnumerable<Type> GetOverridingTargetTypes ()
    {
      if (!MixinMemberDefinitions.Any ())
        return Enumerable.Empty<Type> ();

      return MixinMemberDefinitions.SelectMany (m => m.GetProperty ("Overrides")).Select (o => o.GetProperty ("DeclaringClass").GetProperty ("Type").To<Type> ());
    }

    public MemberInfo MemberInfo { get; private set; }

    public ReflectedObject TargetMemberDefinition { get; private set; }
    public IEnumerable<ReflectedObject> MixinMemberDefinitions { get; private set; }

    public IEnumerable<MemberInfo> OverriddenMixinMembers { get; private set; }
    public IEnumerable<MemberInfo> OverriddenTargetMembers { get; private set; }
    public IEnumerable<Type> OverridingMixinTypes { get; private set; }
    public IEnumerable<Type> OverridingTargetTypes { get; private set; }

    public void Accept (IInvolvedVisitor involvedVisitor)
    {
      involvedVisitor.Visit (this);
    }

    public override bool Equals (object obj)
    {
      var other = obj as InvolvedTypeMember;
      return other != null && s_equalityComparer.Equals (MemberInfo, other.MemberInfo);
    }

    public override int GetHashCode ()
    {
      return MemberInfo.GetHashCode ();
    }

    public override string ToString ()
    {
      return string.Format ("{0}: {1}", typeof (InvolvedTypeMember).FullName, MemberInfo);
    }
  }
}
