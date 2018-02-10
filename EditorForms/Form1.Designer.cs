namespace SadConsole.Editor
{
    partial class Form1
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
            this.game1 = new SadConsole.Editor.Game();
            this.SuspendLayout();
            // 
            // game1
            // 
            this.game1.Location = new System.Drawing.Point(285, 260);
            this.game1.Name = "game1";
            this.game1.Size = new System.Drawing.Size(559, 324);
            this.game1.TabIndex = 7;
            this.game1.Text = "game1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 845);
            this.Controls.Add(this.game1);
            this.Name = "Form1";
            this.Text = "SadConsole Editor";
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion

        private Game game1;
    }
}

