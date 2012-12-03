// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                                    SkipHTMLGeneration = false,
                                    IgnoredAssemblies = Enumerable.Empty<string> (),
                                    AppBaseDirectory = null,
                                    AppConfigFile = null
                                  });

      UpdateEnabledStatusOfShowResultButton ();
      _xrefWorker = new BackgroundWorker ();
      _xrefWorker.DoWork += (sender, args) => RunXRef ((XRefArguments) args.Argument);
      _xrefWorker.RunWorkerCompleted += (sender, args) => OnXRefFinished (args);
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
      ignoreAssembliesTextBox.Text = string.Join (Environment.NewLine, settings.IgnoredAssemblies.ToArray ());
      appBaseDirectory.Text = settings.AppBaseDirectory;
      appConfigFile.Text = settings.AppConfigFile;

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
        ReflectorSource = customReflectorRadioButton.Checked ? ReflectorSource.CustomReflector : ReflectorSource.ReflectorAssembly,
        IgnoredAssemblies = ignoreAssembliesTextBox.Text.Split (new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries),
        AppBaseDirectory = appBaseDirectory.Text,
        AppConfigFile = appConfigFile.Text
      };
    }

    private void RunXRef (XRefArguments options)
    {
      AppendTextToLogTextBoxAsync ("Running MixinXRef...");
      CrossAppDomainCommunicator.MessageReceivedDelegate onMessageReceived = new GUIMessageReceiver (this).MessageReceived;
      new XRefInAppDomainRunner ().Run(options, onMessageReceived);
    }

    private void OnXRefFinished (RunWorkerCompletedEventArgs args)
    {
      if (args.Error != null)
        throw args.Error;

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

    private void ShowAndSetFileBrowserDialogForTextBox (string description, TextBox pathTextBox, string basePath, string basePathNonExistentMessage)
    {
      if (string.IsNullOrEmpty (basePath) || !new DirectoryInfo (basePath).Exists)
      {
        MessageBox.Show (this, basePathNonExistentMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      using (var fileBrowserDialog = new OpenFileDialog { Title = description })
      {
        var objResult = fileBrowserDialog.ShowDialog (this);
        if (objResult == DialogResult.OK)
          pathTextBox.Text = fileBrowserDialog.SafeFileName;
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

    public void AppendTextToLogTextBoxAsync (string message)
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

    private void browseAppBaseDirectory_Click (object sender, EventArgs e)
    {
      ShowAndSetFolderBrowserDialogForTextBox ("Select application base directory", appBaseDirectory);
    }

    private void browseConfigFile_Click (object sender, EventArgs e)
    {
      ShowAndSetFileBrowserDialogForTextBox ("Select configuration file", appConfigFile, appBaseDirectory.Text, "Select an application base directory first!");
    }
  }
}