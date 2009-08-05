using System;
using Remotion.Utilities;

namespace MixinXRef
{
  public class InvolvedType : IInvolvedType
  {
    private readonly Type _realType;
    private bool _isTarget;
    private bool _isMixin;

    public InvolvedType (Type realType)
    {
      ArgumentUtility.CheckNotNull ("realType", realType);

      _realType = realType;
    }

    public InvolvedType (Type realType, bool isTarget, bool isMixin)
    {
      ArgumentUtility.CheckNotNull ("realType", realType);

      _realType = realType;
      _isTarget = isTarget;
      _isMixin = isMixin;
    }

    public Type Type
    {
      get { return _realType; }
    }

    public bool IsTarget
    {
      get { return _isTarget; }
      set { _isTarget = value; }
    }

    public bool IsMixin
    {
      get { return _isMixin; }
      set { _isMixin = value; }
    }


    override public bool Equals (object obj)
    {
      var other = obj as InvolvedType;
      return other != null 
          && other.Type == Type 
          && other.IsTarget == IsTarget 
          && other.IsMixin == IsMixin;
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (Type, IsTarget, IsMixin);
    }

    public override string ToString ()
    {
      return String.Format ("{0}, isTarget: {1}, isMixin: {2}", _realType, IsTarget, IsMixin);
    }
  }
}