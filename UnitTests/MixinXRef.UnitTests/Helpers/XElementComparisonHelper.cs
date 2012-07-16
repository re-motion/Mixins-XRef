using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Helpers
{
  public static class XElementComparisonHelper
  {
    private class XElementComparisonStringGenerator
    {
      private readonly IEnumerable<string> _ignoredAttributes;

      public XElementComparisonStringGenerator (params string[] ignoredAttributes)
      {
        _ignoredAttributes = ignoredAttributes;
      }

      public string Generate (XElement x)
      {
        var s = x.ToString ();
        foreach (var ignoredAttribute in _ignoredAttributes)
          s = Regex.Replace (s, string.Format ("{0}=\"[^\"]*\" ", ignoredAttribute), "");

        return s;
      }
    }

    private static readonly XElementComparisonStringGenerator s_stringGenerator =
      new XElementComparisonStringGenerator("metadataToken");

    public static void Compare(XElement actual, XElement expected)
    {
      Assert.That(s_stringGenerator.Generate(actual), Is.EqualTo(s_stringGenerator.Generate(expected)));
    }
  }

  
}
