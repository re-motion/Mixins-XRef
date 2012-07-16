using System;

namespace MixinXRef.Reflection
{
  [AttributeUsage (AttributeTargets.Class, Inherited = false)]
  public class ReflectorSupportAttribute : Attribute
  {
    public Version MinVersion { get; private set; }

    public string Component { get; private set; }

    public string[] RequiredAssemblies { get; private set; }

    public ReflectorSupportAttribute (string component, string minVersion, params string[] requiredAssemblies)
    {
      Component = component;
      RequiredAssemblies = requiredAssemblies;
      MinVersion = new Version (minVersion);
    }
  }
}
