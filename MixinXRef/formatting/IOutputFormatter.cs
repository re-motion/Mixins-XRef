using System;

namespace MixinXRef.Formatting
{
  public interface IOutputFormatter
  {
    string GetCSharpLikeName (Type type);
    string CreateModifierMarkup (string visibility, bool overridden);
  }
}