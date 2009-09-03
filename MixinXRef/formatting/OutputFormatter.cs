using System;
using System.Reflection;
using System.Text;
using Remotion.Mixins.Definitions;
using Remotion.Collections;

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
      foreach (Type genericArgument in type.GetGenericArguments ())
      {
        result.Append (genericArgument.Name);
        result.Append (", ");
      }
      result.Remove (result.Length - 2, 2);
      result.Append (">");
      return result.ToString ();
    }

    public string CreateModifierMarkup (string visibility, bool overridden)
    {
      var modifiers = new StringBuilder();

      modifiers.Append (CreateSpan ("keyword", visibility));
      modifiers.Append (CreateSpan ("keyword", overridden ? "overridden" : null));

      return modifiers.ToString();
    }

    private string CreateSpan(string className, string content)
    {
      return content == null ? "" : String.Format ("<span class=\"{0}\">{1}</span>", className, content);
    }
  }
}