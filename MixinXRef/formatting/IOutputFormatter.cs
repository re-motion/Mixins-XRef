using System;
using System.Xml.Linq;

namespace MixinXRef.Formatting
{
  public interface IOutputFormatter
  {
    string GetFormattedTypeName (Type type);
    string GetShortName (Type type);
    XElement CreateModifierMarkup (string keywords);
  }
}