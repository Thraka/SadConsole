namespace SadConsole.Editor.Forms
{
    partial class EditorSettings
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
            this.components = new System.ComponentModel.Container();
            this.chkUseOuter = new System.Windows.Forms.CheckBox();
            this.chkDrawBorder = new System.Windows.Forms.CheckBox();
            this.picOuterColor = new System.Windows.Forms.PictureBox();
            this.lblOuterColor = new System.Windows.Forms.Label();
            this.picInnerColor = new System.Windows.Forms.PictureBox();
            this.lblInnerColor = new System.Windows.Forms.Label();
            this.chkUseInner = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picOuterColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInnerColor)).BeginInit();
            this.SuspendLayout();
            // 
            // chkUseOuter
            // 
            this.chkUseOuter.AutoSize = true;
            this.chkUseOuter.Location = new System.Drawing.Point(12, 12);
            this.chkUseOuter.Name = "chkUseOuter";
            this.chkUseOuter.Size = new System.Drawing.Size(124, 17);
            this.chkUseOuter.TabIndex = 0;
            this.chkUseOuter.Text = "Use outer clear color";
            this.toolTip1.SetToolTip(this.chkUseOuter, "When checked, draws a different outer color around\r\nthe SadConsole game frame. Ot" +
        "herwise the\r\nSadConsole.Settings.ClearColor is used.");
            this.chkUseOuter.UseVisualStyleBackColor = true;
            this.chkUseOuter.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // chkDrawBorder
            // 
            this.chkDrawBorder.AutoSize = true;
            this.chkDrawBorder.Location = new System.Drawing.Point(12, 129);
            this.chkDrawBorder.Name = "chkDrawBorder";
            this.chkDrawBorder.Size = new System.Drawing.Size(84, 17);
            this.chkDrawBorder.TabIndex = 3;
            this.chkDrawBorder.Text = "Draw border";
            this.toolTip1.SetToolTip(this.chkDrawBorder, "Draws a border around the SadConsole frame.");
            this.chkDrawBorder.UseVisualStyleBackColor = true;
            this.chkDrawBorder.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // picOuterColor
            // 
            this.picOuterColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.picOuterColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picOuterColor.Location = new System.Drawing.Point(35, 35);
            this.picOuterColor.Name = "picOuterColor";
            this.picOuterColor.Size = new System.Drawing.Size(20, 20);
            this.picOuterColor.TabIndex = 7;
            this.picOuterColor.TabStop = false;
            this.picOuterColor.Click += new System.EventHandler(this.picOuterColor_Click);
            // 
            // lblOuterColor
            // 
            this.lblOuterColor.AutoSize = true;
            this.lblOuterColor.Location = new System.Drawing.Point(61, 38);
            this.lblOuterColor.Name = "lblOuterColor";
            this.lblOuterColor.Size = new System.Drawing.Size(31, 13);
            this.lblOuterColor.TabIndex = 6;
            this.lblOuterColor.Text = "Color";
            // 
            // picInnerColor
            // 
            this.picInnerColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.picInnerColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picInnerColor.Location = new System.Drawing.Point(35, 94);
            this.picInnerColor.Name = "picInnerColor";
            this.picInnerColor.Size = new System.Drawing.Size(20, 20);
            this.picInnerColor.TabIndex = 10;
            this.picInnerColor.TabStop = false;
            this.picInnerColor.Click += new System.EventHandler(this.picInnerColor_Click);
            // 
            // lblInnerColor
            // 
            this.lblInnerColor.AutoSize = true;
            this.lblInnerColor.Location = new System.Drawing.Point(61, 97);
            this.lblInnerColor.Name = "lblInnerColor";
            this.lblInnerColor.Size = new System.Drawing.Size(31, 13);
            this.lblInnerColor.TabIndex = 9;
            this.lblInnerColor.Text = "Color";
            // 
            // chkUseInner
            // 
            this.chkUseInner.AutoSize = true;
            this.chkUseInner.Location = new System.Drawing.Point(12, 71);
            this.chkUseInner.Name = "chkUseInner";
            this.chkUseInner.Size = new System.Drawing.Size(123, 17);
            this.chkUseInner.TabIndex = 8;
            this.chkUseInner.Text = "Use inner clear color";
            this.toolTip1.SetToolTip(this.chkUseInner, "When checked, draws a different color inside\r\nthe SadConsole game frame. Otherwis" +
        "e the\r\nSadConsole.Settings.ClearColor is used. This \r\nmakes it easy to spot wher" +
        "e your surface is\r\ntransparent.");
            this.chkUseInner.UseVisualStyleBackColor = true;
            this.chkUseInner.CheckedChanged += new System.EventHandler(this.CheckChanged);
            // 
            // EditorSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(157, 159);
            this.Controls.Add(this.picInnerColor);
            this.Controls.Add(this.lblInnerColor);
            this.Controls.Add(this.chkUseInner);
            this.Controls.Add(this.picOuterColor);
            this.Controls.Add(this.lblOuterColor);
            this.Controls.Add(this.chkDrawBorder);
            this.Controls.Add(this.chkUseOuter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditorSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.EditorSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picOuterColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picInnerColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUseOuter;
        private System.Windows.Forms.CheckBox chkDrawBorder;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox picOuterColor;
        private System.Windows.Forms.Label lblOuterColor;
        private System.Windows.Forms.PictureBox picInnerColor;
        private System.Windows.Forms.Label lblInnerColor;
        private System.Windows.Forms.CheckBox chkUseInner;
    }
}