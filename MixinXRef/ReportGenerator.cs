using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace MixinXRef
{
  public class ReportGenerator
  {
    public XElement GenerateXml (IEnumerable<Assembly> assemblies)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      
      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (null, assemblies);

      var involvedTypeFinder = new InvolvedTypeFinder (mixinConfiguration);
      var typeIdentifierGenerator = new IdentifierGenerator<Type> ();
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly> ();

      var assemblyReportGenerator = new AssemblyReportGenerator (assemblies, assemblyIdentifierGenerator);
      var involvedTypeReportGenerator = new InvolvedTypeReportGenerator(involvedTypeFinder, typeIdentifierGenerator, assemblyIdentifierGenerator);
      
      return new XElement (
          "MixinXRefReport",
          assemblyReportGenerator.GenerateXml (),
          involvedTypeReportGenerator.GenerateXml());
    }
  }
}