using System;
using System.Text;

namespace MixinXRef.Formatting
{
  public class OutputFormatter : IOutputFormatter
  {
    public string GetCSharpLikeName (Type type)
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
  }
}