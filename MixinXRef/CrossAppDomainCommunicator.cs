using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
      return TryLoadAssemblyFromOriginalDirectory (assemblyName, ".dll") ?? TryLoadAssemblyFromOriginalDirectory (assemblyName, ".exe");
    }

    private static Assembly TryLoadAssemblyFromOriginalDirectory (AssemblyName assemblyName, string extension)
    {
      var assemblyFileName = assemblyName.Name + extension;
      var directoryName = Path.GetDirectoryName (typeof (CrossAppDomainCommunicator).Assembly.Location);
      Trace.Assert (directoryName != null);
      var assemblyPath = Path.Combine (directoryName, assemblyFileName);

      return File.Exists (assemblyPath) ? Assembly.LoadFrom (assemblyPath) : null;
    }
  }
}