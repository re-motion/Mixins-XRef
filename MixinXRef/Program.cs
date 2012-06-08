using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MixinXRef.Formatting;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Report;
using MixinXRef.Utility;
using MixinXRef.Utility.Options;

namespace MixinXRef
{
  public class Program
  {
    private static int Main (string[] args)
    {
      var startTime = DateTime.Now;

      var cmdLineArgs = CommandLineArguments.Instance;
      var showOptionsHelp = false;
      var options = new OptionSet
                      {
                        {
                          "i=|input-directory=", "Directory that contains the assemblies to analyze",
                          v => cmdLineArgs.AssemblyDirectory = v
                        }, 
                        {
                          "o=|output-directory=", "Output directory. Execution is stopped if this directory exists. Force overwrite with -f.",
                          v => cmdLineArgs.OutputDirectory = v
                        }, 
                        {
                          "x=|xml-outputfile=", "File path to a custom output file for the generated XML",
                          v => cmdLineArgs.XMLOutputFileName = v
                        }, 
                        {
                          "f|force-overwrite", "Forces all existing files to be overwritten", 
                          v => cmdLineArgs.OverwriteExistingFiles = true
                        }, 
                        {
                          "s|skip-html", "Skip generation of HTML documentation",
                          v => cmdLineArgs.SkipHTMLGeneration = true
                        },
                        {
                          "r|reflector-assembly=", "File path to an assembly that contains one or more reflectors. " + 
                                                   "You can specify more that one assembly by using wildcards (e.g. MixinXRef.Reflectors*.dll).",
                          v =>
                            {
                              cmdLineArgs.ReflectorSource = ReflectorSource.ReflectorAssembly;
                              cmdLineArgs.ReflectorPath = v;
                            }
                        }, 
                        {
                          "c|custom-reflector=", "An assembly qualified type name that is used as a custom reflector. " + 
                                                 "This type has to implement " + typeof(IRemotionReflector).Name + ".",
                          v =>
                            {
                              cmdLineArgs.ReflectorSource = ReflectorSource.CustomReflector;
                              cmdLineArgs.CustomReflectorAssemblyQualifiedTypeName = v;
                            }
                        }, 
                        {
                          "h|?|help", "Show this help page",
                          v => showOptionsHelp = true
                        }
                      };

      try
      {
        options.Parse (args);
      }
      catch (OptionException e)
      {
        Console.Error.WriteLine ("Error while parsing the command line options: {0}", e.Message);
        return 1;
      }

      if (showOptionsHelp)
      {
        PrintUsage (options);
        return 0;
      }

      var argsExitCode = CheckArguments (cmdLineArgs);
      if (argsExitCode == 1)
      {
        PrintUsage (options);
        return argsExitCode;
      }
      if (argsExitCode != 0)
      {
        return argsExitCode;
      }

      IRemotionReflector reflector = null;
      switch (cmdLineArgs.ReflectorSource)
      {
        case ReflectorSource.ReflectorAssembly:
          reflector = RemotionReflectorFactory.Create (cmdLineArgs.AssemblyDirectory, cmdLineArgs.ReflectorPath);
          break;
        case ReflectorSource.CustomReflector:
          var customReflector = Type.GetType (cmdLineArgs.CustomReflectorAssemblyQualifiedTypeName);
          if (customReflector == null)
          {
            Console.Error.WriteLine ("Custom reflector can not be found");
            return 2;
          }
          if (!typeof (IRemotionReflector).IsAssignableFrom (customReflector))
          {
            Console.WriteLine ("Specified custom reflector {0} does not implement {1}", customReflector, typeof (IRemotionReflector).FullName);
            return 2;
          }
          reflector = RemotionReflectorFactory.Create (cmdLineArgs.AssemblyDirectory, customReflector);
          break;
        case ReflectorSource.Unspecified:
          throw new IndexOutOfRangeException ("Reflector source is unspecified");
      }

      Console.WriteLine ("RemotionReflector '{0}' is used.", reflector.GetType ().FullName);
      Console.WriteLine ("Generating MixinDoc");

      var program = new Program (reflector, new OutputFormatter ());
      var assemblies = program.GetAssemblies (cmdLineArgs.AssemblyDirectory);
      if (!assemblies.Any ())
      {
        Console.Error.WriteLine ("\"{0}\" contains no assemblies", cmdLineArgs.AssemblyDirectory);
        return 1;
      }

      var xmlFile = Path.Combine (cmdLineArgs.OutputDirectory,
                                 !string.IsNullOrEmpty (cmdLineArgs.XMLOutputFileName)
                                   ? cmdLineArgs.XMLOutputFileName
                                   : "MixinXRef.xml");

      var xmlStartTime = DateTime.Now;
      Console.Write ("  Generating XML ... ");
      program.GenerateAndSaveXmlDocument (assemblies, xmlFile);
      Console.WriteLine (GetElapsedTime (xmlStartTime));

      assemblies = null;
      GC.Collect ();
      GC.WaitForPendingFinalizers ();

      if (!cmdLineArgs.SkipHTMLGeneration)
      {
        var xslStartTime = DateTime.Now;
        Console.Write ("  Applying XSLT ... ");
        var transformerExitCode = new XRefTransformer (xmlFile, cmdLineArgs.OutputDirectory).GenerateHtmlFromXml ();
        if (transformerExitCode != 0)
        {
          Console.Error.WriteLine ("Error applying XSLT (code {0})", transformerExitCode);
          return transformerExitCode;
        }
        Console.WriteLine (GetElapsedTime (xslStartTime));

        // copy resources folder
        var xRefPath = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
        new DirectoryInfo (Path.Combine (xRefPath, @"xml_utilities\resources")).CopyTo (Path.Combine (cmdLineArgs.OutputDirectory, "resources"));

        Console.WriteLine ("Mixin Documentation successfully generated to '{0}' in {1}.", cmdLineArgs.OutputDirectory, GetElapsedTime (startTime));
      }
      else
      {
        Console.WriteLine ("  Skipping HTML generation");
      }

      return 0;
    }

    private static void PrintUsage (OptionSet optionSet)
    {
      optionSet.WriteOptionDescriptions (Console.Out);
    }

    private static int CheckArguments (CommandLineArguments cmdLineArgs)
    {
      if (string.IsNullOrEmpty (cmdLineArgs.AssemblyDirectory))
      {
        Console.Error.WriteLine ("Input directory missing");
        return 1;
      }

      if (string.IsNullOrEmpty (cmdLineArgs.OutputDirectory))
      {
        Console.Error.WriteLine ("Output directory missing");
        return 1;
      }

      if (cmdLineArgs.ReflectorSource == ReflectorSource.Unspecified)
      {
        Console.Error.WriteLine ("Reflector is missing. Either provide a reflector assembly or a custom reflector.");
        return 1;
      }

      if (!Directory.Exists (cmdLineArgs.AssemblyDirectory))
      {
        Console.Error.WriteLine ("Input directory does not exist");
        return 2;
      }

      var invalid = cmdLineArgs.OutputDirectory.Intersect (Path.GetInvalidPathChars ());
      if (invalid.Any ())
      {
        Console.Error.WriteLine ("Output directory contains invalid characters: {0}", string.Join (" ", invalid.Select (c => c.ToString ()).ToArray ()));
        return 2;
      }

      if (Directory.Exists (cmdLineArgs.OutputDirectory) && !cmdLineArgs.OverwriteExistingFiles)
      {
        Console.Error.WriteLine ("Output directory already exists. Use -f option if you are sure you want to overwrite all existing files.");
        return 2;
      }

      return 0;
    }

    private static string GetElapsedTime (DateTime startTime)
    {
      var elapsed = new DateTime () + (DateTime.Now - startTime); // TimeSpan does not implement IFormattable, but DateTime does!
      return elapsed.ToString ("mm:ss");
    }

    private readonly IRemotionReflector _remotionReflector;
    private readonly IOutputFormatter _outputFormatter;

    public Program (IRemotionReflector remotionReflector, IOutputFormatter outputFormatter)
    {
      ArgumentUtility.CheckNotNull ("remotionReflector", remotionReflector);
      ArgumentUtility.CheckNotNull ("outputFormatter", outputFormatter);

      _remotionReflector = remotionReflector;
      _outputFormatter = outputFormatter;
    }

    public Assembly[] GetAssemblies (string assemblyDirectory)
    {
      ArgumentUtility.CheckNotNull ("assemblyDirectory", assemblyDirectory);

      return new AssemblyBuilder(assemblyDirectory).GetAssemblies( 
          a => _remotionReflector.IsRelevantAssemblyForConfiguration(a) && 
               !_remotionReflector.IsNonApplicationAssembly(a));
    }

    public void GenerateAndSaveXmlDocument (Assembly[] assemblies, string xmlFile)
    {
      ArgumentUtility.CheckNotNull ("assemblies", assemblies);
      ArgumentUtility.CheckNotNull ("xmlFile", xmlFile);

      var mixinConfiguration = _remotionReflector.BuildConfigurationFromAssemblies (assemblies);
      var configurationErrors = new ErrorAggregator<Exception> ();
      var validationErrors = new ErrorAggregator<Exception> ();
      var involvedTypes =
          new InvolvedTypeFinder (mixinConfiguration, assemblies, configurationErrors, validationErrors, _remotionReflector).FindInvolvedTypes ();

      var reportGenerator = new FullReportGenerator (involvedTypes, configurationErrors, validationErrors, _remotionReflector, _outputFormatter);

      var outputDocument = reportGenerator.GenerateXmlDocument ();

      outputDocument.Save (xmlFile);

      reportGenerator = null;
      GC.Collect ();
    }
  }
}