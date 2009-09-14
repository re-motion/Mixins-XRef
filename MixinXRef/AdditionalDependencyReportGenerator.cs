using System;
using System.Xml.Linq;
using MixinXRef.Reflection;

namespace MixinXRef
{
  public class AdditionalDependencyReportGenerator :IReportGenerator
  {
    private readonly ReflectedObject _explicitDependencies;

    public AdditionalDependencyReportGenerator (ReflectedObject explicitDependencies)
    {
      _explicitDependencies = explicitDependencies;
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);
    }

    public XElement GenerateXml ()
    {
      return new XElement ("AdditionalDependencies");
    }
  }
}