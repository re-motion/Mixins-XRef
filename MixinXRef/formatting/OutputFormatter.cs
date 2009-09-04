using System;
using System.Text;
using System.Xml.Linq;

namespace MixinXRef.Formatting
{
  public class OutputFormatter : IOutputFormatter
  {
    public string GetFormattedTypeName (Type type)
    {
      if (!type.IsGenericType)
        return type.Name;

      var typeName = type.Name.Substring (0, type.Name.IndexOf ('`'));

      StringBuilder result = new StringBuilder (typeName);
      result.Append ("<");
      var genericArguments = type.GetGenericArguments();
      for (int i = 0; i < genericArguments.Length; i++)
      {
        if (i != 0)
          result.Append (", ");

        result.Append (GetShortName(genericArguments[i]));
      }
      result.Append (">");
      return result.ToString();
    }

    public XElement CreateModifierMarkup (string keywords)
    {
      var modifiers = new XElement ("Modifiers");

      foreach (var keyword in keywords.Split(' '))
      {
        modifiers.Add(CreateElement("Keyword", keyword));  
      }

      return modifiers;
    }

    private XElement CreateElement (string elementName, string content)
    {
      return content == null ? null : new XElement (elementName, content);
    }

    public string GetShortName (Type type)
    {
      var name = type.Name;

      var index = name.LastIndexOf ('.');

      if (index == -1)
      {
        switch (name)
        {
          case "Boolean":
            return "bool";
          case "Int16":
            return "short";
          case "Int32":
            return "int";
          case "Int64":
            return "long";
          case "Single":
            return "flaot";
          case "UInt16":
            return "ushort";
          case "UInt32":
            return "uint";
          case "UInt64":
            return "ulong";
          case "Byte":
          case "Char":
          case "Decimal":
          case "Double":
          case "SByte":
          case "String":
            return name.ToLower ();
          default:
            return GetFormattedTypeName (type);
        }
      }

      var shortParameterName = name.Substring (
          index, name.Length - index);

      return shortParameterName;
    }
  }
}