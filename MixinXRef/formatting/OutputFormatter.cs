using System;
using System.Reflection;
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

      var typeName = "";
      var nestedTypeName = "";

      if (type.IsNested)
      {
        typeName = type.FullName.Substring (0, type.FullName.IndexOf ('`'));
        var index = type.FullName.IndexOf ('+');
        if (index > 0)
        {
          nestedTypeName = (type.FullName.Substring (index, type.FullName.Length - index));
          nestedTypeName = "." + nestedTypeName.Substring (1, nestedTypeName.IndexOf('['));
        }
      }
      else
        typeName = type.Name.Substring (0, type.Name.IndexOf ('`'));

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
      result.Append (nestedTypeName);

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
            return "float";
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
          case "Void":
            return name.ToLower ();
          default:
            return GetFormattedTypeName (type);
        }
      }

      var shortParameterName = name.Substring (
          index, name.Length - index);

      return shortParameterName;
    }

    public XElement CreateSignatureMarkup (string signature, MemberTypes memberType)
    {
      switch (memberType)
      {
        case MemberTypes.Constructor:
          return CreateConstructorMarkup (signature);

        default:
          return null;
      }
    }

    public XElement CreateConstructorMarkup (string signature)
    {
      var constructorMarkup = new XElement ("Signature");

      // [0] = type name, [1] = (Param1, Param2, ...)
      var index = signature.IndexOf (' ');
      var parameters = signature.Substring (index + 1, signature.Length - index-1);
      constructorMarkup.Add (CreateElement ("Type", signature.Substring(0, index)));
      CreateParameterMarkup (parameters, constructorMarkup);

      return constructorMarkup;
    }

    public void CreateParameterMarkup (string parameters, XElement signatureElement)
    {
      parameters = parameters.Replace ("(", "").Replace (")", "");

      var parameterList = parameters.Split (new []{", "}, StringSplitOptions.RemoveEmptyEntries);

      signatureElement.Add (CreateElement ("Text", "("));

      for (int i = 0; i < parameterList.Length; i++)
      {
        var typeAndName = parameterList[i].Split (' ');
        
        var delimiter = "";
        if (parameterList.Length > 1 && i < parameterList.Length-1)
          delimiter = ",";

        signatureElement.Add(CreateElement ("Keyword", typeAndName[0]));
        signatureElement.Add (CreateElement ("Text", (typeAndName[1] + delimiter)));
      }

      signatureElement.Add (CreateElement ("Text", ")"));
    }


    private XElement CreateElement (string elementName, string content)
    {
      return content == null ? null : new XElement (elementName, content);
    }
  }
}