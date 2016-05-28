namespace compareWindow
{
    partial class CompareWindow
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompareWindow));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.compTreeViewLeft = new System.Windows.Forms.TreeView();
            this.compTreeViewRight = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(12, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.AutoScroll = true;
            this.splitContainer2.Panel1.Controls.Add(this.compTreeViewLeft);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.AutoScroll = true;
            this.splitContainer2.Panel2.Controls.Add(this.compTreeViewRight);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Size = new System.Drawing.Size(917, 668);
            this.splitContainer2.SplitterDistance = 456;
            this.splitContainer2.TabIndex = 8;
            // 
            // compTreeViewLeft
            // 
            this.compTreeViewLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.compTreeViewLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compTreeViewLeft.FullRowSelect = true;
            this.compTreeViewLeft.HideSelection = false;
            this.compTreeViewLeft.Location = new System.Drawing.Point(0, 29);
            this.compTreeViewLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.compTreeViewLeft.Name = "compTreeViewLeft";
            this.compTreeViewLeft.Size = new System.Drawing.Size(456, 639);
            this.compTreeViewLeft.TabIndex = 5;
            // 
            // compTreeViewRight
            // 
            this.compTreeViewRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.compTreeViewRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compTreeViewRight.FullRowSelect = true;
            this.compTreeViewRight.HideSelection = false;
            this.compTreeViewRight.Location = new System.Drawing.Point(0, 29);
            this.compTreeViewRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.compTreeViewRight.Name = "compTreeViewRight";
            this.compTreeViewRight.Size = new System.Drawing.Size(457, 639);
            this.compTreeViewRight.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(456, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "左侧窗口";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(457, 29);
            this.label2.TabIndex = 1;
            this.label2.Text = "右侧窗口";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(935, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(25, 668);
            this.panel1.TabIndex = 6;
            // 
            // CompareWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 668);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitContainer2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CompareWindow";
            this.Text = "对比";
            this.Load += new System.EventHandler(this.CompareWindow_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView compTreeViewLeft;
        private System.Windows.Forms.TreeView compTreeViewRight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;


    }
}

