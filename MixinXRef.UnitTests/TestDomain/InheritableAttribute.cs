using System;

namespace MixinXRef.UnitTests.TestDomain
{
  [AttributeUsage (AttributeTargets.Class, Inherited = true)]
  public class InheritableAttribute : Attribute { }
}