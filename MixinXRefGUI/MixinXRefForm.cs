using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MixinXRefGUI
{
  public partial class MixinXRefForm : Form
  {
    private readonly string _persistentSettingsFile;

    private delegate void AppendTextToLogTextBoxAsyncDelegate (string message);

    private delegate void setStartMixinRefButtonEnabledDelegate (bool enabled);

    public MixinXRefForm ()
    {
      InitializeComponent();

      var baseDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "MixinXRef");
      _persistentSettingsFile = Path.Combine (baseDir, "MixinXRefGUI.dat");

      GetOrCreateSettings (_persistentSettingsFile);
    }

    private void GetOrCreateSettings (string persistentSettingsFile)
    {
      if (!Directory.Exists (Path.GetDirectoryName (_persistentSettingsFile)))
        CreateDefaultSettings (persistentSettingsFile);

      LoadSettings (persistentSettingsFile);
    }

    private void CreateDefaultSettings (string persistentSettingsFile)
    {
      try
      {
        var baseDirectory = Path.GetDirectoryName (_persistentSettingsFile);
        Directory.CreateDirectory (baseDirectory);
        File.WriteAllLines (persistentSettingsFile, new[] { @"C:\", @"C:\", "", @"False" });
      }
      catch (Exception exception)
      {
        //File.WriteAllText (@".\MixinXRefGUI.log", exception.Message);
        MessageBox.Show (exception.Message, "Failed to save settings.", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void LoadSettings (string persistentSettingsFile)
    {
      if (File.Exists (persistentSettingsFile))
      {
        var settings = File.ReadAllLines (persistentSettingsFile);
        if (settings.Length == 4)
        {
          if (Directory.Exists (settings[0]))
            assemblyPathTextBox.Text = settings[0];
          if (Directory.Exists (settings[1]))
            outputPathTextBox.Text = settings[1];

          customReflectorTextBox.Text = settings[2];

          forceOverrideCheckBox.Checked = bool.Parse (settings[3]);
        }
      }
    }

    private void BrowseAssemblyPath_Click (object sender, EventArgs e)
    {
      ShowAndSetFolderBrowserDialogForTextBox ("Select input folder", assemblyPathTextBox);
    }

    private void BrowseOutputPath_Click (object sender, EventArgs e)
    {
      ShowAndSetFolderBrowserDialogForTextBox ("Select output folder", outputPathTextBox);
    }

    private void ShowAndSetFolderBrowserDialogForTextBox (string description, TextBox pathTextBox)
    {
      var folderBrowserDialog = new FolderBrowserDialog { Description = description };

      if (Directory.Exists (pathTextBox.Text))
        folderBrowserDialog.SelectedPath = Path.GetFullPath (pathTextBox.Text);

      var objResult = folderBrowserDialog.ShowDialog (this);
      if (objResult == DialogResult.OK)
        pathTextBox.Text = folderBrowserDialog.SelectedPath;
    }

    private void StartMixinXRefButton_Click (object sender, EventArgs e)
    {
      SaveSettings();

      var remotionAssembly = Path.Combine (assemblyPathTextBox.Text, "Remotion.dll");
      if (File.Exists (remotionAssembly))
        StartMixinXRefApplication();
      else
      {
        MessageBox.Show (
            "The input directory doesn't contain the remotion assembly. Please select another input directory.",
            "Invalid input directory",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
      }
    }

    private void StartMixinXRefApplication ()
    {
      var xRefPath = Path.GetDirectoryName (Assembly.GetExecutingAssembly().Location);
      var fileName = Path.Combine (xRefPath, "MixinXRef.exe");
      var arguments = GetArguments();

      Cursor = Cursors.AppStarting;
      startMixinXRefButton.Enabled = false;

      var process = new Process();
      process.StartInfo.FileName = fileName;
      process.StartInfo.Arguments = arguments;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.RedirectStandardError = true;
      process.StartInfo.RedirectStandardOutput = true;

      process.OutputDataReceived += WriteProcessStream;
      process.ErrorDataReceived += WriteProcessStream;
      process.EnableRaisingEvents = true;
      process.Exited += ProcessExited;
      process.Disposed += ProcessExited;

      logTextBox.Text = "Started function.  Please stand by.." + Environment.NewLine;

      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();

      cursorIconTimer.Enabled = true;
    }

    private string GetArguments ()
    {
      var arguments = String.Format ("\"{0}\" \"{1}\"", assemblyPathTextBox.Text, outputPathTextBox.Text);

      if (!String.IsNullOrEmpty (customReflectorTextBox.Text))
        arguments += " \"" + customReflectorTextBox.Text + "\"";

      if (forceOverrideCheckBox.Checked)
        arguments += " -force";

      return arguments;
    }

    private void SaveSettings ()
    {
      File.WriteAllLines (
          _persistentSettingsFile,
          new[]
          {
              assemblyPathTextBox.Text,
              outputPathTextBox.Text,
              customReflectorTextBox.Text,
              forceOverrideCheckBox.Checked.ToString()
          }
          );
    }

    private void AppendTextToLogTextBoxAsync (string message)
    {
      if (logTextBox.InvokeRequired)
        logTextBox.Invoke (new AppendTextToLogTextBoxAsyncDelegate (AppendTextToLogTextBoxAsync), new object[] { message });
      else
        logTextBox.AppendText (message + Environment.NewLine);
    }

    private void SetStartMixinRefButtonEnabled (bool enabled)
    {
      if (startMixinXRefButton.InvokeRequired)
        startMixinXRefButton.Invoke (new setStartMixinRefButtonEnabledDelegate (SetStartMixinRefButtonEnabled), new object[] { enabled });
      else
        startMixinXRefButton.Enabled = enabled;
    }

    private void WriteProcessStream (object sender, DataReceivedEventArgs dataReceivedEventArgs)
    {
      if (!String.IsNullOrEmpty (dataReceivedEventArgs.Data))
        AppendTextToLogTextBoxAsync (dataReceivedEventArgs.Data);
    }

    private void ProcessExited (object sender, EventArgs e)
    {
      SetStartMixinRefButtonEnabled (true);
    }

    private void CursorIconTimer_Tick (object sender, EventArgs e)
    {
      if (startMixinXRefButton.Enabled)
      {
        Cursor = Cursors.Default;
        cursorIconTimer.Enabled = false;
      }
    }
  }
}