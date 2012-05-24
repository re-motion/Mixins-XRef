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
      InitializeComponent ();

      var baseDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "MixinXRef");
      _persistentSettingsFile = Path.Combine (baseDir, "MixinXRefGUI.dat");

      GetOrCreateSettings (_persistentSettingsFile);

      UpdateEnabledStatusOfShowResultButton ();
    }

    private void UpdateEnabledStatusOfShowResultButton ()
    {
      showResultsButton.Enabled = File.Exists (GetResultFilePath ());
    }

    private void GetOrCreateSettings (string persistentSettingsFile)
    {
      if (!File.Exists (_persistentSettingsFile))
        CreateDefaultSettings (persistentSettingsFile);

      LoadSettings (persistentSettingsFile);
    }

    private void CreateDefaultSettings (string persistentSettingsFile)
    {
      try
      {
        var baseDirectory = Path.GetDirectoryName (_persistentSettingsFile);
        if (!Directory.Exists (baseDirectory))
          Directory.CreateDirectory (baseDirectory);
        File.WriteAllLines (persistentSettingsFile, new[] { @"C:\", @"C:\", "MixinXRef.Reflectors*.dll", "", @"False" });
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
        if (settings.Length == 5)
        {
          if (Directory.Exists (settings[0]))
            assemblyPathTextBox.Text = settings[0];
          if (Directory.Exists (settings[1]))
            outputPathTextBox.Text = settings[1];

          reflectorAssemblyTextBox.Text = settings[2];
          customReflectorTextBox.Text = settings[3];

          forceOverrideCheckBox.Checked = bool.Parse (settings[4]);
        }
      }
    }

    private void SaveSettings ()
    {
      File.WriteAllLines (
          _persistentSettingsFile,
          new[]
          {
              assemblyPathTextBox.Text,
              outputPathTextBox.Text,
              reflectorAssemblyTextBox.Text,
              customReflectorTextBox.Text,
              forceOverrideCheckBox.Checked.ToString()
          }
          );
    }

    private void BrowseAssemblyPath_Click (object sender, EventArgs e)
    {
      ShowAndSetFolderBrowserDialogForTextBox ("Select input folder", assemblyPathTextBox);
    }

    private void BrowseOutputPath_Click (object sender, EventArgs e)
    {
      ShowAndSetFolderBrowserDialogForTextBox ("Select output folder", outputPathTextBox);
      UpdateEnabledStatusOfShowResultButton ();
    }

    private void ShowAndSetFolderBrowserDialogForTextBox (string description, TextBox pathTextBox)
    {
      using (var folderBrowserDialog = new FolderBrowserDialog { Description = description })
      {
        if (Directory.Exists (pathTextBox.Text))
        {
          folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
          folderBrowserDialog.SelectedPath = Path.GetFullPath (pathTextBox.Text);
        }

        var objResult = folderBrowserDialog.ShowDialog (this);
        if (objResult == DialogResult.OK)
          pathTextBox.Text = folderBrowserDialog.SelectedPath;
      }
    }

    private void StartMixinXRefButton_Click (object sender, EventArgs e)
    {
      SaveSettings ();

      var remotionAssembly = Path.Combine (assemblyPathTextBox.Text, "Remotion.dll");
      if (File.Exists (remotionAssembly))
        StartMixinXRefApplication ();
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
      var xRefPath = Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
      var fileName = Path.Combine (xRefPath, "MixinXRef.exe");
      var arguments = GetArguments ();

      Cursor = Cursors.AppStarting;
      startMixinXRefButton.Enabled = false;
      showResultsButton.Enabled = false;

      var process = new Process ();
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

      process.Start ();
      process.BeginOutputReadLine ();
      process.BeginErrorReadLine ();

      cursorIconTimer.Enabled = true;
    }

    private string GetArguments ()
    {
      var arguments = String.Format ("\"{0}\" \"{1}\"", assemblyPathTextBox.Text, outputPathTextBox.Text);

      if (reflectorAssemblyRadioButton.Checked && !string.IsNullOrEmpty (reflectorAssemblyTextBox.Text))
        arguments += " \"" + reflectorAssemblyTextBox.Text + "\"";
      else if (customReflectorRadioButton.Checked && !string.IsNullOrEmpty (customReflectorTextBox.Text))
        arguments += " \"" + customReflectorTextBox.Text + "\"";

      if (forceOverrideCheckBox.Checked)
        arguments += " -force";

      return arguments;
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

    private void SetShowResultsButtonEnabled (bool enabled)
    {
      if (showResultsButton.InvokeRequired)
        showResultsButton.Invoke (new setStartMixinRefButtonEnabledDelegate (SetShowResultsButtonEnabled), new object[] { enabled });
      else
        showResultsButton.Enabled = enabled;
    }

    private void WriteProcessStream (object sender, DataReceivedEventArgs dataReceivedEventArgs)
    {
      if (!String.IsNullOrEmpty (dataReceivedEventArgs.Data))
        AppendTextToLogTextBoxAsync (dataReceivedEventArgs.Data);
    }

    private void ProcessExited (object sender, EventArgs e)
    {
      SetStartMixinRefButtonEnabled (true);

      if (File.Exists (GetResultFilePath ()))
        SetShowResultsButtonEnabled (true);
    }

    private void CursorIconTimer_Tick (object sender, EventArgs e)
    {
      if (startMixinXRefButton.Enabled)
      {
        Cursor = Cursors.Default;
        cursorIconTimer.Enabled = false;
      }
    }

    private void ShowResultsButton_Click (object sender, EventArgs e)
    {
      var uriString = GetResultFilePath ();
      if (!File.Exists (uriString))
        return;

      var uri = new Uri (uriString);
      var converted = uri.AbsoluteUri;
      Process.Start (converted);
    }

    private string GetResultFilePath ()
    {
      return Path.GetFullPath (Path.Combine (outputPathTextBox.Text, "index.html"));
    }

    private void ReflectorAssemblyRadioButtonCheckedChanged (object sender, EventArgs e)
    {
      if (reflectorAssemblyRadioButton.Checked)
      {
        reflectorAssemblyTextBox.Enabled = true;
        customReflectorTextBox.Enabled = false;
      }
    }

    private void customReflectorRadioButton_CheckedChanged (object sender, EventArgs e)
    {
      if (customReflectorRadioButton.Checked)
      {
        customReflectorTextBox.Enabled = true;
        reflectorAssemblyTextBox.Enabled = false;
      }
    }
  }
}