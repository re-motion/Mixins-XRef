using System.IO;

namespace MixinXRef.Utility
{
  public static class PathUtility
  {
    // Alternative to Path.GetDirectoryName if volume information needs to be kept
    public static string GetDirectoryName (string path)
    {
      ArgumentUtility.CheckNotNull ("path", path);

      var idx = path.IndexOf (Path.DirectorySeparatorChar);
      return idx == -1 ? string.Empty : path.Substring (0, idx);
    }
  }
}
