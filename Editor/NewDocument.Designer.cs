namespace SadConsole.Editor
{
    partial class NewDocument
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
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.clrForeground = new SadConsole.Editor.FormsControls.ColorPresenter();
            this.clrBackground = new SadConsole.Editor.FormsControls.ColorPresenter();
            this.optSurface = new System.Windows.Forms.RadioButton();
            this.optObject = new System.Windows.Forms.RadioButton();
            this.optGameScene = new System.Windows.Forms.RadioButton();
            this.btnChangeFont = new System.Windows.Forms.Button();
            this.txtFontSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFont = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // numWidth
            // 
            this.numWidth.Location = new System.Drawing.Point(158, 12);
            this.numWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(48, 20);
            this.numWidth.TabIndex = 3;
            this.numWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numHeight
            // 
            this.numHeight.Location = new System.Drawing.Point(269, 12);
            this.numHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(48, 20);
            this.numHeight.TabIndex = 4;
            this.numHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(225, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Height";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(117, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Width";
            // 
            // clrForeground
            // 
            this.clrForeground.Location = new System.Drawing.Point(121, 124);
            this.clrForeground.MinimumSize = new System.Drawing.Size(26, 26);
            this.clrForeground.Name = "clrForeground";
            this.clrForeground.TabIndex = 10;
            this.clrForeground.Text = "Default foreground";
            // 
            // clrBackground
            // 
            this.clrBackground.Location = new System.Drawing.Point(121, 92);
            this.clrBackground.MinimumSize = new System.Drawing.Size(26, 26);
            this.clrBackground.Name = "clrBackground";
            this.clrBackground.TabIndex = 11;
            this.clrBackground.Text = "Default background";
            // 
            // optSurface
            // 
            this.optSurface.AutoSize = true;
            this.optSurface.Checked = true;
            this.optSurface.Location = new System.Drawing.Point(12, 12);
            this.optSurface.Name = "optSurface";
            this.optSurface.Size = new System.Drawing.Size(62, 17);
            this.optSurface.TabIndex = 12;
            this.optSurface.TabStop = true;
            this.optSurface.Text = "Surface";
            this.optSurface.UseVisualStyleBackColor = true;
            // 
            // optObject
            // 
            this.optObject.AutoSize = true;
            this.optObject.Location = new System.Drawing.Point(12, 58);
            this.optObject.Name = "optObject";
            this.optObject.Size = new System.Drawing.Size(87, 17);
            this.optObject.TabIndex = 13;
            this.optObject.Text = "Game Object";
            this.optObject.UseVisualStyleBackColor = true;
            // 
            // optGameScene
            // 
            this.optGameScene.AutoSize = true;
            this.optGameScene.Location = new System.Drawing.Point(12, 35);
            this.optGameScene.Name = "optGameScene";
            this.optGameScene.Size = new System.Drawing.Size(87, 17);
            this.optGameScene.TabIndex = 14;
            this.optGameScene.Text = "Game Scene";
            this.optGameScene.UseVisualStyleBackColor = true;
            // 
            // btnChangeFont
            // 
            this.btnChangeFont.Location = new System.Drawing.Point(229, 38);
            this.btnChangeFont.Name = "btnChangeFont";
            this.btnChangeFont.Size = new System.Drawing.Size(88, 23);
            this.btnChangeFont.TabIndex = 25;
            this.btnChangeFont.Text = "Change Font";
            this.btnChangeFont.UseVisualStyleBackColor = true;
            this.btnChangeFont.Click += new System.EventHandler(this.btnChangeFont_Click);
            // 
            // txtFontSize
            // 
            this.txtFontSize.Location = new System.Drawing.Point(175, 66);
            this.txtFontSize.Name = "txtFontSize";
            this.txtFontSize.ReadOnly = true;
            this.txtFontSize.Size = new System.Drawing.Size(42, 20);
            this.txtFontSize.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Font Size";
            // 
            // txtFont
            // 
            this.txtFont.Location = new System.Drawing.Point(152, 40);
            this.txtFont.Name = "txtFont";
            this.txtFont.ReadOnly = true;
            this.txtFont.Size = new System.Drawing.Size(65, 20);
            this.txtFont.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(118, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Font";
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(105, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(3, 148);
            this.label4.TabIndex = 27;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(242, 168);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 28;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(12, 168);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 29;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // NewDocument
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(329, 203);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnChangeFont);
            this.Controls.Add(this.txtFontSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFont);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.optGameScene);
            this.Controls.Add(this.optObject);
            this.Controls.Add(this.optSurface);
            this.Controls.Add(this.clrBackground);
            this.Controls.Add(this.clrForeground);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numHeight);
            this.Controls.Add(this.numWidth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewDocument";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Document";
            this.Load += new System.EventHandler(this.NewDocument_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NumericUpDown numWidth;
        private System.Windows.Forms.NumericUpDown numHeight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private FormsControls.ColorPresenter clrForeground;
        private FormsControls.ColorPresenter clrBackground;
        private System.Windows.Forms.RadioButton optSurface;
        private System.Windows.Forms.RadioButton optObject;
        private System.Windows.Forms.RadioButton optGameScene;
        private System.Windows.Forms.Button btnChangeFont;
        private System.Windows.Forms.TextBox txtFontSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFont;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}