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
    private readonly IList<Type> _targetTypes = new List<Type>();

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
          throw new InvalidOperationException ("Involved type is not a target class");
        return _classContext;
      }
      set { _classContext = value; }
    }

    public IList<Type> TargetTypes
    {
      get { return _targetTypes; }
    }

    public override bool Equals (object obj)
    {
      var other = obj as InvolvedType;
      return other != null
             && object.Equals(other._realType, _realType)
             && object.Equals(other._classContext, _classContext)
             && other._targetTypes.SequenceEqual (_targetTypes);
    }

    public override int GetHashCode ()
    {
      int hashCode = _realType.GetHashCode();
      Rotate (ref hashCode);
      hashCode ^= _classContext == null ? 0 : _classContext.GetHashCode();
      Rotate (ref hashCode);
      hashCode ^= _targetTypes.Count;

      return hashCode;
    }

    public override string ToString ()
    {
      return String.Format ("{0}, isTarget: {1}, isMixin: {2}", _realType, IsTarget, IsMixin);
    }


    private static void Rotate (ref int value)
    {
      const int rotateBy = 11;
      value = (value << rotateBy) ^ (value >> (32 - rotateBy));
    }
  }
}