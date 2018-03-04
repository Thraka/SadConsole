namespace SadConsole.Editor.FormsControls
{
    partial class ColorPresenter
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picColor = new System.Windows.Forms.PictureBox();
            this.lblCaption = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picColor)).BeginInit();
            this.SuspendLayout();
            // 
            // picColor
            // 
            this.picColor.BackColor = System.Drawing.Color.Black;
            this.picColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picColor.Location = new System.Drawing.Point(3, 3);
            this.picColor.Name = "picColor";
            this.picColor.Size = new System.Drawing.Size(20, 20);
            this.picColor.TabIndex = 7;
            this.picColor.TabStop = false;
            this.picColor.Click += new System.EventHandler(this.picColor_Click);
            // 
            // lblCaption
            // 
            this.lblCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(29, 7);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(43, 13);
            this.lblCaption.TabIndex = 6;
            this.lblCaption.Text = "Caption";
            // 
            // ColorPresenter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picColor);
            this.Controls.Add(this.lblCaption);
            this.MinimumSize = new System.Drawing.Size(26, 26);
            this.Name = "ColorPresenter";
            this.Size = new System.Drawing.Size(124, 26);
            ((System.ComponentModel.ISupportInitialize)(this.picColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picColor;
        private System.Windows.Forms.Label lblCaption;
    }
}
