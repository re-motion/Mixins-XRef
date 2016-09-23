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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MixinXRef
{
  public enum ReflectorSource
  {
    Unspecified,
    ReflectorAssembly,
    CustomReflector
  }

  [Serializable]
  public sealed class XRefArguments : ISerializable
  {
    private static XRefArguments s_instance;
    public static XRefArguments Instance { get { return s_instance ?? (s_instance = new XRefArguments ()); } set { s_instance = value; } }

    public string AssemblyDirectory { get; set; }
    public string OutputDirectory { get; set; }
    public string XMLOutputFileName { get; set; }
    public bool OverwriteExistingFiles { get; set; }
    public bool SkipHTMLGeneration { get; set; }
    public bool GenerateOnlyErrorReport { get; set; }
    public ReflectorSource ReflectorSource { get; set; }
    public string ReflectorPath { get; set; }
    public string CustomReflectorAssemblyQualifiedTypeName { get; set; }
    public string IgnoredAssemblies { get; set; }

    public string AppConfigFile { get; set; }
    public string AppBaseDirectory { get; set; }

    public XRefArguments ()
    {
    }

    private XRefArguments (SerializationInfo info, StreamingContext context)
    {
      AssemblyDirectory = info.GetString ("AssemblyDirectory");
      OutputDirectory = info.GetString ("OutputDirectory");
      XMLOutputFileName = info.GetString ("XMLOutputFileName");
      OverwriteExistingFiles = info.GetBoolean ("OverwriteExistingFiles");
      SkipHTMLGeneration = info.GetBoolean ("SkipHTMLGeneration");
      GenerateOnlyErrorReport = info.GetBoolean ("GenerateOnlyErrorReport");
      ReflectorSource = (ReflectorSource) info.GetInt32 ("ReflectorSource");
      ReflectorPath = info.GetString ("ReflectorPath");
      CustomReflectorAssemblyQualifiedTypeName = info.GetString ("CustomReflectorAssemblyQualifiedTypeName");
      AppConfigFile = info.GetString ("AppConfigFile");
      AppBaseDirectory = info.GetString ("AppBaseDirectory");
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("AssemblyDirectory", AssemblyDirectory);
      info.AddValue ("OutputDirectory", OutputDirectory);
      info.AddValue ("XMLOutputFileName", XMLOutputFileName);
      info.AddValue ("OverwriteExistingFiles", OverwriteExistingFiles);
      info.AddValue ("SkipHTMLGeneration", SkipHTMLGeneration);
      info.AddValue ("GenerateOnlyErrorReport", GenerateOnlyErrorReport);
      info.AddValue ("ReflectorSource", (int) ReflectorSource);
      info.AddValue ("ReflectorPath", ReflectorPath);
      info.AddValue ("CustomReflectorAssemblyQualifiedTypeName", CustomReflectorAssemblyQualifiedTypeName);
      info.AddValue ("AppConfigFile", AppConfigFile);
      info.AddValue ("AppBaseDirectory", AppBaseDirectory);
    }
  }
}
