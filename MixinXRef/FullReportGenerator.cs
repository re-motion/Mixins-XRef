using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;

namespace MixinXRef
{
  public class FullReportGenerator
  {
    private readonly Assembly[] _assemblies;
    private readonly InvolvedType[] _involvedTypes;
    // MixinConfiguration _mixinConfiguration;
    private readonly ReflectedObject _mixinConfiguration;
    private readonly IRemotionReflection _remotionReflection;
    private readonly IOutputFormatter _outputFormatter;
    private string _creationTime;

    public FullReportGenerator (
        Assembly[] assemblies,
        InvolvedType[] involvedTypes,
        ReflectedObject mixinConfiguration,
        IRemotionReflection remotionReflection,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("_assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("_involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _assemblies = assemblies;
      _involvedTypes = involvedTypes;
      _mixinConfiguration = mixinConfiguration;
      _remotionReflection = remotionReflection;
      _outputFormatter = outputFormatter;
    }


    public string CreationTime
    {
      get { return _creationTime; }
    }

    public XDocument GenerateXmlDocument ()
    {
      CompositeReportGenerator compositeReportGenerator = CreateCompositeReportGenerator();

      var result = compositeReportGenerator.GenerateXml();

      _creationTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
      result.Add (new XAttribute ("creation-time", _creationTime));

      return new XDocument (result);
    }

    private CompositeReportGenerator CreateCompositeReportGenerator ()
    {
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();
      var readonlyAssemblyIdentifierGenerator =
          assemblyIdentifierGenerator.GetReadonlyIdentiferGenerator ("none");
      var readonlyInvolvedTypeIdentiferGenerator =
          new IdentifierPopulator<Type> (_involvedTypes.Select (it => it.Type)).GetReadonlyIdentifierGenerator ("none");
      var interfaceIdentiferGenerator = new IdentifierGenerator<Type>();
      var attributeIdentiferGenerator = new IdentifierGenerator<Type>();
      var configurationErrors = new ErrorAggregator<Exception>();
      var validationErrors = new ErrorAggregator<Exception>();

      var assemblyReport = new AssemblyReportGenerator (
          _assemblies, _involvedTypes, assemblyIdentifierGenerator, readonlyInvolvedTypeIdentiferGenerator);

      var involvedReport = new InvolvedTypeReportGenerator (
          _involvedTypes,
          _mixinConfiguration,
          readonlyAssemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          interfaceIdentiferGenerator,
          attributeIdentiferGenerator,
          configurationErrors,
          validationErrors,
          _remotionReflection,
          _outputFormatter);
      var interfaceReport = new InterfaceReportGenerator (
          _involvedTypes,
          readonlyAssemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          interfaceIdentiferGenerator,
          _remotionReflection,
          _outputFormatter);
      var attributeReport = new AttributeReportGenerator (
          _involvedTypes,
          readonlyAssemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          attributeIdentiferGenerator,
          _remotionReflection);
      var configurationErrorReport = new ConfigurationErrorReportGenerator (configurationErrors);
      var validationErrorReport = new ValidationErrorReportGenerator (validationErrors);

      return new CompositeReportGenerator (
          assemblyReport,
          involvedReport,
          interfaceReport,
          attributeReport,
          configurationErrorReport,
          validationErrorReport);
    }
  }
}