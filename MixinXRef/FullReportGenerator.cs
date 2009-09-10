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
    private readonly ErrorAggregator<Exception> _configurationErrors;
    private readonly ErrorAggregator<Exception> _validationErrors;
    private readonly IRemotionReflection _remotionReflection;
    private readonly IOutputFormatter _outputFormatter;
    private string _creationTime;

    public FullReportGenerator (
        Assembly[] assemblies,
        InvolvedType[] involvedTypes,
        ReflectedObject mixinConfiguration,
        ErrorAggregator<Exception> configurationErrors,
        ErrorAggregator<Exception> validationErrors,
        IRemotionReflection remotionReflection,
        IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("_assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("_involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);
      ArgumentUtility.CheckNotNull ("configurationErrors", configurationErrors);
      ArgumentUtility.CheckNotNull ("validationErrors", validationErrors);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _assemblies = assemblies;
      _involvedTypes = involvedTypes;
      _mixinConfiguration = mixinConfiguration;
      _configurationErrors = configurationErrors;
      _validationErrors = validationErrors;
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

      var assemblyReport = new AssemblyReportGenerator (
          _assemblies, _involvedTypes, assemblyIdentifierGenerator, readonlyInvolvedTypeIdentiferGenerator);

      var involvedReport = new InvolvedTypeReportGenerator (
          _involvedTypes,
          readonlyAssemblyIdentifierGenerator,
          readonlyInvolvedTypeIdentiferGenerator,
          interfaceIdentiferGenerator,
          attributeIdentiferGenerator,
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
      var configurationErrorReport = new ConfigurationErrorReportGenerator (_configurationErrors);
      var validationErrorReport = new ValidationErrorReportGenerator (_validationErrors);

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