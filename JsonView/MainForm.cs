using System;
using System.Windows.Forms;
using EPocalipse.Json.Viewer;
using System.IO;
using System.Runtime.InteropServices;   //调用WINDOWS API函数时要用到
//写入注册表时要用到

namespace EPocalipse.Json.JsonView
{
    public partial class MainForm : Form
    {
        private KeyboardHook k_hook;

        private DateTime lastCtrlCTime = DateTime.Now;

        private static MainForm self;
        public MainForm()
        {
            InitializeComponent();
            self = this;
            //2.安装Hook，在程序入口中写上下面的代码（本例中用了WinForm，在Form的构造方法中安装Hook即可）
            //安装键盘钩子
            k_hook = new KeyboardHook();
            k_hook.KeyDownEvent += new KeyEventHandler(hook_KeyDown);//钩住键按下
            k_hook.Start();//安装键盘钩子
            //this.notifyIcon1.Visible = true;

        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.C && (int)Control.ModifierKeys == (int)Keys.Control)
            {
                TimeSpan offsetSpan = DateTime.Now - lastCtrlCTime;
                double offset = offsetSpan.TotalMilliseconds;
                if (offset < 500)
                {
                    showWindow();
                    LoadFromClipboard();
                }
                lastCtrlCTime = DateTime.Now;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.Equals("/c", StringComparison.OrdinalIgnoreCase))
                {
                    LoadFromClipboard();
                }
                else if (File.Exists(arg))
                {
                    LoadFromFile(arg);
                }
            }
        }

        private void LoadFromFile(string fileName)
        {
            string json = File.ReadAllText(fileName);
            JsonViewer.ShowTab(Tabs.Viewer);
            JsonViewer.Json = json;
        }

        private void LoadFromClipboard()
        {
            string json = Clipboard.GetText();
            if (!String.IsNullOrEmpty(json))
            {
                JsonViewer.ShowTab(Tabs.Viewer);
                JsonViewer.Json = json;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            JsonViewer.ShowTab(Tabs.Text);
        }

        /// <summary>
        /// Closes the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item File > Exit</remarks>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exitWindow();
        }

        /// <summary>
        /// Open File Dialog  for Yahoo! Pipe files or JSON files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item File > Open</remarks>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter =
               "Yahoo! Pipe files (*.run)|*.run|json files (*.json)|*.json|All files (*.*)|*.*";
            dialog.InitialDirectory = Application.StartupPath;
            dialog.Title = "Select a JSON file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.LoadFromFile(dialog.FileName);
            }
        }

        /// <summary>
        /// Selects all text in the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Select All</remarks>
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ((RichTextBox)c).SelectAll();
        }

        /// <summary>
        /// Deletes selected text in the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Delete</remarks>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            string text;
            if (((RichTextBox)c).SelectionLength > 0)
                text = ((RichTextBox)c).SelectedText;
            else
                text = ((RichTextBox)c).Text;
            ((RichTextBox)c).SelectedText = "";
        }

        /// <summary>
        /// Pastes text in the clipboard into the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Paste</remarks>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ((RichTextBox)c).Paste();
        }

        /// <summary>
        /// Copies text in the textbox into the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Copy</remarks>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ((RichTextBox)c).Copy();
        }

        /// <summary>
        /// Cuts selected text from the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Cut</remarks>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ((RichTextBox)c).Cut();
        }

        /// <summary>
        /// Undo the last action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Edit > Undo</remarks>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("txtJson", true)[0];
            ((RichTextBox)c).Undo();
        }

        /// <summary>
        /// Displays the find prompt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Find</remarks>
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("pnlFind", true)[0];
            ((Panel)c).Visible = true;
            Control t;
            t = this.JsonViewer.Controls.Find("txtFind", true)[0];
            ((TextBox)t).Focus();
        }

        /// <summary>
        /// Expands all the nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Expand All</remarks>
        /// <!---->
        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("tvJson", true)[0];
            ((TreeView)c).BeginUpdate();
            try
            {
                if (((TreeView)c).SelectedNode != null)
                {
                    TreeNode topNode = ((TreeView)c).TopNode;
                    ((TreeView)c).SelectedNode.ExpandAll();
                    ((TreeView)c).TopNode = topNode;
                }
            }
            finally
            {
                ((TreeView)c).EndUpdate();
            }
        }

        /// <summary>
        /// Copies a node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Copy</remarks>
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("tvJson", true)[0];
            TreeNode node = ((TreeView)c).SelectedNode;
            if (node != null)
            {
                Clipboard.SetText(node.Text);
            }
        }

        /// <summary>
        /// Copies just the node's value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Menu item Viewer > Copy Value</remarks>
        /// <!-- JsonViewerTreeNode had to be made public to be accessible here -->
        private void copyValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control c;
            c = this.JsonViewer.Controls.Find("tvJson", true)[0];
            JsonViewerTreeNode node = (JsonViewerTreeNode)((TreeView)c).SelectedNode;
            if (node != null && node.JsonObject.Value != null)
            {
                Clipboard.SetText(node.JsonObject.Value.ToString());
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮 
            if (WindowState == FormWindowState.Minimized)
            {
                hideWindow();

            }
            else
            {
                prevStatus = WindowState;
            }
           
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            hideWindow();
            
        }

        private FormWindowState prevStatus = FormWindowState.Normal;

        public void showWindow() 
        {
            //隐藏任务栏区图标 
            this.TopMost = true;
            this.ShowInTaskbar = true;
            this.Visible = true;
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = prevStatus;
            }
            this.Activate();
            this.TopMost = false;
 
        }

        public void hideWindow() 
        {
            //隐藏任务栏区图标 
            WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
        }

        public void exitWindow()
        {
            k_hook.Stop();
            this.notifyIcon1.Dispose();
            this.Close();
            Application.Exit();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exitWindow();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showWindow();
        }

        private void notifyIcon1_Click(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            showWindow();
        }

        private void JsonViewer_Load(object sender, EventArgs e)
        {

        }

        //响应重复实例激活窗口通知
        /**protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case 0x0407://自定义消息，0400之后即可
                    showWindow();
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }//   */

    }
}