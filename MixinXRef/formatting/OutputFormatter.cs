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

        result.Append (genericArguments[i].Name);
      }
      result.Append (">");
      return result.ToString();
    }

    public XElement CreateModifierMarkup (string visibility, bool overridden)
    {
      var modifiers = new XElement ("Modifiers");

      modifiers.Add (CreateElement ("Keyword", visibility));
      modifiers.Add (CreateElement ("Keyword", overridden ? "overridden" : null));

      return modifiers;
    }

    private XElement CreateElement (string elementName, string content)
    {
      return content == null ? null : new XElement (elementName, content);
    }
  }
}