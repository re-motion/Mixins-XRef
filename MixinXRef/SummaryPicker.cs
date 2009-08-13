using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Remotion.Utilities;

namespace MixinXRef
{
  public class SummaryPicker
  {
    private static readonly XElement s_noSummary = new XElement ("summary", "No summary found.");
    private static readonly Regex s_normalizeTrim = new Regex (@"\s+", RegexOptions.Compiled);

    public XElement GetSummary (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      // get path and filename of xml summary
      var documentationFileName = Path.GetFileNameWithoutExtension (type.Assembly.Location) + ".xml";

      // check if xml document exists
      if (!File.Exists (documentationFileName))
        return s_noSummary;

      // open docu
      var xmlDocument = XDocument.Load (documentationFileName);

      // search for member
      string searchName = "T:" + type.FullName;
      string xpath = String.Format ("//member[@name = '{0}']/summary", searchName);
      var summary = xmlDocument.XPathSelectElement (xpath);

      // xpath expression returned no result
      if (summary == null)
        return s_noSummary;

      // normalize and trim summary content
      return NormalizeAndTrim (summary);
    }

    public XElement NormalizeAndTrim (XElement element)
    {
      ArgumentUtility.CheckNotNull ("element", element);

      var normalizedElement = s_normalizeTrim.Replace (element.ToString(), " ").Replace(" <", "<").Replace("> ", ">");

      return XElement.Parse(normalizedElement);
    }
  }
}