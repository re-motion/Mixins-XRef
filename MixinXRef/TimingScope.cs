using System;
using System.Diagnostics;
using Remotion.Utilities;

namespace MixinXRef
{
  public class TimingScope : IDisposable
  {
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly string _description;

    public TimingScope (string description)
    {
      ArgumentUtility.CheckNotNull ("description", description);
      _description = description;
    }

    public void Dispose ()
    {
      _stopwatch.Stop();
      Console.WriteLine ("{0}: {1}", _description, _stopwatch.Elapsed);
    }
  }
}