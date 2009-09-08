using System;
using System.Reflection;
using System.Xml.Linq;

namespace MixinXRef.Formatting
{
  public interface IOutputFormatter
  {
    string GetFormattedTypeName (Type type);
    string GetShortName (Type type);

    XElement CreateModifierMarkup (string attributes, string keywords);

    XElement CreateConstructorMarkup (string name, ParameterInfo[] parameterInfos);
    XElement CreateMethodMarkup (string methodName, Type returnType, ParameterInfo[] parameterInfos);
    XElement CreateEventMarkup (string eventName, Type handlerType);
    XElement CreateFieldMarkup(string fieldName, Type fieldType);
    XElement CreatePropertyMarkup(string propertyName, Type propertyType);
    XElement CreateNestedTypeMarkup (Type nestedType);
  }
}