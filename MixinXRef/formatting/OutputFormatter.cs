using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace MixinXRef.Formatting
{
  public class OutputFormatter : IOutputFormatter
  {
    public string GetFormattedNestedTypeName (Type type)
    {
      if (type.FullName == null)
        return type.Name;

      var nestedTypeName = new StringBuilder();

      var nestedIndex = type.FullName.IndexOf ('+');
      var nestingType = type.FullName.Substring (0, nestedIndex);
      var nestedType = type.FullName.Substring (nestedIndex);
      
      nestedTypeName.Append ( nestingType.Substring (nestingType.LastIndexOf ('.')+1) );
      nestedTypeName.Append ( nestedType.Replace("+", "."));

      return nestedTypeName.ToString();
    }

    public string GetFormattedGenericTypeName (Type type)
    {
      var typeName = "";
      var nestedTypeName = "";

      if (type.IsNested)
      {
        typeName = (type.FullName.Substring (0, type.FullName.IndexOf ('`')));
        typeName = typeName.Substring (typeName.LastIndexOf ('.') + 1);
        
        var index = type.FullName.IndexOf ('+');
        if (index > 0)
        {
          nestedTypeName = (type.FullName.Substring (index, type.FullName.Length - index));
          nestedTypeName = "." + nestedTypeName.Substring (1, nestedTypeName.IndexOf ('[') - 1);
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

        result.Append (GetShortFormattedTypeName (genericArguments[i]));
      }
      result.Append (">");
      result.Append (nestedTypeName);

      return result.ToString();
    }

    public XElement CreateModifierMarkup (string attributes, string keywords)
    {
      var modifiers = new XElement ("Modifiers");

      foreach (var attribute in attributes.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
      {
        modifiers.Add (CreateElement ("Text", "["));
        modifiers.Add (CreateElement ("Type", attribute));
        modifiers.Add (CreateElement ("Text", "]"));
      }

      foreach (var keyword in keywords.Split (new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
        modifiers.Add (CreateElement ("Keyword", keyword));

      return modifiers;
    }

    public string GetShortFormattedTypeName (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var name = type.Name;

      if (type.IsGenericType)
        return GetFormattedGenericTypeName (type);

      if (type.IsNested)
        return GetFormattedNestedTypeName (type);

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
          return name.ToLower();
        default:
          return type.Name;
      }
    }

    public void AddParameterMarkup (ParameterInfo[] parameterInfos, XElement signatureElement)
    {
      ArgumentUtility.CheckNotNull ("parameterInfos", parameterInfos);
      ArgumentUtility.CheckNotNull ("signatureElement", signatureElement);

      signatureElement.Add (CreateElement ("Text", "("));

      for (int i = 0; i < parameterInfos.Length; i++)
      {
        if (i != 0)
          signatureElement.Add (CreateElement ("Text", ","));

        signatureElement.Add (CreateTypeOrKeywordElement (parameterInfos[i].ParameterType));
        signatureElement.Add (CreateElement ("ParameterName", parameterInfos[i].Name));
      }

      signatureElement.Add (CreateElement ("Text", ")"));
    }

    public XElement CreateConstructorMarkup (string name, ParameterInfo[] parameterInfos)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("parameterInfos", parameterInfos);

      return CreateMemberMarkup (null, null, name, parameterInfos);
    }

    public XElement CreateMethodMarkup (string methodName, Type returnType, ParameterInfo[] parameterInfos)
    {
      ArgumentUtility.CheckNotNull ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("returnType", returnType);
      ArgumentUtility.CheckNotNull ("parameterInfos", parameterInfos);

     return CreateMemberMarkup (null, returnType, methodName, parameterInfos);
    }

    public XElement CreateEventMarkup (string eventName, Type handlerType)
    {
      ArgumentUtility.CheckNotNull ("eventName", eventName);
      ArgumentUtility.CheckNotNull ("handlerType", handlerType);

      return CreateMemberMarkup ("event", handlerType, eventName, null);
    }

    public XElement CreateFieldMarkup (string fieldName, Type fieldType)
    {
      ArgumentUtility.CheckNotNull ("fieldName", fieldName);
      ArgumentUtility.CheckNotNull ("fieldType", fieldType);

      return CreateMemberMarkup (null, fieldType, fieldName, null);
    }

    public XElement CreatePropertyMarkup (string propertyName, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      return CreateMemberMarkup (null, propertyType, propertyName, null);
    }

    public XElement CreateNestedTypeMarkup (Type nestedType)
    {
      if (nestedType.IsEnum)
        return CreateMemberMarkup ("enum", null, nestedType.Name, null);

      string prefix;
      if (nestedType.IsClass)
        prefix = "class";
      else if (nestedType.IsInterface)
        prefix = "interface";
      else if (nestedType.IsValueType)
        prefix = "struct";
      else
        prefix = "unknown";

      var nestedTypeMarkup = CreateMemberMarkup (prefix, null, nestedType.Name, null);

      var inheritance = new List<Type>();
      if (nestedType.BaseType != null && nestedType.BaseType != typeof (object) && nestedType.BaseType != typeof (ValueType))
        inheritance.Add (nestedType.BaseType);
      inheritance.AddRange (nestedType.GetInterfaces());

      for (int i = 0; i < inheritance.Count; i++)
      {
        if (i == 0)
          nestedTypeMarkup.Add (CreateElement ("Text", ":"));
        else
          nestedTypeMarkup.Add (CreateElement ("Text", ","));
        nestedTypeMarkup.Add (CreateElement ("Type", GetShortFormattedTypeName (inheritance[i])));
      }

      return nestedTypeMarkup;
    }


    private XElement CreateMemberMarkup (string prefix, Type type, string memberName, ParameterInfo[] parameterInfos)
    {
      var markup = new XElement ("Signature");

      if (memberName.Contains ("."))
      {
        var parts = memberName.Split ('.');
        var partCount = parts.Length;
        memberName = parts[partCount - 2] + "." + parts[partCount - 1];
      }

      markup.Add (CreateElement ("Keyword", prefix));
      markup.Add (CreateTypeOrKeywordElement (type));
      markup.Add (CreateElement ("Name", memberName));

      if (parameterInfos != null)
        AddParameterMarkup (parameterInfos, markup);

      return markup;
    }

    private XElement CreateTypeOrKeywordElement (Type type)
    {
      if (type == null)
        return null;

      if (type.IsPrimitive || type == typeof (string) || type == typeof (void))
        return CreateElement ("Keyword", GetShortFormattedTypeName (type));
      return CreateElement ("Type", GetShortFormattedTypeName (type));
    }

    private XElement CreateElement (string elementName, string content)
    {
      return content == null ? null : new XElement (elementName, content);
    }
  }
}