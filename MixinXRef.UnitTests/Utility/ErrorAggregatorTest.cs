using System;
using MixinXRef.Utility;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.Linq;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class ErrorAggregatorTest
  {
    [Test]
    public void AddException_RetrieveWithExceptionsProperty()
    {
      var errorAggregator = new ErrorAggregator<Exception>();

      var exception1 = new Exception("test exception");
      var exception2 = new Exception ("another text exception");

      errorAggregator.AddException(exception1);
      errorAggregator.AddException(exception2);

      Assert.That(new[] { exception1, exception2 }, Is.EqualTo(errorAggregator.Exceptions.ToList()));
    }
  }
}