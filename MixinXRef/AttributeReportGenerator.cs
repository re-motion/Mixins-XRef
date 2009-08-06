using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Utilities;

namespace MixinXRef
{
  public class AttributeReportGenerator : IReportGenerator
  {
    private readonly ReportContext _context;

    public AttributeReportGenerator (ReportContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      _context = context;
    }

    public XElement GenerateXml ()
    {
      var allAttributes = GetAllAttributes();

      return new XElement (
          "Attributes",
          from attribute in allAttributes
          where attribute.Assembly != typeof(IInitializableMixin).Assembly
          select
              GenerateAttributeElement (attribute)
          );
    }

 
    private HashSet<Type> GetAllAttributes ()
    {
      var allAttributes = new HashSet<Type>();

      foreach (var involvedType in _context.InvolvedTypes)
      {
        foreach (var attribute in involvedType.Type.GetCustomAttributes (true))
          allAttributes.Add (attribute.GetType());
      }
      return allAttributes;
    }

    private XElement GenerateAttributeElement (Type attribute)
    {
      return new XElement (
          "Attribute",
          new XAttribute ("id", _context.AttributeIdentifierGenerator.GetIdentifier (attribute)),
          new XAttribute ("assembly-ref", _context.AssemblyIdentifierGenerator.GetIdentifier (attribute.Assembly)),
          new XAttribute ("namespace", attribute.Namespace),
          new XAttribute ("name", attribute.Name)
          //new MemberReportGenerator (attribute).GenerateXml ()
          );
    }
  }
}