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
      this.components = new System.ComponentModel.Container ();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager (typeof (MixinXRefForm));
      this.forceOverrideCheckBox = new System.Windows.Forms.CheckBox ();
      this.browseAssemblyPath = new System.Windows.Forms.Button ();
      this.assemblyPathTextBox = new System.Windows.Forms.TextBox ();
      this.pictureBox1 = new System.Windows.Forms.PictureBox ();
      this.outputPathTextBox = new System.Windows.Forms.TextBox ();
      this.browseOutputPath = new System.Windows.Forms.Button ();
      this.customReflectorTextBox = new System.Windows.Forms.TextBox ();
      this.label1 = new System.Windows.Forms.Label ();
      this.label2 = new System.Windows.Forms.Label ();
      this.label3 = new System.Windows.Forms.Label ();
      this.startMixinXRefButton = new System.Windows.Forms.Button ();
      this.logTextBox = new System.Windows.Forms.TextBox ();
      this.cursorIconTimer = new System.Windows.Forms.Timer (this.components);
      this.toolTip1 = new System.Windows.Forms.ToolTip (this.components);
      ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).BeginInit ();
      this.SuspendLayout ();
      // 
      // forceOverrideCheckBox
      // 
      this.forceOverrideCheckBox.AutoSize = true;
      this.forceOverrideCheckBox.Location = new System.Drawing.Point (12, 101);
      this.forceOverrideCheckBox.Name = "forceOverrideCheckBox";
      this.forceOverrideCheckBox.Size = new System.Drawing.Size (131, 17);
      this.forceOverrideCheckBox.TabIndex = 0;
      this.forceOverrideCheckBox.Text = "Replace existing files?";
      this.forceOverrideCheckBox.UseVisualStyleBackColor = true;
      // 
      // browseAssemblyPath
      // 
      this.browseAssemblyPath.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.browseAssemblyPath.Location = new System.Drawing.Point (229, 28);
      this.browseAssemblyPath.Name = "browseAssemblyPath";
      this.browseAssemblyPath.Size = new System.Drawing.Size (54, 20);
      this.browseAssemblyPath.TabIndex = 1;
      this.browseAssemblyPath.Text = "Browse";
      this.browseAssemblyPath.UseVisualStyleBackColor = true;
      this.browseAssemblyPath.Click += new System.EventHandler (this.BrowseAssemblyPath_Click);
      // 
      // assemblyPathTextBox
      // 
      this.assemblyPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.assemblyPathTextBox.Location = new System.Drawing.Point (12, 29);
      this.assemblyPathTextBox.Name = "assemblyPathTextBox";
      this.assemblyPathTextBox.Size = new System.Drawing.Size (211, 20);
      this.assemblyPathTextBox.TabIndex = 2;
      // 
      // pictureBox1
      // 
      this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox1.Image = ((System.Drawing.Image) (resources.GetObject ("pictureBox1.Image")));
      this.pictureBox1.Location = new System.Drawing.Point (307, 26);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size (154, 69);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 3;
      this.pictureBox1.TabStop = false;
      // 
      // outputPathTextBox
      // 
      this.outputPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.outputPathTextBox.Location = new System.Drawing.Point (12, 75);
      this.outputPathTextBox.Name = "outputPathTextBox";
      this.outputPathTextBox.Size = new System.Drawing.Size (211, 20);
      this.outputPathTextBox.TabIndex = 5;
      // 
      // browseOutputPath
      // 
      this.browseOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.browseOutputPath.Location = new System.Drawing.Point (229, 75);
      this.browseOutputPath.Name = "browseOutputPath";
      this.browseOutputPath.Size = new System.Drawing.Size (54, 20);
      this.browseOutputPath.TabIndex = 4;
      this.browseOutputPath.Text = "Browse";
      this.browseOutputPath.UseVisualStyleBackColor = true;
      this.browseOutputPath.Click += new System.EventHandler (this.BrowseOutputPath_Click);
      // 
      // customReflectorTextBox
      // 
      this.customReflectorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.customReflectorTextBox.Location = new System.Drawing.Point (12, 144);
      this.customReflectorTextBox.Name = "customReflectorTextBox";
      this.customReflectorTextBox.Size = new System.Drawing.Size (271, 20);
      this.customReflectorTextBox.TabIndex = 7;
      this.toolTip1.SetToolTip (this.customReflectorTextBox, "FullQualifiedClassName, FullQualifiedAssemblyName");
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point (12, 10);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size (74, 13);
      this.label1.TabIndex = 8;
      this.label1.Text = "Input directory";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point (12, 59);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size (82, 13);
      this.label2.TabIndex = 8;
      this.label2.Text = "Output directory";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font ("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
      this.label3.Location = new System.Drawing.Point (12, 131);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size (129, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Custom reflector (optional)";
      this.toolTip1.SetToolTip (this.label3, "The custom reflector has to be in the input directory");
      // 
      // startMixinXRefButton
      // 
      this.startMixinXRefButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.startMixinXRefButton.Location = new System.Drawing.Point (307, 131);
      this.startMixinXRefButton.Name = "startMixinXRefButton";
      this.startMixinXRefButton.Size = new System.Drawing.Size (154, 33);
      this.startMixinXRefButton.TabIndex = 9;
      this.startMixinXRefButton.Text = "Start Mixin-Cross-Referencer";
      this.startMixinXRefButton.UseVisualStyleBackColor = true;
      this.startMixinXRefButton.Click += new System.EventHandler (this.StartMixinXRefButton_Click);
      // 
      // logTextBox
      // 
      this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.logTextBox.Location = new System.Drawing.Point (15, 192);
      this.logTextBox.Multiline = true;
      this.logTextBox.Name = "logTextBox";
      this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.logTextBox.Size = new System.Drawing.Size (446, 116);
      this.logTextBox.TabIndex = 10;
      // 
      // cursorIconTimer
      // 
      this.cursorIconTimer.Tick += new System.EventHandler (this.CursorIconTimer_Tick);
      // 
      // MixinXRefForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size (480, 326);
      this.Controls.Add (this.logTextBox);
      this.Controls.Add (this.startMixinXRefButton);
      this.Controls.Add (this.label3);
      this.Controls.Add (this.label2);
      this.Controls.Add (this.label1);
      this.Controls.Add (this.customReflectorTextBox);
      this.Controls.Add (this.outputPathTextBox);
      this.Controls.Add (this.browseOutputPath);
      this.Controls.Add (this.pictureBox1);
      this.Controls.Add (this.assemblyPathTextBox);
      this.Controls.Add (this.browseAssemblyPath);
      this.Controls.Add (this.forceOverrideCheckBox);
      this.Icon = ((System.Drawing.Icon) (resources.GetObject ("$this.Icon")));
      this.Name = "MixinXRefForm";
      this.Text = "MixinXRef";
      ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).EndInit ();
      this.ResumeLayout (false);
      this.PerformLayout ();

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
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button startMixinXRefButton;
    private System.Windows.Forms.TextBox logTextBox;
    private System.Windows.Forms.Timer cursorIconTimer;
    private System.Windows.Forms.ToolTip toolTip1;
  }
}

