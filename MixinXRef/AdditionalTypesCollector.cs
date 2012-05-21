using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MixinXRef
{
  public class AdditionalTypesCollector : IInvolvedVisitor
  {
    private readonly List<Type> _additionalTypes = new List<Type> ();

    public void Visit (InvolvedTypeMember involvedTypeMember)
    {
      _additionalTypes.AddRange (involvedTypeMember.OverriddenMixinMembers.Select (m => m.DeclaringType));
      _additionalTypes.AddRange (involvedTypeMember.OverriddenTargetMembers.Select (m => m.DeclaringType));
    }

    public IEnumerable<Type> AdditionalTypes
    {
      get { return _additionalTypes; }
    }
  }
}
