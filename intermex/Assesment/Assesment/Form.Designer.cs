namespace Assesment
{
    partial class Form
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label4 = new System.Windows.Forms.Label();
            this.startBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.countOfThreadsNud = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.goToFileBtn = new System.Windows.Forms.Button();
            this.summaryLb = new System.Windows.Forms.Label();
            this.searchInCb = new System.Windows.Forms.ComboBox();
            this.searchForCb = new System.Windows.Forms.ComboBox();
            this.statusLb = new System.Windows.Forms.Label();
            this.errorsLb = new System.Windows.Forms.Label();
            this.errorsTb = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.countOfThreadsNud)).BeginInit();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(11, 161);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(578, 538);
            this.treeView1.TabIndex = 6;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Search results:";
            // 
            // startBtn
            // 
            this.startBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.startBtn.Location = new System.Drawing.Point(514, 9);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(75, 23);
            this.startBtn.TabIndex = 11;
            this.startBtn.Text = "Start search";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Location = new System.Drawing.Point(514, 38);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 12;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // countOfThreadsNud
            // 
            this.countOfThreadsNud.Location = new System.Drawing.Point(102, 67);
            this.countOfThreadsNud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.countOfThreadsNud.Name = "countOfThreadsNud";
            this.countOfThreadsNud.Size = new System.Drawing.Size(45, 20);
            this.countOfThreadsNud.TabIndex = 14;
            this.countOfThreadsNud.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Search for:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Search in:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Count of threads:";
            // 
            // goToFileBtn
            // 
            this.goToFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.goToFileBtn.Enabled = false;
            this.goToFileBtn.Location = new System.Drawing.Point(514, 132);
            this.goToFileBtn.Name = "goToFileBtn";
            this.goToFileBtn.Size = new System.Drawing.Size(75, 23);
            this.goToFileBtn.TabIndex = 20;
            this.goToFileBtn.Text = "Go to file";
            this.goToFileBtn.UseVisualStyleBackColor = true;
            this.goToFileBtn.Click += new System.EventHandler(this.goToFileBtn_Click);
            // 
            // summaryLb
            // 
            this.summaryLb.AutoSize = true;
            this.summaryLb.Location = new System.Drawing.Point(8, 145);
            this.summaryLb.Name = "summaryLb";
            this.summaryLb.Size = new System.Drawing.Size(0, 13);
            this.summaryLb.TabIndex = 21;
            // 
            // searchInCb
            // 
            this.searchInCb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchInCb.FormattingEnabled = true;
            this.searchInCb.Location = new System.Drawing.Point(102, 35);
            this.searchInCb.Name = "searchInCb";
            this.searchInCb.Size = new System.Drawing.Size(406, 21);
            this.searchInCb.TabIndex = 22;
            // 
            // searchForCb
            // 
            this.searchForCb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchForCb.FormattingEnabled = true;
            this.searchForCb.Location = new System.Drawing.Point(102, 8);
            this.searchForCb.Name = "searchForCb";
            this.searchForCb.Size = new System.Drawing.Size(406, 21);
            this.searchForCb.TabIndex = 23;
            // 
            // statusLb
            // 
            this.statusLb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLb.BackColor = System.Drawing.SystemColors.Control;
            this.statusLb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statusLb.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.statusLb.Location = new System.Drawing.Point(11, 714);
            this.statusLb.Name = "statusLb";
            this.statusLb.Size = new System.Drawing.Size(578, 20);
            this.statusLb.TabIndex = 24;
            // 
            // errorsLb
            // 
            this.errorsLb.AutoSize = true;
            this.errorsLb.Location = new System.Drawing.Point(8, 747);
            this.errorsLb.Name = "errorsLb";
            this.errorsLb.Size = new System.Drawing.Size(37, 13);
            this.errorsLb.TabIndex = 26;
            this.errorsLb.Text = "Errors:";
            // 
            // errorsTb
            // 
            this.errorsTb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorsTb.Location = new System.Drawing.Point(11, 764);
            this.errorsTb.Multiline = true;
            this.errorsTb.Name = "errorsTb";
            this.errorsTb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.errorsTb.Size = new System.Drawing.Size(578, 61);
            this.errorsTb.TabIndex = 27;
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 837);
            this.Controls.Add(this.errorsTb);
            this.Controls.Add(this.errorsLb);
            this.Controls.Add(this.statusLb);
            this.Controls.Add(this.searchForCb);
            this.Controls.Add(this.searchInCb);
            this.Controls.Add(this.summaryLb);
            this.Controls.Add(this.goToFileBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.countOfThreadsNud);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.treeView1);
            this.MinimumSize = new System.Drawing.Size(618, 786);
            this.Name = "Form";
            this.Text = "Find files";
            this.Load += new System.EventHandler(this.Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.countOfThreadsNud)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.NumericUpDown countOfThreadsNud;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button goToFileBtn;
        private System.Windows.Forms.Label summaryLb;
        private System.Windows.Forms.ComboBox searchInCb;
        private System.Windows.Forms.ComboBox searchForCb;
        private System.Windows.Forms.Label statusLb;
        private System.Windows.Forms.Label errorsLb;
        private System.Windows.Forms.TextBox errorsTb;
    }
}

