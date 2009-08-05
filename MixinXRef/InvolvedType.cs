using System;

namespace MixinXRef
{
  public class InvolvedType : IInvolvedType
  {

    private readonly Type _realType;
    private readonly bool _isTarget;
    private readonly bool _isMixin;

    public InvolvedType (Type realType, bool isTarget, bool isMixin)
    {
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
    }

    public bool IsMixin
    {
      get { return _isMixin; }
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
      return 0;
    }

    public override string ToString ()
    {
      return String.Format ("{0}, isTarget: {1}, isMixin: {2}", _realType, IsTarget, IsMixin);
    }
  }
}