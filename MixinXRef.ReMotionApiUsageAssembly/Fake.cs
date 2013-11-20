using System;
using System.Reflection;
using Remotion.Logging;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.Reflection.TypeDiscovery;

namespace MixinXRef.ReMotionApiUsageAssembly
{
  /// <summary>
  /// This class fakes re-motion api usages for the dependDB analysis running on a buildserver. 
  /// </summary>
  public class Fake
  {
    public void ReMotionApiUsages ()
    {
      DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (new Assembly[0]);
      TargetClassDefinitionFactory.CreateAndValidate (null);
      LogManager.GetLogger ("");
      Use (new NonApplicationAssemblyAttribute());
      Use (new ConfigurationException (""));
      Use (new ValidationException (new ValidationLogData()));
      Use (typeof (IInitializableMixin));
      Use<TargetClassDefinition> (_ => _.GetMixinByConfiguredType (typeof (object)));
      Use<TargetClassDefinition> (_ => _.GetAllMembers());
      Use<TargetClassDefinition> (_ => _.ImplementedInterfaces);
      Use<TargetClassDefinition> (_ => _.ReceivedInterfaces);
      Use<ClassContext> (_ => _.ComposedInterfaces);
      Use<ClassContext> (_ => _.Mixins);
      Use<MixinDefinition> (_ => _.NextCallDependencies);
      Use<MixinDefinition> (_ => _.TargetCallDependencies);
      Use<MixinDefinition> (_ => _.GetAllOverrides());
      Use<MixinDefinition> (_ => _.AcceptsAlphabeticOrdering);
      Use<MixinDefinition> (_ => _.MixinIndex);
      Use<MixinDefinition> (_ => _.InterfaceIntroductions);
      Use<MixinDefinition> (_ => _.AttributeIntroductions);
      Use<MixinDefinition> (_ => _.TargetClass);
      Use<MixinContext> (_ => _.MixinType);
      Use<MixinContext> (_ => _.ExplicitDependencies);
      Use<MixinContext> (_ => _.MixinKind);
      Use<MixinConfiguration> (_ => _.ClassContexts);
      Use<ClassContextCollection> (_ => _.GetWithInheritance (typeof (object)));
      Use<ValidationException> (_ => _.Data); //TODO: add validationLogData from re-motion 1.15.6
      Use<AttributeIntroductionDefinition> (_ => _.AttributeType);
      Use<InterfaceIntroductionDefinition> (_ => _.InterfaceType);
      Use<MemberDefinitionBase> (_ => _.MemberType);
      Use<MemberDefinitionBase> (_ => _.Name);
      Use<MemberDefinitionBase> (_ => _.MemberInfo);
      Use<MemberDefinitionBase> (_ => _.BaseAsMember);
      Use<MemberDefinitionBase> (_ => _.Overrides);
      Use<MemberDefinitionBase> (_ => _.DeclaringClass);
      Use<ClassDefinitionBase> (_ => _.Type);
      Use<TargetCallDependencyDefinition> (_ => _.RequiredType);
      Use<RequiredTargetCallTypeDefinition> (_ => _.Type);

      //TODO: add SerializableValidationLogData members
    }

    public void Use<T> (Func<T, object> obj) where T : class
    {
      obj (null).ToString();
    }

    public void Use (object obj)
    {
      obj.ToString();
    }
  }
}