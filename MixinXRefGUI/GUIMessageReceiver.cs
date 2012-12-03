using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalkBack;

namespace MixinXRefGUI
{
  public class GUIMessageReceiver : MarshalByRefObject
  {
    private MixinXRefForm _form;

    public GUIMessageReceiver (MixinXRefForm form)
    {
      _form = form;
    }

    public void MessageReceived (MessageSeverity severity, string message)
    {
      _form.AppendTextToLogTextBoxAsync (message);
    }
  }
}
