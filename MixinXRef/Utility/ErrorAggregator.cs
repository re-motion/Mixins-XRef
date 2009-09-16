using System;
using System.Collections.Generic;

namespace MixinXRef.Utility
{
  public class ErrorAggregator<TException>
  {
    private readonly List<TException> _exceptionList = new List<TException>();

    public void AddException (TException exception)
    {
      ArgumentUtility.CheckNotNull ("exception", exception);

      _exceptionList.Add (exception);
    }

    public IEnumerable<TException> Exceptions
    {
      get { return _exceptionList.AsReadOnly(); }
    }
  }
}