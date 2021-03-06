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
using System.IO;
using System.Linq;
using MixinXRef;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Utility.Options;

namespace MixinXRefAnalyzer
{
  public class Program
  {
    private static int Main (string[] args)
    {
      var remainingArgs = args.SkipWhile (a=>a.StartsWith ("__tb:")); //TalkBackChannel.Initialize (args);

      var cmdLineArgs = XRefArguments.Instance;
      var showOptionsHelp = false;

      var options = new OptionSet
                      {
                        {
                          "i=|input-directory=", "Directory that contains the assemblies to analyze",
                          v => cmdLineArgs.AssemblyDirectory = v
                        }, 
                        {
                          "x=|xml-outputfile=", "File path to the output file for the generated XML",
                          v => cmdLineArgs.XMLOutputFileName = v
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
                          "c=|custom-reflector=", "An assembly qualified type name that is used as a custom reflector. " + 
                                                 "This type has to implement " + typeof (IRemotionReflector).Name + ".",
                          v =>
                            {
                              cmdLineArgs.ReflectorSource = ReflectorSource.CustomReflector;
                              cmdLineArgs.CustomReflectorAssemblyQualifiedTypeName = v;
                            }
                        }, 
                        {
                          "app-config-file=", "Application configuration file for analyzed assemblies. ",
                          v => cmdLineArgs.AppConfigFile = v
                        },
                        {
                          "app-base-directory=", "Application base directory. ",
                          v => cmdLineArgs.AppBaseDirectory = v
                        },
                        {
                          "h|?|help", "Show this help page",
                          v => showOptionsHelp = true
                        },
                      };

      try
      {
        options.Parse (remainingArgs);
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

      cmdLineArgs.SkipHTMLGeneration = true;
      cmdLineArgs.OverwriteExistingFiles = true;
      cmdLineArgs.OutputDirectory = Path.GetDirectoryName (cmdLineArgs.XMLOutputFileName);

      return new XRefInAppDomainRunner().Run (args, cmdLineArgs);
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

      if (string.IsNullOrEmpty (cmdLineArgs.XMLOutputFileName))
      {
        Console.Error.WriteLine ("Output file missing");
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