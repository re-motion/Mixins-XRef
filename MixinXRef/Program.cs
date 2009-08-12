using System;
using System.IO;
using Remotion.Mixins.Context;

namespace MixinXRef
{
  internal class Program
  {
    private static int Main (string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("usage: mixinxref <assemblyDirectory> <outputDirectory>");
        return -1;
      }
      var assemblyDirectory = args[0];
      var outputDirectory = Path.Combine(args[1], "MixinDoc");
      var xmlFile = Path.Combine (outputDirectory, "MixinReport.xml");

      Directory.CreateDirectory (outputDirectory);

      var assemblies = new AssemblyBuilder (assemblyDirectory).GetAssemblies();
      if (assemblies.Length == 0)
      {
        Console.WriteLine("'{0}' contains no assemblies", assemblyDirectory);
        return -2;
      }

      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);
      var involvedTypes = new InvolvedTypeFinder (mixinConfiguration).FindInvolvedTypes();

      var reportGenerator = new FullReportGenerator (assemblies, involvedTypes, mixinConfiguration);
      var outputDocument = reportGenerator.GenerateXmlDocument();

      outputDocument.Save (xmlFile);
      var transformerExitCode = new XRefTransformer (xmlFile).GenerateHtmlFromXml();

      if (transformerExitCode == 0)
        Console.WriteLine ("Mixin Documentation successfully generated to {0}", assemblyDirectory);

      return transformerExitCode;
    }
  }
}