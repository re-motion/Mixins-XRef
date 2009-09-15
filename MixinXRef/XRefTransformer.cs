using System;
using System.Diagnostics;
using System.IO;
using MixinXRef.Utility;

namespace MixinXRef
{
  public class XRefTransformer
  {
    // stylesheet path
    private const string _xsltStyleSheetPath = @"xml_utilities\main.xslt";
    // xslt processor path
    private const string xsltProcessorPath = @"xml_utilities\saxon\Transform.exe";

    private readonly string _xmlInputFile;
    private readonly string _outputDirectory;

    public XRefTransformer (string xmlInputFile, string outputDirectory)
    {
      ArgumentUtility.CheckNotNull ("xmlInputFile", xmlInputFile);
      ArgumentUtility.CheckNotNull ("outputDirectory", outputDirectory);

      _xmlInputFile = xmlInputFile;
      _outputDirectory = outputDirectory;
    }

    public int GenerateHtmlFromXml ()
    {
      // dummy output file - will not be created, just to trick saxon
      var mainOutputFile = Path.Combine (_outputDirectory, "dummy.html");
      string arguments = String.Format ("-s:{0} -xsl:{1} -o:{2}", _xmlInputFile, _xsltStyleSheetPath, mainOutputFile);

      Process xsltProcessor = new Process();
      xsltProcessor.StartInfo.FileName = xsltProcessorPath;
      xsltProcessor.StartInfo.Arguments = arguments;
      xsltProcessor.StartInfo.RedirectStandardError = true;
      xsltProcessor.StartInfo.RedirectStandardOutput = true;
      xsltProcessor.StartInfo.UseShellExecute = false;

      xsltProcessor.Start();
      Console.Error.Write (xsltProcessor.StandardError.ReadToEnd());
      Console.Out.Write (xsltProcessor.StandardOutput.ReadToEnd());
      xsltProcessor.WaitForExit();

      return xsltProcessor.ExitCode;
    }
  }
}