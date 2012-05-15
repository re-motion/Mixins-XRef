using System.Collections.Generic;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;

namespace MixinXRef.Collections
{
  internal class MemberDefinitionCollection : Dictionary<MemberInfo, ReflectedObject>
  {
    private class MemberDefinitionEqualityComparer : IEqualityComparer<MemberInfo>
    {
      public bool Equals (MemberInfo x, MemberInfo y)
      {
        return x.ToString ().Equals (y.ToString ());
      }

      public int GetHashCode (MemberInfo obj)
      {
        return obj.ToString ().GetHashCode ();
      }
    }

    public MemberDefinitionCollection ()
      : base (new MemberDefinitionEqualityComparer ())
    { }

    public MemberDefinitionCollection (IEnumerable<ReflectedObject> memberDefinitions)
      : this ()
    {
      AddRange (memberDefinitions);
    }

    public void AddRange (IEnumerable<ReflectedObject> memberDefinitions)
    {
      foreach (var memberDefinition in memberDefinitions)
      {
        var memberInfo = memberDefinition.GetProperty ("MemberInfo").To<MemberInfo> ();
        if (!ContainsKey (memberInfo))
          Add (memberInfo, memberDefinition);
      }
    }
  }
}
