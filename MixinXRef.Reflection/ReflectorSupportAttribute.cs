using System;

namespace MixinXRef.Reflection
{
  [AttributeUsage (AttributeTargets.Class, Inherited = false)]
  public class ReflectorSupportAttribute : Attribute
  {
    private Version _minVersion;
    public string MinVersion { get; set; }

    private Version _maxVersion;
    public string MaxVersion { get; set; }

    private string _component;
    public string Component
    {
      get { return _component; }
    }

    public ReflectorSupportAttribute (string component)
    {
      _component = component;
    }

    public Version GetMinVersion ()
    {
      if (MinVersion == null)
        return null;

      return _minVersion ?? (_minVersion = new Version (MinVersion));
    }

    public Version GetMaxVersion ()
    {
      if (MaxVersion == null)
        return null;

      return _maxVersion ?? (_maxVersion = new Version (MaxVersion));
    }
  }
}
