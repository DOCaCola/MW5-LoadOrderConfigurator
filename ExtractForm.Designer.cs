namespace MW5_Mod_Manager
{
    partial class ExtractForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            progressBarExtract = new System.Windows.Forms.ProgressBar();
            richTextBoxExtractLog = new System.Windows.Forms.RichTextBox();
            buttonAction = new System.Windows.Forms.Button();
            progressBarFilePercentage = new System.Windows.Forms.ProgressBar();
            labelFileProgress = new System.Windows.Forms.Label();
            labelTotalProgress = new System.Windows.Forms.Label();
            labelCurrentFile = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // progressBarExtract
            // 
            progressBarExtract.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressBarExtract.Location = new System.Drawing.Point(4, 236);
            progressBarExtract.Name = "progressBarExtract";
            progressBarExtract.Size = new System.Drawing.Size(437, 16);
            progressBarExtract.TabIndex = 0;
            // 
            // richTextBoxExtractLog
            // 
            richTextBoxExtractLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            richTextBoxExtractLog.Location = new System.Drawing.Point(4, 4);
            richTextBoxExtractLog.Name = "richTextBoxExtractLog";
            richTextBoxExtractLog.ReadOnly = true;
            richTextBoxExtractLog.Size = new System.Drawing.Size(437, 152);
            richTextBoxExtractLog.TabIndex = 1;
            richTextBoxExtractLog.Text = "";
            richTextBoxExtractLog.WordWrap = false;
            // 
            // buttonAction
            // 
            buttonAction.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            buttonAction.Location = new System.Drawing.Point(176, 261);
            buttonAction.Name = "buttonAction";
            buttonAction.Size = new System.Drawing.Size(88, 26);
            buttonAction.TabIndex = 2;
            buttonAction.UseVisualStyleBackColor = true;
            buttonAction.Click += buttonAction_Click;
            // 
            // progressBarFilePercentage
            // 
            progressBarFilePercentage.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            progressBarFilePercentage.Location = new System.Drawing.Point(4, 188);
            progressBarFilePercentage.Name = "progressBarFilePercentage";
            progressBarFilePercentage.Size = new System.Drawing.Size(437, 16);
            progressBarFilePercentage.TabIndex = 3;
            // 
            // labelFileProgress
            // 
            labelFileProgress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            labelFileProgress.Location = new System.Drawing.Point(369, 170);
            labelFileProgress.Name = "labelFileProgress";
            labelFileProgress.Size = new System.Drawing.Size(72, 15);
            labelFileProgress.TabIndex = 4;
            labelFileProgress.Text = "0%";
            labelFileProgress.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelTotalProgress
            // 
            labelTotalProgress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            labelTotalProgress.Location = new System.Drawing.Point(369, 218);
            labelTotalProgress.Name = "labelTotalProgress";
            labelTotalProgress.Size = new System.Drawing.Size(72, 15);
            labelTotalProgress.TabIndex = 5;
            labelTotalProgress.Text = "0%";
            labelTotalProgress.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelCurrentFile
            // 
            labelCurrentFile.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            labelCurrentFile.AutoEllipsis = true;
            labelCurrentFile.Location = new System.Drawing.Point(4, 170);
            labelCurrentFile.Name = "labelCurrentFile";
            labelCurrentFile.Size = new System.Drawing.Size(359, 15);
            labelCurrentFile.TabIndex = 6;
            labelCurrentFile.Text = "labelCurrentFile";
            labelCurrentFile.Visible = false;
            // 
            // ExtractForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonAction;
            ClientSize = new System.Drawing.Size(445, 296);
            Controls.Add(labelCurrentFile);
            Controls.Add(labelTotalProgress);
            Controls.Add(labelFileProgress);
            Controls.Add(progressBarFilePercentage);
            Controls.Add(buttonAction);
            Controls.Add(richTextBoxExtractLog);
            Controls.Add(progressBarExtract);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExtractForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Extracting";
            FormClosing += ExtractForm_FormClosing;
            Load += ExtractForm_Load;
            Shown += ExtractForm_Shown;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarExtract;
        private System.Windows.Forms.RichTextBox richTextBoxExtractLog;
        private System.Windows.Forms.Button buttonAction;
        private System.Windows.Forms.ProgressBar progressBarFilePercentage;
        private System.Windows.Forms.Label labelFileProgress;
        private System.Windows.Forms.Label labelTotalProgress;
        private System.Windows.Forms.Label labelCurrentFile;
    }
}