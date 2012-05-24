using System;

namespace MixinXRef.Reflection
{
  [AttributeUsage (AttributeTargets.Class, Inherited = false)]
  public class ReflectorSupportAttribute : Attribute
  {
    public Version MinVersion { get; private set; }

    public string Component { get; private set; }

    public ReflectorSupportAttribute (string component, string minVersion)
    {
      Component = component;
      MinVersion = new Version (minVersion);
    }
  }
}
