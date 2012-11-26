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
    public CrossAppDomainCommunicator()
    {
      AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
    }

    public int Run (string[] talkbackArgs, XRefArguments cmdLineArgs)
    {
      TalkBackChannel.Initialize (talkbackArgs);

      XRefArguments.Instance = cmdLineArgs;
      
      return TalkBackInvoke.Action (sender => XRef.Run (cmdLineArgs, sender), MessageReceived) ? 0 : 1;
    }

    private static void MessageReceived (Message message)
    {
      if (TalkBackChannel.Out != null)
        TalkBackChannel.Out.SendMessage (message);

      switch (message.Severity)
      {
        case MessageSeverity.Error:
          Console.Error.WriteLine ("ERROR: {0}", message.Text);
          break;
        case MessageSeverity.Warning:
          Console.WriteLine ("WARNING: {0}", message.Text);
          break;
        default:
          Console.WriteLine (message.Text);
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
        return null;
      }
    }
  }
}