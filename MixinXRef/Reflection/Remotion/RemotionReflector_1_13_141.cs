using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MixinXRef.Utility;

namespace MixinXRef.Reflection.Remotion
{
  public class RemotionReflector_1_13_141 : RemotionReflector_1_13_23
  {
    public RemotionReflector_1_13_141 (Assembly remotionAssembly, Assembly remotionInterfaceAssembly)
        : base(remotionAssembly, remotionInterfaceAssembly)
    {
    }

    public override IEnumerable<string> GetRemotionAssemblyNames ()
    {
      return new[] { "Remotion.dll", "Remotion.Mixins.dll" };
    }

    public override bool IsInfrastructureType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.Assembly.GetName ().Name == "Remotion.Mixins";
    }

    public override Assembly FindRemotionAssembly (Assembly[] assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);

      return assemblies.SingleOrDefault (a => a.GetName ().Name == "Remotion.Mixins");
    }
  }
}