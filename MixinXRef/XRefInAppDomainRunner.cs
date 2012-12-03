using System;
using TalkBack;

namespace MixinXRef
{
  public class XRefInAppDomainRunner
  {
    public int Run (string[] talkBackArgs, XRefArguments xRefArgs, CrossAppDomainCommunicator.MessageReceivedDelegate onMessageReceived = null)
    {
      // Create new application domain and run cross referencer
      var appDomain = AppDomain.CurrentDomain;
      
      var setupInformation = appDomain.SetupInformation;
      setupInformation.ApplicationBase = xRefArgs.AssemblyDirectory;

      if(!string.IsNullOrEmpty(xRefArgs.AppBaseDirectory))
      {
        setupInformation.ApplicationBase = xRefArgs.AppBaseDirectory;

        var appBaseDirectory = xRefArgs.AppBaseDirectory;
        if (!appBaseDirectory.EndsWith ("\\"))
          appBaseDirectory += "\\";

        if(xRefArgs.AssemblyDirectory.StartsWith(appBaseDirectory))
        {
          var relativeSearchPath = xRefArgs.AssemblyDirectory.Remove (0, appBaseDirectory.Length);
          if (!string.IsNullOrEmpty (relativeSearchPath))
          {
            setupInformation.PrivateBinPath = relativeSearchPath;
          }
        }
        else
        {
          throw new ArgumentException ("Input directory is not a sub directory of application base directory!");
        }
      }
      if(xRefArgs.AppConfigFile != null)
        setupInformation.ConfigurationFile = xRefArgs.AppConfigFile;

      var newAppDomain = AppDomain.CreateDomain ("XRefAppDomain", appDomain.Evidence, setupInformation);

      var crossAppDomainCommunicatorType = typeof (CrossAppDomainCommunicator);
      var proxy = (CrossAppDomainCommunicator) newAppDomain.CreateInstanceFromAndUnwrap (crossAppDomainCommunicatorType.Assembly.Location, crossAppDomainCommunicatorType.FullName);
      if(onMessageReceived != null)
        proxy.SetMessageReceivedDelegate(new MessageReceivedDelegateWrapper(onMessageReceived).OnMessageReceived);
      return proxy.Run (talkBackArgs, xRefArgs);
    }

    private class MessageReceivedDelegateWrapper : MarshalByRefObject
    {
      private readonly CrossAppDomainCommunicator.MessageReceivedDelegate _onMessageReceived;

      public MessageReceivedDelegateWrapper (CrossAppDomainCommunicator.MessageReceivedDelegate onMessageReceived)
      {
        _onMessageReceived = onMessageReceived;
      }

      public void OnMessageReceived (MessageSeverity severity, string message)
      {
        _onMessageReceived (severity, message);
      }
    }
  }
}
