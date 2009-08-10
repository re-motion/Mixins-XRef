using System;
using System.Collections.Generic;
using Remotion.Mixins.Context;
using Remotion.Utilities;
using System.Linq;

namespace MixinXRef
{
  public class InvolvedType
  {
    private readonly Type _realType;
    private ClassContext _classContext;
    private readonly IList<Type> _targetTypes = new List<Type> ();

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

    public ClassContext ClassContext
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
             && other._realType == _realType
             && other._classContext == _classContext
             && other._targetTypes.SequenceEqual (_targetTypes);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_realType, _classContext, _targetTypes.Count);
    }

    public override string ToString ()
    {
      return String.Format ("{0}, isTarget: {1}, isMixin: {2}", _realType, IsTarget, IsMixin);
    }
  }
}