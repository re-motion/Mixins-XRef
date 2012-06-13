using System;
using MixinXRef;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility.Options;
using TalkBack;

namespace MixinXRefConsole
{
  public class Program
  {
    private static int Main (string[] args)
    {
      args = TalkBackChannel.Initialize (args);

      var cmdLineArgs = XRefArguments.Instance;
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

      TalkBackInvoke.Action (sender => XRef.Run (cmdLineArgs, sender), MessageReceived);
      return 0;
    }

    private static void MessageReceived (Message message)
    {
      TalkBackChannel.Out.SendMessage (message);

      switch (message.Severity)
      {
        case MessageSeverity.Error:
          Console.Error.WriteLine ("ERROR: {0}", message.Text);
          break;
        case MessageSeverity.Warning:
          Console.WriteLine ("WARNING: {0}", message.Text);
          break;
        default:
          Console.WriteLine (message.Text);
          break;
      }
    }

    private static void PrintUsage (OptionSet optionSet)
    {
      optionSet.WriteOptionDescriptions (Console.Out);
    }

    private static int CheckArguments (XRefArguments cmdLineArgs)
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

      return 0;
    }
  }
}