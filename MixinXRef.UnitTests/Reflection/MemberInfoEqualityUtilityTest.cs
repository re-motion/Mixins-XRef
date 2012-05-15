using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class MemberInfoEqualityUtilityTest
  {
    [Test]
    public void MemberInfo_Equals_False ()
    {
      var targetType = typeof (BaseMemberOverrideTestClass.Target);

      var mixinConfiguration =
          MixinConfiguration.BuildNew ()
              .ForClass<BaseMemberOverrideTestClass.Target> ().AddMixin<BaseMemberOverrideTestClass.Mixin1> ()
              .BuildConfiguration ();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
                         {
                           TargetClassDefinition = targetClassDefinition,
                           ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ())
                         };

      var memberInfo1 = targetType.GetMember ("OverriddenMethod")[0];

      var output =
        involvedType.TargetClassDefinition.CallMethod ("GetAllMembers").SingleOrDefault (
          mdb => mdb.GetProperty ("MemberInfo").To<MemberInfo> () == memberInfo1);

      Assert.That (output, Is.Null);
    }

    [Test]
    public void MemberEquals_True ()
    {
      var targetType = typeof (BaseMemberOverrideTestClass.Target);

      var mixinConfiguration =
          MixinConfiguration.BuildNew ()
              .ForClass<BaseMemberOverrideTestClass.Target> ().AddMixin<BaseMemberOverrideTestClass.Mixin1> ()
              .BuildConfiguration ();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
                         {
                           TargetClassDefinition = targetClassDefinition,
                           ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ())
                         };

      var memberInfo1 = targetType.GetMember ("OverriddenMethod")[0];

      var output =
        involvedType.TargetClassDefinition.CallMethod ("GetAllMembers").SingleOrDefault (
          mdb => MemberInfoEqualityUtility.MemberEquals (mdb.GetProperty ("MemberInfo").To<MemberInfo> (), memberInfo1));

      Assert.That (output.To<object> (), Is.Not.Null);
    }

    [Test]
    public void MemberEquals_False ()
    {
      var targetType = typeof (BaseMemberOverrideTestClass.Target);

      var mixinConfiguration =
          MixinConfiguration.BuildNew ()
              .ForClass<BaseMemberOverrideTestClass.Target> ().AddMixin<BaseMemberOverrideTestClass.Mixin1> ()
              .BuildConfiguration ();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
      {
        TargetClassDefinition = targetClassDefinition,
        ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ())
      };

      var memberInfo1 = typeof (HiddenMemberTestClass.Target).GetMember ("HiddenMethod")[0];

      var output =
        involvedType.TargetClassDefinition.CallMethod ("GetAllMembers").SingleOrDefault (
          mdb => MemberInfoEqualityUtility.MemberEquals (mdb.GetProperty ("MemberInfo").To<MemberInfo> (), memberInfo1));

      Assert.That (output, Is.Null);
    }
  }
}