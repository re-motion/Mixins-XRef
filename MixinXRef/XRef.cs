﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MixinXRef.Formatting;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Report;
using MixinXRef.Utility;
using TalkBack;
using TalkBack.Brokers;
using TalkBack.Brokers.Delegate;

namespace MixinXRef
{
  public static class XRef
  {
    private static readonly object s_locker = new object ();
    private static readonly MessageSender s_defaultChannel = new DelegateMessageBroker (LogToConsole);

    private static IMessageSender s_log;
    internal static IMessageSender Log
    {
      get { return s_log ?? s_defaultChannel; }
    }

    private static void LogToConsole (Message message)
    {
      if (message.Severity == MessageSeverity.Error)
        Console.Error.WriteLine (message.Text);
      else
        Console.WriteLine (message.Text);
    }

    public static bool Run (XRefArguments arguments, IMessageSender logger = null)
    {
      lock (s_locker)
      {
        var success = false;
        s_log = logger;
        try
        {
          success = InternalRun (arguments);
        }
        catch (Exception ex)
        {
          Log.SendError (ex.ToString ());
        }
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

      try
      {
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
              Log.SendError ("Specified custom reflector {0} does not implement {1}", customReflector,
                            typeof (IRemotionReflector).FullName);
              return false;
            }
            reflector = RemotionReflectorFactory.Create (arguments.AssemblyDirectory, customReflector);
            break;
          case ReflectorSource.Unspecified:
            throw new IndexOutOfRangeException ("Reflector source is unspecified");
        }
      }
      catch (Exception ex)
      {
        Log.SendError("Error while initializing the reflector: {0}", ex.Message);
        return false;
      }

      var assemblies = new AssemblyBuilder (arguments.AssemblyDirectory, arguments.IgnoredAssemblies)
        .GetAssemblies (a => !reflector.IsRelevantAssemblyForConfiguration (a) || !reflector.IsNonApplicationAssembly (a));

      if (!assemblies.Any ())
      {
        Log.SendError ("\"{0}\" contains no assemblies or only assemblies on the ignore list", arguments.AssemblyDirectory);
        return false;
      }

      var xmlFile = Path.Combine (arguments.OutputDirectory,
                                 !string.IsNullOrEmpty (arguments.XMLOutputFileName)
                                   ? arguments.XMLOutputFileName
                                   : "MixinXRef.xml");

      var refAssemblyNames = assemblies.SelectMany (assembly => assembly.GetReferencedAssemblies()).DistinctBy(assemblyName => assemblyName.FullName);
      var alreadyLoadedAssemblyName = assemblies.Select (assembly => assembly.GetName()).ToArray();
      
      // Load referenced assemblies
      var assemblyNamesToLoad = refAssemblyNames
          .Except (alreadyLoadedAssemblyName)
          .Where (assemblyName => !arguments.IgnoredAssemblies.Contains (assemblyName.Name));
      var addtionalReferencedAssemblies = LoadAdditionalReferencedAssemblies(assemblyNamesToLoad);

      var allAssemblies = assemblies.Concat (addtionalReferencedAssemblies).DistinctBy (assemblyName => assemblyName.FullName).ToArray ();

      var mixinConfiguration = reflector.BuildConfigurationFromAssemblies (allAssemblies);
      var outputFormatter = new OutputFormatter ();
      var configurationErrors = new ErrorAggregator<Exception> ();
      var validationErrors = new ErrorAggregator<Exception> ();

      var involvedTypes = new InvolvedTypeFinder (mixinConfiguration, allAssemblies, configurationErrors, validationErrors, reflector).FindInvolvedTypes ();
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

    private static IEnumerable<Assembly> LoadAdditionalReferencedAssemblies (IEnumerable<AssemblyName> assemblyNamesToLoad)
    {
      foreach (var assemblyName in assemblyNamesToLoad)
      {
        Assembly assembly = null;

        try
        {
          assembly = Assembly.Load (assemblyName);
        }
        catch (FileNotFoundException ex)
        {
          Log.SendWarning ("Could not load assembly '{0}' due to '{1}'.", ex.FileName, ex.Message);
        }
        catch (FileLoadException ex)
        {
          Log.SendError ("Could not load assembly '{0}' due to '{1}'.", assemblyName, ex.Message);
        }

        if(assembly != null)
          yield return assembly;
      }
    }

    private static bool CheckArguments (XRefArguments arguments)
    {
      if (string.IsNullOrEmpty (arguments.AssemblyDirectory))
      {
        Log.SendError ("Input directory missing");
        return false;
      }

      if (!File.Exists (Path.Combine (arguments.AssemblyDirectory, "Remotion.dll")))
      {
        Log.SendError("The input directory '" + arguments.AssemblyDirectory + "' doesn't contain the remotion assembly.");
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
        Log.SendError ("Input directory '" + arguments.AssemblyDirectory + "' does not exist");
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
