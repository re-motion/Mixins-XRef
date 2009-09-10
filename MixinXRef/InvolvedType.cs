using System;
using System.Collections.Generic;
using System.Linq;
using MixinXRef.Reflection;

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
    private readonly IDictionary<Type, ReflectedObject> _targetTypes = new Dictionary<Type, ReflectedObject>();

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
      set { _classContext = value; }
    }

    public ReflectedObject TargetClassDefintion
    {
      get {
        if (!IsTarget || _realType.IsGenericType)
          throw new InvalidOperationException("Involved type is either not a target class or a generic target class.");
        return _targetClassDefintion;
      }
      set { _targetClassDefintion = value; }
    }

    public IDictionary<Type, ReflectedObject> TargetTypes
    {
      get { return _targetTypes; }
    }

    public override bool Equals (object obj)
    {
      var other = obj as InvolvedType;
      return other != null
             && Equals(other._realType, _realType)
             && Equals(other._classContext, _classContext)
             && Equals(other._targetClassDefintion, _targetClassDefintion)
             && other._targetTypes.SequenceEqual (_targetTypes);
    }

    public override int GetHashCode ()
    {
      int hashCode = _realType.GetHashCode();
      Rotate (ref hashCode);
      hashCode ^= _classContext == null ? 0 : _classContext.GetHashCode();
      Rotate (ref hashCode);
      hashCode ^= _targetClassDefintion == null ? 0 : _targetClassDefintion.GetHashCode();
      Rotate (ref hashCode);
      hashCode ^= _targetTypes.Sum(typeAndMixinDefintionPair => typeAndMixinDefintionPair.GetHashCode());
      
      return hashCode;
    }

    public override string ToString ()
    {
      return String.Format ("{0}, isTarget: {1}, isMixin: {2}, # of targets: {3}", _realType, IsTarget, IsMixin, _targetTypes.Count);
    }


    private static void Rotate (ref int value)
    {
      const int rotateBy = 11;
      value = (value << rotateBy) ^ (value >> (32 - rotateBy));
    }
  }
}