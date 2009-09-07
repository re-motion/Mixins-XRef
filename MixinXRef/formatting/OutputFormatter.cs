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

    public void AddParameterMarkup (ParameterInfo[] parameterInfos, XElement signatureElement)
    {
      ArgumentUtility.CheckNotNull ("parameterInfos", parameterInfos);
      ArgumentUtility.CheckNotNull ("signatureElement", signatureElement);

      signatureElement.Add (CreateElement ("Text", "("));

      for (int i = 0; i < parameterInfos.Length; i++)
      {

        signatureElement.Add (CreateTypeOrKeywordElement (parameterInfos[i].ParameterType));
        signatureElement.Add (CreateElement ("Text", (parameterInfos[i].Name + ((i < parameterInfos.Length - 1) ? "," : ""))));
      }

      signatureElement.Add (CreateElement ("Text", ")"));
    }

    public XElement CreateConstructorMarkup(string name, ParameterInfo[] parameterInfos)
    {
      ArgumentUtility.CheckNotNull("name", name);
      ArgumentUtility.CheckNotNull("parameterInfos", parameterInfos);

      var constructorMarkup = new XElement("Signature");

      constructorMarkup.Add(CreateElement("Name", name));
      AddParameterMarkup(parameterInfos, constructorMarkup);

      return constructorMarkup;
    }

    public XElement CreateMethodMarkup (string methodName, Type returnType, ParameterInfo[] parameterInfos)
    {
      ArgumentUtility.CheckNotNull ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("returnType", returnType);
      ArgumentUtility.CheckNotNull ("parameterInfos", parameterInfos);

      var methodMarkup = new XElement ("Signature");
      methodMarkup.Add (CreateTypeOrKeywordElement (returnType));
      methodMarkup.Add (CreateElement ("Name", methodName));
      AddParameterMarkup (parameterInfos, methodMarkup);

      return methodMarkup;
    }

    public XElement CreateEventMarkup (string eventName, Type handlerType)
    {
      ArgumentUtility.CheckNotNull ("eventName", eventName);
      ArgumentUtility.CheckNotNull ("handlerType", handlerType);

      var eventMarkup = new XElement("Signature");
      eventMarkup.Add (CreateElement ("Keyword", "event"));
      eventMarkup.Add(CreateTypeOrKeywordElement(handlerType));
      eventMarkup.Add(CreateElement("Name", eventName));

      return eventMarkup;
    }

    public XElement CreateFieldMarkup(string fieldName, Type fieldType)
    {
      ArgumentUtility.CheckNotNull ("fieldName", fieldName);
      ArgumentUtility.CheckNotNull ("fieldType", fieldType);

      var eventMarkup = new XElement("Signature");
      eventMarkup.Add(CreateTypeOrKeywordElement(fieldType));
      eventMarkup.Add(CreateElement("Name", fieldName));

      return eventMarkup;
    }

    public XElement CreatePropertyMarkup (string propertyName, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      var eventMarkup = new XElement("Signature");
      eventMarkup.Add(CreateTypeOrKeywordElement(propertyType));
      eventMarkup.Add(CreateElement("Name", propertyName));

      return eventMarkup;
    }


    private XElement CreateTypeOrKeywordElement (Type type)
    {
      if (type.IsPrimitive || type == typeof (string))
        return CreateElement ("Keyword", GetShortName (type));
      return CreateElement ("Type", GetShortName (type));
    }

    private XElement CreateElement (string elementName, string content)
    {
      return content == null ? null : new XElement (elementName, content);
    }
  }
}