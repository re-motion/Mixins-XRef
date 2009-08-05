using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class MemberReportGenerator : IReportGenerator
  {
    private readonly IEnumerable<Type> _types;

    public MemberReportGenerator (IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("types", types);
      _types = types;
    }


    public XElement GenerateXml ()
    {
      return new XElement("Members");
    }
  }
}