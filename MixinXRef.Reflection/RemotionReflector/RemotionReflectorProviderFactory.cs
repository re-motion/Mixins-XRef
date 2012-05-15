using System;
using System.Collections.Generic;

namespace MixinXRef.Reflection.RemotionReflector
{
  public static class RemotionReflectorProviderFactory
  {
    private static readonly IDictionary<string, IDictionary<Version, IRemotionReflector>> s_reflectorProviders = new Dictionary<string, IDictionary<Version, IRemotionReflector>> ();

    public static IRemotionReflector GetReflector (string component, Version version, string assemblyDirectory)
    {
      IDictionary<Version, IRemotionReflector> componentProviders;
      if (!s_reflectorProviders.TryGetValue (component, out componentProviders))
        s_reflectorProviders.Add (component, componentProviders = new Dictionary<Version, IRemotionReflector> ());

      IRemotionReflector provider;
      if (!componentProviders.TryGetValue (version, out provider))
        componentProviders.Add (version, provider = CreateReflectorProvider (component, version, assemblyDirectory));

      return provider;
    }

    private static IRemotionReflector CreateReflectorProvider (string component, Version version, string assemblyDirectory)
    {
      return new RemotionReflectorProvider (component, version, assemblyDirectory);
    }
  }
}
