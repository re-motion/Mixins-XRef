using System;

namespace MixinXRef.Formatting
{
  public interface IOutputFormatter
  {
    string GetFormattedTypeName (Type type);
    string CreateModifierMarkup (string visibility, bool overridden);
  }
}