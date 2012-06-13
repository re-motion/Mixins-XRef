// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using Remotion.Mixins;

namespace MixinXRef.UnitTests.TestDomain
{
  public class TargetClassWithOverriddenBaseClassMember
  {
    public virtual void MyBaseClassMethod()
    {
    }

    public virtual void MyNonRelevantBaseClassMethod ()
    {
    }
  }

  public class InheritatedTargetClass : TargetClassWithOverriddenBaseClassMember
  {
    public virtual void MyNewMethod ()
    {
    }

    public override void MyNonRelevantBaseClassMethod ()
    {
    }
  }

  public class MixinOverridesTargetClassMember
  {
    [OverrideTarget]
    public void MyBaseClassMethod ()
    {
    }

    [OverrideTarget]
    public void MyNonRelevantBaseClassMethod ()
    {
    }
  }
}