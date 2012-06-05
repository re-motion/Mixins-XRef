using System.Collections.Generic;
using MixinXRef.Utility;
using Rhino.Mocks;

namespace MixinXRef.UnitTests.Helpers
{
  internal static class StubFactory
  {
    public static IIdentifierGenerator<T> CreateIdentifierGeneratorStub<T> (IDictionary<string, T> values)
    {
      var identifierGeneratorStub = MockRepository.GenerateStub<IIdentifierGenerator<T>> ();

      foreach (var value in values)
        identifierGeneratorStub.Stub (ig => ig.GetIdentifier (value.Value)).Return (value.Key);

      identifierGeneratorStub.Stub (ig => ig.Elements).Return (values.Values);

      return identifierGeneratorStub;
    }

    public static IIdentifierGenerator<T> CreateIdentifierGeneratorStub<T> (IEnumerable<T> values)
    {
      var identifierGeneratorStub = MockRepository.GenerateStub<IIdentifierGenerator<T>> ();

      var i = 0;
      foreach (var value in values)
        identifierGeneratorStub.Stub (ig => ig.GetIdentifier (value)).Return ((i++).ToString ());

      identifierGeneratorStub.Stub (ig => ig.Elements).Return (values);

      return identifierGeneratorStub;
    }
  }
}
