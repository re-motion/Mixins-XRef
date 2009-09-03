using System;

namespace MixinXRef
{
  public class TypeModifierUtility
  {
    public string GetTypeModifiers (Type type)
    {
      var modifiers = "";

      if (type.IsPublic || type.IsNestedPublic)
        modifiers = "public";
      else if (type.IsNestedFamily)
        modifiers = "protected";
      else if (type.IsNestedFamORAssem)
        modifiers = "protected internal";
      else if (type.IsNestedAssembly)
        modifiers = "internal";
      else if (type.IsNestedPrivate)
        modifiers = "private";
      // non nested internal class - no own flag?
      else if (type.IsNotPublic)
        modifiers = "internal";
      
      if (type.IsAbstract)
        modifiers += " abstract";
      else if (type.IsSealed)
        modifiers += " sealed";

      return modifiers;
    }
  }
}