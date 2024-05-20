namespace MW5_Mod_Manager
{
    partial class ModCheckForm
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
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            buttonValidate = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new System.Drawing.Point(50, 39);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(679, 355);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // buttonValidate
            // 
            buttonValidate.Location = new System.Drawing.Point(50, 415);
            buttonValidate.Name = "buttonValidate";
            buttonValidate.Size = new System.Drawing.Size(108, 23);
            buttonValidate.TabIndex = 1;
            buttonValidate.Text = "&Validate mod files";
            buttonValidate.UseVisualStyleBackColor = true;
            buttonValidate.Click += buttonValidate_Click;
            // 
            // ModValidityForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(buttonValidate);
            Controls.Add(richTextBox1);
            Name = "ModValidityForm";
            Text = "ModValidatorForm";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button buttonValidate;
    }
}