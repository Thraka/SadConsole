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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.game1 = new SadConsole.Editor.Game();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlScreenSettings = new SadConsole.Editor.Panels.ScreenSettingsPanel();
            this.pnlScreens = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlToolsList = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cboToolsList = new System.Windows.Forms.ComboBox();
            this.editorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chkEditMode = new System.Windows.Forms.CheckBox();
            this.btnSettings = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlScreens.SuspendLayout();
            this.pnlToolsList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorBindingSource)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 32);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.game1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.pnlScreens);
            this.splitContainer1.Panel2.Controls.Add(this.pnlToolsList);
            this.splitContainer1.Panel2MinSize = 240;
            this.splitContainer1.Size = new System.Drawing.Size(784, 509);
            this.splitContainer1.SplitterDistance = 518;
            this.splitContainer1.TabIndex = 1;
            // 
            // game1
            // 
            this.game1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.game1.Location = new System.Drawing.Point(0, 0);
            this.game1.Name = "game1";
            this.game1.Size = new System.Drawing.Size(518, 509);
            this.game1.TabIndex = 0;
            this.game1.Text = "game1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlScreenSettings);
            this.panel1.Location = new System.Drawing.Point(3, 145);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(228, 180);
            this.panel1.TabIndex = 7;
            // 
            // pnlScreenSettings
            // 
            this.pnlScreenSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlScreenSettings.Location = new System.Drawing.Point(3, 3);
            this.pnlScreenSettings.Name = "pnlScreenSettings";
            this.pnlScreenSettings.Size = new System.Drawing.Size(223, 174);
            this.pnlScreenSettings.TabIndex = 8;
            // 
            // pnlScreens
            // 
            this.pnlScreens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlScreens.Controls.Add(this.treeView1);
            this.pnlScreens.Controls.Add(this.label1);
            this.pnlScreens.Location = new System.Drawing.Point(3, 3);
            this.pnlScreens.Name = "pnlScreens";
            this.pnlScreens.Size = new System.Drawing.Size(258, 136);
            this.pnlScreens.TabIndex = 5;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(6, 16);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(246, 117);
            this.treeView1.TabIndex = 4;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Screens";
            // 
            // pnlToolsList
            // 
            this.pnlToolsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlToolsList.Controls.Add(this.label2);
            this.pnlToolsList.Controls.Add(this.cboToolsList);
            this.pnlToolsList.Location = new System.Drawing.Point(2, 331);
            this.pnlToolsList.Name = "pnlToolsList";
            this.pnlToolsList.Size = new System.Drawing.Size(262, 27);
            this.pnlToolsList.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tools";
            // 
            // cboToolsList
            // 
            this.cboToolsList.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.editorBindingSource, "SelectedTool", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cboToolsList.DataBindings.Add(new System.Windows.Forms.Binding("DataSource", this.editorBindingSource, "Tools", true));
            this.cboToolsList.DisplayMember = "Name";
            this.cboToolsList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboToolsList.FormattingEnabled = true;
            this.cboToolsList.Location = new System.Drawing.Point(42, 3);
            this.cboToolsList.Name = "cboToolsList";
            this.cboToolsList.Size = new System.Drawing.Size(184, 21);
            this.cboToolsList.TabIndex = 3;
            this.cboToolsList.SelectedIndexChanged += new System.EventHandler(this.cboToolsList_SelectedIndexChanged);
            // 
            // editorBindingSource
            // 
            this.editorBindingSource.DataSource = typeof(SadConsole.Editor.DataContext);
            this.editorBindingSource.CurrentItemChanged += new System.EventHandler(this.editorBindingSource_CurrentItemChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.statusStrip1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(790, 564);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip1.Location = new System.Drawing.Point(0, 544);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(790, 20);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.chkEditMode);
            this.flowLayoutPanel1.Controls.Add(this.btnSettings);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(784, 23);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // chkEditMode
            // 
            this.chkEditMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkEditMode.AutoSize = true;
            this.chkEditMode.Location = new System.Drawing.Point(0, 0);
            this.chkEditMode.Margin = new System.Windows.Forms.Padding(0);
            this.chkEditMode.Name = "chkEditMode";
            this.chkEditMode.Size = new System.Drawing.Size(78, 23);
            this.chkEditMode.TabIndex = 4;
            this.chkEditMode.Text = "Pause Game";
            this.chkEditMode.UseVisualStyleBackColor = true;
            this.chkEditMode.CheckedChanged += new System.EventHandler(this.chkEditMode_CheckedChanged);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(78, 0);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(0);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSettings.TabIndex = 5;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 564);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "SadConsole Editor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.pnlScreens.ResumeLayout(false);
            this.pnlScreens.PerformLayout();
            this.pnlToolsList.ResumeLayout(false);
            this.pnlToolsList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorBindingSource)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Game game1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel pnlScreens;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlToolsList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboToolsList;
        private System.Windows.Forms.BindingSource editorBindingSource;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox chkEditMode;
        private System.Windows.Forms.Panel panel1;
        private Panels.ScreenSettingsPanel pnlScreenSettings;
        private System.Windows.Forms.Button btnSettings;
    }
}

