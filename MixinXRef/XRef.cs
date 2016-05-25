﻿// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
          if (ex is ReflectionTypeLoadException)
          {
            foreach (var loaderException in ((ReflectionTypeLoadException) ex).LoaderExceptions)
              Log.SendError (loaderException.ToString());
          }
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

      IRemotionReflector reflector;
      if (!CreateReflector(arguments, out reflector))
        return false;

      var assemblyResolver = AssemblyResolver.Create();
      AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver.HandleAssemblyResolve;

      reflector.InitializeLogging ();

      var typeDiscoveryService = reflector.GetTypeDiscoveryService();

      ICollection allTypes;
      try
      {
        allTypes = typeDiscoveryService.GetTypes (null, true);
      }
      catch (Exception ex)
      {
        Log.SendError (ex.ToString());
        return false;
      }

      var allAssemblies = allTypes.Cast<Type>().Select (t => t.Assembly)
          .Distinct()
          .Where (a => !reflector.IsRelevantAssemblyForConfiguration (a) || !reflector.IsNonApplicationAssembly (a))
          .ToArray();

      if (!allAssemblies.Any())
      {
        Log.SendError ("\"{0}\" contains no assemblies or only assemblies on the ignore list", arguments.AssemblyDirectory);
        return false;
      }

      var mixinConfiguration = reflector.BuildConfigurationFromAssemblies (allAssemblies);
      var outputFormatter = new OutputFormatter ();
      var configurationErrors = new ErrorAggregator<Exception> ();
      var validationErrors = new ErrorAggregator<Exception> ();

      var involvedTypes = new InvolvedTypeFinder (mixinConfiguration, allAssemblies, configurationErrors, validationErrors, reflector).FindInvolvedTypes ();
      var reportGenerator = new FullReportGenerator (involvedTypes, configurationErrors, validationErrors, reflector, outputFormatter);
      var outputDocument = reportGenerator.GenerateXmlDocument ();

      var xmlFile = Path.Combine (arguments.OutputDirectory,
                                 !string.IsNullOrEmpty (arguments.XMLOutputFileName)
                                   ? arguments.XMLOutputFileName
                                   : "MixinXRef.xml");

      outputDocument.Save (xmlFile);

      reportGenerator = null;
      GC.Collect ();

      allAssemblies = null;
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

    private static bool CreateReflector (XRefArguments arguments, out IRemotionReflector reflector)
    {
      try
      {
        switch (arguments.ReflectorSource)
        {
          case ReflectorSource.ReflectorAssembly:
            reflector = RemotionReflectorFactory.Create (arguments.AssemblyDirectory, arguments.ReflectorPath);
            return true;
          case ReflectorSource.CustomReflector:
            var customReflector = Type.GetType (arguments.CustomReflectorAssemblyQualifiedTypeName);
            if (customReflector == null)
            {
              Log.SendError ("Custom reflector can not be found");
              reflector = null;
              return false;
            }
            if (!typeof (IRemotionReflector).IsAssignableFrom (customReflector))
            {
              Log.SendError (
                  "Specified custom reflector {0} does not implement {1}",
                  customReflector,
                  typeof (IRemotionReflector).FullName);
              reflector = null;
              return false;
            }
            reflector = RemotionReflectorFactory.Create (arguments.AssemblyDirectory, customReflector);
            return true;
          case ReflectorSource.Unspecified:
            throw new IndexOutOfRangeException ("Reflector source is unspecified");
        }
      }
      catch (Exception ex)
      {
        Log.SendError ("Error while initializing the reflector: {0}", ex.Message);
        reflector = null;
        return false;
      }
      throw new InvalidOperationException ("Unreachable codepath reached.");
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
