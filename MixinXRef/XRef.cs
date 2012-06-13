using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MixinXRef.Formatting;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Report;
using MixinXRef.Utility;
using TalkBack;

namespace MixinXRef
{
  public static class XRef
  {
    private static readonly object s_locker = new object ();
    private static readonly TalkbackChannel s_defaultChannel = new TalkbackChannel ();

    private static ITalkbackChannel s_log;
    internal static ITalkbackChannel Log
    {
      get { return s_log ?? s_defaultChannel; }
    }

    static XRef ()
    {
      s_defaultChannel.Subscribe (message => Console.WriteLine (message.Text), MessageSeverity.Info | MessageSeverity.Debug | MessageSeverity.Warning);
      s_defaultChannel.Subscribe (message => Console.Error.WriteLine (message.Text), MessageSeverity.Error);
    }

    public static bool Run (XRefArguments arguments, ITalkbackChannel logger = null)
    {
      lock (s_locker)
      {
        s_log = logger;
        var success = InternalRun (arguments);
        s_log = null;
        return success;
      }
    }

    private static bool InternalRun (XRefArguments arguments)
    {
      var argsOk = CheckArguments (arguments);
      if (!argsOk)
        return false;

      IRemotionReflector reflector = null;
      switch (arguments.ReflectorSource)
      {
        case ReflectorSource.ReflectorAssembly:
          reflector = RemotionReflectorFactory.Create (arguments.AssemblyDirectory, arguments.ReflectorPath);
          break;
        case ReflectorSource.CustomReflector:
          var customReflector = Type.GetType (arguments.CustomReflectorAssemblyQualifiedTypeName);
          if (customReflector == null)
          {
            Log.SendError ("Custom reflector can not be found");
            return false;
          }
          if (!typeof (IRemotionReflector).IsAssignableFrom (customReflector))
          {
            Log.SendError ("Specified custom reflector {0} does not implement {1}", customReflector, typeof (IRemotionReflector).FullName);
            return false;
          }
          reflector = RemotionReflectorFactory.Create (arguments.AssemblyDirectory, customReflector);
          break;
        case ReflectorSource.Unspecified:
          throw new IndexOutOfRangeException ("Reflector source is unspecified");
      }

      var assemblies = new AssemblyBuilder (arguments.AssemblyDirectory)
        .GetAssemblies (a => !reflector.IsRelevantAssemblyForConfiguration (a) || !reflector.IsNonApplicationAssembly (a));

      if (!assemblies.Any ())
      {
        Log.Send (new Message (MessageSeverity.Error, "\"{0}\" contains no assemblies", arguments.AssemblyDirectory));
        return false;
      }

      var xmlFile = Path.Combine (arguments.OutputDirectory,
                                 !string.IsNullOrEmpty (arguments.XMLOutputFileName)
                                   ? arguments.XMLOutputFileName
                                   : "MixinXRef.xml");

      var relevantAssemblies = Array.FindAll (assemblies, a => reflector.IsRelevantAssemblyForConfiguration (a));
      var mixinConfiguration = reflector.BuildConfigurationFromAssemblies (relevantAssemblies);
      var outputFormatter = new OutputFormatter ();
      var configurationErrors = new ErrorAggregator<Exception> ();
      var validationErrors = new ErrorAggregator<Exception> ();

      var involvedTypes = new InvolvedTypeFinder (mixinConfiguration, assemblies, configurationErrors, validationErrors, reflector).FindInvolvedTypes ();
      var reportGenerator = new FullReportGenerator (involvedTypes, configurationErrors, validationErrors, reflector, outputFormatter);
      var outputDocument = reportGenerator.GenerateXmlDocument ();

      outputDocument.Save (xmlFile);

      reportGenerator = null;
      GC.Collect ();

      assemblies = null;
      GC.Collect ();
      GC.WaitForPendingFinalizers ();

      if (!arguments.SkipHTMLGeneration)
      {
        var transformerExitCode = new XRefTransformer (xmlFile, arguments.OutputDirectory).GenerateHtmlFromXml ();
        if (transformerExitCode != 0)
        {
          Log.SendError ("Error applying XSLT (code {0})", transformerExitCode);
          return false;
        }

        // copy resources folder
        var xRefPath = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
        new DirectoryInfo (Path.Combine (xRefPath, @"xml_utilities\resources")).CopyTo (Path.Combine (arguments.OutputDirectory, "resources"));

        Log.SendInfo ("Mixin Documentation successfully generated to '{0}'.", arguments.OutputDirectory);
      }
      else
      {
        Log.SendInfo ("  Skipping HTML generation");
      }

      return true;
    }

    private static bool CheckArguments (XRefArguments arguments)
    {
      if (string.IsNullOrEmpty (arguments.AssemblyDirectory))
      {
        Log.SendError ("Input directory missing");
        return false;
      }

      if (string.IsNullOrEmpty (arguments.OutputDirectory))
      {
        Log.SendError ("Output directory missing");
        return false;
      }

      if (arguments.ReflectorSource == ReflectorSource.Unspecified)
      {
        Log.SendError ("Reflector is missing. Either provide a reflector assembly or a custom reflector.");
        return false;
      }

      if (!Directory.Exists (arguments.AssemblyDirectory))
      {
        Log.SendError ("Input directory does not exist");
        return false;
      }

      var invalid = arguments.OutputDirectory.Intersect (Path.GetInvalidPathChars ());
      if (invalid.Any ())
      {
        Log.SendError ("Output directory contains invalid characters: {0}", string.Join (" ", invalid.Select (c => c.ToString ()).ToArray ()));
        return false;
      }

      if (Directory.Exists (arguments.OutputDirectory) && !arguments.OverwriteExistingFiles)
      {
        Log.SendError ("Output directory already exists. Use -f option if you are sure you want to overwrite all existing files.");
        return false;
      }

      return true;
    }
  }
}
