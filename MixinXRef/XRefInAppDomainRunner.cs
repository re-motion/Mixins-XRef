using System;

namespace MixinXRef
{
  public class XRefInAppDomainRunner
  {
    public int Run (XRefArguments options, CrossAppDomainCommunicator.MessageReceivedDelegate onMessageReceived = null)
    {
      // Create new application domain and run cross referencer
      var appDomain = AppDomain.CurrentDomain;
      var setupInformation = appDomain.SetupInformation;
      if(options.AppBaseDirectory != null)
      {
        setupInformation.ApplicationBase = options.AppBaseDirectory;

        if(options.AssemblyDirectory.StartsWith(options.AppBaseDirectory))
        {
          // does cutting work?
          var relativeSearchPath = options.AssemblyDirectory.Remove (0, options.AppBaseDirectory.Length);
          // is privatebinpath empty?
          setupInformation.PrivateBinPath = "." + relativeSearchPath;
        }
      }
      if(options.AppConfigFile != null)
        setupInformation.ConfigurationFile = options.AppConfigFile;

      var newAppDomain = AppDomain.CreateDomain ("XRefAppDomain", null, setupInformation);

      var crossAppDomainCommunicatorType = typeof (CrossAppDomainCommunicator);
      var proxy = (CrossAppDomainCommunicator) newAppDomain.CreateInstanceFromAndUnwrap (crossAppDomainCommunicatorType.Assembly.Location, crossAppDomainCommunicatorType.FullName);
      if(onMessageReceived != null)
        proxy.SetMessageReceivedDelegate (onMessageReceived);
      return proxy.Run (null, options);
    }
  }
}
