using System;
using System.Linq;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.Utility;

namespace MixinXRef.Report
{
  public class AdditionalDependencyReportGenerator : IReportGenerator
  {
    private readonly ReflectedObject _explicitDependencies;
    private readonly IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private readonly IOutputFormatter _outputFormatter;

    public AdditionalDependencyReportGenerator (
        ReflectedObject explicitDependencies,
        IIdentifierGenerator<Type> involvedTypeIdentifierGenerator,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);
      ArgumentUtility.CheckNotNull ("involvedTypeIdentifierGenerator", involvedTypeIdentifierGenerator);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _explicitDependencies = explicitDependencies;
      _involvedTypeIdentifierGenerator = involvedTypeIdentifierGenerator;
      _outputFormatter = outputFormatter;
    }

    public XElement GenerateXml ()
    {
      return new XElement (
          "AdditionalDependencies",
          from explicitDependencyType in _explicitDependencies
          select new XElement (
              "AdditionalDependency",
              new XAttribute ("ref", _involvedTypeIdentifierGenerator.GetIdentifier (explicitDependencyType.To<Type>())),
              new XAttribute ("instance-name", _outputFormatter.GetShortFormattedTypeName (explicitDependencyType.To<Type>()))
              )
          );
    }
  }
}