using System;

namespace MixinXRef
{
  public class TypeModifierUtility
  {
    public string GetTypeModifiers (Type type)
    {
      if (type.IsPublic || type.IsNestedPublic)
        return "public";
      if (type.IsNestedFamily)
        return "protected";
      if (type.IsNestedFamORAssem)
        return "protected internal";
      if (type.IsNestedAssembly)
        return "internal";
      if (type.IsNestedPrivate)
        return "private";

      // non nested internal class - no own flag?
      if (type.IsNotPublic)
        return "internal";      

      return "unknown visibility";
    }
  }
}