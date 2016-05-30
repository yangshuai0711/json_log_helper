using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using EPocalipse.Json.Viewer.Properties;
using System.Collections;
using compareWindow;

namespace EPocalipse.Json.Viewer
{
    public partial class JsonViewer : UserControl
    {
        private string _json;
        private int _maxErrorCount = 25;
        private ErrorDetails _errorDetails;
        private PluginsManager _pluginsManager = new PluginsManager();
        private bool ignoreSelChange;
        private List<string> hisStringList = new List<string>();
        private Dictionary<string, JsonObjectTree> hisJsonMap = new Dictionary<string, JsonObjectTree>();

        public JsonViewer()
        {
            InitializeComponent();
            try
            {
                //hisStringList.RemoveAt(1);
                _pluginsManager.Initialize();
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format(Resources.ConfigMessage, e.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string Json
        {
            get
            {
                return _json;
            }
            set
            {
                if (_json != value)
                {
                    _json = value.Trim();
                    txtJson.Text = _json;
                    Redraw();
                }
            }
        }

        [DefaultValue(25)]
        public int MaxErrorCount
        {
            get
            {
                return _maxErrorCount;
            }
            set
            {
                _maxErrorCount = value;
            }
        }

        private void updateHisBoxTitle(int index) {
            string title = (String)hisListBox1.Items[index];
            int pos = title.IndexOf("→") + 1;
            title = DateTime.Now.ToLongTimeString() + "→" + (pos>=title.Length?"":title.Substring(pos));
            hisListBox1.Items[index] = title;
            return;
        }
        private string prevText = null;

        private void Redraw()
        {
            if (_json == prevText) 
            {
                updateHisBoxTitle(hisListBox1.Items.Count-1);
                return;
            }
            try
            {
                tvJson.BeginUpdate();
                try
                {
                    Reset();
                    if (!String.IsNullOrEmpty(_json))
                    {
                    
                        JsonObjectTree tree = null;
                       if (hisJsonMap.ContainsKey(_json)) 
                        {
                            tree = hisJsonMap[_json];
                            int index = hisStringList.IndexOf(_json);
                            if (index >= 0) {
                                hisStringList.Add(hisStringList[index]);
                                hisStringList.RemoveAt(index);
                                hisListBox1.Items.Add(hisListBox1.Items[index]);
                                hisListBox1.Items.RemoveAt(index);
                                hisListBox1.SetSelected(hisListBox1.Items.Count-1, true);
                                updateHisBoxTitle(hisListBox1.Items.Count - 1);
                                lastSelectHisItem = (string)hisListBox1.Items[hisListBox1.Items.Count - 1];
                            }
                        }
                        else
                        {
                            tree = JsonObjectTree.Parse(_json);
                            hisJsonMap.Add(_json, tree);
                            hisStringList.Add(_json);
                            string title = DateTime.Now.ToLongTimeString() + "→";
                            if (tree.Root.Fields.Count > 0)
                            {
                                title += tree.Root.Fields[0].Text.Replace("\n", " ");
                            }
                            hisListBox1.Items.Add(title, CheckState.Unchecked);
                            hisListBox1.SetSelected(hisListBox1.Items.Count - 1, true);
                            lastSelectHisItem = title;
                            if (hisListBox1.Items.Count > 40)
                            {//超过40条历史记录每多一条删除第一条
                                hisJsonMap.Remove(hisStringList[0]);
                                hisStringList.RemoveAt(0);
                                hisListBox1.Items.RemoveAt(0);
                            }
                        }
                        
                        VisualizeJsonTree(tree);
                    }
                }
                finally
                {
                    tvJson.EndUpdate();
                }
            }
            catch (JsonParseError e)
            {
                GetParseErrorDetails(e);
            }
            catch (Exception e)
            {
                ShowException(e);
            }
            prevText = _json;
        }

        private void Reset()
        {
            ClearInfo();
            tvJson.Nodes.Clear();
        }

        private void GetParseErrorDetails(Exception parserError)
        {
            UnbufferedStringReader strReader = new UnbufferedStringReader(_json);
            using (JsonReader reader = new JsonReader(strReader))
            {
                try
                {
                    while (reader.Read()) { };
                }
                catch (Exception e)
                {
                    _errorDetails._err = e.Message;
                    _errorDetails._pos = strReader.Position;
                }
            }
            if (_errorDetails.Error == null)
                _errorDetails._err = parserError.Message;
            if (_errorDetails.Position == 0)
                _errorDetails._pos = _json.Length;
            if (!txtJson.ContainsFocus)
                MarkError(_errorDetails);
            ShowInfo(_errorDetails);
        }

        private void MarkError(ErrorDetails _errorDetails)
        {
            ignoreSelChange = true;
            try
            {
                txtJson.Select(Math.Max(0, _errorDetails.Position - 1), 10);
                txtJson.ScrollToCaret();
            }
            finally
            {
                ignoreSelChange = false;
            }
        }

        private void VisualizeJsonTree(JsonObjectTree tree)
        {
            levelNodesCount = new int[20];
            AddNode(tvJson.Nodes, tree.Root);
            JsonViewerTreeNode node = GetRootNode();
            int total = 0;
            int index = 0;
            for (int i = 0; i < 20; i++)
            {
                total += levelNodesCount[i];
                if (total * 18 > tvJson.Bounds.Height*1)
                { 
                    break;
                }
                else {
                    index = i;
                }
               
            }
            expandSubNodes(node, index-1<0?0:index-1);
            //node.TreeView.Sort();
            tvJson.SelectedNode = node;
        }

        public static void expandSubNodes(TreeNode node ,int level){
            if (node.Level > level)
            {
                node.Collapse();
                return;
            }
            node.Expand();
            if (node.Level > level - 1) return;
            if (node.Nodes != null && node.Nodes.Count > 0) 
            {
                TreeNodeCollection subs = node.Nodes;
                for (int i = 0; i < node.Nodes.Count;i++ )
                {
                    expandSubNodes(node.Nodes[i], level);
                }
            }
        }

        private int[] levelNodesCount ;
        

        private void AddNode(TreeNodeCollection nodes, JsonObject jsonObject)
        {
            JsonViewerTreeNode newNode = new JsonViewerTreeNode(jsonObject);
            
            nodes.Add(newNode);
            newNode.Text = jsonObject.Text.Trim();
            newNode.Tag = jsonObject;
            newNode.ImageIndex = (int)jsonObject.JsonType;
            newNode.SelectedImageIndex = newNode.ImageIndex;
            
            foreach (JsonObject field in jsonObject.Fields)
            {
                AddNode(newNode.Nodes, field);
            }
           
            levelNodesCount[newNode.Level]++;
        }

        [Browsable(false)]
        public ErrorDetails ErrorDetails
        {
            get
            {
                return _errorDetails;
            }
        }

        public void Clear()
        {
            Json = String.Empty;
        }

        public void ShowInfo(string info)
        {
            lblError.Text = info;
            lblError.Tag = null;
            lblError.Enabled = false;
            tabControl.SelectedTab = pageTextView;
        }

        public void ShowInfo(ErrorDetails error)
        {
            ShowInfo(error.Error);
            lblError.Text = error.Error;
            lblError.Tag = error;
            lblError.Enabled = true;
            tabControl.SelectedTab = pageTextView;
        }

        public void ClearInfo()
        {
            lblError.Text = String.Empty;
        }

        [Browsable(false)]
        public bool HasErrors
        {
            get
            {
                return _errorDetails._err != null;
            }
        }

        private void txtJson_TextChanged(object sender, EventArgs e)
        {
            Json = txtJson.Text;
            btnViewSelected.Checked = false;
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            txtFind.BackColor = SystemColors.Window;
            FindNext(true, true);
        }

        public bool FindNext(bool includeSelected)
        {
            return FindNext(txtFind.Text, includeSelected);
        }

        public void FindNext(bool includeSelected, bool fromUI)
        {
            if (!FindNext(includeSelected) && fromUI)
                txtFind.BackColor = Color.LightCoral;
        }

        public bool FindNext(string text, bool includeSelected)
        {
            TreeNode startNode = tvJson.SelectedNode;
            if (startNode == null && HasNodes())
                startNode = GetRootNode();
            if (startNode != null)
            {
                startNode = FindNext(startNode, text, includeSelected);
                if (startNode != null)
                {
                    tvJson.SelectedNode = startNode;
                    return true;
                }
            }
            return false;
        }

        public TreeNode FindNext(TreeNode startNode, string text, bool includeSelected)
        {
            if (text == String.Empty)
                return startNode;

            if (includeSelected && IsMatchingNode(startNode, text))
                return startNode;

            TreeNode originalStartNode = startNode;
            startNode = GetNextNode(startNode);
            text = text.ToLower();
            while (startNode != originalStartNode)
            {
                if (IsMatchingNode(startNode, text))
                    return startNode;
                startNode = GetNextNode(startNode);
            }

            return null;
        }

        private TreeNode GetNextNode(TreeNode startNode)
        {
            TreeNode next = startNode.FirstNode ?? startNode.NextNode;
            if (next == null)
            {
                while (startNode != null && next == null)
                {
                    startNode = startNode.Parent;
                    if (startNode != null)
                        next = startNode.NextNode;
                }
                if (next == null)
                {
                    next = GetRootNode();
                    FlashControl(txtFind, Color.Cyan);
                }
            }
            return next;
        }

        private bool IsMatchingNode(TreeNode startNode, string text)
        {
            return (startNode.Text.ToLower().Contains(text));
        }

        private JsonViewerTreeNode GetRootNode()
        {
            if (tvJson.Nodes.Count > 0)
                return (JsonViewerTreeNode)tvJson.Nodes[0];
            return null;
        }

        private bool HasNodes()
        {
            return (tvJson.Nodes.Count > 0);
        }

        private void txtFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FindNext(false, true);
            }
            if (e.KeyCode == Keys.Escape)
            {
                HideFind();
            }
        }

        private void FlashControl(Control control, Color color)
        {
            Color prevColor = control.BackColor;
            try
            {
                control.BackColor = color;
                control.Refresh();
                Thread.Sleep(25);
            }
            finally
            {
                control.BackColor = prevColor;
                control.Refresh();
            }
        }

        public void ShowTab(Tabs tab)
        {
            tabControl.SelectedIndex = (int)tab;
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            try
            {
                string json = txtJson.Text;
                JsonSerializer s = new JsonSerializer();
                JsonReader reader = new JsonReader(new StringReader(json));
                Object jsonObject = s.Deserialize(reader);
                if (jsonObject != null)
                {
                    StringWriter sWriter = new StringWriter();
                    JsonWriter writer = new JsonWriter(sWriter);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 4;
                    writer.IndentChar = ' ';
                    s.Serialize(writer, jsonObject);
                    txtJson.Text = sWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex);
            }
        }

        private void ShowException(Exception e)
        {
            MessageBox.Show(this, e.Message, "Json Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnStripToSqr_Click(object sender, EventArgs e)
        {
            StripTextTo('[', ']');
        }

        private void btnStripToCurly_Click(object sender, EventArgs e)
        {
            StripTextTo('{', '}');
        }

        private void StripTextTo(char sChr, char eChr)
        {
            string text = txtJson.Text;
            int start = text.IndexOf(sChr);
            int end = text.LastIndexOf(eChr);
            int newLen = end - start + 1;
            if (newLen > 1)
            {
                txtJson.Text = text.Substring(start, newLen);
            }
        }

        private void tvJson_AfterSelect(object sender, TreeViewEventArgs e)
        {
           
        }

        private JsonViewerTreeNode GetSelectedTreeNode()
        {
            return (JsonViewerTreeNode)tvJson.SelectedNode;
        }

        private void tvJson_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            
        }



        private void btnCloseFind_Click(object sender, EventArgs e)
        {
            HideFind();
        }

        private void JsonViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && e.Control)
            {
                ShowFind();
            }
        }

        private void HideFind()
        {
            pnlFind.Visible = false;
            tvJson.Focus();
        }

        private void ShowFind()
        {
            pnlFind.Visible = true;
            txtFind.Focus();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFind();
        }

        private void expandallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvJson.BeginUpdate();
            try
            {
                if (tvJson.SelectedNode != null)
                {
                    TreeNode topNode = tvJson.TopNode;
                    tvJson.SelectedNode.ExpandAll();
                    tvJson.TopNode = topNode;
                }
            }
            finally
            {
                tvJson.EndUpdate();
            }
        }

        private void tvJson_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = tvJson.GetNodeAt(e.Location);
                if (node != null)
                {
                    tvJson.SelectedNode = node;
                }
            }
        }


        private void cbVisualizers_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = ((IJsonViewerPlugin)e.ListItem).DisplayName;
        }

        private void mnuTree_Opening(object sender, CancelEventArgs e)
        {
            mnuFind.Enabled = (GetRootNode() != null);
            mnuExpandAll.Enabled = (GetSelectedTreeNode() != null);

            mnuCopy.Enabled = mnuExpandAll.Enabled;
            mnuCopyValue.Enabled = mnuExpandAll.Enabled;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            string text;
            if (txtJson.SelectionLength > 0)
                text = txtJson.SelectedText;
            else
                text = txtJson.Text;
            Clipboard.SetText(text);
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            txtJson.Text = Clipboard.GetText();
        }

        private void mnuCopy_Click(object sender, EventArgs e)
        {
            JsonViewerTreeNode node = GetSelectedTreeNode();
            if (node != null)
            {
                Clipboard.SetText(node.Text);
            }
        }

        private void mnuCopyName_Click(object sender, EventArgs e)
        {
            JsonViewerTreeNode node = GetSelectedTreeNode();

            if (node != null && node.JsonObject.Id != null)
            {
                JsonObject obj = node.Tag as JsonObject;
                Clipboard.SetText(obj.Id);
            }
            else
            {
                Clipboard.SetText("");
            }

        }

        private void mnuCopyValue_Click(object sender, EventArgs e)
        {
            JsonViewerTreeNode node = GetSelectedTreeNode();
            if (node != null && node.Tag != null)
            {
                JsonObject obj = node.Tag as JsonObject;
                string valueString = obj.Value == null ? "" : (obj.JsonType == JsonType.Value ? JavaScriptConvert.ToString(obj.Value) : JavaScriptConvert.SerializeObject(obj.Value));
                Clipboard.SetText(valueString);
            }
            else
            {
                Clipboard.SetText("");
            }
        }

        private void lblError_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lblError.Enabled && lblError.Tag != null)
            {
                ErrorDetails err = (ErrorDetails)lblError.Tag;
                MarkError(err);
            }
        }

        private void removeNewLineMenuItem_Click(object sender, EventArgs e)
        {
            StripFromText('\n', '\r');
        }

        private void removeSpecialCharsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = txtJson.Text;
            text = text.Replace(@"\""", @"""");
            txtJson.Text = text;
        }

        private void StripFromText(params char[] chars)
        {
            string text = txtJson.Text;
            foreach (char ch in chars)
            {
                text = text.Replace(ch.ToString(), "");
            }
            txtJson.Text = text;
        }

        private void btnViewSelected_Click(object sender, EventArgs e)
        {
            if (btnViewSelected.Checked)
                _json = txtJson.SelectedText.Trim();
            else
                _json = txtJson.Text.Trim();
            Redraw();
        }

        private void txtJson_SelectionChanged(object sender, EventArgs e)
        {
            if (btnViewSelected.Checked && !ignoreSelChange)
            {
                _json = txtJson.SelectedText.Trim();
                Redraw();
            }
        }


        private string lastSelectHisItem = null;
        //显示选中记录
        private void hisListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _json = hisStringList[hisListBox1.SelectedIndex];
            txtJson.Text = _json;
            lastSelectHisItem = (string)hisListBox1.SelectedItem;
            Redraw();
        }



        //对比功能
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
           CheckedListBox.CheckedIndexCollection checkeds = hisListBox1.CheckedIndices;
           if (checkeds.Count == 2)
           {
               JsonObjectTree tree1 = hisJsonMap[hisStringList[checkeds[0]]];
               JsonObjectTree tree2 = hisJsonMap[hisStringList[checkeds[1]]];
               CheckedListBox.CheckedItemCollection items = hisListBox1.CheckedItems;
               KeyValuePair<string, JsonObjectTree>[] data = new KeyValuePair<string, JsonObjectTree>[]{
               new KeyValuePair<string, JsonObjectTree>((string)items[0],tree1),new KeyValuePair<string, JsonObjectTree>((string)items[1],tree2)
               };
               CompareWindow com = new CompareWindow(data);
               com.Show();
               //com.markDiff();
           }
           else {
               MessageBox.Show("请选择两条记录");
           }
        }
        //删除功能
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CheckedListBox.CheckedIndexCollection checkeds = hisListBox1.CheckedIndices;
            CheckedListBox.CheckedItemCollection checkedItems = hisListBox1.CheckedItems;
            if (checkeds.Count >0)
            {
                for (int i = checkeds.Count-1; i >=0; i--)
                {
                    hisJsonMap.Remove(hisStringList[checkeds[i]]);
                    hisStringList.RemoveAt(checkeds[i]);
                    string item = (string)checkedItems[i];
                    if (item == lastSelectHisItem)
                    {
                        Reset();
                    }
                    prevText = null;
                    hisListBox1.Items.RemoveAt(checkeds[i]);
                    
                }
            }
            else
            {
                MessageBox.Show("请选择记录");
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            bool falg = hisListBox1.CheckedIndices.Count > 0 ? false : true;
            for (int i = 0; i < hisListBox1.Items.Count; i++) {
                hisListBox1.SetItemChecked(i, falg);
            }
        }

        private ToolTip toolTip = new ToolTip();


        private void hisListBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int idx = hisListBox1.IndexFromPoint(e.Location);// 获取鼠标所在的项索引  
            if (idx == -1) //鼠标所在位置没有 项  
            {
                return;
            }
            string txt = hisListBox1.GetItemText(hisListBox1.Items[idx]); //获取项文本 
            if(txt!=toolTip.GetToolTip(hisListBox1)){
                toolTip.SetToolTip(hisListBox1, txt);
            }
            
        }

        private void 添加到列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JsonViewerTreeNode node = GetSelectedTreeNode();
            if (node == GetRootNode()) {
                MessageBox.Show("这么干有啥意义？");
            }
            if (node != null && node.Tag != null)
            {
                JsonObject obj = node.Tag as JsonObject;
                string valueString = obj.Id;
                valueString+= obj.Value == null ? "" : (obj.JsonType == JsonType.Value ? ":"+JavaScriptConvert.ToString(obj.Value) : JavaScriptConvert.SerializeObject(obj.Value));
                txtJson.Text = valueString;
                Redraw();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            tvJson.BeginUpdate();
            try
            {
                if (tvJson.SelectedNode != null)
                {
                    TreeNode topNode = tvJson.TopNode;
                    tvJson.SelectedNode.Collapse(false) ;
                    tvJson.TopNode = topNode;
                }
            }
            finally
            {
                tvJson.EndUpdate();
            }
        }

    }

    public struct ErrorDetails
    {
        internal string _err;
        internal int _pos;

        public string Error
        {
            get
            {
                return _err;
            }
        }

        public int Position
        {
            get
            {
                return _pos;
            }
        }

        public void Clear()
        {
            _err = null;
            _pos = 0;
        }
    }

    public class JsonViewerTreeNode : TreeNode
    {
        JsonObject _jsonObject;
        List<ICustomTextProvider> _textVisualizers = new List<ICustomTextProvider>();
        List<IJsonVisualizer> _visualizers = new List<IJsonVisualizer>();
        private bool _init;
        private IJsonVisualizer _lastVisualizer;

        public JsonViewerTreeNode(JsonObject jsonObject)
        {
            _jsonObject = jsonObject;
        }

        public List<ICustomTextProvider> TextVisualizers
        {
            get
            {
                return _textVisualizers;
            }
        }

        public List<IJsonVisualizer> Visualizers
        {
            get
            {
                return _visualizers;
            }
        }

        public JsonObject JsonObject
        {
            get
            {
                return _jsonObject;
            }
        }

        internal bool Initialized
        {
            get
            {
                return _init;
            }
            set
            {
                _init = value;
            }
        }

        internal void RefreshText()
        {
            StringBuilder sb = new StringBuilder(_jsonObject.Text);
            foreach (ICustomTextProvider textVisualizer in _textVisualizers)
            {
                try
                {
                    string customText = textVisualizer.GetText(_jsonObject);
                    sb.Append(" (" + customText + ")");
                }
                catch
                {
                    //silently ignore
                }
            }
            string text = sb.ToString();
            if (text != this.Text)
                this.Text = text;
        }

        public IJsonVisualizer LastVisualizer
        {
            get
            {
                return _lastVisualizer;
            }
            set
            {
                _lastVisualizer = value;
            }
        }
    }

    public enum Tabs { Viewer, Text };
}