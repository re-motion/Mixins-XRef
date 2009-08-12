using System;
using System.IO;
using System.Reflection;
using Remotion.Mixins.Context;

namespace MixinXRef
{
  internal class Program
  {
    private static int Main (string[] args)
    {
      var argumentCheckResult =CheckArguments(args);
      if (argumentCheckResult != 0)
        System.Environment.Exit(argumentCheckResult);

      var assemblyDirectory = args[0];
      var outputDirectory = Path.Combine(args[1], "MixinDoc");
      var xmlFile = Path.Combine(outputDirectory, "MixinReport.xml");

      if (CreateOrOverrideOutputDirectory(outputDirectory) != 0)
        System.Environment.Exit(0);

      var assemblies = GetAssemblies(assemblyDirectory);
      if (assemblies == null)
        System.Environment.Exit(-4);

      var mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);
      var involvedTypes = new InvolvedTypeFinder (mixinConfiguration).FindInvolvedTypes();

      var reportGenerator = new FullReportGenerator (assemblies, involvedTypes, mixinConfiguration);
      var outputDocument = reportGenerator.GenerateXmlDocument();
      outputDocument.Save (xmlFile);

      var transformerExitCode = new XRefTransformer (xmlFile, outputDirectory).GenerateHtmlFromXml();
      if (transformerExitCode == 0)
        Console.WriteLine ("Mixin Documentation successfully generated to '{0}'", assemblyDirectory);

      return transformerExitCode;
    }

    public static Assembly[] GetAssemblies (string assemblyDirectory)
    {
      var assemblies = new AssemblyBuilder (assemblyDirectory).GetAssemblies();
      if (assemblies.Length == 0)
      {
        Console.WriteLine("'{0}' contains no assemblies", assemblyDirectory);
        return null;
      }
      return assemblies;
    }

    public static int CreateOrOverrideOutputDirectory (string outputDirectory)
    {
      if (Directory.Exists(outputDirectory))
      {
        Console.WriteLine("Output directory '{0}' does already exist", outputDirectory);
        Console.Write("Do you want override the directory and including files? [y/N] ");
        var overrideOutputDirectory = Console.ReadLine().ToLower().StartsWith("y");

        if (!overrideOutputDirectory)
          return 1;
      }
      Directory.CreateDirectory (outputDirectory);
      return 0;
    }

    public static int CheckArguments (string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("usage: mixinxref <assemblyDirectory> <outputDirectory>");
        return -1;
      }
      
      if (!Directory.Exists(args[0]))
      {
        Console.WriteLine("Input directory '{0}' does not exist", assemblyDirectory);
        return -2;
      }

      if (!Directory.Exists(args[1]))
      {
        Console.WriteLine("Output directory '{0}' does not exist", args[1]);
        return -3;
      }
      
      return 0;
    }
  }
}