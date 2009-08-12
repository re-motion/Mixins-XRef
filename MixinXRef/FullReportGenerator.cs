using System;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace MixinXRef
{
  public class FullReportGenerator
  {
    private readonly Assembly[] _assemblies;
    private readonly InvolvedType[] _involvedTypes;
    private readonly MixinConfiguration _mixinConfiguration;
    private string _creationTime;

    public FullReportGenerator (Assembly[] assemblies, InvolvedType[] involvedTypes, MixinConfiguration mixinConfiguration)
    {
      ArgumentUtility.CheckNotNull ("_assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("_involvedTypes", involvedTypes);
      ArgumentUtility.CheckNotNull ("mixinConfiguration", mixinConfiguration);

      _assemblies = assemblies;
      _involvedTypes = involvedTypes;
      _mixinConfiguration = mixinConfiguration;
    }


    public string CreationTime
    {
      get { return _creationTime; }
    }

    public XDocument GenerateXmlDocument ()
    {
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();
      var readonlyassemblyIdentifierGenerator = assemblyIdentifierGenerator.GetReadonlyIdentiferGenerator ("none");
      var involvedTypeIdentiferGenerator = new IdentifierGenerator<Type>();
      var interfaceIdentiferGenerator = new IdentifierGenerator<Type>();
      var attributeIdentiferGenerator = new IdentifierGenerator<Type>();
      var configurationErrors = new ErrorAggregator<ConfigurationException>();
      var validationErrors = new ErrorAggregator<ValidationException>();

      var assemblyReport = new AssemblyReportGenerator (
          _assemblies, _involvedTypes, assemblyIdentifierGenerator, involvedTypeIdentiferGenerator);

      var involvedReport = new InvolvedTypeReportGenerator (
          _involvedTypes,
          _mixinConfiguration,
          readonlyassemblyIdentifierGenerator,
          involvedTypeIdentiferGenerator,
          interfaceIdentiferGenerator,
          attributeIdentiferGenerator,
          configurationErrors,
          validationErrors);
      var interfaceReport = new InterfaceReportGenerator (
          _involvedTypes, readonlyassemblyIdentifierGenerator, involvedTypeIdentiferGenerator, interfaceIdentiferGenerator);
      var attributeReport = new AttributeReportGenerator (
          _involvedTypes, readonlyassemblyIdentifierGenerator, involvedTypeIdentiferGenerator, attributeIdentiferGenerator);
      var configurationErrorReport = new ConfigurationErrorReportGenerator (configurationErrors);
      var validationErrorReport = new ValidationErrorReportGenerator (validationErrors);
      
      var compositeReportGenerator = new CompositeReportGenerator (
          assemblyReport,
          involvedReport,
          interfaceReport,
          attributeReport,
          configurationErrorReport,
          validationErrorReport);

      var result = compositeReportGenerator.GenerateXml();

      _creationTime = DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
      result.Add (new XAttribute ("creation-time", _creationTime));

      return new XDocument(result);
    }
  }
}