using System;
using System.Text;

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
      for(int i = 0; i < type.GetGenericArguments().Length; i++)
      {
        if(i != 0)
          result.Append(", ");

        result.Append (type.GetGenericArguments()[i].Name);
      }
      result.Append (">");
      return result.ToString();
    }

    public string CreateModifierMarkup (string visibility, bool overridden)
    {
      var modifiers = new StringBuilder();

      modifiers.Append (CreateSpan ("keyword", visibility));
      modifiers.Append (CreateSpan ("keyword", overridden ? "overridden" : null));

      return modifiers.ToString();
    }

    private string CreateSpan (string className, string content)
    {
      return content == null ? "" : String.Format ("<span class=\"{0}\">{1}</span>", className, content);
    }
  }
}