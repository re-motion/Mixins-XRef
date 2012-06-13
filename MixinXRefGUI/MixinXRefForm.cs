using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MixinXRef;
using TalkBack;

namespace MixinXRefGUI
{
  public partial class MixinXRefForm : Form
  {
    private delegate void AppendTextToLogTextBoxAsyncDelegate (string message);

    private delegate void SetStartMixinRefButtonEnabledDelegate (bool enabled);

    private readonly string _persistentSettingsFile;
    private readonly Settings _settings;

    private readonly BackgroundWorker _xrefWorker;

    public MixinXRefForm ()
    {
      InitializeComponent ();

      var baseDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "MixinXRef");
      _persistentSettingsFile = Path.Combine (baseDir, "MixinXRefGUI.dat");

      _settings = new Settings (_persistentSettingsFile,
                                ApplySettings,
                                GetSettings,
                                new XRefArguments
                                  {
                                    AssemblyDirectory = @"C:\",
                                    OutputDirectory = @"C:\",
                                    OverwriteExistingFiles = false,
                                    XMLOutputFileName = "",
                                    ReflectorSource = ReflectorSource.ReflectorAssembly,
                                    ReflectorPath = "MixinXRef.Reflectors*.dll",
                                    CustomReflectorAssemblyQualifiedTypeName = "",
                                    SkipHTMLGeneration = false
                                  });

      UpdateEnabledStatusOfShowResultButton ();
      _xrefWorker = new BackgroundWorker ();
      _xrefWorker.DoWork += (sender, args) => RunXRef ((XRefArguments) args.Argument);
      _xrefWorker.RunWorkerCompleted += (sender, args) => OnXRefFinished ();
    }

    protected override void OnClosing (CancelEventArgs e)
    {
      if (_xrefWorker.IsBusy)
        e.Cancel = true;
    }

    private void ApplySettings (XRefArguments settings)
    {
      assemblyPathTextBox.Text = settings.AssemblyDirectory;
      outputPathTextBox.Text = settings.OutputDirectory;
      reflectorAssemblyTextBox.Text = settings.ReflectorPath;
      customReflectorTextBox.Text = settings.CustomReflectorAssemblyQualifiedTypeName;
      forceOverrideCheckBox.Checked = settings.OverwriteExistingFiles;

      switch (settings.ReflectorSource)
      {
        case ReflectorSource.ReflectorAssembly:
          reflectorAssemblyRadioButton.Checked = true;
          break;
        case ReflectorSource.CustomReflector:
          customReflectorRadioButton.Checked = true;
          break;
      }
    }

    private XRefArguments GetSettings ()
    {
      return new XRefArguments
      {
        AssemblyDirectory = assemblyPathTextBox.Text,
        OutputDirectory = outputPathTextBox.Text,
        ReflectorPath = reflectorAssemblyTextBox.Text,
        CustomReflectorAssemblyQualifiedTypeName = customReflectorTextBox.Text,
        OverwriteExistingFiles = forceOverrideCheckBox.Checked,
        ReflectorSource = customReflectorRadioButton.Checked ? ReflectorSource.CustomReflector : ReflectorSource.ReflectorAssembly
      };
    }

    private void RunXRef (XRefArguments options)
    {
      AppendTextToLogTextBoxAsync ("Running MixinXRef...");
      TalkBackInvoke.Action (sender => XRef.Run (options, sender), message => AppendTextToLogTextBoxAsync (message.Text));
    }

    private void OnXRefFinished ()
    {
      SetStartMixinRefButtonEnabled (true);

      if (File.Exists (GetResultFilePath ()))
        SetShowResultsButtonEnabled (true);
    }

    private void UpdateEnabledStatusOfShowResultButton ()
    {
      showResultsButton.Enabled = File.Exists (GetResultFilePath ());
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
      var remotionAssembly = Path.Combine (GetSettings ().AssemblyDirectory, "Remotion.dll");

      if (File.Exists (remotionAssembly))
      {
        _settings.Save ();
        SetStartMixinRefButtonEnabled (false);

        _xrefWorker.RunWorkerAsync (_settings.Arguments);
      }
      else
      {
        MessageBox.Show (
            "The input directory doesn't contain the remotion assembly. Please select another input directory.",
            "Invalid input directory",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
      }
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
        startMixinXRefButton.Invoke (new SetStartMixinRefButtonEnabledDelegate (SetStartMixinRefButtonEnabled), new object[] { enabled });
      else
        startMixinXRefButton.Enabled = enabled;
    }

    private void SetShowResultsButtonEnabled (bool enabled)
    {
      if (showResultsButton.InvokeRequired)
        showResultsButton.Invoke (new SetStartMixinRefButtonEnabledDelegate (SetShowResultsButtonEnabled), new object[] { enabled });
      else
        showResultsButton.Enabled = enabled;
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
      return Path.GetFullPath (Path.Combine (_settings.Arguments.OutputDirectory, "index.html"));
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