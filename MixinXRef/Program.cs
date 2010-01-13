using System;
using System.IO;
using System.Reflection;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Remotion;
using MixinXRef.Report;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class Program
  {
    private static int Main (string[] args)
    {
      var startTime = DateTime.Now;

      var program = new Program (Console.In, Console.Out, new OutputFormatter());

      var argumentCheckResult = program.CheckArguments (args);
      if (argumentCheckResult != 0)
        return (argumentCheckResult);

      var assemblyDirectory = args[0];
      var outputDirectory = Path.GetFullPath (args[1]);
      var xmlFile = Path.Combine (outputDirectory, "MixinReport.xml");

      if (!Directory.Exists (outputDirectory))
        Directory.CreateDirectory (outputDirectory);

      if (ArgumentsContainCustomReflector(args))
        try
        {
          program.SetRemotionReflector (new RemotionReflectorFactory ().Create (assemblyDirectory, args[2]));
        }
        catch (Exception fileNotFoundOrTypeLoadException)
        {
          Console.WriteLine (fileNotFoundOrTypeLoadException.Message);
          return -5;
        }
      else
        program.SetRemotionReflector (new RemotionReflectorFactory().Create (assemblyDirectory));
      
      Console.WriteLine ("RemotionReflector '{0}' is used.", program._remotionReflector.GetType().FullName);

      var assemblies = program.GetAssemblies (assemblyDirectory);
      if (assemblies == null)
        return (-6);

      Console.WriteLine ("Generating MixinDoc");
      Console.Write ("  Generating XML ... ");
      program.SaveXmlDocument (assemblies, xmlFile);
      Console.WriteLine (GetElapsedTime(startTime));

      Console.Write ("  Applying XSLT ... ");
      var transformerExitCode = new XRefTransformer (xmlFile, outputDirectory).GenerateHtmlFromXml();
      if (transformerExitCode != 0)
      {
        Console.Error.WriteLine ("Error applying XSLT (code {0})", transformerExitCode);
        return transformerExitCode;
      }
      Console.WriteLine (GetElapsedTime (startTime));

      // copy resources folder
      new DirectoryInfo (@"xml_utilities\resources").CopyTo (Path.Combine (outputDirectory, "resources"));

      Console.WriteLine ("Mixin Documentation successfully generated to '{0}' in {1}.", outputDirectory, GetElapsedTime (startTime));

      return 0;
    }

    private readonly TextReader _input;
    private readonly TextWriter _output;
    private IRemotionReflector _remotionReflector;
    private readonly IOutputFormatter _outputFormatter;

    public Program (TextReader input, TextWriter output, IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("input", input);
      ArgumentUtility.CheckNotNull ("output", output);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _input = input;
      _output = output;
      _outputFormatter = outputFormatter;
    }

    public int CheckArguments (string[] arguments)
    {
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      var forceOverride = false;

      if (arguments.Length == 3 && arguments[2].ToLower().Equals ("-force"))
        forceOverride = true;
      if (arguments.Length == 4 && arguments[3].ToLower ().Equals ("-force"))
        forceOverride = true;

      if (arguments.Length < 2 || arguments.Length > 4)
      {
        _output.WriteLine("usage: mixinxref assemblyDirectory outputDirectory [customRemotionReflectorAssemblyQualifiedName] [-force]");
        _output.WriteLine ("Quitting MixinXRef");
        return -1;
      }

      if (!Directory.Exists (arguments[0]))
      {
        _output.WriteLine ("Input directory '{0}' does not exist", arguments[0]);
        _output.WriteLine ("Quitting MixinXRef");
        return -2;
      }

      if (Directory.Exists (arguments[1]) && !IsEmptyDirectory (arguments[1]) && forceOverride == false)
      {
        _output.WriteLine ("Output directory '{0}' is not empty", arguments[1]);
        _output.WriteLine ("Quitting MixinXRef");
        return -3;
      }

      if (arguments[1].IndexOfAny(Path.GetInvalidPathChars()) >= 0)
      {
        _output.WriteLine ("Output directory '{0}' contains invalid characters", arguments[1]);
        _output.WriteLine ("Quitting MixinXRef");
        return -4;
      }

      return 0;
    }

    public Assembly[] GetAssemblies (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      var assemblies = new AssemblyBuilder (assemblyDirectory, _remotionReflector).GetAssemblies();
      if (assemblies.Length == 0)
      {
        _output.WriteLine ("'{0}' contains no assemblies", assemblyDirectory);
        return null;
      }

      var remotionAssembly = _remotionReflector.FindRemotionAssembly (assemblies);
      if (remotionAssembly == null)
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

      var mixinConfiguration = _remotionReflector.BuildConfigurationFromAssemblies (assemblies);
      var configurationErrors = new ErrorAggregator<Exception>();
      var validationErrors = new ErrorAggregator<Exception>();
      var involvedTypes =
          new InvolvedTypeFinder (mixinConfiguration, assemblies, configurationErrors, validationErrors, _remotionReflector).FindInvolvedTypes();

      var reportGenerator = new FullReportGenerator (
          assemblies, involvedTypes, mixinConfiguration, configurationErrors, validationErrors, _remotionReflector, _outputFormatter);

      var outputDocument = reportGenerator.GenerateXmlDocument();

      outputDocument.Save (xmlFile);
    }

    public void SetRemotionReflector (IRemotionReflector remotionReflector)
    {
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);

      _remotionReflector = remotionReflector;
    }

    private bool IsEmptyDirectory (string path)
    {
      return Directory.GetFiles (path).Length == 0 && Directory.GetDirectories (path).Length == 0;
    }

    private static bool ArgumentsContainCustomReflector (string[] arguments)
    {
      if ((arguments.Length == 3 && !arguments[2].ToLower ().Equals ("-force")) || arguments.Length == 4)
        return true;
      else
        return false;
    }

    private static string GetElapsedTime (DateTime startTime)
    {
      DateTime elapsed = new DateTime () + (DateTime.Now - startTime); // TimeSpan does not implement IFormattable, but DateTime does!
      return elapsed.ToString ("mm:ss");
    }
  }
}