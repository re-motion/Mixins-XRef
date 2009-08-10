using System;
using System.Linq;
using System.Xml.Linq;
using Remotion.Utilities;

namespace MixinXRef
{
  public class MemberReportGenerator : IReportGenerator
  {
    private readonly Type _type;

    public MemberReportGenerator (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _type = type;
    }


    public XElement GenerateXml ()
    {
      return new XElement (
          "PublicMembers",
          from memberInfo in _type.GetMembers()
          where memberInfo.DeclaringType == _type
          select
              new XElement (
              "Member",
              new XAttribute ("type", memberInfo.MemberType),
              new XAttribute ("name", memberInfo.Name)
              )
          );
    }
  }
}