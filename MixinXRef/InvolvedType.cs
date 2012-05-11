using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Collections;
using MixinXRef.Reflection;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class InvolvedType
  {
    private readonly Type _realType;
    // ClassContext _classContext;
    private ReflectedObject _classContext;
    // TargetClassDefinition
    private ReflectedObject _targetClassDefintion;
    // keys are the types of target class, values are from type MixinDefinition
    private readonly IDictionary<InvolvedType, ReflectedObject> _targetTypes = new Dictionary<InvolvedType, ReflectedObject> ();

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

    public ReflectedObject ClassContext
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

    public ReflectedObject TargetClassDefintion
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

    public IDictionary<InvolvedType, ReflectedObject> TargetTypes
    {
      get { return _targetTypes; }
    }

    private MemberDefinitionCollection _targetMemberDefinitions;
    public IDictionary<MemberInfo, ReflectedObject> TargetMemberDefinitions
    {
      get
      {
        if (_targetMemberDefinitions == null)
        {
          _targetMemberDefinitions = new MemberDefinitionCollection ();

          if (HasTargetClassDefintion)
            _targetMemberDefinitions.AddRange (TargetClassDefintion.CallMethod ("GetAllMembers"));
        }
        return _targetMemberDefinitions;
      }
    }

    private MemberDefinitionCollection _mixinMemberDefinitions;
    public IDictionary<MemberInfo, ReflectedObject> MixinMemberDefinitions
    {
      get
      {
        if (_mixinMemberDefinitions == null)
        {
          _mixinMemberDefinitions = new MemberDefinitionCollection ();

          if (IsMixin)
            _mixinMemberDefinitions.AddRange (TargetTypes.Values.Where (t => t != null).SelectMany (t => t.CallMethod ("GetAllMembers")));
        }
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