using System;
using System.Diagnostics;
using System.IO;
using Remotion.Utilities;

namespace MixinXRef
{
  public class XRefTransformer
  {
    // stylesheet path
    private const string _xsltStyleSheetPath = @"xml_utilities\main.xslt";
    // xslt processor path
    private const string xsltProcessorPath = @"xml_utilities\saxon\Transform.exe";
    // schema for xml
    //private const string _schema = @"xml_utilities\XrefMixin.xsd";
    // css for html
    //private const string _cssHtml = @"xml_utilities\style.css";

    // main output file
    private const string mainOutputFile = "index.html";

    private readonly string _xmlInputFile;

    public XRefTransformer (string xmlInputFile)
    {
      ArgumentUtility.CheckNotNull ("xmlInputFile", xmlInputFile);

      _xmlInputFile = xmlInputFile;
    }

    public int GenerateHtmlFromXml()
    {
      string arguments = String.Format("-s:{0} -xsl:{1} -o:{2}", _xmlInputFile, _xsltStyleSheetPath, mainOutputFile);

      Process xsltProcessor = new Process();
      xsltProcessor.StartInfo.FileName = xsltProcessorPath;
      xsltProcessor.StartInfo.Arguments = arguments;
      xsltProcessor.StartInfo.RedirectStandardError = true;
      xsltProcessor.StartInfo.RedirectStandardOutput = true;
      xsltProcessor.StartInfo.UseShellExecute = false;

      xsltProcessor.Start();
      Console.Error.Write(xsltProcessor.StandardError.ReadToEnd());
      Console.Out.Write(xsltProcessor.StandardOutput.ReadToEnd());
      xsltProcessor.WaitForExit();

      return xsltProcessor.ExitCode;
    }
  }
}