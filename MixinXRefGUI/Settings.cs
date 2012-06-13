using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using MixinXRef;

namespace MixinXRefGUI
{
  internal class Settings
  {
    private readonly Action<XRefArguments> _apply;
    private readonly Func<XRefArguments> _get;
    private readonly XRefArguments _defaultSettings;
    private readonly string _persistentSettingsFile;

    public Settings (string persistentSettingsFile, Action<XRefArguments> apply, Func<XRefArguments> get, XRefArguments defaultSettings = null)
    {
      _persistentSettingsFile = persistentSettingsFile;
      _apply = apply;
      _get = get;
      _defaultSettings = defaultSettings;

      Arguments = Load (_persistentSettingsFile) ?? defaultSettings;
    }

    private XRefArguments Load (string persistentSettingsFile)
    {
      XRefArguments arguments = null;

      if (File.Exists (persistentSettingsFile))
        arguments = DeserializeFromFile (persistentSettingsFile);

      return arguments ?? _defaultSettings;
    }

    public void Save ()
    {
      var baseDirectory = Path.GetDirectoryName (_persistentSettingsFile);
      if (!Directory.Exists (baseDirectory))
        Directory.CreateDirectory (baseDirectory);

      try
      {
        SerializeToFile (_persistentSettingsFile, Arguments);
      }
      catch (Exception exception)
      {
        MessageBox.Show (exception.Message, "Failed to save settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private static void SerializeToFile (string file, XRefArguments args)
    {
      using (var stream = File.OpenWrite (file))
      {
        var formatter = new BinaryFormatter ();
        formatter.Serialize (stream, args);
      }
    }

    private static XRefArguments DeserializeFromFile (string file)
    {
      using (var stream = File.OpenRead (file))
      {
        var formatter = new BinaryFormatter ();
        try
        {
          return (XRefArguments) formatter.Deserialize (stream);
        }
        catch (Exception exception)
        {
          MessageBox.Show (string.Format ("{1}{0}{0}This is most likely caused by an old settings file that is no longer supported. ", Environment.NewLine, exception.Message), "Failed to load settings", MessageBoxButtons.OK, MessageBoxIcon.Information);          
        }
      }

      // If we are here something went wrong during the deserialization.
      // The file seems to be invalid, delete it. 
      File.Delete (file);
      return null;
    }

    public XRefArguments Arguments
    {
      get
      {
        return _get != null ? _get () : null;
      }
      set
      {
        if (_apply != null)
          _apply (value);
      }
    }
  }
}
