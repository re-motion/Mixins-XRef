using System;
using System.IO;

namespace MixinXRef
{
  public static class RecursiveDirectoryCopy
  {
    public static void CopyTo (this DirectoryInfo sourceDirectory, string destinationDirectoryPath)
    {
      ArgumentUtility.CheckNotNull ("sourceDirectory", sourceDirectory);
      ArgumentUtility.CheckNotNull ("destinationDirectoryPath", destinationDirectoryPath);

      if (!sourceDirectory.Exists)
        throw new DirectoryNotFoundException ("source directory '" + sourceDirectory.FullName + "' not found ");

      DirectoryInfo destinationDirectory = new DirectoryInfo (destinationDirectoryPath);

      // does nothing if the directory already exists
      destinationDirectory.Create();

      // copy all files
      foreach (FileInfo file in sourceDirectory.GetFiles())
        file.CopyTo (Path.Combine (destinationDirectory.FullName, file.Name), true);

      // recursive call for subdirectories
      foreach (DirectoryInfo subDirectory in sourceDirectory.GetDirectories())
        CopyTo (subDirectory, Path.Combine (destinationDirectory.FullName, subDirectory.Name));
    }
  }
}