using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Utility;

namespace MixinXRef.Formatting
{
  public class OutputFormatter : IOutputFormatter
  {
    public string GetShortFormattedTypeName (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var name = BuildUnnestedTypeName(type);

      if (type.IsNested)
        name = GetShortFormattedTypeName (type.DeclaringType) + "." + name;

      return name;
    }

    private string BuildUnnestedTypeName (Type type)
    {
      var name = type.Name;

      name = FormatSimpleName (name);

      if (type.IsGenericType)
      {
        int index = name.IndexOf ('`');
        if (index != -1) // Happens for weird types
          name = name.Substring (0, index);
        name = name + BuildGenericSignature (type);
      }
      return name;
    }

    private string BuildGenericSignature (Type type)
    {
      var enclosingType = type.DeclaringType;
      int genericParameterCountInEnclosingType = enclosingType == null ? 0 : enclosingType.GetGenericArguments().Count();

      var genericArguments = type.GetGenericArguments()
          .Skip (genericParameterCountInEnclosingType)
          .Select (argType => argType.IsGenericParameter ? BuildUnnestedTypeName (argType) : GetShortFormattedTypeName (argType))
          .ToArray();

      return "<" + string.Join (", ", genericArguments) + ">";
    }

    private string FormatSimpleName (string name)
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
          return name.ToLower();
        default:
          return name;
      }
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


      markup.Add (CreateElement ("Keyword", prefix));

      markup.Add (CreateTypeOrKeywordElement (type));

      if (memberName.Contains ("."))
      {
        var parts = memberName.Split ('.');
        var partCount = parts.Length;
        memberName = parts[partCount - 1];
        markup.Add (CreateElement ("ExplicitInterfaceName", parts[partCount - 2]));
        markup.Add (CreateElement ("Text", "."));
      }
      
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