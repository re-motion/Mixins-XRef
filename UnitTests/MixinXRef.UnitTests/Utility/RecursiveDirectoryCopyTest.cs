using System;
using System.IO;
using MixinXRef.Utility;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class RecursiveDirectoryCopyTest
  {
    private const string sourceDirectoryPath = "sourceDirectory";
    private const string destinationDirectoryPath = "destinationDirectory";

    [TearDown]
    public void TearDown ()
    {
      // clean up when test fails
      if (Directory.Exists (sourceDirectoryPath))
        Directory.Delete (sourceDirectoryPath, true);
      if (Directory.Exists (destinationDirectoryPath))
        Directory.Delete (destinationDirectoryPath, true);
    }

    [Test]
    public void CopyTo_SourceDirectoryDoesNotExist ()
    {
      var sourceDirectory = new DirectoryInfo ("nonExistingDirectory");
      try
      {
        sourceDirectory.CopyTo ("doesNotMatter");
        Assert.Fail ("expected exception was not thrown");
      }
      catch (DirectoryNotFoundException ex)
      {
        Assert.That (ex.Message, Is.EqualTo ("source directory '" + sourceDirectory.FullName + "' not found "));
      }
    }

    [Test]
    public void CopyTo_DestinationDirectoryDoesNotExist ()
    {
      var subDirectoryPath = "sub1" + Path.DirectorySeparatorChar + "sub2";
      var testFilePath = Path.Combine (subDirectoryPath, "testFile.txt");

      var sourceDirectory = new DirectoryInfo (sourceDirectoryPath);
      sourceDirectory.Create ();
      sourceDirectory.CreateSubdirectory (subDirectoryPath);
      File.Create (Path.Combine (sourceDirectoryPath, testFilePath)).Close ();

      Assert.That (Directory.Exists (destinationDirectoryPath), Is.False);
      sourceDirectory.CopyTo (destinationDirectoryPath);
      Assert.That (File.Exists (Path.Combine (destinationDirectoryPath, testFilePath)), Is.True);

      Directory.Delete (sourceDirectoryPath, true);
      Directory.Delete (destinationDirectoryPath, true);
    }

    [Test]
    public void CopyTo_DestinationDirectoryDoesExist ()
    {
      var subDirectoryPath = "sub1" + Path.DirectorySeparatorChar + "sub2";
      var testFilePath = Path.Combine (subDirectoryPath, "testFile.txt");
      var sourceFilePath = Path.Combine (sourceDirectoryPath, testFilePath);
      var destinationFilePath = Path.Combine (destinationDirectoryPath, testFilePath);

      var sourceDirectory = new DirectoryInfo (sourceDirectoryPath);
      sourceDirectory.Create ();
      sourceDirectory.CreateSubdirectory (subDirectoryPath);
      File.Create (sourceFilePath).Close ();

      // call copyTo a second time, wanted behavior: silently overwrite directories and files
      Assert.That (Directory.Exists (destinationDirectoryPath), Is.False);
      sourceDirectory.CopyTo (destinationDirectoryPath);
      var lastWriteTime1 = File.GetLastWriteTime (destinationFilePath);

      // last write time of the copied file is 'inherited' from source file, so we have to update it
      File.Create (sourceFilePath).Close ();

      Assert.That (Directory.Exists (destinationDirectoryPath), Is.True);
      sourceDirectory.CopyTo (destinationDirectoryPath);
      var lastWriteTime2 = File.GetLastWriteTime (destinationFilePath);

      // creation time does not change, when file is overwritten -> use last write time
      Assert.Less (lastWriteTime1, lastWriteTime2);

      Directory.Delete (sourceDirectoryPath, true);
      Directory.Delete (destinationDirectoryPath, true);
    }
  }
}