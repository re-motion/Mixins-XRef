namespace MixinXRef
{
  public interface IInvolvedVisitor
  {
    void Visit (InvolvedTypeMember involvedTypeMember);
  }

  public interface IVisitableInvolved
  {
    void Accept (IInvolvedVisitor visitor);
  }
}
