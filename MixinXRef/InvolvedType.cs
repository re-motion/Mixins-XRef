using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Collections;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class InvolvedType
  {
    private readonly Type _realType;
    private ReflectedObject /* ClassContext */ _classContext;
    private ReflectedObject /* TargetClassDefinition */ _targetClassDefintion;
    private readonly IDictionary<InvolvedType, ReflectedObject /* MixinDefinition */> _targetTypes = new Dictionary<InvolvedType, ReflectedObject> ();

    public InvolvedType (Type realType)
    {
      ArgumentUtility.CheckNotNull ("realType", realType);

      _realType = realType;
    }

    public Type Type
    {
      get { return _realType; }
    }

    public bool IsTarget
    {
      get { return _classContext != null; }
    }

    public bool IsMixin
    {
      get { return _targetTypes.Count > 0; }
    }

    public ReflectedObject /* ClassContext */ ClassContext
    {
      get
      {
        if (!IsTarget)
          throw new InvalidOperationException ("Involved type is not a target class.");

        return _classContext;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        _classContext = value;
      }
    }

    public bool HasTargetClassDefintion
    {
      get { return _targetClassDefintion != null; }
    }

    public ReflectedObject /* TargetClassDefinition */ TargetClassDefinition
    {
      get
      {
        if (!HasTargetClassDefintion)
          throw new InvalidOperationException ("Involved type is either not a target class or a generic target class.");

        return _targetClassDefintion;
      }
      set
      {
        _targetClassDefintion = value;
      }
    }

    public IDictionary<InvolvedType, ReflectedObject /* MixinDefinition */> TargetTypes
    {
      get { return _targetTypes; }
    }

    private MemberDefinitionCollection _targetMemberDefinitions;
    public IDictionary<MemberInfo, ReflectedObject /* MemberDefinitionBase */> TargetMemberDefinitions
    {
      get
      {
        if (!HasTargetClassDefintion)
          throw new InvalidOperationException ("Involved type is either not a target class or a generic target class.");

        if (_targetMemberDefinitions == null)
          _targetMemberDefinitions = new MemberDefinitionCollection (/* IEnumerable<MemberDefinitionBase> */ TargetClassDefinition.CallMethod ("GetAllMembers"));

        return _targetMemberDefinitions;
      }
    }

    private MemberDefinitionCollection _mixinMemberDefinitions;
    public IDictionary<MemberInfo, ReflectedObject /* MemberDefinitionBase */> MixinMemberDefinitions
    {
      get
      {
        if (!IsMixin)
          throw new InvalidOperationException ("Involved type is not a mixin.");

        if (_mixinMemberDefinitions == null)
          _mixinMemberDefinitions =
            new MemberDefinitionCollection (
              TargetTypes.Values.Where (t => t != null).SelectMany (t => /* IEnumerable<MemberDefinitionBase> */ t.CallMethod ("GetAllMembers")));

        return _mixinMemberDefinitions;
      }
    }

    public override bool Equals (object obj)
    {
      var other = obj as InvolvedType;
      return other != null
             && Equals (other._realType, _realType)
             && Equals (other._classContext, _classContext)
             && Equals (other._targetClassDefintion, _targetClassDefintion)
             && other._targetTypes.SequenceEqual (_targetTypes);
    }

    public override int GetHashCode ()
    {
      int hashCode = _realType.GetHashCode ();
      Rotate (ref hashCode);
      hashCode ^= _classContext == null ? 0 : _classContext.GetHashCode ();
      Rotate (ref hashCode);
      hashCode ^= _targetClassDefintion == null ? 0 : _targetClassDefintion.GetHashCode ();
      Rotate (ref hashCode);
      hashCode ^= _targetTypes.Aggregate (0, (current, typeAndMixinDefintionPair) => current ^ typeAndMixinDefintionPair.GetHashCode ());

      return hashCode;
    }

    public override string ToString ()
    {
      return string.Format ("{0}, isTarget: {1}, isMixin: {2}, # of targets: {3}", _realType, IsTarget, IsMixin, _targetTypes.Count);
    }


    private static void Rotate (ref int value)
    {
      const int rotateBy = 11;
      value = (value << rotateBy) ^ (value >> (32 - rotateBy));
    }
  }
}