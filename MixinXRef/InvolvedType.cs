using System;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InvolvedType
  {
    private readonly Type _realType;
    private bool _isMixin;
    private ClassContext _classContext;

    public InvolvedType (Type realType)
    {
      ArgumentUtility.CheckNotNull ("realType", realType);

      _realType = realType;
    }

    public InvolvedType (Type realType, bool isMixin)
    {
      ArgumentUtility.CheckNotNull ("realType", realType);

      _realType = realType;
      _isMixin = isMixin;
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
      get { return _isMixin; }
      set { _isMixin = value; }
    }

    public bool IsGenericTypeDefinition
    {
      get { return _realType.IsGenericTypeDefinition; }
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




    public override bool Equals (object obj)
    {
      var other = obj as InvolvedType;
      return other != null
             && other._realType == _realType
             && other._isMixin == _isMixin
             && other._classContext == _classContext;
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (Type, IsMixin, _classContext);
    }

    public override string ToString ()
    {
      return String.Format ("{0}, isTarget: {1}, isMixin: {2}", _realType, IsTarget, IsMixin);
    }
  }
}