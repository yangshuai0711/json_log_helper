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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompareWindow));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.compTreeViewLeft = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.compTreeViewRight = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.highLightTimer = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.AutoScroll = true;
            this.splitContainer2.Panel1.Controls.Add(this.compTreeViewLeft);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.splitContainer2_Panel_Scroll);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.AutoScroll = true;
            this.splitContainer2.Panel2.Controls.Add(this.compTreeViewRight);
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Size = new System.Drawing.Size(900, 668);
            this.splitContainer2.SplitterDistance = 446;
            this.splitContainer2.TabIndex = 8;
            this.splitContainer2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.splitContainer2_Panel_Scroll);
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
            this.compTreeViewLeft.PathSeparator = "#";
            this.compTreeViewLeft.Size = new System.Drawing.Size(446, 639);
            this.compTreeViewLeft.TabIndex = 5;
            this.compTreeViewLeft.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.compTreeView_AfterExpandChange);
            this.compTreeViewLeft.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.compTreeView_AfterExpandChange);
            this.compTreeViewLeft.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.compTreeView_AfterCheck);
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(446, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "左侧窗口";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.compTreeViewRight.PathSeparator = "#";
            this.compTreeViewRight.Size = new System.Drawing.Size(450, 639);
            this.compTreeViewRight.TabIndex = 4;
            this.compTreeViewRight.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.compTreeView_AfterExpandChange);
            this.compTreeViewRight.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.compTreeView_AfterExpandChange);
            this.compTreeViewRight.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.compTreeView_AfterCheck);
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(450, 29);
            this.label2.TabIndex = 1;
            this.label2.Text = "右侧窗口";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Font = new System.Drawing.Font("微软雅黑", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(900, 0);
            this.panel1.MinimumSize = new System.Drawing.Size(60, 2);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            this.panel1.Size = new System.Drawing.Size(60, 668);
            this.panel1.TabIndex = 6;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // highLightTimer
            // 
            this.highLightTimer.Interval = 300;
            this.highLightTimer.Tick += new System.EventHandler(this.highLightTimer_Tick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(900, 668);
            this.panel2.TabIndex = 6;
            this.panel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.splitContainer2_Panel_Scroll);
            // 
            // CompareWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 668);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CompareWindow";
            this.Text = "对比";
            this.Activated += new System.EventHandler(this.CompareWindow_Activated);
            this.Click += new System.EventHandler(this.CompareWindow_Click);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView compTreeViewLeft;
        private System.Windows.Forms.TreeView compTreeViewRight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer highLightTimer;
        private System.Windows.Forms.Panel panel2;


    }
}

