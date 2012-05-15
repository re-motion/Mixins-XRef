using System.IO;
using System.Reflection;

namespace MixinXRef.Utility
{
  public static class AssemblyHelper
  {
    public static Assembly LoadFileOrNull (string assemblyDirectory, string assemblyFileName)
    {
      var fullPath = Path.GetFullPath (Path.Combine (assemblyDirectory, assemblyFileName));
      if (!File.Exists (fullPath))
        return null;

      return Assembly.LoadFile (fullPath);
    }
  }
}