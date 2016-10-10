
namespace MixinXRefGUI
{
  partial class MixinXRefForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose (bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose ();
      }
      base.Dispose (disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MixinXRefForm));
      this.forceOverrideCheckBox = new System.Windows.Forms.CheckBox();
      this.browseAssemblyPath = new System.Windows.Forms.Button();
      this.assemblyPathTextBox = new System.Windows.Forms.TextBox();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.outputPathTextBox = new System.Windows.Forms.TextBox();
      this.browseOutputPath = new System.Windows.Forms.Button();
      this.customReflectorTextBox = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.startMixinXRefButton = new System.Windows.Forms.Button();
      this.logTextBox = new System.Windows.Forms.TextBox();
      this.cursorIconTimer = new System.Windows.Forms.Timer(this.components);
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.reflectorAssemblyTextBox = new System.Windows.Forms.TextBox();
      this.showResultsButton = new System.Windows.Forms.Button();
      this.reflectorAssemblyRadioButton = new System.Windows.Forms.RadioButton();
      this.customReflectorRadioButton = new System.Windows.Forms.RadioButton();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.browseConfigFile = new System.Windows.Forms.Button();
      this.browseAppBaseDirectory = new System.Windows.Forms.Button();
      this.appBaseDirectory = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.appConfigFile = new System.Windows.Forms.TextBox();
      this.generateErrorReportsOnlyCheckBox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      // 
      // forceOverrideCheckBox
      // 
      this.forceOverrideCheckBox.AutoSize = true;
      this.forceOverrideCheckBox.Location = new System.Drawing.Point(350, 190);
      this.forceOverrideCheckBox.Margin = new System.Windows.Forms.Padding(6);
      this.forceOverrideCheckBox.Name = "forceOverrideCheckBox";
      this.forceOverrideCheckBox.Size = new System.Drawing.Size(230, 29);
      this.forceOverrideCheckBox.TabIndex = 0;
      this.forceOverrideCheckBox.Text = "Replace existing files?";
      this.forceOverrideCheckBox.UseVisualStyleBackColor = true;
      // 
      // browseAssemblyPath
      // 
      this.browseAssemblyPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.browseAssemblyPath.Location = new System.Drawing.Point(975, 57);
      this.browseAssemblyPath.Margin = new System.Windows.Forms.Padding(6);
      this.browseAssemblyPath.Name = "browseAssemblyPath";
      this.browseAssemblyPath.Size = new System.Drawing.Size(99, 37);
      this.browseAssemblyPath.TabIndex = 1;
      this.browseAssemblyPath.Text = "Browse";
      this.browseAssemblyPath.UseVisualStyleBackColor = true;
      this.browseAssemblyPath.Click += new System.EventHandler(this.BrowseAssemblyPath_Click);
      // 
      // assemblyPathTextBox
      // 
      this.assemblyPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.assemblyPathTextBox.Location = new System.Drawing.Point(350, 57);
      this.assemblyPathTextBox.Margin = new System.Windows.Forms.Padding(6);
      this.assemblyPathTextBox.Name = "assemblyPathTextBox";
      this.assemblyPathTextBox.Size = new System.Drawing.Size(611, 29);
      this.assemblyPathTextBox.TabIndex = 2;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point(33, 33);
      this.pictureBox1.Margin = new System.Windows.Forms.Padding(6);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(154, 69);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      // 
      // outputPathTextBox
      // 
      this.outputPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.outputPathTextBox.Location = new System.Drawing.Point(350, 142);
      this.outputPathTextBox.Margin = new System.Windows.Forms.Padding(6);
      this.outputPathTextBox.Name = "outputPathTextBox";
      this.outputPathTextBox.Size = new System.Drawing.Size(611, 29);
      this.outputPathTextBox.TabIndex = 5;
      // 
      // browseOutputPath
      // 
      this.browseOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.browseOutputPath.Location = new System.Drawing.Point(975, 142);
      this.browseOutputPath.Margin = new System.Windows.Forms.Padding(6);
      this.browseOutputPath.Name = "browseOutputPath";
      this.browseOutputPath.Size = new System.Drawing.Size(99, 37);
      this.browseOutputPath.TabIndex = 4;
      this.browseOutputPath.Text = "Browse";
      this.browseOutputPath.UseVisualStyleBackColor = true;
      this.browseOutputPath.Click += new System.EventHandler(this.BrowseOutputPath_Click);
      // 
      // customReflectorTextBox
      // 
      this.customReflectorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.customReflectorTextBox.Enabled = false;
      this.customReflectorTextBox.Location = new System.Drawing.Point(35, 168);
      this.customReflectorTextBox.Margin = new System.Windows.Forms.Padding(6);
      this.customReflectorTextBox.Name = "customReflectorTextBox";
      this.customReflectorTextBox.Size = new System.Drawing.Size(1014, 29);
      this.customReflectorTextBox.TabIndex = 7;
      this.toolTip1.SetToolTip(this.customReflectorTextBox, "FullQualifiedClassName, FullQualifiedAssemblyName");
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(350, 22);
      this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(134, 25);
      this.label1.TabIndex = 8;
      this.label1.Text = "Input directory";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(350, 113);
      this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(150, 25);
      this.label2.TabIndex = 8;
      this.label2.Text = "Output directory";
      // 
      // startMixinXRefButton
      // 
      this.startMixinXRefButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.startMixinXRefButton.Location = new System.Drawing.Point(818, 486);
      this.startMixinXRefButton.Margin = new System.Windows.Forms.Padding(6);
      this.startMixinXRefButton.Name = "startMixinXRefButton";
      this.startMixinXRefButton.Size = new System.Drawing.Size(293, 79);
      this.startMixinXRefButton.TabIndex = 9;
      this.startMixinXRefButton.Text = "Start Mixin-Cross-Referencer";
      this.startMixinXRefButton.UseVisualStyleBackColor = true;
      this.startMixinXRefButton.Click += new System.EventHandler(this.StartMixinXRefButton_Click);
      // 
      // logTextBox
      // 
      this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.logTextBox.Location = new System.Drawing.Point(28, 685);
      this.logTextBox.Margin = new System.Windows.Forms.Padding(6);
      this.logTextBox.Multiline = true;
      this.logTextBox.Name = "logTextBox";
      this.logTextBox.ReadOnly = true;
      this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.logTextBox.Size = new System.Drawing.Size(1080, 517);
      this.logTextBox.TabIndex = 10;
      this.logTextBox.WordWrap = false;
      // 
      // cursorIconTimer
      // 
      this.cursorIconTimer.Tick += new System.EventHandler(this.CursorIconTimer_Tick);
      // 
      // reflectorAssemblyTextBox
      // 
      this.reflectorAssemblyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.reflectorAssemblyTextBox.Location = new System.Drawing.Point(35, 78);
      this.reflectorAssemblyTextBox.Margin = new System.Windows.Forms.Padding(6);
      this.reflectorAssemblyTextBox.Name = "reflectorAssemblyTextBox";
      this.reflectorAssemblyTextBox.Size = new System.Drawing.Size(1014, 29);
      this.reflectorAssemblyTextBox.TabIndex = 14;
      this.toolTip1.SetToolTip(this.reflectorAssemblyTextBox, "FullQualifiedClassName, FullQualifiedAssemblyName");
      // 
      // showResultsButton
      // 
      this.showResultsButton.Enabled = false;
      this.showResultsButton.Location = new System.Drawing.Point(818, 576);
      this.showResultsButton.Margin = new System.Windows.Forms.Padding(6);
      this.showResultsButton.Name = "showResultsButton";
      this.showResultsButton.Size = new System.Drawing.Size(293, 55);
      this.showResultsButton.TabIndex = 11;
      this.showResultsButton.Text = "Show results";
      this.showResultsButton.UseVisualStyleBackColor = true;
      this.showResultsButton.Click += new System.EventHandler(this.ShowResultsButton_Click);
      // 
      // reflectorAssemblyRadioButton
      // 
      this.reflectorAssemblyRadioButton.AutoSize = true;
      this.reflectorAssemblyRadioButton.Checked = true;
      this.reflectorAssemblyRadioButton.Location = new System.Drawing.Point(11, 35);
      this.reflectorAssemblyRadioButton.Margin = new System.Windows.Forms.Padding(6);
      this.reflectorAssemblyRadioButton.Name = "reflectorAssemblyRadioButton";
      this.reflectorAssemblyRadioButton.Size = new System.Drawing.Size(485, 29);
      this.reflectorAssemblyRadioButton.TabIndex = 12;
      this.reflectorAssemblyRadioButton.TabStop = true;
      this.reflectorAssemblyRadioButton.Text = "Reflector assembly (file path with * wildcard support)";
      this.reflectorAssemblyRadioButton.UseVisualStyleBackColor = true;
      this.reflectorAssemblyRadioButton.CheckedChanged += new System.EventHandler(this.ReflectorAssemblyRadioButtonCheckedChanged);
      // 
      // customReflectorRadioButton
      // 
      this.customReflectorRadioButton.AutoSize = true;
      this.customReflectorRadioButton.Location = new System.Drawing.Point(11, 126);
      this.customReflectorRadioButton.Margin = new System.Windows.Forms.Padding(6);
      this.customReflectorRadioButton.Name = "customReflectorRadioButton";
      this.customReflectorRadioButton.Size = new System.Drawing.Size(454, 29);
      this.customReflectorRadioButton.TabIndex = 13;
      this.customReflectorRadioButton.Text = "Custom reflector (assembly qualified type name)";
      this.customReflectorRadioButton.UseVisualStyleBackColor = true;
      this.customReflectorRadioButton.CheckedChanged += new System.EventHandler(this.customReflectorRadioButton_CheckedChanged);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.reflectorAssemblyRadioButton);
      this.groupBox1.Controls.Add(this.customReflectorRadioButton);
      this.groupBox1.Controls.Add(this.reflectorAssemblyTextBox);
      this.groupBox1.Controls.Add(this.customReflectorTextBox);
      this.groupBox1.Location = new System.Drawing.Point(22, 231);
      this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(6);
      this.groupBox1.Size = new System.Drawing.Size(1089, 223);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Remotion reflector source";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(22, 655);
      this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(104, 25);
      this.label3.TabIndex = 12;
      this.label3.Text = "Log output";
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.browseConfigFile);
      this.groupBox3.Controls.Add(this.browseAppBaseDirectory);
      this.groupBox3.Controls.Add(this.appBaseDirectory);
      this.groupBox3.Controls.Add(this.label5);
      this.groupBox3.Controls.Add(this.label4);
      this.groupBox3.Controls.Add(this.appConfigFile);
      this.groupBox3.Location = new System.Drawing.Point(22, 465);
      this.groupBox3.Margin = new System.Windows.Forms.Padding(6);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Padding = new System.Windows.Forms.Padding(6);
      this.groupBox3.Size = new System.Drawing.Size(785, 185);
      this.groupBox3.TabIndex = 13;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Special application configuration (optional)";
      // 
      // browseConfigFile
      // 
      this.browseConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.browseConfigFile.Location = new System.Drawing.Point(675, 140);
      this.browseConfigFile.Margin = new System.Windows.Forms.Padding(6);
      this.browseConfigFile.Name = "browseConfigFile";
      this.browseConfigFile.Size = new System.Drawing.Size(99, 37);
      this.browseConfigFile.TabIndex = 5;
      this.browseConfigFile.Text = "Browse";
      this.browseConfigFile.UseVisualStyleBackColor = true;
      this.browseConfigFile.Click += new System.EventHandler(this.browseConfigFile_Click);
      // 
      // browseAppBaseDirectory
      // 
      this.browseAppBaseDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.browseAppBaseDirectory.Location = new System.Drawing.Point(675, 66);
      this.browseAppBaseDirectory.Margin = new System.Windows.Forms.Padding(6);
      this.browseAppBaseDirectory.Name = "browseAppBaseDirectory";
      this.browseAppBaseDirectory.Size = new System.Drawing.Size(99, 37);
      this.browseAppBaseDirectory.TabIndex = 4;
      this.browseAppBaseDirectory.Text = "Browse";
      this.browseAppBaseDirectory.UseVisualStyleBackColor = true;
      this.browseAppBaseDirectory.Click += new System.EventHandler(this.browseAppBaseDirectory_Click);
      // 
      // appBaseDirectory
      // 
      this.appBaseDirectory.Location = new System.Drawing.Point(13, 68);
      this.appBaseDirectory.Margin = new System.Windows.Forms.Padding(6);
      this.appBaseDirectory.Name = "appBaseDirectory";
      this.appBaseDirectory.Size = new System.Drawing.Size(648, 29);
      this.appBaseDirectory.TabIndex = 3;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(13, 37);
      this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(660, 25);
      this.label5.TabIndex = 2;
      this.label5.Text = "Application base directory (if given, input directory must be a sub-directory):";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(11, 111);
      this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(321, 25);
      this.label4.TabIndex = 1;
      this.label4.Text = "App/Web.config file (filename only):";
      // 
      // appConfigFile
      // 
      this.appConfigFile.Location = new System.Drawing.Point(11, 140);
      this.appConfigFile.Margin = new System.Windows.Forms.Padding(6);
      this.appConfigFile.Name = "appConfigFile";
      this.appConfigFile.Size = new System.Drawing.Size(649, 29);
      this.appConfigFile.TabIndex = 0;
      // 
      // generateErrorReportsOnlyCheckBox
      // 
      this.generateErrorReportsOnlyCheckBox.AutoSize = true;
      this.generateErrorReportsOnlyCheckBox.Location = new System.Drawing.Point(697, 190);
      this.generateErrorReportsOnlyCheckBox.Name = "generateErrorReportsOnlyCheckBox";
      this.generateErrorReportsOnlyCheckBox.Size = new System.Drawing.Size(281, 29);
      this.generateErrorReportsOnlyCheckBox.TabIndex = 14;
      this.generateErrorReportsOnlyCheckBox.Text = "Generate only error report?";
      this.generateErrorReportsOnlyCheckBox.UseVisualStyleBackColor = true;
      this.generateErrorReportsOnlyCheckBox.CheckedChanged += new System.EventHandler(this.generateErrorReportsOnlyCheckBox_CheckedChanged);
      // 
      // MixinXRefForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1133, 1228);
      this.Controls.Add(this.generateErrorReportsOnlyCheckBox);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.startMixinXRefButton);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.showResultsButton);
      this.Controls.Add(this.logTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.outputPathTextBox);
      this.Controls.Add(this.browseOutputPath);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.assemblyPathTextBox);
      this.Controls.Add(this.browseAssemblyPath);
      this.Controls.Add(this.forceOverrideCheckBox);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(6);
      this.MinimumSize = new System.Drawing.Size(1142, 958);
      this.Name = "MixinXRefForm";
      this.Text = "MixinXRef";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox forceOverrideCheckBox;
    private System.Windows.Forms.Button browseAssemblyPath;
    private System.Windows.Forms.TextBox assemblyPathTextBox;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.TextBox outputPathTextBox;
    private System.Windows.Forms.Button browseOutputPath;
    private System.Windows.Forms.TextBox customReflectorTextBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button startMixinXRefButton;
    private System.Windows.Forms.TextBox logTextBox;
    private System.Windows.Forms.Timer cursorIconTimer;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.Button showResultsButton;
    private System.Windows.Forms.RadioButton reflectorAssemblyRadioButton;
    private System.Windows.Forms.RadioButton customReflectorRadioButton;
    private System.Windows.Forms.TextBox reflectorAssemblyTextBox;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox appConfigFile;
    private System.Windows.Forms.Button browseAppBaseDirectory;
    private System.Windows.Forms.TextBox appBaseDirectory;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button browseConfigFile;
    private System.Windows.Forms.CheckBox generateErrorReportsOnlyCheckBox;
  }
}

