using System;
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
  public sealed class CommandLineArguments : ISerializable
  {
    private static CommandLineArguments s_instance;
    public static CommandLineArguments Instance { get { return s_instance ?? (s_instance = new CommandLineArguments ()); } }

    public string AssemblyDirectory { get; set; }
    public string OutputDirectory { get; set; }
    public string XMLOutputFileName { get; set; }
    public bool OverwriteExistingFiles { get; set; }
    public bool SkipHTMLGeneration { get; set; }
    public ReflectorSource ReflectorSource { get; set; }
    public string ReflectorPath { get; set; }
    public string CustomReflectorAssemblyQualifiedTypeName { get; set; }

    public CommandLineArguments()
    { }

    protected CommandLineArguments (SerializationInfo info, StreamingContext context)
    {
      AssemblyDirectory = info.GetString ("AssemblyDirectory");
      OutputDirectory = info.GetString ("OutputDirectory");
      XMLOutputFileName = info.GetString ("XMLOutputFileName");
      OverwriteExistingFiles = info.GetBoolean ("OverwriteExistingFiles");
      SkipHTMLGeneration = info.GetBoolean ("SkipHTMLGeneration");
      ReflectorSource = (ReflectorSource) info.GetInt32 ("ReflectorSource");
      ReflectorPath = info.GetString ("ReflectorPath");
      CustomReflectorAssemblyQualifiedTypeName = info.GetString ("CustomReflectorAssemblyQualifiedTypeName");
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      info.AddValue ("AssemblyDirectory", AssemblyDirectory);
      info.AddValue ("OutputDirectory", OutputDirectory);
      info.AddValue ("XMLOutputFileName", XMLOutputFileName);
      info.AddValue ("OverwriteExistingFiles", OverwriteExistingFiles);
      info.AddValue ("SkipHTMLGeneration", SkipHTMLGeneration);
      info.AddValue ("ReflectorSource", (int) ReflectorSource);
      info.AddValue ("ReflectorPath", ReflectorPath);
      info.AddValue ("CustomReflectorAssemblyQualifiedTypeName", CustomReflectorAssemblyQualifiedTypeName);
    }
  }
}
