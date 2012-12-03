using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TalkBack;

namespace MixinXRef
{
  public class CrossAppDomainCommunicator : MarshalByRefObject
  {
    public delegate void MessageReceivedDelegate (MessageSeverity severity, string message);

    private MessageReceivedDelegate _messageReceived;

    public CrossAppDomainCommunicator()
    {
      AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
      _messageReceived = DefaultMessageReceived;
    }

    public void SetMessageReceivedDelegate (MessageReceivedDelegate messageReceived)
    {
      _messageReceived = messageReceived;
    }

    public int Run (string[] talkbackArgs, XRefArguments cmdLineArgs)
    {
      if(talkbackArgs != null)
        TalkBackChannel.Initialize (talkbackArgs);

      XRefArguments.Instance = cmdLineArgs;
      
      return TalkBackInvoke.Action (sender => XRef.Run (cmdLineArgs, sender), MessageReceived) ? 0 : 1;
    }

    private void MessageReceived (Message message)
    {
      if (TalkBackChannel.Out != null)
        TalkBackChannel.Out.SendMessage (message);

      _messageReceived (message.Severity, message.Text);
    }

    private static void DefaultMessageReceived (MessageSeverity severity, string message)
    {
      switch (severity)
      {
        case MessageSeverity.Error:
          Console.Error.WriteLine ("ERROR: {0}", message);
          break;
        case MessageSeverity.Warning:
          Console.WriteLine ("WARNING: {0}", message);
          break;
        default:
          Console.WriteLine (message);
          break;
      }
    }

    private Assembly AssemblyResolve (object sender, ResolveEventArgs args)
    {
      var assemblyName = new AssemblyName (args.Name);
      var assemblyFileName = assemblyName.Name + ".dll";
      var assemblyPath = Path.Combine (Path.GetDirectoryName(typeof(CrossAppDomainCommunicator).Assembly.Location), assemblyFileName);

      try
      {
        return Assembly.LoadFrom (assemblyPath);
      }
      catch (Exception)
      {
        try
        {
          return Assembly.LoadFrom (Path.ChangeExtension(assemblyPath, ".exe"));
        }
        catch (Exception)
        {
          return null;
        }
      }
    }
  }
}