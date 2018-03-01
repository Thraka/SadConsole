namespace SadConsole.Editor.Panels
{
    partial class GlyphEditPanel
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.picGlyph = new System.Windows.Forms.PictureBox();
            this.picBackground = new System.Windows.Forms.PictureBox();
            this.picForeground = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGlyph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picForeground)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.picGlyph);
            this.groupBox1.Controls.Add(this.picBackground);
            this.groupBox1.Controls.Add(this.picForeground);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 103);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // picGlyph
            // 
            this.picGlyph.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.picGlyph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picGlyph.Location = new System.Drawing.Point(9, 71);
            this.picGlyph.Name = "picGlyph";
            this.picGlyph.Size = new System.Drawing.Size(20, 20);
            this.picGlyph.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picGlyph.TabIndex = 7;
            this.picGlyph.TabStop = false;
            this.picGlyph.Click += new System.EventHandler(this.picGlyph_Click);
            // 
            // picBackground
            // 
            this.picBackground.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.picBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBackground.Location = new System.Drawing.Point(9, 45);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(20, 20);
            this.picBackground.TabIndex = 5;
            this.picBackground.TabStop = false;
            this.picBackground.Click += new System.EventHandler(this.picBackground_Click);
            // 
            // picForeground
            // 
            this.picForeground.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.picForeground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picForeground.Location = new System.Drawing.Point(9, 19);
            this.picForeground.Name = "picForeground";
            this.picForeground.Size = new System.Drawing.Size(20, 20);
            this.picForeground.TabIndex = 4;
            this.picForeground.TabStop = false;
            this.picForeground.Click += new System.EventHandler(this.picForeground_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Glyph";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Background Color";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Foreground Color";
            // 
            // GlyphEditPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(140, 0);
            this.Name = "GlyphEditPanel";
            this.Size = new System.Drawing.Size(206, 109);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picGlyph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picForeground)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox picGlyph;
        private System.Windows.Forms.PictureBox picBackground;
        private System.Windows.Forms.PictureBox picForeground;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
