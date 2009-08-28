using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Reflection;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace MixinXRef
{
  public class Program
  {
    private static int Main (string[] args)
    {
      var program = new Program (Console.In, Console.Out, new RemotionReflection());

      var argumentCheckResult = program.CheckArguments (args);
      if (argumentCheckResult != 0)
        return (argumentCheckResult);

      var assemblyDirectory = args[0];
      var outputDirectory = Path.Combine (args[1], "MixinDoc");
      var xmlFile = Path.Combine (outputDirectory, "MixinReport.xml");

      if (program.CreateOrOverrideOutputDirectory (outputDirectory) != 0)
        return (0);

      var assemblies = program.GetAssemblies (assemblyDirectory);
      if (assemblies == null)
        return (-4);

      program.SaveXmlDocument (assemblies, xmlFile);

      int transformerExitCode;
      using (new TimingScope ("GenerateHtmlFromXml"))
      {
        transformerExitCode = new XRefTransformer (xmlFile, outputDirectory).GenerateHtmlFromXml();
      }
      if (transformerExitCode == 0)
      {
        // copy resources folder
        new DirectoryInfo (@"xml_utilities\resources").CopyTo (Path.Combine (outputDirectory, "resources"));
        Console.WriteLine ("Mixin Documentation successfully generated to '{0}'", assemblyDirectory);
      }
      Console.ReadLine();
      return (transformerExitCode);
    }


    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly IRemotionReflection _remotionReflection;

    public Program (TextReader input, TextWriter output, IRemotionReflection remotionReflection)
    {
      ArgumentUtility.CheckNotNull ("input", input);
      ArgumentUtility.CheckNotNull ("output", output);
      ArgumentUtility.CheckNotNull ("remotionReflection", remotionReflection);


      _input = input;
      _output = output;
      _remotionReflection = remotionReflection;
    }

    public int CheckArguments (string[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      if (arguments.Length != 2)
      {
        _output.WriteLine ("usage: mixinxref <assemblyDirectory> <outputDirectory>");
        return -1;
      }

      if (!Directory.Exists (arguments[0]))
      {
        _output.WriteLine ("Input directory '{0}' does not exist", arguments[0]);
        return -2;
      }

      if (!Directory.Exists (arguments[1]))
      {
        _output.WriteLine ("Output directory '{0}' does not exist", arguments[1]);
        return -3;
      }

      return 0;
    }

    public int CreateOrOverrideOutputDirectory (string outputDirectory)
    {
      ArgumentUtility.CheckNotNull ("outputDirectory", outputDirectory);

      if (Directory.Exists (outputDirectory))
      {
        _output.WriteLine ("Output directory '{0}' does already exist", outputDirectory);
        _output.Write ("Do you want override the directory and including files? [y/N] ");

        var userInput = _input.ReadLine();
        if (userInput == null || !userInput.ToLower().StartsWith ("y"))
          return 1;
      }
      Directory.CreateDirectory (outputDirectory);

      return 0;
    }

    public Assembly[] GetAssemblies (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      var assemblies = new AssemblyBuilder (assemblyDirectory, _remotionReflection).GetAssemblies();
      if (assemblies.Length == 0)
      {
        _output.WriteLine ("'{0}' contains no assemblies", assemblyDirectory);
        return null;
      }
      return assemblies;
    }

    public void SaveXmlDocument (Assembly[] assemblies, string xmlFile)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("xmlFile", xmlFile);

      using (new TimingScope ("Complete SaveXmlDocument"))
      {
        MixinConfiguration mixinConfiguration;
        using (new TimingScope ("BuildConfigurationFromAssemblies"))
        {
          mixinConfiguration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (assemblies);
        }

        InvolvedType[] involvedTypes;
        using (new TimingScope ("FindInvolvedTypes"))
        {
          involvedTypes = new InvolvedTypeFinder (mixinConfiguration, assemblies).FindInvolvedTypes();
        }

        FullReportGenerator reportGenerator = new FullReportGenerator (assemblies, involvedTypes, mixinConfiguration);

        XDocument outputDocument;
        using (new TimingScope ("GenerateXmlDocument"))
        {
          outputDocument = reportGenerator.GenerateXmlDocument();
        }

        using (new TimingScope ("Save"))
        {
          outputDocument.Save (xmlFile);
        }
      }
    }
  }
}